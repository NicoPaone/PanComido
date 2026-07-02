using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class DatosTransferenciaEntityMapper
    {
        public DOM.DatosTransferencia paraDominio(EF.DatosTransferencium efDatosTransferencia)
        {
            return new DOM.DatosTransferencia
            {
                Id = efDatosTransferencia.Id,
                RestauranteId = efDatosTransferencia.RestauranteId,
                Alias = efDatosTransferencia.Alias,
                Cbu = efDatosTransferencia.Cbu,
                NumeroCuenta = efDatosTransferencia.NumeroCuenta,
                TitularCuenta = efDatosTransferencia.TitularCuenta,
            };
        }
        public void paraActualizarEntidad(EF.DatosTransferencium efDatosExistente, DOM.DatosTransferencia datosTransferencia)
        {
            efDatosExistente.Alias = datosTransferencia.Alias;
            efDatosExistente.Cbu = datosTransferencia.Cbu;
            efDatosExistente.NumeroCuenta = datosTransferencia.NumeroCuenta;
            efDatosExistente.TitularCuenta = datosTransferencia.TitularCuenta;
        }
    }
}
