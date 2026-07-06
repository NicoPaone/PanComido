using System;
using System.Collections.Generic;
using System.Linq;

namespace PanComido.Dominio.Entidades
{
    public static class EmpleadoConstantes
    {
        public const string RolGerente = "Gerente";
        public const string RolMozo = "Mozo";
        public const string RolCocina = "Cocina";

        public const string EstadoActivo = "activo";
        public const string EstadoInactivo = "inactivo";

        private static readonly HashSet<string> RolesValidos = new(StringComparer.OrdinalIgnoreCase)
        {
            RolGerente,
            RolMozo,
            RolCocina
        };

        private static readonly HashSet<string> EstadosValidos = new(StringComparer.OrdinalIgnoreCase)
        {
            EstadoActivo,
            EstadoInactivo
        };

        public static bool EsRolValido(string? rol)
        {
            return !string.IsNullOrWhiteSpace(rol) && RolesValidos.Contains(rol.Trim());
        }

        public static bool EsEstadoValido(string? estado)
        {
            return !string.IsNullOrWhiteSpace(estado) && EstadosValidos.Contains(estado.Trim());
        }

        public static string NormalizarRol(string rol)
        {
            return RolesValidos.First(r => r.Equals(rol.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public static string NormalizarEstado(string estado)
        {
            return estado.Trim().Equals(EstadoActivo, StringComparison.OrdinalIgnoreCase)
                ? EstadoActivo
                : EstadoInactivo;
        }
    }
}
