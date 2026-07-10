using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Infraestructura.Persistencia.Entidades;
using System.Collections.Generic;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class ComandaEntityMapper
    {
        private readonly ArticuloEntityMapper _articuloMapper;
        public ComandaEntityMapper(ArticuloEntityMapper articuloMapper)
        {
            _articuloMapper = articuloMapper;
        }

        public DOM.Comanda ParaDominio(EF.Comandum efComanda)
        {
            if (efComanda == null) return null;

            var comandaDominio = new DOM.Comanda
            {
                Id = efComanda.Id,
                MesaId = efComanda.MesaId,
                RestauranteId = efComanda.RestauranteId,
                //PagoID = efComanda.PagoId,
                CantComensales = efComanda.CantComensales,
                HoraInicio = efComanda.HoraInicio,
                HoraFin = efComanda.HoraFin,
                HoraUltimoCambioEstado = efComanda.HoraUltimoCambioEstado,
                Estado = (EstadoComanda)efComanda.EstadoComandaId,

                Items = new List<DOM.ArticuloComanda>(),
                MozoId = efComanda.Mesa?.Mozos.FirstOrDefault()?.IdEmpleado
            };

            if(efComanda.Mesa != null)
            {
                comandaDominio.NumeroDeMesa = efComanda.Mesa.Numero;
            }

            if (efComanda.ArticuloComanda != null)
            {
                foreach (var relacion in efComanda.ArticuloComanda)
                {
                    if (relacion.Articulo != null)
                    {
                        var idsExcluidos = relacion.ArticuloComandaIngredienteExcluidos?
                            .Select(i => i.IngredienteId).ToList() ?? new List<int>();

                        var entidadesExcluidas = new List<DOM.Articulo>();
                        if (relacion.ArticuloComandaIngredienteExcluidos != null)
                        {
                            foreach (var efExcluido in relacion.ArticuloComandaIngredienteExcluidos)
                            {
                                var efArticuloDelInsumo = efExcluido.Ingrediente?.IdInsumoNavigation?.IdArticuloNavigation;
                                if (efArticuloDelInsumo != null)
                                    entidadesExcluidas.Add(_articuloMapper.paraDominio(efArticuloDelInsumo));
                            }
                        }

                        comandaDominio.Items.Add(new DOM.ArticuloComanda
                        {
                            Id = relacion.Id,
                            ArticuloId = relacion.ArticuloId,
                            Cantidad = relacion.Cantidad,
                            ObservacionesGenerales = relacion.ObservacionesGenerales,
                            Entregado = relacion.Entregado,
                            NombreComensal = relacion.NombreComensal,
                            Articulo = _articuloMapper.paraDominio(relacion.Articulo),

                            IngredientesExcluidosIds = idsExcluidos,
                            IngredientesExcluidos = entidadesExcluidas
                        });
                    }
                }
            }
            return comandaDominio;
        }

        public Comandum paraEntidad(DOM.Comanda comandaDominio)
        {
            return new EF.Comandum
            {
                Id = comandaDominio.Id,
                MesaId = comandaDominio.MesaId,
                RestauranteId = comandaDominio.RestauranteId,
                EstadoComandaId = (int)comandaDominio.Estado,
                CantComensales = comandaDominio.CantComensales,
                HoraInicio = comandaDominio.HoraInicio,
                HoraFin = comandaDominio.HoraFin,
                HoraUltimoCambioEstado = comandaDominio.HoraUltimoCambioEstado ?? DateTime.Now,
                //PagoId = comandaDominio.PagoID,

                ArticuloComanda = comandaDominio.Items?.Select(item => new EF.ArticuloComandum
                {
                    Id = item.Id,
                    ArticuloId = item.ArticuloId,
                    Cantidad = item.Cantidad,
                    Entregado = item.Entregado,
                    ObservacionesGenerales = item.ObservacionesGenerales,
                    ComandaId = comandaDominio.Id,
                    NombreComensal = item.NombreComensal,

                    ArticuloComandaIngredienteExcluidos = item.Id == 0
                        ? item.IngredientesExcluidosIds?.Select(id => new EF.ArticuloComandaIngredienteExcluido
                        {
                            IngredienteId = id
                        }).ToList()
                        : null

                }).ToList() ?? new List<EF.ArticuloComandum>()
            };
        }
    }
}