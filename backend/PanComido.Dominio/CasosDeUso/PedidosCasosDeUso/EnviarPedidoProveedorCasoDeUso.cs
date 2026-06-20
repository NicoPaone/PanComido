using Microsoft.Extensions.Logging;
using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PedidosCasosDeUso
{
    public class EnviarPedidoProveedorCasoDeUso
    {
        private readonly IPedidoRepositorio _pedidoRepositorio;
        private readonly ILogger<EnviarPedidoProveedorCasoDeUso> _logger;

        public EnviarPedidoProveedorCasoDeUso(IPedidoRepositorio pedidoRepositorio, ILogger<EnviarPedidoProveedorCasoDeUso> logger)
        {
            _pedidoRepositorio = pedidoRepositorio;
            _logger = logger;
        }

        public async Task<(DOM.Pedido pedido, string linkWpp)> EjecutarAsync(int pedidoId, List<DOM.PedidoInsumo> itemsNuevos)
        {
            DOM.Pedido pedidoExistente = await _pedidoRepositorio.ObtenerPedidoPorIdAsync(pedidoId);
            if (pedidoExistente == null) throw new KeyNotFoundException("Pedido no encontrado");
            if (pedidoExistente.Estado != "Pendiente") throw new InvalidOperationException("Solo se pueden confirmar pedidos en estado Pendiente");
            if (itemsNuevos == null || itemsNuevos.Count == 0) throw new ArgumentException("El pedido debe contener al menos un item");

            DOM.Pedido pedidoConfirmado = await _pedidoRepositorio.EnviarPedidoAsync(pedidoId, itemsNuevos);

            // msj wpp
            var sb = new StringBuilder();
            sb.AppendLine($"Hola {pedidoConfirmado.ProveedorNombre}, le hago el siguiente pedido:");
            sb.AppendLine();
            foreach (var item in pedidoConfirmado.ItemsInsumo)
            {
                sb.AppendLine($"- {item.Cantidad} {item.UnidadMedida} de {item.NombreInsumo}");
            }
            sb.AppendLine();
            sb.Append("Muchas gracias, saludos.");

            string mensajeEncodeado = Uri.EscapeDataString(sb.ToString());
            string linkWpp = $"https://wa.me/{pedidoConfirmado.ProveedorTelefono}?text={mensajeEncodeado}";

            _logger.LogInformation("Pedido enviado a proveedor. PedidoId: {PedidoId}, ProveedorNombre: {ProveedorNombre}", pedidoId, pedidoConfirmado.ProveedorNombre);
            return (pedidoConfirmado, linkWpp);
        }
    }
}