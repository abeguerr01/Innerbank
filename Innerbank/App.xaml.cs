using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Windows;
using Innerbank.Comandos;
using Innerbank.Modelos;
using Innerbank.Servicios;
using Innerbank.Stores;
using Innerbank.VistaModelo;
using Innerbank.Vistas;

namespace Innerbank
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Banco _banco;
        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;

        /// <summary>
        /// El constructor del programa, solo inicializa el parametro modelo del banco y navigation store para poder movernos con comodidad por las escenas o pantallas
        /// </summary>
        public App()
        {
            _banco = new Banco("Innerbank");
            _navigationStore = new NavigationStore();
            _modalNavigationStore = new ModalNavigationStore();

            ControlDatosFicheroService.ControlDeVersiones("usuarios");
            ControlDatosFicheroService.ControlDeVersiones("cuentas");

            ControlDatosFicheroService cdfs = new ControlDatosFicheroService(); //Carga los datos
        }

        /// <summary>
        /// Esta función ejecuta la ventana y nos lleva a la pestaña inicial, "IdentificacionUsuario"
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            _navigationStore.CurrentViewModel = CrearIdentificacionUsuarioVM();

            MainWindow mainWindow = new MainWindow()
            {
                DataContext = new MainVM(_navigationStore, _modalNavigationStore),
            };
            MainWindow.Show();
            base.OnStartup(e);
        }

        /// <summary>
        /// Función para navegar a IdentificacionUsuario
        /// </summary>
        /// <returns></returns>
        private IdentificacionUsuarioVM CrearIdentificacionUsuarioVM()
        {
            return new IdentificacionUsuarioVM(
                //_banco,
                new NavigationService<SeleccionCuentaVM>(_navigationStore, CrearSeleccionCuentaVM),
                new ModalNavigationService<CntraseñaVerTodoVM>(
                    _modalNavigationStore,
                    CrearContraseñaVerTodo
                )
            );
        }

        /// <summary>
        /// Función para navegar a SeleccionCuenta
        /// </summary>
        /// <returns></returns>
        private SeleccionCuentaVM CrearSeleccionCuentaVM()
        {
            return new SeleccionCuentaVM(
                new NavigationService<ConsultaCuentaVM>(_navigationStore, CrearConsultarCuenta),
                new NavigationService<IdentificacionUsuarioVM>(
                    _navigationStore,
                    CrearIdentificacionUsuarioVM
                )
            );
        }

        /// <summary>
        /// Función para navegar a ConsultarCuenta
        /// </summary>
        /// <returns></returns>
        private ConsultaCuentaVM CrearConsultarCuenta()
        {
            return new ConsultaCuentaVM(
                new NavigationService<SeleccionCuentaVM>(_navigationStore, CrearSeleccionCuentaVM)
            );
        }

        /// <summary>
        /// Función para volver a IdentificacionUsuario
        /// </summary>
        /// <returns></returns>
        private CntraseñaVerTodoVM CrearContraseñaVerTodo()
        {
            return new CntraseñaVerTodoVM(
                new ModalNavigationService<IdentificacionUsuarioVM>(
                    _modalNavigationStore,
                    () => null!
                )
            );
        }
    }
}
