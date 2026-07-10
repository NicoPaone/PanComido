namespace PanComido.Dominio.Entidades
{
   public class MesaConPosiciones
   {
      public int Id { get; set; }
      public int Numero { get; set; }
      public int CantPersonasMax { get; set; }
      public Enums.EstadoMesa EstadoMesa { get; set; }
      public int PosicionXInicio { get; set; }
      public int PosicionXFin { get; set; }
      public int PosicionYInicio { get; set; }
      public int PosicionYFin { get; set; }
      public int DimensionMesaId { get; set; }
      public string Forma { get; set; } = string.Empty;
      public int TipoElemento { get; set; } = 1;
      public string? Color { get; set; }
      public string? TextoObjeto { get; set; }

      // para devolver comanda que se genera cuando se ocupa la mesa
      public int? idComanda { get; set; }

      public List<int> MozosAsignadosIds { get; set; } = new List<int>();

    }
}
