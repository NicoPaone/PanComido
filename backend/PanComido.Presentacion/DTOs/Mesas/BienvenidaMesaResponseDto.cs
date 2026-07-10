namespace PanComido.Presentacion.DTOs.Mesas
{
    public class BienvenidaMesaResponseDto
    {
        public int IdMesa { get; set; }
        public int NumeroMesa { get; set; }
        public int CantidadMaximaComensales { get; set; }
        public string EstadoActual { get; set; }
        public int RestauranteId { get; set; }

    }
}
