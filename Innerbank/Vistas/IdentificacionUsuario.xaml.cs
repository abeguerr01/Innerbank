using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Innerbank.Stores;

namespace Innerbank.Vistas
{
    /// <summary>
    /// Lógica de interacción para LogRegUsuario.xaml
    /// </summary>
    public partial class IdentificacionUsuario : UserControl
    {
        private NavigationStore? navigationStore;

        public IdentificacionUsuario()
        {
            InitializeComponent();
        }

        public IdentificacionUsuario(NavigationStore navigationStore)
        {
            this.navigationStore = navigationStore;
        }
    }
}
