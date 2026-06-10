namespace PanComido.Presentacion.DTOs.TurnoLaboral
{
    public class TurnoLaboralResponseDto
    {
        public int Id { get; set; }
        public int RestauranteId { get; set; }
        public TimeOnly HorarioInicio { get; set; }
        public TimeOnly HorarioFin { get; set; }
        public bool EsNocturno { get; set; }
    }
}
