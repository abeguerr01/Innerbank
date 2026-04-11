using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Innerbank.Comandos;
using Innerbank.Servicios;
using Innerbank.Stores;

namespace Innerbank.VistaModelo
{
    /// <summary>
    /// Esta clase se encarga de manejar todo el VisualModel de "CntraseñaVerTodo"
    /// </summary>
    public class CntraseñaVerTodoVM : VistaModeloBase
    {
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

        public ICommand ConfirmarContraseña { get; }
        public ICommand CancelarVerTodoCommand { get; }

        public CntraseñaVerTodoVM(INavigationService servicioCerrar)
        {
            ConfirmarContraseña = new ConfirmarContraseñaCommand(this);
            CancelarVerTodoCommand = new NavigateCommand(servicioCerrar);
        }
    }
}
