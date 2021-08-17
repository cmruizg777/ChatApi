using System;
using System.Collections.Generic;

#nullable disable

namespace ChatApi.Context.Model
{
    public partial class TbMensaje
    {
        public string Id { get; set; }
        public string Contenido { get; set; }
        public DateTime Fecha { get; set; }
        public int IdEstadoMensaje { get; set; }
        public string EmisorUserId { get; set; }
        public string ReceptorUserId { get; set; }
        public virtual User EmisorUser { get; set; }
        public virtual TbEstadoMensaje IdEstadoMensajeNavigation { get; set; }
        public virtual User ReceptorUser { get; set; }
    }
}
