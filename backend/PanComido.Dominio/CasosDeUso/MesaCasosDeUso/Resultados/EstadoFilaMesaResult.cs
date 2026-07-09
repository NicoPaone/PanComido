namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso.Resultados
{
    public class EstadoFilaMesaResult
    {
        public bool MesaLista { get; set; }
        public int? MesaAsignadaId { get; set; }
        public int MinutosRestantesParaOcupar { get; set; }
        public int NumeroTurno { get; set; }
        public int TurnosAdelante { get; set; }
        public int TiempoEstimadoMinutos { get; set; }
        public string TiempoEstimadoVisual { get; set; }
    }
}
