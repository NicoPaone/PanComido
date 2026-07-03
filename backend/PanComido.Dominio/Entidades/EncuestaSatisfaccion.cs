using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class EncuestaSatisfaccion
    {
        public int Id { get; set; }
        public int ComandaId { get; set; }
        public int PuntuacionLugar { get; set; }
        public int PuntuacionComida { get; set; }
        public int PuntuacionMozo { get; set; }
        public DateTime Fecha { get; set; }
    }
}
