using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Innerbank.Commands;
using Innerbank.Modelos;
using Innerbank.VistaModelo;

namespace Innerbank.Comandos
{
    /// <summary>
    /// Este comando se encarga de crear nuevos usuarios, comprobar que no existen e iniciar sesion con el mismo.
    /// </summary>
    public class CrearUsuarioCommand : ComandoBase
    {
        private readonly IdentificacionUsuarioVM iuvm;
        private List<Usuario> _usuarios;
        ControlDatosFicheroService bdd = new ControlDatosFicheroService();

        public CrearUsuarioCommand(IdentificacionUsuarioVM iuvm)
        {
            _usuarios = bdd.CargarUsuarios();
            this.iuvm = iuvm ?? throw new ArgumentNullException(nameof(iuvm));
            iuvm.PropertyChanged += OnViewModelPropertyChanged;
        }

        /// <summary>
        /// Solo permite ejecutarse si nungún campo está vacio, el DNI tiene 9 caracteres y el correo contiene una @.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(iuvm.Nombre)
                && !string.IsNullOrEmpty(iuvm.Apellido)
                && !string.IsNullOrEmpty(iuvm.Dni)
                && iuvm.Dni.Length == 9
                && !string.IsNullOrEmpty(iuvm.Email)
                && iuvm.Email.Contains('@')
                && !string.IsNullOrEmpty(iuvm.Tlfn)
                && !string.IsNullOrEmpty(iuvm.Contraseña)
                && base.CanExecute(parameter);
        }

        public void OnViewModelPropertyChanged(
            object? sender,
            System.ComponentModel.PropertyChangedEventArgs e
        )
        {
            if (
                e.PropertyName == nameof(iuvm.Nombre)
                || e.PropertyName == nameof(iuvm.Apellido)
                || e.PropertyName == nameof(iuvm.Dni)
                || e.PropertyName == nameof(iuvm.Email)
                || e.PropertyName == nameof(iuvm.Tlfn)
                || e.PropertyName == nameof(iuvm.Contraseña)
            )
            {
                OnCanExecuteChanged();
            }
        }

        public override void Execute(object? parameter)
        {
            if (_usuarios.Any(u => u.Dni == iuvm.Dni))
            {
                MessageBox.Show(
                    "Ya existe un usuario con ese DNI",
                    "DNI duplicado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            Usuario nuevoUsuario = new Usuario(
                iuvm.Dni!,
                iuvm.Nombre!,
                iuvm.Apellido!,
                iuvm.Email!,
                iuvm.Tlfn!,
                iuvm.Contraseña!
            );
            nuevoUsuario.CuentasTitular.Add(
                new Cuenta(Usuario.generarNumeroCuenta(), nuevoUsuario.Dni, 0.0)
            );
            //_banco.AñadirUsuario(nuevoUsuario);

            MessageBox.Show(
                "Hemos podido darte de alta correctamente",
                "ALTA",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
            iuvm.Dni = string.Empty;
            iuvm.Nombre = string.Empty;
            iuvm.Apellido = string.Empty;
            iuvm.Email = string.Empty;
            iuvm.Tlfn = string.Empty;
            iuvm.Contraseña = string.Empty;

            ControlDatosFicheroService bdd = new ControlDatosFicheroService();
            bdd.AñadirUsuario(nuevoUsuario);
            bdd.SeleccionarUsuario(nuevoUsuario.Dni);

            if (iuvm.AccederCommand.CanExecute(parameter))
            {
                iuvm.AccederCommand.Execute(parameter);
            }
        }
    }
}
