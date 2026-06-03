    using PanComido.Dominio.Entidades;
    using PanComido.Presentacion.DTOs.Articulos;
    using PanComido.Presentacion.DTOs.Ingredientes;

    namespace PanComido.Presentacion.Mappers
    {
        public class ArticuloMapper
        {
            public DetalleArticuloResponseDto aDto(Articulo articulo)
            {
                var dto = new DetalleArticuloResponseDto
                {
                    Id = articulo.Id,
                    Nombre = articulo.Nombre,
                    Descripcion = articulo.Descripcion,
                    Precio = articulo.PrecioVentaFinal ?? 0m,
                    UrlImagen = articulo.UrlImagen
                    
                };

                if (articulo is Plato plato)
                {
                    dto.TiempoPreparacionBase = plato.TiempoPreparacionBase;
                    dto.CategoriaPlato = plato.Categoria;
                    dto.TipoPlato = plato.TipoPlato;
                dto.Restricciones = plato.Restricciones?.Select(r => r.Descripcion).ToList() ?? new List<string>();

                dto.IngredientesOpcionales = plato.Ingredientes
                        .Where(i => i.Opcional == true)
                        .Select(i => new IngredientePersonalizableDto
                        {
                            IngredienteId = i.InsumoId,
                            Nombre = i.Insumo?.Nombre ?? "Ingrediente"
                        }).ToList();
                }
                else if (articulo is Insumo bebida)
                {
                    dto.CategoriaBebida = bebida.Categoria;
                }

                return dto;
            }
        }
    }
