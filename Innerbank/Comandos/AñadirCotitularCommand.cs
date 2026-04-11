using System;
using System.Windows;
using Innerbank.Commands;
using Innerbank.VistaModelo;

namespace Innerbank.Comandos
{
    /// <summary>
    ///Este comando sirve para añadir un cotitular nuevo a una cuenta ya creada.
    /// </summary>
    public class AñadirCotitularCommand : ComandoBase
    {
        private ConsultaCuentaVM _ccvm;

        public AñadirCotitularCommand(ConsultaCuentaVM ccvm)
        {
            _ccvm = ccvm;
        }

        public override void Execute(object? parameter)
        {
            if (_ccvm.NuevoCotitularDni != string.Empty)
            {
                MessageBoxResult res = MessageBox.Show(
                    "Hemos visto que quieres añadir un cotitular,\nesta accion es irreversible y no se podrá eliminar de la cuenta.\n\n¿Estas seguro?",
                    "¿Seguro de que quieres añadir un cotitular?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No
                );
                if (res == MessageBoxResult.No)
                {
                    return;
                }
            }
            string dni = _ccvm.NuevoCotitularDni!;
            ControlDatosFicheroService bdd = new ControlDatosFicheroService();

            if (string.IsNullOrWhiteSpace(dni))
            {
                return;
            }

            //Verificar si el usuario existe en el sistema
            var usuarioNuevo = bdd.BuscarUsuario(dni);
            if (usuarioNuevo == null)
            {
                MessageBox.Show(
                    "No existe ningún usuario con ese DNI.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            //Verificar si ya es titular o cotitular
            if (dni == _ccvm.cuenta.DniTitular || _ccvm.cuenta.DniCoTitular.Contains(dni))
            {
                MessageBox.Show(
                    "Este usuario ya está vinculado a la cuenta.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            bdd.AñadirCoTitularCuenta(_ccvm.cuenta.IDCuenta!, dni);

            _ccvm.cuenta.DniCoTitular.Add(dni);
            _ccvm.Cotitulares.Add($"{usuarioNuevo.Nombre} {usuarioNuevo.Apellido} ({dni})");

            //Limpiar el campo de texto
            _ccvm.NuevoCotitularDni = string.Empty;
        }
    }
}
