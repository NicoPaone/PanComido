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
                    ObservacionesIngredientes = ac.ObservacionesIngredientes,
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
                ObservacionesIngredientes = item.ObservacionesIngredientes,
                ObservacionesGenerales = item.ObservacionesGenerales,
                Entregado = false,

                NombreComensal = dto.NombreComensal
            }).ToList();
        }

        public ComandaClienteEstadoResponseDto ParaEstadoClienteDto(DOM.Comanda comanda)
        {
            return new ComandaClienteEstadoResponseDto
            {
                ComandaId = comanda.Id,
                EstadoUI = TraducirEstadoParaUI(comanda.Estado),
                TotalAPagar = comanda.Items?.Sum(i => (i.Articulo?.PrecioVentaFinal ?? 0m) * i.Cantidad) ?? 0m,

                // Mapeamos los ítems con su precio para que Angular dibuje el ticket
                Items = comanda.Items?.Select(ac => new ItemPedidoClienteResponseDto
                {
                    ArticuloId = ac.Articulo.Id,
                    Nombre = ac.Articulo.Nombre,
                    Cantidad = ac.Cantidad,
                    Entregado = ac.Entregado,
                    PrecioUnitario = ac.Articulo.PrecioVentaFinal ?? 0m,
                    Subtotal = (ac.Articulo.PrecioVentaFinal ?? 0m) * ac.Cantidad,

                    ObservacionesIngredientes = ac.ObservacionesIngredientes,
                    ObservacionesGenerales = ac.ObservacionesGenerales,

                    NombreComensal = ac.NombreComensal
                }).ToList() ?? new List<ItemPedidoClienteResponseDto>()
            };
        }

        // Método privado de ayuda para traducir los estados que va a tener la vista
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



    }
}
