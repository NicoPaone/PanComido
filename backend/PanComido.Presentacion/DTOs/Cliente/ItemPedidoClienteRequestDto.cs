namespace PanComido.Presentacion.DTOs.Cliente
{
    public class ItemPedidoClienteRequestDto
    {
        public int ArticuloId { get; set; }
        public int Cantidad { get; set; }
        public string? ObservacionesIngredientes { get; set; } // - con cebolla - sin cebolla
        public string? ObservacionesGenerales { get; set; } // texto libre
        //public List<int> IdIngredientesPersonalizadosSacados { get; set; }
    }
}
