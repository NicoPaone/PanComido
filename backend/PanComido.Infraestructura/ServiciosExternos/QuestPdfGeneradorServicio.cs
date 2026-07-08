using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.ValueObjects;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PanComido.Infraestructura.ServiciosExternos
{
    public class QuestPdfGeneradorServicio : IPdfGeneradorServicio
    {
        private const string Rojo = "#D8081C";
        private const string RojoOscuro = "#A8041A";
        private const string Mozo = "#02596C";
        private const string Cocina = "#F08F1A";
        private const string Activo = "#6ABF3F";
        private const string Negro = "#0F172A";
        private const string GrisOscuro = "#475569";
        private const string GrisMedio = "#64748B";
        private const string GrisClaro = "#E2E8F0";
        private const string GrisFondo = "#F8FAFC";
        private const string Blanco = "#FFFFFF";

        private const string FontPrincipal = Fonts.Arial;
        private const float MargenPagina = 36f;

        public byte[] GenerarReporteDashboard(
            ResumenOperativo resumen, 
            ResumenRendimientoComercial rendimiento, 
            List<Insumo> criticidad, 
            DateTime desde, 
            DateTime hasta)
        {
            using (var stream = new MemoryStream())
            {
                var platosMasVendidos = rendimiento.MasVendidos
                    .OrderByDescending(p => p.UnidadesVendidas)
                    .ThenByDescending(p => p.FacturacionTotal)
                    .ToList();
                var platosMenosVendidos = rendimiento.MenosVendidos
                    .OrderBy(p => p.UnidadesVendidas)
                    .ThenBy(p => p.FacturacionTotal)
                    .ToList();
                var insumosCriticos = (criticidad ?? new List<Insumo>())
                    .OrderBy(i => i.StockActual)
                    .ThenBy(i => i.Nombre)
                    .ToList();
                var facturacionPlatos = platosMasVendidos.Sum(p => p.FacturacionTotal);
                var unidadesPlatos = platosMasVendidos.Sum(p => p.UnidadesVendidas);
                var topPlato = platosMasVendidos.FirstOrDefault();
                var top3Facturacion = platosMasVendidos.Take(3).Sum(p => p.FacturacionTotal);
                var insumosSinLotes = insumosCriticos.Count(i => i.Lotes == null || !i.Lotes.Any());
                var insumosBajoMinimo = insumosCriticos.Count(i => i.StockActual <= i.StockMinimo);
                var accionesDashboard = ConstruirAccionesDashboard(resumen, platosMasVendidos, platosMenosVendidos, insumosCriticos);

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        ConfigurarPaginaComun(page, "Reporte Ejecutivo del Dashboard", desde, hasta);

                        page.Content().Column(col =>
                        {
                            col.Spacing(16);

                            col.Item().Row(row =>
                            {
                                row.Spacing(8);
                                row.RelativeItem().Component(new MetricCard("Ventas totales", FormatearMoneda(resumen.TotalVentas), FormatearVariacion(resumen.VariacionVentas)));
                                row.RelativeItem().Component(new MetricCard("Pedidos", resumen.TotalPedidos.ToString("N0", CulturaArgentina), FormatearVariacion(resumen.VariacionPedidos)));
                                row.RelativeItem().Component(new MetricCard("Ticket promedio", FormatearMoneda(resumen.TicketPromedio), FormatearVariacion(resumen.VariacionTicket)));
                                row.RelativeItem().Component(new SummaryCard("Promedio diario", resumen.PromedioDiarioPedidos.ToString("N0", CulturaArgentina), "Pedidos por día", Mozo));
                            });

                            col.Item().Row(row =>
                            {
                                row.Spacing(8);

                                row.RelativeItem().Element(CardStyle).Column(card =>
                                {
                                    card.Spacing(7);
                                    card.Item().Text("Diagnóstico comercial").FontSize(13).Bold().FontColor(Negro);
                                    card.Item().Text($"Plato líder: {(topPlato != null ? topPlato.Nombre : "Sin ventas registradas")}").FontSize(10).FontColor(Negro).Bold();
                                    card.Item().Text($"Top 3 explica {Porcentaje(top3Facturacion, facturacionPlatos)} de la facturación de platos reportada.").FontSize(9).FontColor(GrisOscuro);
                                    card.Item().Text($"Unidades analizadas: {unidadesPlatos.ToString("N0", CulturaArgentina)}.").FontSize(9).FontColor(GrisOscuro);
                                });

                                row.RelativeItem().Element(CardStyle).Column(card =>
                                {
                                    card.Spacing(7);
                                    card.Item().Text("Riesgo operativo").FontSize(13).Bold().FontColor(Negro);
                                    card.Item().Text($"{insumosCriticos.Count.ToString("N0", CulturaArgentina)} insumo(s) críticos reportados").FontSize(10).FontColor(insumosCriticos.Any() ? Rojo : Activo).Bold();
                                    card.Item().Text($"{insumosBajoMinimo.ToString("N0", CulturaArgentina)} bajo mínimo o en mínimo").FontSize(9).FontColor(GrisOscuro);
                                    card.Item().Text($"{insumosSinLotes.ToString("N0", CulturaArgentina)} sin lotes activos").FontSize(9).FontColor(GrisOscuro);
                                });
                            });

                            col.Item().Element(CardStyle).Column(card =>
                            {
                                card.Spacing(6);
                                card.Item().Text("Acciones sugeridas").FontSize(13).Bold().FontColor(Negro);

                                foreach (var accion in accionesDashboard)
                                {
                                    card.Item().Text($"- {accion}").FontSize(9).FontColor(GrisOscuro);
                                }
                            });

                            col.Item().Element(SectionTitle).Text("Rendimiento de Platos");
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderStyle).Text("Plato");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Unidades");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Total facturado");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Participación");
                                });

                                bool alternate = false;
                                foreach (var plato in platosMasVendidos.Take(10))
                                {
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(plato.Nombre);
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(plato.UnidadesVendidas.ToString());
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(FormatearMoneda(plato.FacturacionTotal));
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(Porcentaje(plato.FacturacionTotal, facturacionPlatos)).Bold();
                                    alternate = !alternate;
                                }
                            });

                            if (platosMenosVendidos.Any())
                            {
                                col.Item().Element(SectionTitle).Text("Platos de baja rotación");
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1.5f);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderStyle).Text("Plato");
                                        header.Cell().Element(HeaderStyle).AlignRight().Text("Unidades");
                                        header.Cell().Element(HeaderStyle).AlignRight().Text("Total facturado");
                                    });

                                    bool alternate = false;
                                    foreach (var plato in platosMenosVendidos.Take(5))
                                    {
                                        table.Cell().Element(c => RowStyle(c, alternate)).Text(plato.Nombre);
                                        table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(plato.UnidadesVendidas.ToString("N0", CulturaArgentina));
                                        table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(FormatearMoneda(plato.FacturacionTotal));
                                        alternate = !alternate;
                                    }
                                });
                            }

                            if (insumosCriticos.Any())
                            {
                                col.Item().PageBreak();
                                col.Item().Element(SectionTitle).Text("Alertas de Stock y Vencimientos");
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1.5f);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderStyle).Text("Insumo");
                                        header.Cell().Element(HeaderStyle).Text("Unidad");
                                        header.Cell().Element(HeaderStyle).AlignRight().Text("Stock actual");
                                        header.Cell().Element(HeaderStyle).AlignRight().Text("Stock mínimo");
                                        header.Cell().Element(HeaderStyle).AlignRight().Text("Lotes activos");
                                    });

                                    bool alternate = false;
                                    foreach (var insumo in insumosCriticos.Take(10))
                                    {
                                        table.Cell().Element(c => RowStyle(c, alternate)).Text(insumo.Nombre);
                                        table.Cell().Element(c => RowStyle(c, alternate)).Text(insumo.UnidadMedida ?? "");
                                        table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(insumo.StockActual.ToString("N1", CulturaArgentina)).FontColor(insumo.StockActual <= insumo.StockMinimo ? Rojo : Negro).Bold();
                                        table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(insumo.StockMinimo.ToString());
                                        table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text((insumo.Lotes?.Count ?? 0).ToString());
                                        alternate = !alternate;
                                    }
                                });
                            }
                        });
                    });
                }).GeneratePdf(stream);

                return stream.ToArray();
            }
        }

        public byte[] GenerarReportePersonal(List<Empleado> empleados)
        {
            using (var stream = new MemoryStream())
            {
                var empleadosOrdenados = empleados
                    .OrderBy(e => OrdenRol(e.Rol))
                    .ThenBy(e => e.Nombre)
                    .ToList();
                var totalEmpleados = empleadosOrdenados.Count;
                var activos = empleadosOrdenados.Count(e => EsActivo(e.Estado));
                var sinTurno = empleadosOrdenados.Count(e => !TieneTurnos(e));
                var dobleTurno = empleadosOrdenados.Count(e => CantidadTurnos(e) > 1);
                var roles = empleadosOrdenados
                    .GroupBy(e => string.IsNullOrWhiteSpace(e.Rol) ? "Sin rol" : e.Rol)
                    .OrderBy(g => OrdenRol(g.Key))
                    .ThenBy(g => g.Key)
                    .ToList();
                var turnos = empleadosOrdenados
                    .SelectMany(e => (e.Turnos ?? new List<TurnoLaboral>())
                        .Select(t => new TurnoEmpleadoInfo
                        {
                            Turno = t,
                            Empleado = e
                        }))
                    .GroupBy(x => FormatearTurno(x.Turno))
                    .OrderBy(g => g.Min(x => x.Turno.HorarioInicio))
                    .ToList();
                var alertas = ConstruirAlertasPersonal(empleadosOrdenados, turnos);

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        ConfigurarPaginaComun(page, "Reporte de Personal y Turnos", null, null);

                        page.Content().Column(col =>
                        {
                            col.Spacing(16);

                            col.Item().Row(row =>
                            {
                                row.Spacing(8);
                                row.RelativeItem().Component(new SummaryCard("Total empleados", totalEmpleados.ToString("N0", CulturaArgentina), "Dotación registrada", Negro));
                                row.RelativeItem().Component(new SummaryCard("Activos", activos.ToString("N0", CulturaArgentina), $"{Porcentaje(activos, totalEmpleados)} de la dotación", Activo));
                                row.RelativeItem().Component(new SummaryCard("Sin turno", sinTurno.ToString("N0", CulturaArgentina), "Requieren asignación", sinTurno > 0 ? Rojo : Activo));
                                row.RelativeItem().Component(new SummaryCard("Doble turno", dobleTurno.ToString("N0", CulturaArgentina), "Carga extendida", dobleTurno > 0 ? Cocina : GrisMedio));
                            });

                            col.Item().Row(row =>
                            {
                                row.Spacing(8);

                                row.RelativeItem().Element(CardStyle).Column(card =>
                                {
                                    card.Spacing(7);
                                    card.Item().Text("Distribución por rol").FontSize(13).Bold().FontColor(Negro);

                                    foreach (var rol in roles)
                                    {
                                        card.Item().Row(item =>
                                        {
                                            item.RelativeItem().Text(rol.Key).FontSize(10).FontColor(ColorRol(rol.Key)).Bold();
                                            item.ConstantItem(42).AlignRight().Text(rol.Count().ToString("N0", CulturaArgentina)).FontSize(10).FontColor(Negro).Bold();
                                        });
                                    }
                                });

                                row.RelativeItem().Element(CardStyle).Column(card =>
                                {
                                    card.Spacing(7);
                                    card.Item().Text("Cobertura por turno").FontSize(13).Bold().FontColor(Negro);

                                    if (turnos.Any())
                                    {
                                        foreach (var turno in turnos)
                                        {
                                            card.Item().Row(item =>
                                            {
                                                item.RelativeItem().Text(turno.Key).FontSize(10).FontColor(GrisOscuro);
                                                item.ConstantItem(72).AlignRight().Text($"{turno.Select(x => x.Empleado.Id).Distinct().Count()} personas").FontSize(10).FontColor(Negro).Bold();
                                            });
                                        }
                                    }
                                    else
                                    {
                                        card.Item().Text("No hay turnos asignados.").FontSize(10).FontColor(GrisMedio);
                                    }
                                });
                            });

                            col.Item().Element(CardStyle).Column(card =>
                            {
                                card.Spacing(6);
                                card.Item().Text("Alertas operativas").FontSize(13).Bold().FontColor(Negro);

                                foreach (var alerta in alertas)
                                {
                                    card.Item().Text($"- {alerta}").FontSize(9).FontColor(GrisOscuro);
                                }
                            });

                            col.Item().Element(SectionTitle).Text("Detalle de carga laboral");
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2.2f);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2.3f);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1.8f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderStyle).Text("Empleado");
                                    header.Cell().Element(HeaderStyle).Text("Rol");
                                    header.Cell().Element(HeaderStyle).Text("Estado");
                                    header.Cell().Element(HeaderStyle).Text("Turnos laborales");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Carga");
                                    header.Cell().Element(HeaderStyle).Text("Observación");
                                });

                                bool alternate = false;
                                foreach (var emp in empleadosOrdenados)
                                {
                                    table.Cell().Element(c => RowStyle(c, alternate)).Column(cell =>
                                    {
                                        cell.Item().Text(emp.Nombre).Bold();
                                        cell.Item().Text(emp.Email).FontSize(8).FontColor(GrisMedio);
                                    });
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(emp.Rol).FontColor(ColorRol(emp.Rol)).Bold();
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(FormatearEstado(emp.Estado)).FontColor(ColorEstado(emp.Estado)).Bold();
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(FormatearTurnos(emp));
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(FormatearHoras(CalcularHoras(emp))).Bold();
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(ObservacionPersonal(emp)).FontColor(ColorObservacionPersonal(emp)).Bold();
                                    alternate = !alternate;
                                }
                            });
                        });
                    });
                }).GeneratePdf(stream);

                return stream.ToArray();
            }
        }

        public byte[] GenerarReporteVentas(List<VentaReporteDetalle> ventas, DateTime desde, DateTime hasta)
        {
            using (var stream = new MemoryStream())
            {
                var ventasOrdenadas = ventas.OrderBy(v => v.FechaHora).ThenBy(v => v.ComandaId).ToList();
                var totalFacturado = ventasOrdenadas.Sum(v => v.Total);
                var totalComandas = ventasOrdenadas.Count;
                var totalArticulos = ventasOrdenadas.Sum(v => v.CantidadArticulos);
                var ticketPromedio = totalComandas == 0 ? 0 : totalFacturado / totalComandas;
                var articulosPromedio = totalComandas == 0 ? 0 : (decimal)totalArticulos / totalComandas;
                var ventasPorMetodo = ventasOrdenadas
                    .GroupBy(v => string.IsNullOrWhiteSpace(v.MetodoPago) ? "Sin método" : v.MetodoPago)
                    .Select(g => new AgrupacionVenta(g.Key, g.Count(), g.Sum(v => v.CantidadArticulos), g.Sum(v => v.Total)))
                    .OrderByDescending(g => g.Total)
                    .ToList();
                var ventasPorDia = ventasOrdenadas
                    .GroupBy(v => v.FechaHora.Date)
                    .Select(g => new AgrupacionVenta(g.Key.ToString("dd/MM/yyyy"), g.Count(), g.Sum(v => v.CantidadArticulos), g.Sum(v => v.Total)))
                    .OrderByDescending(g => g.Total)
                    .ToList();
                var ventasPorMesa = ventasOrdenadas
                    .GroupBy(v => v.NumeroMesa)
                    .Select(g => new AgrupacionVenta($"Mesa {g.Key}", g.Count(), g.Sum(v => v.CantidadArticulos), g.Sum(v => v.Total)))
                    .OrderByDescending(g => g.Total)
                    .ToList();
                var mejorDia = ventasPorDia.FirstOrDefault();
                var mejorMesa = ventasPorMesa.FirstOrDefault();

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        ConfigurarPaginaComun(page, "Reporte Detallado de Ventas", desde, hasta);

                        page.Content().Column(col =>
                        {
                            col.Spacing(16);

                            col.Item().Row(row =>
                            {
                                row.Spacing(8);
                                row.RelativeItem().Component(new SummaryCard("Facturación", FormatearMoneda(totalFacturado), "Total del período", Negro));
                                row.RelativeItem().Component(new SummaryCard("Comandas", totalComandas.ToString("N0", CulturaArgentina), "Operaciones cerradas", Mozo));
                                row.RelativeItem().Component(new SummaryCard("Ticket promedio", FormatearMoneda(ticketPromedio), "Por comanda", Activo));
                                row.RelativeItem().Component(new SummaryCard("Artículos", totalArticulos.ToString("N0", CulturaArgentina), $"{articulosPromedio:0.#} por comanda", Cocina));
                            });

                            col.Item().Row(row =>
                            {
                                row.Spacing(8);

                                row.RelativeItem().Element(CardStyle).Column(card =>
                                {
                                    card.Spacing(7);
                                    card.Item().Text("Métodos de pago").FontSize(13).Bold().FontColor(Negro);

                                    if (ventasPorMetodo.Any())
                                    {
                                        foreach (var metodo in ventasPorMetodo.Take(5))
                                        {
                                            card.Item().Row(item =>
                                            {
                                                item.RelativeItem().Text(metodo.Nombre).FontSize(9).FontColor(GrisOscuro);
                                                item.ConstantItem(72).AlignRight().Text(FormatearMoneda(metodo.Total)).FontSize(9).FontColor(Negro).Bold();
                                                item.ConstantItem(48).AlignRight().Text(Porcentaje(metodo.Total, totalFacturado)).FontSize(9).FontColor(GrisMedio);
                                            });
                                        }
                                    }
                                    else
                                    {
                                        card.Item().Text("No hay ventas registradas en el período.").FontSize(10).FontColor(GrisMedio);
                                    }
                                });

                                row.RelativeItem().Element(CardStyle).Column(card =>
                                {
                                    card.Spacing(7);
                                    card.Item().Text("Picos del período").FontSize(13).Bold().FontColor(Negro);
                                    card.Item().Text($"Día con mayor facturación: {(mejorDia != null ? mejorDia.Nombre : "Sin datos")}").FontSize(9).FontColor(Negro).Bold();
                                    card.Item().Text(mejorDia != null ? FormatearMoneda(mejorDia.Total) : FormatearMoneda(0)).FontSize(11).FontColor(Activo).Bold();
                                    card.Item().Text($"Mesa líder: {(mejorMesa != null ? mejorMesa.Nombre : "Sin datos")}").FontSize(9).FontColor(Negro).Bold();
                                    card.Item().Text(mejorMesa != null ? $"{FormatearMoneda(mejorMesa.Total)} en {mejorMesa.Cantidad.ToString("N0", CulturaArgentina)} comanda(s)" : "Sin ventas").FontSize(9).FontColor(GrisOscuro);
                                });
                            });

                            col.Item().Element(SectionTitle).Text("Actividad diaria");
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderStyle).Text("Día");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Comandas");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Artículos");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Total");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Participación");
                                });

                                bool alternate = false;
                                foreach (var dia in ventasPorDia.Take(10))
                                {
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(dia.Nombre);
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(dia.Cantidad.ToString("N0", CulturaArgentina));
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(dia.Articulos.ToString("N0", CulturaArgentina));
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(FormatearMoneda(dia.Total));
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(Porcentaje(dia.Total, totalFacturado)).Bold();
                                    alternate = !alternate;
                                }
                            });

                            col.Item().Element(SectionTitle).Text("Mesas con mayor facturación");
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1.5f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderStyle).Text("Mesa");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Comandas");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Total");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Ticket prom.");
                                });

                                bool alternate = false;
                                foreach (var mesa in ventasPorMesa.Take(8))
                                {
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(mesa.Nombre);
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(mesa.Cantidad.ToString("N0", CulturaArgentina));
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(FormatearMoneda(mesa.Total));
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(FormatearMoneda(mesa.Cantidad == 0 ? 0 : mesa.Total / mesa.Cantidad));
                                    alternate = !alternate;
                                }
                            });

                            col.Item().PageBreak();
                            col.Item().Element(SectionTitle).Text("Detalle de ventas");
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderStyle).Text("Comanda ID");
                                    header.Cell().Element(HeaderStyle).Text("Mesa");
                                    header.Cell().Element(HeaderStyle).Text("Fecha y hora");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Artículos");
                                    header.Cell().Element(HeaderStyle).Text("Método de pago");
                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Total");
                                });

                                bool alternate = false;
                                foreach (var venta in ventasOrdenadas)
                                {
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(venta.ComandaId.ToString());
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text($"Mesa {venta.NumeroMesa}");
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(venta.FechaHora.ToString("dd/MM/yyyy HH:mm"));
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(venta.CantidadArticulos.ToString());
                                    table.Cell().Element(c => RowStyle(c, alternate)).Text(venta.MetodoPago);
                                    table.Cell().Element(c => RowStyle(c, alternate)).AlignRight().Text(FormatearMoneda(venta.Total));
                                    
                                    alternate = !alternate;
                                }

                                table.Cell().Element(TotalRowStyle).Text("TOTAL GENERAL").Bold();
                                table.Cell().Element(TotalRowStyle).Text("");
                                table.Cell().Element(TotalRowStyle).Text("");
                                table.Cell().Element(TotalRowStyle).AlignRight().Text(totalArticulos.ToString()).Bold();
                                table.Cell().Element(TotalRowStyle).Text("");
                                table.Cell().Element(TotalRowStyle).AlignRight().Text(FormatearMoneda(totalFacturado)).Bold();
                            });
                        });
                    });
                }).GeneratePdf(stream);

                return stream.ToArray();
            }
        }

        private void ConfigurarPaginaComun(PageDescriptor page, string titulo, DateTime? desde, DateTime? hasta)
        {
            page.Size(PageSizes.A4);
            page.Margin(MargenPagina);
            page.PageColor(Blanco);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(FontPrincipal).FontColor(Negro));

            page.Header().Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(titleCol =>
                    {
                        titleCol.Item().Text("PAN COMIDO").FontSize(7.5f).Bold().FontColor(Rojo);
                        titleCol.Item().Text(titulo).FontSize(22).Bold().FontColor(Negro);
                        titleCol.Item().PaddingTop(2).Text(ObtenerPeriodo(desde, hasta)).FontSize(10).FontColor(GrisOscuro);
                    });

                    row.ConstantItem(180).AlignRight().Column(metaCol =>
                    {
                        metaCol.Item().AlignRight().Text("PanComido").FontSize(13).Bold().FontColor(Negro);
                        metaCol.Item().AlignRight().PaddingTop(3).Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(GrisMedio);
                    });
                });

                col.Item().PaddingVertical(10).Row(row =>
                {
                    row.ConstantItem(24).Height(2).Background(Rojo);
                    row.RelativeItem().PaddingTop(0.75f).Height(0.5f).Background(GrisClaro);
                });
            });

            page.Footer().Column(col =>
            {
                col.Item().BorderTop(0.5f).BorderColor(GrisClaro).PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Text("Generado por PanComido - Sistema de Gestión de Restaurantes").FontSize(8).FontColor(GrisMedio);
                    row.ConstantItem(90).AlignRight().Text(txt =>
                    {
                        txt.Span("Página ").FontSize(8).FontColor(GrisMedio);
                        txt.CurrentPageNumber().FontSize(8).FontColor(GrisMedio);
                        txt.Span(" de ").FontSize(8).FontColor(GrisMedio);
                        txt.TotalPages().FontSize(8).FontColor(GrisMedio);
                    });
                });
            });
        }

        private static CultureInfo CulturaArgentina => CultureInfo.GetCultureInfo("es-AR");

        private IContainer HeaderStyle(IContainer container)
        {
            return container
                .Background(Negro)
                .PaddingVertical(8)
                .PaddingHorizontal(10)
                .DefaultTextStyle(x => x.FontSize(8).FontColor(Blanco).Bold());
        }

        private IContainer RowStyle(IContainer container, bool alternate)
        {
            return container
                .Background(alternate ? GrisFondo : Blanco)
                .BorderBottom(0.5f)
                .BorderColor(GrisClaro)
                .PaddingVertical(7)
                .PaddingHorizontal(10)
                .DefaultTextStyle(x => x.FontSize(10).FontColor(Negro));
        }

        private IContainer TotalRowStyle(IContainer container)
        {
            return container
                .BorderTop(1)
                .BorderColor(Negro)
                .PaddingVertical(8)
                .PaddingHorizontal(10)
                .Background(GrisClaro)
                .DefaultTextStyle(x => x.FontSize(10).FontColor(Negro));
        }

        private IContainer CardStyle(IContainer container)
        {
            return container
                .Border(0.5f)
                .BorderColor(GrisClaro)
                .Background(Blanco)
                .Padding(12);
        }

        private IContainer SectionTitle(IContainer container)
        {
            return container
                .PaddingTop(4)
                .DefaultTextStyle(x => x.FontSize(13).Bold().FontColor(Negro));
        }

        private static string ObtenerPeriodo(DateTime? desde, DateTime? hasta)
        {
            return desde.HasValue && hasta.HasValue
                ? $"Período: {desde.Value:dd/MM/yyyy} al {hasta.Value:dd/MM/yyyy}"
                : "Período: estado actual";
        }

        private static string FormatearMoneda(decimal valor)
        {
            return valor.ToString("C", CulturaArgentina);
        }

        private static string FormatearVariacion(decimal variacion)
        {
            return $"{variacion:+0.0;-0.0;0.0}% vs anterior";
        }

        private static string ColorRol(string rol)
        {
            return rol switch
            {
                RolEmpleado.Gerente => Rojo,
                RolEmpleado.Mozo => Mozo,
                RolEmpleado.Cocina => Cocina,
                _ => GrisMedio
            };
        }

        private static string ColorEstado(string estado)
        {
            return EsActivo(estado) ? Activo : RojoOscuro;
        }

        private static string FormatearEstado(string estado)
        {
            return EsActivo(estado) ? "Activo" : "Inactivo";
        }

        private static bool EsActivo(string estado)
        {
            return string.Equals(estado, EstadoEmpleado.Activo, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TieneTurnos(Empleado empleado)
        {
            return empleado.Turnos != null && empleado.Turnos.Any();
        }

        private static int CantidadTurnos(Empleado empleado)
        {
            return empleado.Turnos?.Count ?? 0;
        }

        private static decimal CalcularHoras(Empleado empleado)
        {
            if (!TieneTurnos(empleado))
            {
                return 0;
            }

            return empleado.Turnos.Sum(CalcularHorasTurno);
        }

        private static decimal CalcularHorasTurno(TurnoLaboral turno)
        {
            var inicio = turno.HorarioInicio.ToTimeSpan();
            var fin = turno.HorarioFin.ToTimeSpan();
            var duracion = fin - inicio;

            if (duracion <= TimeSpan.Zero)
            {
                duracion = duracion.Add(TimeSpan.FromHours(24));
            }

            return (decimal)duracion.TotalHours;
        }

        private static string FormatearHoras(decimal horas)
        {
            return horas == 0 ? "0 hs" : $"{horas:0.#} hs";
        }

        private static string FormatearTurno(TurnoLaboral turno)
        {
            return $"{turno.HorarioInicio:HH:mm}-{turno.HorarioFin:HH:mm}";
        }

        private static string FormatearTurnos(Empleado empleado)
        {
            return TieneTurnos(empleado)
                ? string.Join(", ", empleado.Turnos.OrderBy(t => t.HorarioInicio).Select(FormatearTurno))
                : "Sin turno asignado";
        }

        private static string ObservacionPersonal(Empleado empleado)
        {
            if (!EsActivo(empleado.Estado))
            {
                return "No operativo";
            }

            if (!TieneTurnos(empleado))
            {
                return "Sin turno";
            }

            if (CantidadTurnos(empleado) > 1)
            {
                return "Doble turno";
            }

            return "Cobertura regular";
        }

        private static string ColorObservacionPersonal(Empleado empleado)
        {
            if (!EsActivo(empleado.Estado) || !TieneTurnos(empleado))
            {
                return Rojo;
            }

            return CantidadTurnos(empleado) > 1 ? Cocina : GrisOscuro;
        }

        private static int OrdenRol(string rol)
        {
            return rol switch
            {
                RolEmpleado.Gerente => 0,
                RolEmpleado.Mozo => 1,
                RolEmpleado.Cocina => 2,
                _ => 3
            };
        }

        private static string Porcentaje(int cantidad, int total)
        {
            return total == 0 ? "0%" : $"{cantidad * 100m / total:0.#}%";
        }

        private static string Porcentaje(decimal cantidad, decimal total)
        {
            return total == 0 ? "0%" : $"{cantidad * 100m / total:0.#}%";
        }

        private static List<string> ConstruirAccionesDashboard(
            ResumenOperativo resumen,
            List<RendimientoPlato> platosMasVendidos,
            List<RendimientoPlato> platosMenosVendidos,
            List<Insumo> insumosCriticos)
        {
            var acciones = new List<string>();
            var platoLider = platosMasVendidos.FirstOrDefault();
            var platosBajaRotacion = platosMenosVendidos.Take(3).Select(p => p.Nombre).Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
            var insumosSinLotes = insumosCriticos.Where(i => i.Lotes == null || !i.Lotes.Any()).Take(3).Select(i => i.Nombre).ToList();
            var insumosBajoMinimo = insumosCriticos.Where(i => i.StockActual <= i.StockMinimo).Take(3).Select(i => i.Nombre).ToList();

            if (platoLider != null)
            {
                acciones.Add($"Asegurar disponibilidad de insumos para {platoLider.Nombre}, el plato con mayor tracción del período.");
            }

            if (platosBajaRotacion.Any())
            {
                acciones.Add($"Revisar promoción, precio o visibilidad de platos de baja rotación: {string.Join(", ", platosBajaRotacion)}.");
            }

            if (insumosBajoMinimo.Any())
            {
                acciones.Add($"Priorizar reposición de insumos en mínimo o bajo mínimo: {string.Join(", ", insumosBajoMinimo)}.");
            }

            if (insumosSinLotes.Any())
            {
                acciones.Add($"Validar carga o recepción de lotes para: {string.Join(", ", insumosSinLotes)}.");
            }

            if (resumen.VariacionVentas > 0 && resumen.VariacionTicket < 0)
            {
                acciones.Add("Las ventas crecen con ticket promedio a la baja; revisar combos, adicionales o estrategia de upselling.");
            }

            if (!acciones.Any())
            {
                acciones.Add("No se detectan acciones críticas con los datos disponibles; mantener seguimiento del período.");
            }

            return acciones;
        }

        private static List<string> ConstruirAlertasPersonal(
            List<Empleado> empleados,
            IEnumerable<IGrouping<string, TurnoEmpleadoInfo>> turnos)
        {
            var alertas = new List<string>();
            var activosSinTurno = empleados.Where(e => EsActivo(e.Estado) && !TieneTurnos(e)).ToList();
            var inactivosConTurno = empleados.Where(e => !EsActivo(e.Estado) && TieneTurnos(e)).ToList();
            var empleadosDobleTurno = empleados.Where(e => CantidadTurnos(e) > 1).ToList();
            var gruposTurno = turnos.ToList();

            if (activosSinTurno.Any())
            {
                alertas.Add($"{activosSinTurno.Count} empleado(s) activo(s) no tienen turno asignado: {string.Join(", ", activosSinTurno.Take(4).Select(e => e.Nombre))}{(activosSinTurno.Count > 4 ? "..." : "")}.");
            }

            if (inactivosConTurno.Any())
            {
                alertas.Add($"{inactivosConTurno.Count} empleado(s) inactivo(s) conservan turnos asignados.");
            }

            if (empleadosDobleTurno.Any())
            {
                alertas.Add($"{empleadosDobleTurno.Count} empleado(s) tienen más de un turno asignado; revisar carga laboral.");
            }

            if (gruposTurno.Any())
            {
                var menorCobertura = gruposTurno.Min(g => g.Select(x => (int)x.Empleado.Id).Distinct().Count());
                var turnosConMenorCobertura = gruposTurno
                    .Where(g => g.Select(x => (int)x.Empleado.Id).Distinct().Count() == menorCobertura)
                    .Select(g => g.Key)
                    .ToList();

                alertas.Add($"Turno(s) con menor cobertura: {string.Join(", ", turnosConMenorCobertura)} ({menorCobertura} persona(s)).");
            }

            if (!alertas.Any())
            {
                alertas.Add("No se detectan alertas operativas con la información disponible.");
            }

            return alertas;
        }

        private class TurnoEmpleadoInfo
        {
            public TurnoLaboral Turno { get; set; } = null!;
            public Empleado Empleado { get; set; } = null!;
        }

        private class AgrupacionVenta
        {
            public AgrupacionVenta(string nombre, int cantidad, int articulos, decimal total)
            {
                Nombre = nombre;
                Cantidad = cantidad;
                Articulos = articulos;
                Total = total;
            }

            public string Nombre { get; }
            public int Cantidad { get; }
            public int Articulos { get; }
            public decimal Total { get; }
        }
    }

}
