using PanComido.Presentacion.DTOs.Proveedores;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class ProveedorMapper
    {
        public ProveedorResponseDto aDto(DOM.Proveedor proveedor)
        {
            return new ProveedorResponseDto
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                NumeroTelefonoWsp = proveedor.NumeroTelefonoWsp,
                FechaUltimoPedido = proveedor.FechaUltimoPedido?.ToString("dd/MM/yyyy"),
                Categorias = proveedor.Categorias
                                  .Select(c => c)
                                  .ToList()
            };
        }

        public List<ProveedorResponseDto> aListaDto(
            List<DOM.Proveedor> proveedores)
        {
            return proveedores
                .Select(p => aDto(p))
                .ToList();
        }

        public DOM.Proveedor aDominio(ProveedorRequestDto proveedorRequest)
        {
            return new DOM.Proveedor
            {
                Nombre = proveedorRequest.Nombre,
                NumeroTelefonoWsp = proveedorRequest.NumeroTelefonoWsp,
                CategoriaIds = proveedorRequest.CategoriaIds
                              .Select(c => c)
                              .ToList()
            };
        }
    }
}
