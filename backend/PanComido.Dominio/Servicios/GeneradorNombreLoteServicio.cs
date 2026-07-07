using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Threading.Tasks;

namespace PanComido.Dominio.Servicios
{
    public class GeneradorNombreLoteServicio : IGeneradorNombreLoteServicio
    {
        private readonly ILoteRepositorio _loteRepositorio;

        public GeneradorNombreLoteServicio(ILoteRepositorio loteRepositorio)
        {
            _loteRepositorio = loteRepositorio;
        }

        public async Task<string> GenerarNombreUnicoAsync(string nombreItem)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string nombreBase = $"{nombreItem}-{timestamp}";
            int cantNombreDuplicado = await _loteRepositorio.ContarLotesConNombreBaseAsync(nombreBase);
            if (cantNombreDuplicado == 0) return $"{nombreItem}-{timestamp}";
            return $"{nombreItem} ({cantNombreDuplicado + 1})-{timestamp}";
        }
    }
}
