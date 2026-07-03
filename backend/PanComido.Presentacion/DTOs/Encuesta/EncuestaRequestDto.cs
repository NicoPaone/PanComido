namespace PanComido.Presentacion.DTOs.Encuesta
{
    public class EncuestaRequestDto
    {
        public int ComandaId { get; set; }
        public int PuntuacionLugar { get; set; }
        public int PuntuacionComida { get; set; }
        public int PuntuacionMozo { get; set; }
    }
}
