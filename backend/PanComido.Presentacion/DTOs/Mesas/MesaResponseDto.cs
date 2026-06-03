namespace PanComido.Presentacion.DTOs.Mesas
{
   public class MesaResponseDto
   {

      public int Id { get; set; }
      public int NumeroMesa { get; set; }
      public int CantidadPersonasMax { get; set; }
      public string EstadoMesa { get; set; } = string.Empty;
      public int PosicionXInicio { get; set; }
      public int PosicionXFin { get; set; }
      public int PosicionYInicio { get; set; }
      public int PosicionYFin { get; set; }
      public string CodigoInvitacion { get; set; } = string.Empty;
      public DimensionMesaDto DimensionMesa { get; set; } = null!;

    }
   public class DimensionMesaDto
   {
      public int Id { get; set; }
      public string Forma { get; set; } = string.Empty;
      public string? Imagen { get; set; }
   }
}
