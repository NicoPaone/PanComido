namespace PanComido.Presentacion.DTOs.PorcetajesGanancia
{
    public class PorcentajesGananciaRequestDto
    {
        public List<PorcentajeItemRequestDto> Platos { get; set; }
        public List<PorcentajeItemRequestDto> Bebidas { get; set; }
    }
}
