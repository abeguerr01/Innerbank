using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Innerbank.Modelos
{
    /// <summary>
    /// Este es el modelo del banco, su estructura y metodos.
    /// </summary>
    public class Banco
    {
        public string Nombre { get; set; }
        public static List<Usuario> Usuarios { get; } = [];

        public Banco(string nombre)
        {
            Nombre = nombre;
        }

        public void CrearUsuario(
            string dni,
            string nombre,
            string apellido,
            string email,
            string telefono,
            string contraseña
        )
        {
            Usuario user = new Usuario(dni, nombre, apellido, email, telefono, contraseña);
            Usuarios.Add(user);
        }

        public static void AñadirUsuario(Usuario user)
        {
            Usuarios.Add(user);
        }
    }
}
