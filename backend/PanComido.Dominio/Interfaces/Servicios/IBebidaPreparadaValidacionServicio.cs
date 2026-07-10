using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IBebidaPreparadaValidacionServicio
    {
        void ValidarDatosBasicos(BebidaPreparada bebidaPreparada);
    }
}
