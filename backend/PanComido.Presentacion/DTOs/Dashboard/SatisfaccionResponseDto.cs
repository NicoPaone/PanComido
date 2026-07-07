namespace PanComido.Presentacion.DTOs.Dashboard
{
    public class SatisfaccionResponseDto
    {
        public double PromedioComida { get; set; }
        public double PromedioLugar { get; set; }
        public double PromedioAtencion { get; set; }
        public int TotalEncuestas { get; set; }
        public int TotalDerivadosGoogleMaps { get; set; }
        public double PorcentajeDerivados { get; set; }
    }
}
