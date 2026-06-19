using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Presentacion.DTOs.Articulo;
using PanComido.Presentacion.DTOs.Cliente;
using PanComido.Presentacion.DTOs.Articulo;
using PanComido.Presentacion.DTOs.Comanda;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class ComandaMapper
    {
        //Para una sola comanda 
        public ComandaResponseDto ComandaResponseDto(DOM.Comanda comanda)
        {
            return new ComandaResponseDto
            {
                Id = comanda.Id,
                MesaId = comanda.MesaId,
                CantComensales = comanda.CantComensales,
                Estado = comanda.Estado.ToString(),
                HoraInicio = comanda.HoraInicio.ToString("dd/MM/yyyy HH:mm"),
                HoraFin = comanda.HoraFin?.ToString("dd/MM/yyyy HH:mm"),
                  HoraUltimoCambioEstado = comanda.HoraUltimoCambioEstado?.ToString("O"),

               TiempoEstimadoTotal = comanda.Items
                    .Select(i => i.Articulo)
                    .OfType<Plato>()
                    .Select(plato => plato.TiempoPreparacionBase)
                    .DefaultIfEmpty(0)
                    .Max(),

                Items = comanda.Items.Select(ac => new ArticuloComandaResponseDto
                {
                    Id = ac.Id,
                    Entregado = ac.Entregado,
                    Cantidad = ac.Cantidad,
                    ObservacionesGenerales = ac.ObservacionesGenerales,
                    ObservacionesIngredientes = ac.IngredientesExcluidos != null 
                                                    && ac.IngredientesExcluidos.Any()
                        ? ac.IngredientesExcluidos.Select(i => $"- Sin {i.Nombre}").ToList()
                        : new List<string>(),
                    Articulo = new ArticuloResponseDto
                    {
                        Id = ac.Articulo.Id,
                        Nombre = ac.Articulo.Nombre,
                    }
                }).ToList()
            };
        }

        public List<ComandaResponseDto> ComandaResponseDtoList(List<DOM.Comanda> comandas)
        {
            return comandas.Select(c => ComandaResponseDto(c)).ToList();
        }

        public List<DOM.ArticuloComanda> ParaListaArticuloComandaDominio(ConfirmarPedidoClienteRequestDto dto)
        {
            if (dto == null || dto.Items == null)
                return new List<DOM.ArticuloComanda>();

            return dto.Items.Select(item => new DOM.ArticuloComanda
            {
                ArticuloId = item.ArticuloId,
                Cantidad = item.Cantidad,
                ObservacionesGenerales = item.ObservacionesGenerales,
                Entregado = false,
                NombreComensal = dto.NombreComensal,
                IngredientesExcluidosIds = item.IdIngredientesPersonalizadosSacados ?? new List<int>()

            }).ToList();
        }

        public ComandaClienteEstadoResponseDto ParaEstadoClienteDto(DOM.Comanda comanda)
        {
            return new ComandaClienteEstadoResponseDto
            {
                ComandaId = comanda.Id,
                EstadoUI = TraducirEstadoParaUI(comanda.Estado),
                TotalAPagar = comanda.Items?.Sum(i => (i.Articulo?.PrecioVentaFinal ?? 0m) * i.Cantidad) ?? 0m,

                Items = comanda.Items?.Select(ac => new ItemPedidoClienteResponseDto
                {
                    ArticuloId = ac.Articulo.Id,
                    Nombre = ac.Articulo.Nombre,
                    Cantidad = ac.Cantidad,
                    Entregado = ac.Entregado,
                    PrecioUnitario = ac.Articulo.PrecioVentaFinal ?? 0m,
                    Subtotal = (ac.Articulo.PrecioVentaFinal ?? 0m) * ac.Cantidad,
                    ObservacionesGenerales = ac.ObservacionesGenerales,
                    NombreComensal = ac.NombreComensal,
                    ObservacionesIngredientes = ac.IngredientesExcluidos != null
                                                    && ac.IngredientesExcluidos.Any()
                        ? ac.IngredientesExcluidos.Select(i => $"- Sin {i.Nombre}").ToList()
                        : new List<string>(),
                }).ToList() ?? new List<ItemPedidoClienteResponseDto>()
            };
        }

        private string TraducirEstadoParaUI(EstadoComanda estado)
        {
            return estado switch
            {
                EstadoComanda.Nueva => "Recibido",
                EstadoComanda.EnPreparacion => "Preparación",
                EstadoComanda.EnEspera => "Listo",
                EstadoComanda.Abierta => "Esperando pedido",
                _ => "Esperando pedido"
            };
        }

        public BienvenidaDatosInvitadoComandaResponseDto aInvitadoBienvenidaComandaDto(BienvenidaDatosInvitadoComanda datosDominio)
        {
            return new BienvenidaDatosInvitadoComandaResponseDto
            {
                ComandaId = datosDominio.IdComanda,
                IdMesa = datosDominio.Mesa.Id,
                NumeroMesa = datosDominio.Mesa.Numero,
                CantComensales = datosDominio.CantComensales,
                RestauranteId = datosDominio.RestauranteDatos.Id,
                NombreRestaurante = datosDominio.RestauranteDatos.Nombre,
                LogoUrl = datosDominio.RestauranteDatos.Imagen,
                ColorPrincipal = datosDominio.RestauranteDatos.ColorPrincipal,
                ColorSecundario = datosDominio.RestauranteDatos.ColorSecundario,
                TipografiaTitulo = datosDominio.RestauranteDatos.FamiliaTipografica.TipografiaTitulo,
                TipografiaCuerpo = datosDominio.RestauranteDatos.FamiliaTipografica.TipografiaCuerpo
            };
        }
    }
}
