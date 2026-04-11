using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innerbank.VistaModelo;

namespace Innerbank.Stores
{
    /// <summary>
    /// Este store se encarga de manejar el control de escena o pantalla
    /// </summary>
    public class NavigationStore
    {
        public VistaModeloBase? _currentViewModel { get; private set; }
        public VistaModeloBase CurrentViewModel
        {
            get => _currentViewModel!;
            set
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        public event Action? CurrentViewModelChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
