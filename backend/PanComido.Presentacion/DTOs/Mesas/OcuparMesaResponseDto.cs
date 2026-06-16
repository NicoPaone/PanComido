
namespace PanComido.Presentacion.DTOs.Mesas
{
    public class OcuparMesaResponseDto
    {
        public MesaSinPosicionesResponseDto Mesa { get; set; }
        public int IdComandaGenerada { get; set; }
    }
}
