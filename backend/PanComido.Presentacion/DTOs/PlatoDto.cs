namespace PanComido.Presentacion.DTOs
{
    public class PlatoDto
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }

        public bool Entregado { get; set; }
        public string ? ObservacionesGenerales { get; set; }
        public string ? ObservacionesIngredientes { get; set; }


    }
}
