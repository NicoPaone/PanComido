using System;
using System.Collections.Generic;
using System.Linq;

namespace PanComido.Dominio.ValueObjects
{
    public sealed class EstadoEmpleado : IEquatable<EstadoEmpleado>
    {
        public const string Activo = "activo";
        public const string Inactivo = "inactivo";

        private static readonly HashSet<string> ValoresValidos = new(StringComparer.OrdinalIgnoreCase)
        {
            Activo,
            Inactivo
        };

        private EstadoEmpleado(string valor)
        {
            Valor = valor;
        }

        public string Valor { get; }

        public static EstadoEmpleado Crear(string? valor)
        {
            if (!EsValido(valor))
                throw new ArgumentException("El estado del empleado no es valido.");

            return new EstadoEmpleado(Normalizar(valor!));
        }

        public static EstadoEmpleado ActivoPorDefecto()
        {
            return new EstadoEmpleado(Activo);
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

        public bool Equals(EstadoEmpleado? other)
        {
            return other is not null && Valor == other.Valor;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as EstadoEmpleado);
        }

        public override int GetHashCode()
        {
            return Valor.GetHashCode();
        }

    }
}
