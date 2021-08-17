using AutoMapper;
using ChatApi.Context.Model;
using ChatApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Helpers
{
    public class CustomMapper: Profile
    {
       public CustomMapper()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, PerfilDto>();
            CreateMap<User, NamesUserDto>();
            CreateMap<TbMensaje, MensajeDto>();
            CreateMap<TbMensaje, TbMensajeDto>().ReverseMap();
            CreateMap<TbEstadoMensaje, EstadoMensajeDto>();
            CreateMap<TbConexion, ConexionDto>().ReverseMap();
        }
    }


}
