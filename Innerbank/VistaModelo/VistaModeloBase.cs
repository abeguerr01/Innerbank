using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innerbank.Vistas;

namespace Innerbank.VistaModelo
{
    /// <summary>
    /// El ViewModel por defecto, el que define en que escea o pantalla estamos
    /// </summary>
    public class VistaModeloBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
