using AutoMapper;
using ChatApi.Context.Model;
using ChatApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR;
using ChatApi.Hubs;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("api/Mensaje")]
    public class MensajeController: ControllerBase
    {
        DateTime currentTimePacific = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
        private readonly ChatAppContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IHubContext<ChatHub> hubcontext;

        public MensajeController(
            ChatAppContext context,
            IMapper mapper,
            IConfiguration configuration,
            IHubContext<ChatHub> hubcontext
        )
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.hubcontext = hubcontext;
        }

        [HttpPost("Enviar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MensajeDto>> Enviar([FromBody] NuevoMensajeDto mensaje)
        {
            var claims   = HttpContext.User.Claims.ToList();
            var fromId   = claims[2].Value;
            var toId     = mensaje.ReceptorId;

            var nuevoMensaje = new TbMensajeDto(){
                Id              = Guid.NewGuid().ToString(),
                EmisorUserId    = fromId,
                ReceptorUserId  = toId,
                Contenido       = mensaje.Contenido,
                Fecha           = currentTimePacific,
                IdEstadoMensaje = 1 // 1 ENVIADO , 2 RECIBIDO, 3 LEIDO
            };
            var nuevoMensajeView = mapper.Map<TbMensajeDto, TbMensaje>(nuevoMensaje);
            await context.AddAsync(nuevoMensajeView);
            await context.SaveChangesAsync();
            var nm = await context.TbMensajes.Include(m=>m.EmisorUser).Include(m=>m.ReceptorUser).Where(m=>m.Id == nuevoMensaje.Id).FirstOrDefaultAsync();
            
            var newMessage = mapper.Map<TbMensaje, MensajeDto>(nm);
            /*
            newMessage.Entrante = true;
            var conexionesReceptor = await context.TbConexions.Where(c => c.UserId == mensaje.ReceptorId).ToListAsync();
            var conexionIds = conexionesReceptor.Select(c => c.Id).ToList();
            await hubcontext.Clients.Clients(conexionIds).SendAsync("ReceiveMessage", newMessage);
            newMessage.Entrante = false;
            var conexionesEmisor = await context.TbConexions.Where(c => c.UserId == mensaje.ReceptorId).ToListAsync();
            var conexionIdsEmisor = conexionesEmisor.Select(c => c.Id).ToList();
            await hubcontext.Clients.Clients(conexionIdsEmisor).SendAsync("ReceiveMessage", newMessage);
            */
            await hubcontext.Clients.All.SendAsync("ReceiveMessage", newMessage);
            return Ok(newMessage);
        }

        [HttpGet("Listar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<MensajeDto>>> Listar([FromQuery] UserIdDto user)
        {
            var claims  = HttpContext.User.Claims.ToList();
            var userId  = claims[2].Value;
            var toId    = user.Id;
            var mensajes = await context.TbMensajes.Include(m => m.EmisorUser).Include(m => m.ReceptorUser)
                                .Where(m =>
                                (m.EmisorUser.Id == userId && m.ReceptorUser.Id == toId) ||
                                (m.EmisorUser.Id == toId && m.ReceptorUser.Id == userId)).OrderBy(m => m.Fecha)
                                .ToListAsync();
            var mensajesView = mapper.Map<List<TbMensaje>, List<MensajeDto>>(mensajes);
            return mensajesView.Select(m => { m.Entrante = m.ReceptorUser.Id == userId ? true : false; return m; }).ToList();
        }

    }
    
}
