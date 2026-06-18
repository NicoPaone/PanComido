using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Dominio.Entidades
{
    public class MesaMapaDominio
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public int CantPersonasMax { get; set; }
        public EstadoMesa EstadoMesa { get; set; }
        public int PosicionXInicio { get; set; }
        public int PosicionXFin { get; set; }
        public int PosicionYInicio { get; set; }
        public int PosicionYFin { get; set; }
        public int DimensionMesaId { get; set; }
        public string Forma { get; set; }
        public int TipoElemento { get; set; } = 1;
        public string? Color { get; set; }
        public string? TextoObjeto { get; set; }
    }
}
