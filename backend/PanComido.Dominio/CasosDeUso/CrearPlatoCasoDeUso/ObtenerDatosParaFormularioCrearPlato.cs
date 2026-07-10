using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.CrearPlatoCasoDeUso
{
    public class ObtenerDatosParaFormularioCrearPlato
    {
        private readonly IFormularioPlatoRepositorio _repositorio;
        private readonly IPorcentajesCategoriaRepositorio _porcentajesRepositorio;
        private readonly IUltimoPrecioCompraInsumoServicio _ultimoPrecioCompraServicio;

        public ObtenerDatosParaFormularioCrearPlato(
            IFormularioPlatoRepositorio repositorio,
            IPorcentajesCategoriaRepositorio porcentajesRepositorio,
            IUltimoPrecioCompraInsumoServicio ultimoPrecioCompraServicio)
        {
            _repositorio = repositorio;
            _porcentajesRepositorio = porcentajesRepositorio;
            _ultimoPrecioCompraServicio = ultimoPrecioCompraServicio;
        }

        public async Task < DatosFormularioCrearPlato> Ejecutar (int restauranteId)
        {
            var ingredientes = await _repositorio.ObtenerIngredientesBaseAsync(restauranteId);
            foreach (var ingrediente in ingredientes)
            {
                ingrediente.CostoUnitario = _ultimoPrecioCompraServicio.ObtenerUltimoPrecioCompraRecibido(ingrediente.PedidoInsumos);
            }

            var datos = new DatosFormularioCrearPlato
            {
                TiposPlato = await _repositorio.ObtenerTiposPlatoAsync(),
                CategoriasPlato = await _repositorio.ObtenerCategoriasPlatoAsync(),
                Restricciones = await _repositorio.ObtenerRestriccionesAsync(),
                Ingredientes = ingredientes,
                IngredientePreparados = await _repositorio.ObtenerIngredientesPreparadosAsync(restauranteId),
                Porcentajes = await _porcentajesRepositorio.ObtenerPorcentajesGananciaAsync(restauranteId)
            };
            return datos;
        }

    }
}
