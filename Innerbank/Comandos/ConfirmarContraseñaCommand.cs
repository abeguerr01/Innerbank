using System.IO.Packaging;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Innerbank;
using Innerbank.Commands;
using Innerbank.Modelos;
using Innerbank.VistaModelo;

/// <summary>
/// Este comando se encarga de verificar que la contraseña introducida coincida con la contraseña asignada
/// </summary>
public class ConfirmarContraseñaCommand : ComandoBase
{
    private readonly CntraseñaVerTodoVM _cvtvm;
    private static string CONTRASEÑA = "25082005";

    public ConfirmarContraseñaCommand(CntraseñaVerTodoVM cvtvm)
    {
        _cvtvm = cvtvm;

        _cvtvm.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(
        object? sender,
        System.ComponentModel.PropertyChangedEventArgs e
    )
    {
        if (e.PropertyName == nameof(CntraseñaVerTodoVM.Contraseña))
        {
            OnCanExecuteChanged();
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return !string.IsNullOrEmpty(_cvtvm.Contraseña) && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        if (_cvtvm.Contraseña != CONTRASEÑA)
        {
            MessageBox.Show(
                "Contraseña incorrecta",
                "ACCESO",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
        else
        {
            ControlDatosFicheroService cdfs = new ControlDatosFicheroService();
            _cvtvm.Contraseña = string.Empty;
            MessageBox.Show(
                "Contraseña correcta\nA continuacion apareceran todos los usuarios y cuentas guardadas",
                "ACCESO",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
            List<Usuario> lUsuarios = cdfs.CargarUsuarios();
            StringBuilder sbu = new StringBuilder();
            foreach (Usuario item in lUsuarios)
            {
                sbu.Append($"{item.ToString()}\n\n");
            }
            MessageBox.Show($"{sbu}", "USUARIOS");

            List<Cuenta> lCuentas = cdfs.CargarCuentas();
            StringBuilder sbc = new StringBuilder();
            foreach (Cuenta item in lCuentas)
            {
                sbc.Append($"{item.ToString()}\n\n");
            }

            MessageBox.Show($"{sbc}", "Cuentas");

            MessageBoxResult res = MessageBox.Show(
                "¿Deseas obtener los archivos sin encriptar?",
                "Obtencion de archivos",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No
            );

            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    OpenFolderDialog folderDialog = new OpenFolderDialog();
                    folderDialog.Title = "Seleciona donde guardar los archivos";

                    if (folderDialog.ShowDialog() == true)
                    {
                        var folderName = folderDialog.FolderName;
                        cdfs.GuardarArchivosSinCifrar(folderName);

                        MessageBox.Show(
                            "Los archivos se han guardado correctamente",
                            "EXITO",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error:\n{ex}", "ERROR");
                }
            }
        }
    }
}
