using ChatApi.Context.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.DTOs
{
    public class MensajeDto
    {
        public string   Contenido { get; set; }
        public DateTime Fecha { get; set; }
        public bool     Entrante { get; set; }
        public virtual EstadoMensajeDto IdEstadoMensajeNavigation { get; set; }
        public virtual NamesUserDto     EmisorUser { get; set; }
        public virtual NamesUserDto     ReceptorUser { get; set; }
    }
    public class TbMensajeDto
    {
        
            public string Id { get; set; }
            public string Contenido { get; set; }
            public DateTime Fecha { get; set; }
            public int IdEstadoMensaje { get; set; }
            public string EmisorUserId { get; set; }
            public string ReceptorUserId { get; set; }
        
    }

    public class NuevoMensajeDto
    {
        [Required(ErrorMessage = "Debe proporcionar un destinario válido.")]
        public string ReceptorId { get; set; }

        [Required(ErrorMessage = "Debe proporcionar un mensaje para enviar.")]
        public string Contenido { get; set; }
    }
}
