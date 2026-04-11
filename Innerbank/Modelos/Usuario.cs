using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace Innerbank.Modelos
{
    /// <summary>
    /// Este es el modelo de los usuarios, su estructura y metodos.
    /// </summary>
    /// <param name="dni"></param>
    /// <param name="nombre"></param>
    /// <param name="apellido"></param>
    /// <param name="email"></param>
    /// <param name="telefono"></param>
    /// <param name="contraseña"></param>
    public class Usuario(
        string dni,
        string nombre,
        string apellido,
        string email,
        string telefono,
        string contraseña
    )
    {
        public string Dni { get; set; } = dni;
        public string Nombre { get; set; } = nombre;
        public string Apellido { get; set; } = apellido;
        public string Email { get; set; } = email;
        public string Telefono { get; set; } = telefono;
        public string Contraseña { get; set; } = contraseña;

        [JsonIgnore]
        public List<Cuenta> CuentasTitular { get; set; } = [];

        [JsonIgnore]
        public List<Cuenta> CuentasCotiTular { get; set; } = [];

        public static string generarNumeroCuenta()
        {
            Random random = new Random();
            string numeroCuenta = string.Empty;
            for (int i = 0; i < 22; i++)
            {
                numeroCuenta += random.Next(0, 10);
            }
            return "ES" + numeroCuenta;
        }

        public override string ToString()
        {
            return $"DNI:{Dni}, Nombre:{Nombre} {Apellido}, Correo:{Email}, Telefono:{Telefono}, Contraseña:{Contraseña}";
        }
    }
}
