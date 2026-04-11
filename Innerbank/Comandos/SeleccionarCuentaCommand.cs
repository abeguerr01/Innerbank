using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Innerbank.Commands;
using Innerbank.Modelos;
using Innerbank.VistaModelo;

namespace Innerbank.Comandos
{
    /// <summary>
    /// Este comando se encarga de seleccionar la cuenta a la que quieres acceder
    /// </summary>
    public class SeleccionarCuentaCommand : ComandoBase
    {
        private readonly SeleccionCuentaVM _scvm;

        public SeleccionarCuentaCommand(SeleccionCuentaVM scvm)
        {
            _scvm = scvm;
        }

        public override void Execute(object? parameter)
        {
            if (_scvm.CuentaSeleccionada == null)
            {
                MessageBox.Show(
                    "No se ha seleccionado ninguna cuenta.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
            ControlDatosFicheroService bdd = new ControlDatosFicheroService();
            bdd.SeleccionarCuenta(_scvm.CuentaSeleccionada);

            if (_scvm.GoToCuenta.CanExecute(parameter))
            {
                _scvm.GoToCuenta.Execute(parameter);
            }
        }
    }
}
