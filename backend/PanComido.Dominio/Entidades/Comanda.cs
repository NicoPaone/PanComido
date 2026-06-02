using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Comanda
    {
        public int Id { get; set; }
        public int MesaId { get; set; }
        public int RestauranteId { get; set; }
        public int? PagoID { get; set; }
        public int CantComensales { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public DateTime? HoraUltimoCambioEstado { get; set; }

        public  EstadoComanda Estado { get; set; }

        //public List<Plato> Platos { get; set; }
        public List<ArticuloComanda> Items { get; set; } = new List<ArticuloComanda>();
    }
}
