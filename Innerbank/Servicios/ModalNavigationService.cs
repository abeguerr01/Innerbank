using System;
using System.Text;
using System.Threading.Tasks;
using Innerbank.Stores;
using Innerbank.VistaModelo;

namespace Innerbank.Servicios
{
    /// <summary>
    /// Este servicio se encarga de manejar el control de modales de manera generica
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public class ModalNavigationService<TViewModel> : INavigationService
        where TViewModel : VistaModeloBase
    {
        private readonly ModalNavigationStore _navigationStore;
        private readonly Func<TViewModel> _createViewModel;

        public ModalNavigationService(
            ModalNavigationStore navigationStore,
            Func<TViewModel> createViewModel
        )
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
