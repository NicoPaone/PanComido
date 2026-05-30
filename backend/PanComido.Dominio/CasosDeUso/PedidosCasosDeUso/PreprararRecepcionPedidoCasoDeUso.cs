using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.CasosDeUso.PedidosCasosDeUso
{
    public class PreprararRecepcionPedidoCasoDeUso
    {
        private readonly IPedidoRepositorio _pedidoRepositorio;
        private readonly IBodegaRepositorio _bodegaRepositorio;

        public PreprararRecepcionPedidoCasoDeUso(IPedidoRepositorio pedidoRepositorio, IBodegaRepositorio bodegaRepositorio)
        {
            _pedidoRepositorio = pedidoRepositorio;
            _bodegaRepositorio = bodegaRepositorio;
        }

        public async Task<List<DOM.RecepcionItemSugerido>> EjecutarAsync(int pedidoId)
        {
            DOM.Pedido pedidoExistente = await _pedidoRepositorio.ObtenerPedidoPorIdAsync(pedidoId);
            if (pedidoExistente == null) throw new KeyNotFoundException("Pedido no encontrado");
            if (pedidoExistente.Estado != "Enviado") throw new InvalidOperationException("Solo se pueden recibir pedidos en estado Enviado");

            var sugerencias = new List<RecepcionItemSugerido>();

            foreach (var item in pedidoExistente.ItemsInsumo)
            {
                sugerencias.Add(new RecepcionItemSugerido
                {
                    InsumoId = item.InsumoId,
                    NombreInsumo = item.NombreInsumo,
                    Cantidad = item.Cantidad,
                    NombreLote = SugerenciaNombreLote(item.NombreInsumo),
                    BodegaIdSug = SugerenciaBodegaId(item.CateoriaInsumoId),
                    FechaVencimientoSug = SugerirFechaVencimiento(item.CateoriaInsumoId)
                });

            }

            return (sugerencias);
        }

        private string SugerenciaNombreLote(string nombreInsumo)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            return $"{nombreInsumo}-{timestamp}";
        }

        private int SugerenciaBodegaId(int categoriaInsumoId)
        {
            return categoriaInsumoId switch
            {
                1 or 2  or 4 or 7 => 2, // frio
                3 or 6 => 3, // congelados
                _ => 1 //almacen
            };

        }
        private DateOnly SugerirFechaVencimiento(int categoriaInsumoId)
        {
            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
            return categoriaInsumoId switch
            {
                1 or 2 => hoy.AddDays(7),    // Fruta, Verdura
                3 => hoy.AddDays(5),         // Carne
                4 => hoy.AddDays(14),        // Lácteos
                6 => hoy.AddDays(4),         // Pescado y Mariscos
                11 => hoy.AddDays(90),       // Harinas y Panificados
                _ => hoy.AddDays(30)         // Resto
            };
        }
    }
}
