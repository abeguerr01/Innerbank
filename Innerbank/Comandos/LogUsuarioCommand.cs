using System;
using System.Linq;
using System.Windows;
using Innerbank.Commands;
using Innerbank.Modelos;
using Innerbank.VistaModelo;

namespace Innerbank.Comandos
{
    /// <summary>
    /// Este comando permite acceder a un usuario ya creado.
    /// </summary>
    internal class LogUsuarioCommand : ComandoBase
    {
        //private readonly Banco _banco;
        private List<Usuario> _usuarios;
        private readonly IdentificacionUsuarioVM iuvm;

        public LogUsuarioCommand(IdentificacionUsuarioVM iuvm)
        {
            ControlDatosFicheroService bdd = new ControlDatosFicheroService();
            _usuarios = bdd.CargarUsuarios();
            this.iuvm = iuvm ?? throw new ArgumentNullException(nameof(iuvm));
        }

        public override void Execute(object? parameter)
        {
            // Validaciones básicas de entrada
            if (string.IsNullOrEmpty(iuvm.LogDni) || iuvm.LogDni.Length != 9)
            {
                MessageBox.Show(
                    "El DNI no coincide con ninguna cuenta",
                    "DNI no válido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (string.IsNullOrEmpty(iuvm.LogContraseña))
            {
                MessageBox.Show(
                    "Introduce la contraseña",
                    "Contraseña vacía",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            //Aseguramos de que la colección de usuarios está inicializada
            var usuarios = _usuarios;
            if (usuarios == null || !usuarios.Any())
            {
                MessageBox.Show(
                    "No hay usuarios registrados",
                    "Sin usuarios",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            // Buscar el usuario por el DNI introducido en el login (LogDni)
            var usuarioEncontrado = usuarios.FirstOrDefault(u => u.Dni == iuvm.LogDni);
            if (usuarioEncontrado == null)
            {
                MessageBox.Show(
                    "El DNI no coincide con ninguna cuenta",
                    "DNI no encontrado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            // Comparar la contraseña guardada con la introducida
            if (usuarioEncontrado.Contraseña != iuvm.LogContraseña)
            {
                MessageBox.Show(
                    "La contraseña no coincide con ninguna cuenta",
                    "Contraseña no válida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            // Login correcto
            MessageBox.Show(
                $"Bienvenido/a {usuarioEncontrado.Nombre}",
                "Login correcto",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            ControlDatosFicheroService bdd = new ControlDatosFicheroService();
            bdd.SeleccionarUsuario(bdd.BuscarUsuario(iuvm.LogDni)!.Dni);

            if (iuvm.AccederCommand.CanExecute(parameter))
            {
                iuvm.AccederCommand.Execute(parameter);
            }
        }
    }
}
