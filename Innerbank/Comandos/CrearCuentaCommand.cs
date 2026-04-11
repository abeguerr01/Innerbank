using System;
using System.Net;
using System.Windows;
using Innerbank.Commands;
using Innerbank.Modelos;
using Innerbank.Stores;
using Innerbank.VistaModelo;

namespace Innerbank.Comandos
{
    /// <summary>
    /// Este comando crea una cuenta nueva pudiendo ajustar el saldo y los cotitulares base.
    /// </summary>
    public class CrearCuentaCommand : ComandoBase
    {
        private readonly SeleccionCuentaVM _scvm;
        private readonly ControlDatosFicheroService _bdd;

        public CrearCuentaCommand(SeleccionCuentaVM scvm)
        {
            _scvm = scvm ?? throw new ArgumentNullException(nameof(scvm));
            _bdd = new ControlDatosFicheroService();
        }

        public override void Execute(object? parameter)
        {
            List<string> listaCotit = new List<string>();
            List<MovimientosStore> movimientos = [];
            if (_scvm.Cotit is not null || _scvm.Cotit == "")
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

                var usuarioNuevo = _bdd.BuscarUsuario(_scvm.Cotit);
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
                else
                {
                    listaCotit.Add(usuarioNuevo.Dni);
                }
            }

            string numeroGenerado = Usuario.generarNumeroCuenta();
            Cuenta nuevaCuenta = new Cuenta(
                numeroGenerado,
                _scvm._user.Dni,
                listaCotit,
                _scvm.SaldoInicial,
                movimientos
            );

            _bdd.AñadirCuenta(nuevaCuenta);

            _scvm.Cuentas.Add(nuevaCuenta);

            _scvm._user.CuentasTitular.Add(nuevaCuenta);

            MessageBox.Show($"Cuenta {numeroGenerado} creada con éxito.");

            _scvm.SaldoInicial = 0;
            _scvm.Cotit = null;
        }
    }
}
