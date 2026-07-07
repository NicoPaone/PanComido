using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso
{
    public class ModificarEmpleadoCasoDeUso
    {
        private readonly IEmpleadoRepositorio _repositorio;
        private readonly IContraseniaHasher _hasher;

        public ModificarEmpleadoCasoDeUso(IEmpleadoRepositorio repositorio, IContraseniaHasher hasher)
        {
            _repositorio = repositorio;
            _hasher = hasher;
        }

        public async Task<Empleado> EjecutarAsync(int restauranteId, Empleado empleadoModificado, string? nuevaContrasenia, List<int> turnosIds)
        {
            var empleadoExistente = await _repositorio.ObtenerPorIdYRestauranteAsync(empleadoModificado.Id, restauranteId);
            if (empleadoExistente == null)
                throw new KeyNotFoundException("Empleado no encontrado.");

            if (string.IsNullOrWhiteSpace(empleadoModificado.Nombre))
                throw new ArgumentException("El nombre del empleado no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(empleadoModificado.Email))
                throw new ArgumentException("El email del empleado no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(empleadoModificado.Rol))
                throw new ArgumentException("El rol del empleado no puede estar vacío.");

            if (!EmpleadoConstantes.EsRolValido(empleadoModificado.Rol))
                throw new ArgumentException("El rol del empleado no es válido.");

            if (!EmpleadoConstantes.EsEstadoValido(empleadoModificado.Estado))
                throw new ArgumentException("El estado del empleado no es válido.");

            // Validar que el email no esté tomado por otro empleado
            var emailEnUso = await _repositorio.ObtenerPorEmailAsync(empleadoModificado.Email);
            if (emailEnUso != null && emailEnUso.Id != empleadoModificado.Id)
                throw new ArgumentException($"El email '{empleadoModificado.Email}' ya se encuentra en uso por otro empleado.");

            // Asignar campos actualizados
            empleadoExistente.Nombre = empleadoModificado.Nombre;
            empleadoExistente.Email = empleadoModificado.Email;
            empleadoExistente.Estado = EmpleadoConstantes.NormalizarEstado(empleadoModificado.Estado);
            empleadoExistente.Rol = EmpleadoConstantes.NormalizarRol(empleadoModificado.Rol);

            if (!string.IsNullOrWhiteSpace(nuevaContrasenia))
            {
                empleadoExistente.ContraseniaHash = _hasher.Hash(nuevaContrasenia);
            }
            else
            {
                empleadoExistente.ContraseniaHash = string.Empty; // No cambia la contraseña existente en la DB
            }

            await _repositorio.ModificarAsync(empleadoExistente, turnosIds);

            var actualizado = await _repositorio.ObtenerPorIdYRestauranteAsync(empleadoModificado.Id, restauranteId);
            return actualizado ?? empleadoExistente;
        }
    }
}
