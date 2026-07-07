using System.Collections.Generic;
using System.Threading.Tasks;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.ReporteCasosDeUso
{
    public class GenerarReportePersonalPdfCasoDeUso
    {
        private readonly ListarEmpleadosCasoDeUso _listarEmpleadosCasoDeUso;
        private readonly IPdfGeneradorServicio _pdfGenerador;

        public GenerarReportePersonalPdfCasoDeUso(
            ListarEmpleadosCasoDeUso listarEmpleadosCasoDeUso,
            IPdfGeneradorServicio pdfGenerador)
        {
            _listarEmpleadosCasoDeUso = listarEmpleadosCasoDeUso;
            _pdfGenerador = pdfGenerador;
        }

        public async Task<byte[]> EjecutarAsync(int restauranteId)
        {
            var empleados = await _listarEmpleadosCasoDeUso.EjecutarAsync(restauranteId);
            return _pdfGenerador.GenerarReportePersonal(empleados);
        }
    }
}
