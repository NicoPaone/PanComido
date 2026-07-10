using System;

namespace PanComido.Dominio.Entidades
{
    public class IngredienteExcluidoStat
    {
        public int IngredienteId { get; set; }
        public string NombreIngrediente { get; set; } = string.Empty;
        public int CantidadExclusiones { get; set; }
        public string PlatoMasExcluido { get; set; } = string.Empty;
        public int ExclusionesEnPlatoMasExcluido { get; set; }
        public int TotalPedidosPlatoMasExcluido { get; set; }
    }
}
