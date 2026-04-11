using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innerbank.Stores;

namespace Innerbank.Modelos
{
    /// <summary>
    /// Este es el modelo de las cuentas, su estructura y metodos.
    /// </summary>
    public class Cuenta
    {
        public string? IDCuenta { get; set; }
        public string? DniTitular { get; set; }
        public List<string> DniCoTitular { get; set; }
        public double Saldo { get; set; }
        public List<MovimientosStore> Movimientos { get; set; }

        public Cuenta()
        {
            DniCoTitular = new List<string>();
            Movimientos = new List<MovimientosStore>();
        }

        public Cuenta(
            string idCuenta,
            string titular,
            List<string> coTitular,
            double saldo,
            List<MovimientosStore> movimientos
        )
        {
            IDCuenta = idCuenta;
            DniTitular = titular;
            DniCoTitular = coTitular ?? new List<string>();
            Saldo = saldo;
            Movimientos = movimientos;
        }

        public Cuenta(string idCuenta, string titular, double saldo)
            : this()
        {
            IDCuenta = idCuenta;
            DniTitular = titular;
            Saldo = saldo;
        }

        public override string ToString()
        {
            StringBuilder sbCotit = new StringBuilder();
            foreach (string item in DniCoTitular)
            {
                sbCotit.Append($"{item}, ");
            }

            if (sbCotit.Length >= 2)
            {
                sbCotit.Remove(sbCotit.Length - 2, 2);
            }

            StringBuilder sbMovimientos = new StringBuilder();
            if (Movimientos != null)
            {
                foreach (object item in Movimientos)
                {
                    sbMovimientos.Append($"{item.ToString()}, ");
                }

                if (sbMovimientos.Length >= 2)
                {
                    sbMovimientos.Remove(sbMovimientos.Length - 2, 2);
                }
            }

            return $"ID:{IDCuenta}, Titular:{DniTitular}, Cotitular/es:{sbCotit}, Saldo:{Saldo}, Movimientos:{sbMovimientos}";
        }
    }
}
