using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Restaurante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string? ColorPrincipal { get; set; }
        public string? ColorSecundario { get; set; }
        public string? TextoPrincipal { get; set; }
        public string? TextoSecundario { get; set; }
        public int DireccionId { get; set; }
        public Ubicacion Ubicacion { get; set; }
    }
}
