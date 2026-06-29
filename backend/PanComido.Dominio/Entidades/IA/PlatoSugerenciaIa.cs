namespace PanComido.Dominio.Entidades.IA
{
    public class PlatoSugerenciaIa
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty; // "descuento", "destacado", "combo", "pausa"
        public string Accion { get; set; } = string.Empty;
        public string Impacto { get; set; } = string.Empty;
        public string Dificultad { get; set; } = "baja"; // "baja", "media", "alta"
        public bool EsAplicable { get; set; } = true;
        public bool Aplicada { get; set; } = false;
    }
}
