namespace PanComido.Presentacion.DTOs.Mesas
{
    public class BienvenidaMesaResponseDto
    {
        public int IdMesa { get; set; }
        public int NumeroMesa { get; set; }
        public int CantidadMaximaComensales { get; set; }
        public string EstadoActual { get; set; }
        public int RestauranteId { get; set; }
        public string NombreRestaurante { get; set; }
        public string LogoUrl { get; set; }
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public string TipografiaTitulo { get; set; }
        public string TipografiaCuerpo { get; set; }


    }
}
