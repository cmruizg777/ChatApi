using AutoMapper;
using ChatApi.Context.Model;
using ChatApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("api/Conexion")]
    public class ConexionController : ControllerBase
    {
        private readonly ChatAppContext context;
        private IMapper mapper;
        private readonly IConfiguration configuration;

        public ConexionController(
            ChatAppContext context,
            IMapper mapper,
            IConfiguration configuration
        )
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpPost("Nueva")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConexionDto>> Nueva([FromBody] ConexionIdDto conexion)
        {
            var claims = HttpContext.User.Claims.ToList();
            var userId = claims[2].Value;
            var conexionId = conexion.Id;

            var nuevaConexion = new ConexionDto()
            {
                Id = conexionId,
                UserId = userId
            };
            var nuevaConexionRow = mapper.Map<ConexionDto, TbConexion>(nuevaConexion);
            await context.AddAsync(nuevaConexionRow);
            await context.SaveChangesAsync();
            return Ok(nuevaConexion);
        }
        [HttpDelete("Borrar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConexionDto>> Borrar([FromQuery] ConexionIdDto conexion)
        {
            var conexionId = conexion.Id;
            var cnx =  context.TbConexions.Where(c => c.Id == conexionId).FirstOrDefault();
            context.TbConexions.Remove(cnx);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("VerConexion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConexionDto>> VerConexion([FromQuery] ConexionIdDto conexion)
        {
            var claims = HttpContext.User.Claims.ToList();
            var userId = claims[2].Value;
            var conexionId = conexion.Id;
            var cnx = context.TbConexions.Where(c => c.Id == conexionId).FirstOrDefault();
            if (cnx == null)
            {
                return Ok(new { success = false });
            }
            if (cnx.UserId == userId)
            {
                return Ok(new { success = true });
            }
            else
            {
                context.TbConexions.Remove(cnx);
                await context.SaveChangesAsync();
                return Ok(new { success = false });
            }
        }
    }
}
