using System;
using System.Collections.Generic;
using System.Linq;

namespace PanComido.Dominio.ValueObjects
{
    public sealed class RolEmpleado : IEquatable<RolEmpleado>
    {
        public const string Gerente = "Gerente";
        public const string Mozo = "Mozo";
        public const string Cocina = "Cocina";

        private static readonly HashSet<string> ValoresValidos = new(StringComparer.OrdinalIgnoreCase)
        {
            Gerente,
            Mozo,
            Cocina
        };

        private RolEmpleado(string valor)
        {
            Valor = valor;
        }

        public string Valor { get; }

        public static RolEmpleado Crear(string? valor)
        {
            if (!EsValido(valor))
                throw new ArgumentException("El rol del empleado no es valido.");

            return new RolEmpleado(Normalizar(valor!));
        }

        public static bool EsValido(string? valor)
        {
            return !string.IsNullOrWhiteSpace(valor) && ValoresValidos.Contains(valor.Trim());
        }

        public static string Normalizar(string valor)
        {
            return ValoresValidos.First(v => v.Equals(valor.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public override string ToString()
        {
            return Valor;
        }

        public bool Equals(RolEmpleado? other)
        {
            return other is not null && Valor == other.Valor;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RolEmpleado);
        }

        public override int GetHashCode()
        {
            return Valor.GetHashCode();
        }

    }
}
