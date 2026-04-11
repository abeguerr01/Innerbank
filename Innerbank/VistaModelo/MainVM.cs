using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innerbank.Modelos;
using Innerbank.Stores;

namespace Innerbank.VistaModelo
{
    /// <summary>
    /// Esta clase es la base o plantilla todos los VisualModel
    /// </summary>
    public class MainVM : VistaModeloBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore? _modalNavigationStore;

        public VistaModeloBase CuerrentViewModel => _navigationStore.CurrentViewModel;
        public VistaModeloBase CurrentModalViewModel => _modalNavigationStore!.CurrentViewModel;
        public bool IsModalOpen => _modalNavigationStore!.IsOpen;

        public MainVM(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _modalNavigationStore.CurrentViewModelChanged += OnCurrentModalViewModelChanged;
        }

        public MainVM(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CuerrentViewModel));
        }

        public void OnCurrentModalViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentModalViewModel));
            OnPropertyChanged(nameof(IsModalOpen));
        }
    }
}
