using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innerbank.Commands;
using Innerbank.Modelos;
using Innerbank.Servicios;
using Innerbank.Stores;
using Innerbank.VistaModelo;

namespace Innerbank.Comandos
{
    /// <summary>
    /// Este comando permite navegar entre pantallas
    /// </summary>
    public class NavigateCommand : ComandoBase
    {
        private readonly INavigationService _navigationService;

        public NavigateCommand(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Execute(object? parameter)
        {
            _navigationService.Navigate();
        }
    }
}
