namespace PanComido.Presentacion.DTOs.MiseAndPlace
{
    public class IngredienteMiseAndPlaceResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal CostoUnitario { get; set; }
    }
}
