using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innerbank.Stores;
using Innerbank.VistaModelo;

namespace Innerbank.Servicios
{
    /// <summary>
    /// Este serviico se encarga de manejar el control de escena o pantalla de manera generica
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public class NavigationService<TViewModel> : INavigationService
        where TViewModel : VistaModeloBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TViewModel> _createViewModel;

        public NavigationService(NavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = _createViewModel();
        }
    }
}
