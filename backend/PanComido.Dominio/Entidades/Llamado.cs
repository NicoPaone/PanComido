using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class Llamado
    {
        public int Id { get; set; }
        public int? MozoId { get; set; }
        public int? GerenteId { get; set; }
        public int? MesaId { get; set; }
        public int CategoriaLlamadoId { get; set; }
        public string CategoriaDescripcion { get; set; }
        public string Descripcion { get; set; }
        public bool Resuelto { get; set; }
    }
}
