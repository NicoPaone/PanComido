using System;
using System.Collections.Generic;
using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IPdfGeneradorServicio
    {
        byte[] GenerarReporteDashboard(
            ResumenOperativo resumen, 
            ResumenRendimientoComercial rendimiento, 
            List<Insumo> criticidad, 
            DateTime desde, 
            DateTime hasta
        );

        byte[] GenerarReportePersonal(List<Empleado> empleados);

        byte[] GenerarReporteVentas(List<VentaReporteDetalle> ventas, DateTime desde, DateTime hasta);
    }
}
