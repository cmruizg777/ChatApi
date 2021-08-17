using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Model
{
    public class UserRegisterModel
    {
        [Required(ErrorMessage = "Debes proporcionar tus nombres.")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "Debes proporcionar tus apellidos.")]
        public string Apellidos { get; set; }
        [Required(ErrorMessage = "Debes proporcionar un email.")]
        [EmailAddress(ErrorMessage = "Debes proporcionar un email válido.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Debes proporcionar una contraseña.")]
        public string Password { get; set; }
    }
}
