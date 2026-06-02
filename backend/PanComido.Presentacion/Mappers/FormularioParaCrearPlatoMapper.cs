using PanComido.Presentacion.DTOs;
using DOM = PanComido.Dominio.Entidades;



namespace PanComido.Presentacion.Mappers
{
    public class FormularioParaCrearPlatoMapper
    {
        public ItemDesplegableDto aDto(DOM.TipoPlato d) => new ItemDesplegableDto { Id = d.Id, Descripcion = d.Descripcion };
        public ItemDesplegableDto aDto(DOM.CategoriaPlato d) => new ItemDesplegableDto { Id = d.Id, Descripcion = d.Descripcion };
        public ItemDesplegableDto aDto(DOM.Restriccion d) => new ItemDesplegableDto { Id = d.Id, Descripcion = d.Descripcion };

        public IngredienteDisponibleDto aDto(DOM.Ingrediente d) => new IngredienteDisponibleDto { Id = d.Id, Nombre = d.Nombre, UnidadMedida = d.UnidadMedida, CostoUnitario = d.CostoUnitario };
        public IngredienteDisponibleDto aDto(DOM.IngredientePreparado d) => new IngredienteDisponibleDto { Id = d.Id, Nombre = d.Nombre, UnidadMedida = d.unidadMedida, CostoUnitario = 0m };

        public DatosFormularioCrearPlatoResponseDto aDto(DOM.DatosFormularioCrearPlato dom)
        {
            return new DatosFormularioCrearPlatoResponseDto
            {
                TiposPlato = dom.TiposPlato.Select(aDto).ToList(),
                CategoriasPlato = dom.CategoriasPlato.Select(aDto).ToList(),
                Restricciones = dom.Restricciones.Select(aDto).ToList(),

                // Concatenamos las dos listas del dominio en la lista única del DTO
                Ingredientes = dom.Ingredientes.Select(aDto)
                    .Concat(dom.IngredientePreparados.Select(aDto))
                    .ToList()
            };
        }



    }
}
