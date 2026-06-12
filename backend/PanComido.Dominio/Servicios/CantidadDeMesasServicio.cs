//using PanComido.Dominio.Entidades;
//using PanComido.Dominio.Interfaces.Repositorios;
//using PanComido.Dominio.Interfaces.Servicios;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PanComido.Dominio.Servicios
//{
//    public class CantidadDeMesasServicio : ICantidadDeMesasServicio
//    {
//        private readonly IMesaRepositorio _mesaRepositorio;

//        public CantidadDeMesasServicio(IMesaRepositorio mesaRepositorio)
//        {
//            _mesaRepositorio = mesaRepositorio;
//        }

//        public int ObtenerCantidadDeMesasTotal(int restauranteId)
//        {            
//            return _mesaRepositorio.ObtenerTodasAsync(restauranteId).Result.Count;
//        }

//        public int ObtenerCantidadDeMesasOcupadas(int restauranteId)
//        {
//            return _mesaRepositorio.ObtenerOcupadasAsync(restauranteId).Result.Count;
//        }

//        public int ObtenerCantidadDeMesasDisponibles(int restauranteId)
//        {
//            return _mesaRepositorio.ObtenerDisponiblesAsync(restauranteId).Result.Count;
//        }
//    }
//}
