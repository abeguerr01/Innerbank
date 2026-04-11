using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;
using Innerbank.Comandos;
using Innerbank.Modelos;
using Innerbank.Servicios;
using Innerbank.Stores;

namespace Innerbank.VistaModelo
{
    /// <summary>
    /// Esta clase se encarga de manejar todo el VisualModel de "SeleccionCuenta"
    /// </summary>
    public class SeleccionCuentaVM : VistaModeloBase
    {
        public Usuario _user;
        private readonly ObservableCollection<Modelos.Cuenta> _cuentas;
        private readonly ObservableCollection<MovimientosStore> _lista;

        ControlDatosFicheroService bdd = new ControlDatosFicheroService();

        private string? _idCuenta;
        public string? IDCuenta
        {
            get { return _idCuenta; }
            set
            {
                if (_idCuenta != value)
                {
                    _idCuenta = value;
                    OnPropertyChanged(nameof(_idCuenta));
                }
            }
        }

        private Cuenta? _cuentaSeleccionada;
        public Cuenta? CuentaSeleccionada
        {
            get => _cuentaSeleccionada;
            set
            {
                _cuentaSeleccionada = value;
                OnPropertyChanged(nameof(CuentaSeleccionada));
            }
        }

        private string? _cotit;
        public string? Cotit
        {
            get => _cotit;
            set
            {
                _cotit = value;
                OnPropertyChanged(nameof(Cotit));
            }
        }
        private double _saldoInicial;
        public double SaldoInicial
        {
            get => _saldoInicial;
            set
            {
                _saldoInicial = value;
                OnPropertyChanged(nameof(SaldoInicial));
            }
        }

        public ObservableCollection<Cuenta> Cuentas => _cuentas;
        public ObservableCollection<MovimientosStore> ListaMovimientos => _lista;

        public ICommand CrearNuevaCuentaComand { get; }
        public ICommand SeleccionarCuentaCommand { get; }
        public ICommand GoToCuenta { get; }
        public ICommand CerrarSesion { get; }

        public SeleccionCuentaVM(
            INavigationService consultarCuentaNavigationService,
            INavigationService IdentificacionUsuarioNavigationService
        )
        {
            _lista = new ObservableCollection<MovimientosStore>();

            _user = bdd.CargarTmpUsuario()!;

            var todasLasCuentas = bdd.CargarCuentas() ?? new List<Cuenta>();

            var cuentasFiltradas = todasLasCuentas
                .Where(c =>
                    c.DniTitular == _user.Dni
                    || (c.DniCoTitular != null && c.DniCoTitular.Contains(_user.Dni))
                )
                .ToList();

            _cuentas = new ObservableCollection<Cuenta>(cuentasFiltradas);

            CrearNuevaCuentaComand = new CrearCuentaCommand(this);
            SeleccionarCuentaCommand = new SeleccionarCuentaCommand(this);
            GoToCuenta = new NavigateCommand(consultarCuentaNavigationService);
            CerrarSesion = new NavigateCommand(IdentificacionUsuarioNavigationService);
        }
    }
}
