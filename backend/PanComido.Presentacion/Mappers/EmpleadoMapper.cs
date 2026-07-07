using System.Collections.Generic;
using System.Linq;
using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Empleado;

namespace PanComido.Presentacion.Mappers
{
    public class EmpleadoMapper
    {
        private readonly TurnoLaboralMapper _turnoLaboralMapper;

        public EmpleadoMapper(TurnoLaboralMapper turnoLaboralMapper)
        {
            _turnoLaboralMapper = turnoLaboralMapper;
        }

        public EmpleadoResponseDto aDto(Empleado empleado)
        {
            return new EmpleadoResponseDto
            {
                Id = empleado.Id,
                Nombre = empleado.Nombre,
                Email = empleado.Email,
                Estado = empleado.Estado,
                Rol = empleado.Rol,
                Turnos = empleado.Turnos != null 
                    ? _turnoLaboralMapper.aListaDto(empleado.Turnos)
                    : new List<DTOs.TurnoLaboral.TurnoLaboralResponseDto>()
            };
        }

        public List<EmpleadoResponseDto> aListaDto(List<Empleado> empleados)
        {
            return empleados.Select(aDto).ToList();
        }

        public Empleado aDominio(CrearEmpleadoRequestDto dto)
        {
            return new Empleado
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Estado = dto.Estado,
                Rol = dto.Rol
            };
        }

        public Empleado aDominio(ModificarEmpleadoRequestDto dto, int id)
        {
            return new Empleado
            {
                Id = id,
                Nombre = dto.Nombre,
                Email = dto.Email,
                Estado = dto.Estado,
                Rol = dto.Rol
            };
        }
    }
}
