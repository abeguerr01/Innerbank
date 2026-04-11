using System.Windows;
using Innerbank.Commands;
using Innerbank.VistaModelo;

namespace Innerbank.Comandos
{
    /// <summary>
    /// Este comando se encarga de añadir dinero a una cuenta ya existende desde dentro de ella.
    /// </summary>
    public class IngresarCommand : ComandoBase
    {
        private ConsultaCuentaVM _ccvm;

        public IngresarCommand(ConsultaCuentaVM ccvm)
        {
            _ccvm = ccvm;
        }

        public override void Execute(object? parameter)
        {
            ControlDatosFicheroService bdd = new ControlDatosFicheroService();
            try
            {
                if (_ccvm.Cantidad < Math.Pow(10, 15) && _ccvm.Cantidad > 0)
                {
                    bdd.ActualizarSaldoCuenta(
                        bdd.CargarTmpCuenta()!.IDCuenta!,
                        _ccvm.Saldo += Convert.ToDouble(_ccvm.Cantidad)
                    );
                    _ccvm.RegistrarNuevoMovimiento("Ingreso", _ccvm.Cantidad);
                    _ccvm.Cantidad = 0;
                }
                else
                {
                    MessageBox.Show(
                        $"No se puede ingresar esa cantidad dinero,el minimo que se puede ingresar es de 0.01, y el maximo es {Math.Pow(10, 15) - 1}€",
                        "Cantidad demasiado alta",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
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
