using System;
using System.Collections.Generic;

#nullable disable

namespace ChatApi.Context.Model
{
    public partial class TbEstadoMensaje
    {
        public TbEstadoMensaje()
        {
            TbMensajes = new HashSet<TbMensaje>();
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<TbMensaje> TbMensajes { get; set; }
    }
}
