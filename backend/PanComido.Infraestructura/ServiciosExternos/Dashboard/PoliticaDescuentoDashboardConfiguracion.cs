namespace PanComido.Infraestructura.ServiciosExternos.Dashboard
{
    public class PoliticaDescuentoDashboardConfiguracion
    {
        public decimal PorcentajeDescuentoMaximo { get; set; } = 80m;
        public decimal MargenMinimoPermitido { get; set; } = 20m;
    }
}
