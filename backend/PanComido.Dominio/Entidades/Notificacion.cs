using System;

namespace PanComido.Dominio.Entidades
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int RestauranteId { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool Resuelta { get; set; }
    }
}
