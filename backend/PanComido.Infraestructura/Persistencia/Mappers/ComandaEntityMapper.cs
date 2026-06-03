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
                PagoID = efComanda.PagoId,
                CantComensales = efComanda.CantComensales,
                HoraInicio = efComanda.HoraInicio,
                HoraFin = efComanda.HoraFin,
                HoraUltimoCambioEstado = efComanda.HoraUltimoCambioEstado,
                Estado = (EstadoComanda)efComanda.EstadoComandaId,

                Items = new List<DOM.ArticuloComanda>(),
                MozoId = efComanda.Mesa?.Mozos.FirstOrDefault()?.IdEmpleado
            };

            if (efComanda.ArticuloComanda != null)
            {
                foreach (var relacion in efComanda.ArticuloComanda)
                {
                    if (relacion.Articulo != null)
                    {
                        comandaDominio.Items.Add(new DOM.ArticuloComanda
                        {
                            Id = relacion.Id,
                            Cantidad = relacion.Cantidad,
                            ObservacionesGenerales = relacion.ObservacionesGenerales,
                            ObservacionesIngredientes = relacion.ObservacionesIngrediente,
                            Entregado = relacion.Entregado,

                            Articulo = _articuloMapper.paraDominio(relacion.Articulo)
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
               HoraUltimoCambioEstado = comandaDominio.HoraUltimoCambioEstado,
               PagoId = comandaDominio.PagoID,
            };
        }
    }
}