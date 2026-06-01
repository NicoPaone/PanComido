using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class ProveedorEntityMapper
    {
        public DOM.Proveedor paraDominio(EF.Proveedor efProveedor)
        {
            if (efProveedor == null) return null;
            return new DOM.Proveedor
            {
                Id = efProveedor.Id,
                RestauranteId = efProveedor.RestauranteId,
                Nombre = efProveedor.Nombre,
                NumeroTelefonoWsp = efProveedor.NumeroTelefonoWsp,
                Categorias = efProveedor.CategoriaInsumos
                .Select(ci => ci.Descripcion)
                .ToList()
            };
        }
    }
}
