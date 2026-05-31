namespace PanComido.Presentacion.DTOs
{
    public class ComandaResponseDto
    {
        public int Id { get; set; }
        public int MesaId { get; set; }
        public int cantComensales { get; set; }
        public string Estado { get; set; }
        public string HoraInicio { get; set; }
         public string? HoraFin { get; set; }

        public int TiempoEstimadoTotal { get; set; }
         public List<PlatoDto> Platos { get; set; }


    }
}
