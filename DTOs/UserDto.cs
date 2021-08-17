using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        public virtual MensajeDto Mensaje { get; set; }
    }
    public class PerfilDto
    {
        public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
    }
    public class NamesUserDto
    {
        public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }

    public class UserIdDto
    {
        public string Id { get; set; }
    }
}
