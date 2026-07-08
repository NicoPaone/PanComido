using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.ValueObjects;

namespace PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso
{
    public class CrearEmpleadoCasoDeUso
    {
        private readonly IEmpleadoRepositorio _repositorio;
        private readonly IContraseniaHasher _hasher;

        public CrearEmpleadoCasoDeUso(IEmpleadoRepositorio repositorio, IContraseniaHasher hasher)
        {
            _repositorio = repositorio;
            _hasher = hasher;
        }

        public async Task<Empleado> EjecutarAsync(int restauranteId, Empleado empleado, string contraseniaPlana, List<int> turnosIds)
        {
            if (string.IsNullOrWhiteSpace(empleado.Nombre))
                throw new ArgumentException("El nombre del empleado no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(empleado.Email))
                throw new ArgumentException("El email del empleado no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(contraseniaPlana))
                throw new ArgumentException("La contraseña del empleado no puede estar vacía.");

            if (string.IsNullOrWhiteSpace(empleado.Rol))
                throw new ArgumentException("El rol del empleado no puede estar vacío.");

            if (!RolEmpleado.EsValido(empleado.Rol))
                throw new ArgumentException("El rol del empleado no es válido.");

            if (!string.IsNullOrWhiteSpace(empleado.Estado) && !EstadoEmpleado.EsValido(empleado.Estado))
                throw new ArgumentException("El estado del empleado no es válido.");

            // Validar si el email ya existe
            var empleadoExistente = await _repositorio.ObtenerPorEmailAsync(empleado.Email);
            if (empleadoExistente != null)
                throw new ArgumentException($"El email '{empleado.Email}' ya se encuentra registrado.");

            // Hashear la contraseña
            empleado.ContraseniaHash = _hasher.Hash(contraseniaPlana);
            empleado.RestauranteId = restauranteId;
            empleado.Rol = RolEmpleado.Crear(empleado.Rol).Valor;
            
            if (string.IsNullOrWhiteSpace(empleado.Estado))
            {
                empleado.Estado = EstadoEmpleado.ActivoPorDefecto().Valor;
            }
            else
            {
                empleado.Estado = EstadoEmpleado.Crear(empleado.Estado).Valor;
            }

            await _repositorio.CrearAsync(empleado, turnosIds);

            // Recuperar la entidad creada completa (con turnos)
            var creado = await _repositorio.ObtenerPorIdYRestauranteAsync(empleado.Id, restauranteId);
            return creado ?? empleado;
        }
    }
}
