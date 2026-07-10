namespace PanComido.Presentacion.DTOs.Dashboard
{
    public class IngredienteExcluidoStatDto
    {
        public int IngredienteId { get; set; }
        public string NombreIngrediente { get; set; } = string.Empty;
        public int CantidadExclusiones { get; set; }
        public string PlatoMasExcluido { get; set; } = string.Empty;
        public int ExclusionesEnPlatoMasExcluido { get; set; }
        public int TotalPedidosPlatoMasExcluido { get; set; }
        public string TasaExclusionPlatoMasExcluido { get; set; } = string.Empty;
    }
}
