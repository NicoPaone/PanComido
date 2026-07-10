using PanComido.Presentacion.DTOs.Articulo;

namespace PanComido.Presentacion.DTOs.Comanda
{
    public class ComandaResponseDto
    {
        public int Id { get; set; }
        public int MesaId { get; set; }

        public int NumeroDeMesa {  get; set; }
        public int CantComensales { get; set; }
        public string Estado { get; set; }
        public string HoraInicio { get; set; }
        public string? HoraFin { get; set; }
        public string? HoraUltimoCambioEstado { get; set; }
        public int TiempoEstimadoTotal { get; set; }
      //public List<PlatoDto> Platos { get; set; }
      public List<ArticuloComandaResponseDto> Items { get; set; }
    }
}
