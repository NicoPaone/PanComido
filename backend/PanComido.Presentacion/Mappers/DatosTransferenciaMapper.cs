using PanComido.Presentacion.DTOs.DatosTransferencia;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class DatosTransferenciaMapper
    {
        public DatosTransferenciaResponseDto aDto(DOM.DatosTransferencia datosTransferencia)
        {
            return new DatosTransferenciaResponseDto
            {
                Id = datosTransferencia.Id,
                Alias = datosTransferencia.Alias,
                Cbu = datosTransferencia.Cbu,
                NumeroCuenta = datosTransferencia.NumeroCuenta,
                TitularCuenta = datosTransferencia.TitularCuenta
            };
        }

        public DOM.DatosTransferencia aDominio(DatosTransferenciaRequestDto datosTransferenciaRequest)
        {
            return new DOM.DatosTransferencia
            {
                Alias = datosTransferenciaRequest.Alias,
                Cbu = datosTransferenciaRequest.Cbu,
                NumeroCuenta = datosTransferenciaRequest.NumeroCuenta,
                TitularCuenta = datosTransferenciaRequest.TitularCuenta
            };
        }
    }
}
