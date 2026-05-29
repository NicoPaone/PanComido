using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Proveedor
    {
        public int Id { get; set; }
        public int RestauranteId { get; set; }
        public string Nombre { get; set; }
        public string? NumeroTelefonoWsp { get; set; }
        public DateOnly? FechaUltimoPedido { get; set; }
        public List<string> Categorias { get; set; } = new();
    }
}
