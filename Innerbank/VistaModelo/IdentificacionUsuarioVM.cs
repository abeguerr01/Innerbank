using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Innerbank.Comandos;
using Innerbank.Modelos;
using Innerbank.Servicios;
using Innerbank.Stores;

namespace Innerbank.VistaModelo
{
    /// <summary>
    /// Esta clase se encarga de manejar todo el VisualModel de "IdentificarUsuario"
    /// </summary>
    public class IdentificacionUsuarioVM : VistaModeloBase
    {
        private readonly NavigationStore _navigationStore;

        // ===================================================== //
        // ====== Propiedades para el registro de usuario ====== //
        // ===================================================== //

        private string? _nombre;
        public string? Nombre
        {
            get { return _nombre; }
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }

        private string? _apellido;
        public string? Apellido
        {
            get { return _apellido; }
            set
            {
                if (_apellido != value)
                {
                    _apellido = value;
                    OnPropertyChanged(nameof(Apellido));
                }
            }
        }

        private string? _dni;
        public string? Dni
        {
            get { return _dni; }
            set
            {
                if (_dni != value)
                {
                    _dni = value;
                    OnPropertyChanged(nameof(Dni));
                }
            }
        }

        private string? _email;
        public string? Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        private string? _tlfn;
        public string? Tlfn
        {
            get { return _tlfn!; }
            set
            {
                if (_tlfn != value)
                {
                    _tlfn = value;
                    OnPropertyChanged(nameof(Tlfn));
                }
            }
        }

        private string? _contraseña;
        public string? Contraseña
        {
            get { return _contraseña; }
            set
            {
                if (_contraseña != value)
                {
                    _contraseña = value;
                    OnPropertyChanged(nameof(Contraseña));
                }
            }
        }

        // ===================================================== //
        // == Propiedades para el inicio de sesion de usuario == //
        // ===================================================== //

        private string? _logDni;
        public string? LogDni
        {
            get { return _logDni; }
            set
            {
                if (_logDni != value)
                {
                    _logDni = value;
                    OnPropertyChanged(nameof(LogDni));
                }
            }
        }

        private string? _logContraseña;
        public string? LogContraseña
        {
            get { return _logContraseña; }
            set
            {
                if (_logContraseña != value)
                {
                    _logContraseña = value;
                    OnPropertyChanged(nameof(LogContraseña));
                }
            }
        }

        private bool _cargando = true;
        public bool Cargando
        {
            get { return _cargando; }
            set
            {
                if (_cargando != value)
                {
                    _cargando = value;
                    OnPropertyChanged(nameof(Cargando));
                }
            }
        }

        public ICommand LogUsuarioCommand { get; }
        public ICommand CrearUsuarioCommand { get; }
        public ICommand AccederCommand { get; }
        public ICommand ContraseñasVerTodo { get; }

        public IdentificacionUsuarioVM(
            //Banco banco,
            INavigationService navegarSeleccionCuenta,
            INavigationService navegarContraseñaVerTodo
        )
        {
            LogUsuarioCommand = new Comandos.LogUsuarioCommand(this);
            CrearUsuarioCommand = new Comandos.CrearUsuarioCommand(this);

            _navigationStore = new NavigationStore();

            AccederCommand = new NavigateCommand(navegarSeleccionCuenta);
            ContraseñasVerTodo = new NavigateCommand(navegarContraseñaVerTodo);

            DispatcherTimer timer = new DispatcherTimer();

            //Temporizador (Simulacion de carga)
            timer.Interval = TimeSpan.FromMilliseconds(1500);
            timer.Tick += (sender, e) =>
            {
                Cargando = false;
                timer.Stop();
            };
            timer.Start();
        }
    }
}
