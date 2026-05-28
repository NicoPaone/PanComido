using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateOnly Fecha { get; set; }
        public string Estado { get; set; }
        public List<PedidoInsumo> ItemsInsumo { get; set; } = new();
    }
}
