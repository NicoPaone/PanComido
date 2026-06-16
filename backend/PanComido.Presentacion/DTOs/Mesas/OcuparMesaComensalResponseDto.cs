
namespace PanComido.Presentacion.DTOs.Mesas
{
    public class OcuparMesaComensalResponseDto
    {
        public MesaSinPosicionesResponseDto Mesa { get; set; }
        public int IdComandaGenerada { get; set; }
    }
}
