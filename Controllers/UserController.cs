using AutoMapper;
using ChatApi.Context.Model;
using ChatApi.DTOs;
using ChatApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("api/User")]
    public class UserController: ControllerBase
    {
        private readonly ChatAppContext context;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signinManager;
        private readonly IConfiguration configuration;
        
        public UserController(
            ChatAppContext context, 
            IMapper mapper, 
            UserManager<User> userManager, 
            SignInManager<User> signinManager,
            IConfiguration configuration
        )
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.signinManager = signinManager;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserTokenModel>> Login([FromBody] UserLoginModel model)
        {
            var identityUser = await userManager.FindByEmailAsync(model.Email);
            if (identityUser != null)
            {
                
                var resultado = await signinManager.PasswordSignInAsync(model.Email,
                    model.Password, isPersistent: true, lockoutOnFailure: false);
                if (resultado.Succeeded)
                {
                    return await ConstruirToken(model);
                }
                else
                {
                    return BadRequest("Intento de inicio de sesión no válido");
                }
            }
            else
            {
                return BadRequest("Usuario No existe");
            }

        }

        private async Task<UserTokenModel> ConstruirToken(UserLoginModel userInfo)
        {
            var identityUser = await userManager.FindByEmailAsync(userInfo.Email);
            

            if (identityUser != null)
            {
                var claims = new List<Claim>()
                            {
                                new Claim(ClaimTypes.Name, identityUser.Nombres + " " + identityUser.Apellidos),
                                new Claim(ClaimTypes.Email, userInfo.Email),
                                new Claim(ClaimTypes.NameIdentifier, identityUser.Id )
                            };

                var claimsDB = await userManager.GetClaimsAsync(identityUser);

                claims.AddRange(claimsDB);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var expiracion = DateTime.UtcNow.AddMonths(6);

                JwtSecurityToken token = new JwtSecurityToken(
                        issuer: null,
                        audience: null,
                        claims: claims,
                    expires: expiracion,
                    signingCredentials: creds);
                //String[] roles = { claimsDB[0].Value.ToString(), DatosUserName.CodigoIdentificador };

                return new UserTokenModel()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                };
            }

            return null;

        }

        [AllowAnonymous]
        [HttpPost("Registro")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserTokenModel>> Register([FromBody] UserRegisterModel model)
        {
            if (model == null)
            {
                return BadRequest(ModelState);
            }
            var user = new User { UserName = model.Email, Email = model.Email, Nombres = model.Nombres, Apellidos = model.Apellidos };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var identityUser = await userManager.FindByEmailAsync(model.Email);
                return await ConstruirToken(new UserLoginModel { Email = identityUser.Email, Password = model.Password }) ;
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Lista de usuarios con su ultimo mensaje
        /// </summary>
        /// <returns></returns>
        [HttpGet("Listar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<UserDto>>> Listar()
        {
            var claims      = HttpContext.User.Claims.ToList();   
            
            var userId      = claims[2].Value;
            var users = context.Users.Where(u => u.Id != userId).ToList();
            var usersView   =  mapper.Map<List<User>, List<UserDto>>(users);
            foreach(var userView in usersView)
            {
                var contactId = userView.Id;
                if(contactId != userId)
                {
                    var lastMsg = await context.TbMensajes.Include(m => m.EmisorUser).Include(m => m.ReceptorUser)
                                .Where(m => 
                                (m.EmisorUser.Id == userId && m.ReceptorUser.Id == contactId) || 
                                (m.EmisorUser.Id == contactId && m.ReceptorUser.Id == userId)).OrderByDescending(m => m.Fecha)
                                .FirstOrDefaultAsync();
                    var lastMsgView = mapper.Map<TbMensaje, MensajeDto>(lastMsg);
                    if (lastMsgView != null)
                    {
                        lastMsgView.Entrante = lastMsg.ReceptorUserId == userId ? true:false;
                        userView.Mensaje = lastMsgView;
                    }
                    
                }
            }
            return usersView;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Perfil")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<PerfilDto>>> Perfil()
        {
            var claims = HttpContext.User.Claims.ToList();
            var userId = claims[2].Value;
            var user = await context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync() ;
            var usersView = mapper.Map<User, PerfilDto>(user);
            return Ok(usersView);
        }

    }
}
