using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Context.Model
{
    public class TbConexion
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
