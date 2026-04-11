using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Innerbank.Comandos;
using Innerbank.Modelos;
using Innerbank.Servicios;
using Innerbank.Stores;

namespace Innerbank.VistaModelo
{
    /// <summary>
    /// Esta clase se encarga de manejar todo el VisualModel de "ConsultarCuenta"
    /// </summary>
    public class ConsultaCuentaVM : VistaModeloBase
    {
        public Cuenta cuenta;
        ControlDatosFicheroService bdd = new ControlDatosFicheroService();

        public ObservableCollection<MovimientosStore> ListaMovimientos { get; set; }
        public ObservableCollection<string> Cotitulares { get; set; }

        private double _cantidad;
        public double Cantidad
        {
            get => _cantidad;
            set
            {
                _cantidad = value;
                OnPropertyChanged(nameof(Cantidad));
            }
        }

        private double _saldo;
        public double Saldo
        {
            get => _saldo;
            set
            {
                _saldo = value;
                cuenta.Saldo = value;
                OnPropertyChanged(nameof(Saldo));
            }
        }

        private string? _titular;
        public string? Titular
        {
            get => _titular;
            set
            {
                _titular = value;
                OnPropertyChanged(nameof(Titular));
            }
        }

        private string? _numCuenta;
        public string? NumCuenta
        {
            get => _numCuenta;
            set
            {
                _numCuenta = value;
                OnPropertyChanged(nameof(NumCuenta));
            }
        }

        private string? _nuevoCotitularDni;
        public string? NuevoCotitularDni
        {
            get => _nuevoCotitularDni;
            set
            {
                _nuevoCotitularDni = value;
                OnPropertyChanged(nameof(NuevoCotitularDni));
            }
        }

        public ICommand IngresarCommand { get; }
        public ICommand RetirarCommand { get; }
        public ICommand AñadirCotitularCommand { get; }
        public ICommand VolverCommand { get; }

        public ConsultaCuentaVM(INavigationService consultarCuentaNavigationService)
        {
            cuenta = bdd.CargarTmpCuenta()!;
            Saldo = cuenta.Saldo;
            NumCuenta = cuenta.IDCuenta!;

            var u = bdd.BuscarUsuario(cuenta.DniTitular!);
            Titular = u != null ? $"{u.Nombre} {u.Apellido}" : cuenta.DniTitular!;

            var listaNombres = (cuenta.DniCoTitular ?? new List<string>()).Select(dni =>
            {
                var user = bdd.BuscarUsuario(dni);
                return user != null ? $"{user.Nombre} {user.Apellido} ({dni})" : dni;
            });
            Cotitulares = new ObservableCollection<string>(listaNombres);

            var movs = cuenta.Movimientos ?? new List<MovimientosStore>();
            ListaMovimientos = new ObservableCollection<MovimientosStore>(movs);

            // Comandos
            IngresarCommand = new IngresarCommand(this);
            RetirarCommand = new RetirarCommand(this);
            AñadirCotitularCommand = new AñadirCotitularCommand(this);
            VolverCommand = new NavigateCommand(consultarCuentaNavigationService);
        }

        public void RegistrarNuevoMovimiento(string tipo, double monto)
        {
            if (monto != 0)
            {
                var nuevoMov = new MovimientosStore
                {
                    Movimiento = tipo,
                    Dia = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    Total = (tipo == "Retirada" ? "-" : "+") + monto.ToString("N2") + "€",
                };

                ListaMovimientos.Insert(0, nuevoMov);

                if (cuenta.Movimientos == null)
                    cuenta.Movimientos = new List<MovimientosStore>();
                cuenta.Movimientos.Add(nuevoMov);

                var todas = bdd.CargarCuentas();
                var index = todas.FindIndex(c => c.IDCuenta == cuenta.IDCuenta);
                if (index != -1)
                {
                    todas[index] = cuenta;
                    bdd.GuardarCuentas(todas);
                }
            }
        }
    }
}
