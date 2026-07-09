namespace PanComido.Dominio.Entidades.Dashboard
{
    public class PoliticaDescuentoDashboard
    {
        public decimal PorcentajeDescuentoMaximo { get; set; } = 80m;
        public decimal MargenMinimoPermitido { get; set; } = 20m;
    }
}
