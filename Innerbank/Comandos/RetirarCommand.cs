using System.Windows;
using Innerbank.Commands;
using Innerbank.VistaModelo;

namespace Innerbank.Comandos
{
    /// <summary>
    /// Este comando se encarga de quitar dinero a una cuenta ya existende desde dentro de ella sin permitir retirar mas de lo que hay en ella.
    /// </summary>
    public class RetirarCommand : ComandoBase
    {
        private ConsultaCuentaVM _ccvm;

        public RetirarCommand(ConsultaCuentaVM ccvm)
        {
            _ccvm = ccvm;
        }

        public override void Execute(object? parameter)
        {
            if (_ccvm.Cantidad > _ccvm.Saldo)
            {
                MessageBox.Show(
                    "No puedes sacar mas de lo que hay en la cuenta",
                    "No hay saldo suficiente",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (_ccvm.Cantidad <= 0)
            {
                _ccvm.Cantidad = 0;
                return;
            }

            ControlDatosFicheroService bdd = new ControlDatosFicheroService();

            try
            {
                bdd.ActualizarSaldoCuenta(
                    bdd.CargarTmpCuenta()!.IDCuenta!,
                    _ccvm.Saldo -= Convert.ToDouble(_ccvm.Cantidad)
                );
                _ccvm.RegistrarNuevoMovimiento("Retirada", Convert.ToDouble(_ccvm.Cantidad));
                _ccvm.Cantidad = 0;
            }
            catch (FormatException)
            {
                MessageBox.Show(
                    "Formato de cantidad no válido.",
                    "ERROR DE FORMATO",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
            catch
            {
                MessageBox.Show(
                    "Error desconocido al ingresar la cantidad.",
                    "ERROR DESCONOCIDO",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
        }
    }
}
