using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innerbank.Stores
{
    public class MovimientosStore
    {
        public string? Movimiento { get; set; }
        public string? Dia { get; set; }
        public string? Total { get; set; }

        public override string ToString()
        {
            return $"{Movimiento}, {Dia}, {Total}";
        }
    }
}
