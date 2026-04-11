using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innerbank.Modelos;

namespace Innerbank.Servicios
{
    /// <summary>
    /// Interfaz para el coltrol de datos
    /// </summary>
    interface IControlDatos
    {
        void AñadirUsuario(Usuario user);
        void BorrarUsuario(string dni);
        void AñadirCuenta(Cuenta cuenta);
        void BorrarCuenta(string dni);
        void GuardarUsuarios(List<Usuario> data);
        void GuardarCuentas(List<Cuenta> data);
        List<Usuario> CargarUsuarios();
        List<Cuenta> CargarCuentas();
        Usuario? BuscarUsuario(string dni);
        void SeleccionarUsuario(string dni);
        Usuario? CargarTmpUsuario();
        void SeleccionarCuenta(Cuenta c);
        abstract Cuenta? CargarTmpCuenta();
        void ActualizarSaldoCuenta(string idCuenta, double nuevoSaldo);
        void AñadirCoTitularCuenta(string idCuenta, string dniNuevo);
    }
}
