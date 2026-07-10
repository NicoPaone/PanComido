namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso.Resultados
{
    public class TurnoMesaResult
    {
        public int TurnoId { get; set; }
        public int NumeroTurno { get; set; }
        public int TurnosAdelante { get; set; }
        public int TiempoEstimadoMinutos { get; set; }
        public bool MesaLista { get; set; }
        public int? MesaAsignadaId { get; set; }
        public int? MinutosRestantesParaOcupar { get; set; }
    }
}
