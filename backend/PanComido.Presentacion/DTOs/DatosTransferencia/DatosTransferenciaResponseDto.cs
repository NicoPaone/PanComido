namespace PanComido.Presentacion.DTOs.DatosTransferencia
{
    public class DatosTransferenciaResponseDto
    {
        public int Id { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string? Cbu { get; set; }
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TitularCuenta { get; set; } = string.Empty;
    }
}
