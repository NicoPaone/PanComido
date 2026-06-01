using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso
{
    public class ResolverLlamadoCasoDeUso
    {
        private readonly ILlamadoRepositorio _llamadoRepositorio;

        public ResolverLlamadoCasoDeUso(ILlamadoRepositorio llamadoRepositorio)
        {
            _llamadoRepositorio = llamadoRepositorio;
        }

        public async Task EjecutarAsync(int llamadoId)
        {
         bool respuesta = await _llamadoRepositorio.ResolverLlamadoAsync(llamadoId);
            if (!respuesta)
                throw new KeyNotFoundException("El llamado no existe.");
        }
    }
}
