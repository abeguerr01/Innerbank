using System.Diagnostics;
using System.IO;
using System.IO.Hashing;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Shapes;
using Innerbank.Modelos;
using Innerbank.Servicios;

namespace Innerbank
{
    /// <summary>
    /// Este servicio se encarga de manejar y guardar los datos en ficheros de texto.
    /// </summary>
    public class ControlDatosFicheroService : IControlDatos
    {
        private static string VERSION = "V3";

        //Rutas
        private static string RUTA_USUARIOS { get; } = "innerbankdata/usuarios.innerbankdata";
        private static string RUTA_TMPUSUARIO { get; } = "innerbankdata/tmpUsuario.innerbankdata";
        private static string RUTA_CUENTAS { get; } = "innerbankdata/cuentas.innerbankdata";
        private static string RUTA_TMPCUENTA { get; } = "innerbankdata/tmpCuentas.innerbankdata";
        public static string RUTA { get; } = "innerbankdata/";

        // Variables de control
        private static string _llaveCifrado = "12345678901234567890123456789012"; //32 bytes
        private static string _ivCifrado = "1234567890123456"; //16 bytes
        public Usuario? TmpUsuario;
        public List<Usuario> LUsuarios = [];
        public JsonArray JUsuarios = [];
        public List<Cuenta> LCuentas = [];
        public JsonArray JCuentas = [];

        /// <summary>
        /// El constructor vacio creará el directorio si no existe y carga todos los datos de cuentas y usuarios.
        /// </summary>
        public ControlDatosFicheroService()
        {
            if (!Directory.Exists("innerbankdata"))
            {
                Directory.CreateDirectory("innerbankdata");
            }
            LUsuarios = CargarUsuarios();
            LCuentas = CargarCuentas();
        }

        /// <summary>
        /// Este método calcula y devuelve el crc de un archivo en formato hexadecimal.
        /// </summary>
        /// <param name="texto"></param>
        /// <returns>string</returns>
        private static string CalcularCrc32DeTexto(string texto)
        {
            Crc32 crc = new Crc32();
            byte[] bytes = Encoding.UTF8.GetBytes(texto);
            crc.Append(bytes);
            byte[] hashBytes = crc.GetCurrentHash();

            if (BitConverter.IsLittleEndian)
                Array.Reverse(hashBytes);
            return BitConverter.ToUInt32(hashBytes, 0).ToString("X8");
        }

        /// <summary>
        /// En caso de que "VerificarCrcTodo" sea false, este método se encarga de mover todos los ficheros a un directorio "backup" para que no se puedan usar pero tampoco perder.
        /// </summary>
        public static void ErrorValidacion()
        {
            MessageBox.Show(
                "Los archivos han sido alterados, por lo que no son validos, no se podrán usar.",
                "DATOS ALTERADOS",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            const string ORIGEN = "innerbankdata";
            string DESTINO = System.IO.Path.Combine(ORIGEN, "backup");

            if (!Directory.Exists(DESTINO))
            {
                Directory.CreateDirectory(DESTINO);
            }

            string[] archivos = Directory.GetFiles(ORIGEN);

            foreach (string RUTA_ARCHIVO in archivos)
            {
                string NOMBRE_ARCHIVO = System.IO.Path.GetFileName(RUTA_ARCHIVO);

                string rutaDestino = System.IO.Path.Combine(DESTINO, NOMBRE_ARCHIVO);

                try
                {
                    if (File.Exists(rutaDestino))
                    {
                        File.Delete(rutaDestino);
                    }

                    File.Move(RUTA_ARCHIVO, rutaDestino);
                    Console.WriteLine($"Movido: {NOMBRE_ARCHIVO}");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"{ex.Message}", $"Error al mover {NOMBRE_ARCHIVO}");
                }
            }
        }

        /// <summary>
        /// Este método cifra y escribe ese contenido en un fichero para que sea ilegible para una persona.
        /// </summary>
        /// <param name="ruta">La ruta en donde se guardara el fichero</param>
        /// <param name="contenido">El texto que contendrá el archivo cifrado</param>
        private static void escribirArchivoCifrado(string ruta, string contenido)
        {
            byte[] llave = Encoding.UTF8.GetBytes(_llaveCifrado);
            byte[] iv = Encoding.UTF8.GetBytes(_ivCifrado);

            using (Aes aes = Aes.Create())
            {
                aes.Key = llave;
                aes.IV = iv;

                using (FileStream fs = new FileStream(ruta, FileMode.Create))
                {
                    using (
                        CryptoStream cs = new CryptoStream(
                            fs,
                            aes.CreateEncryptor(),
                            CryptoStreamMode.Write
                        )
                    )
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(contenido);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Este método se encarga de traducir el fichero cifrado y devolverlo en un lenguaje legible.
        /// </summary>
        /// <param name="ruta">La ruta del archivo cifrado a leer</param>
        /// <returns>string</returns>
        private static string leerArchivoCifrado(string ruta)
        {
            try
            {
                byte[] llave = Encoding.UTF8.GetBytes(_llaveCifrado);
                byte[] iv = Encoding.UTF8.GetBytes(_ivCifrado);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = llave;
                    aes.IV = iv;

                    using (FileStream fs = new FileStream(ruta, FileMode.Open))
                    {
                        using (
                            CryptoStream cs = new CryptoStream(
                                fs,
                                aes.CreateDecryptor(),
                                CryptoStreamMode.Read
                            )
                        )
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                JsonObject jsonVacio = new JsonObject();
                ErrorValidacion();
                return jsonVacio.ToJsonString();
            }
        }

        /// <summary>
        /// Este metodo guarda todos los datos en un fichero json legible para las personas
        /// </summary>
        /// <param name="ruta">La ruta donde se guardaran los ficheros</param>
        public void GuardarArchivosSinCifrar(string ruta)
        {
            GuardarJsonUsuariosSinCifrar(LUsuarios, ruta + "\\usuarios.json");

            GuardarJsonCuentasSinCifrar(LCuentas, ruta + "\\cuentas.json");
        }

        /// <summary>
        /// Este método añade un usuario a la memoria y guarda el cambio en el fichero.
        /// </summary>
        /// <param name="user">El usuario que se guardara</param>
        public void AñadirUsuario(Usuario user)
        {
            LUsuarios.Add(user);
            GuardarUsuarios(LUsuarios);
        }

        /// <summary>
        /// Este método elimina un usuario de la memoria buscandolo mediante su DNI y guarda el cambio en el fichero.
        /// </summary>
        /// <param name="dni">El dni que se guardara</param>
        public void BorrarUsuario(string dni)
        {
            LUsuarios.RemoveAll(x => x.Dni == dni);
            GuardarUsuarios(LUsuarios);
        }

        /// <summary>
        /// Este método añade una cuenta a la memoria y guarda el cambio en el fichero.
        /// </summary>
        /// <param name="cuenta">La cuenta que se guardara</param>
        public void AñadirCuenta(Cuenta cuenta)
        {
            LCuentas.Add(cuenta);
            GuardarCuentas(LCuentas);
        }

        /// <summary>
        /// Este método elimina una cuenta de la memoria mediante su ID
        /// </summary>
        /// <remarks>
        /// SIN USO
        /// </remarks>
        /// <param name="id">El id que se utilizara para encontrar la cuenta</param>
        public void BorrarCuenta(string id)
        {
            LCuentas.RemoveAll(x => x.IDCuenta!.ToString() == id);
            GuardarCuentas(LCuentas);
        }

        /// <summary>
        /// Este método guarda en el fichero la lista de usuarios cifrada y con su crc.
        /// </summary>
        /// <param name="data">La lista de usuarios que se guardara en el archivo</param>
        public void GuardarUsuarios(List<Usuario> data)
        {
            GuardarJsonUsuarios(data, RUTA_USUARIOS);
        }

        /// <summary>
        /// Este método guarda en el fichero la lista de cuentas cifrada y con su crc.
        /// </summary>
        /// <param name="data">La lista de cuentas que se guardara en el archivo</param>
        public void GuardarCuentas(List<Cuenta> data)
        {
            GuardarJsonCuentas(data, RUTA_CUENTAS);
        }

        /// <summary>
        /// Este método lee del fichero la lista de usuarios cifrada y los trae a la memoria.
        /// </summary>
        /// <returns></returns>
        public List<Usuario> CargarUsuarios()
        {
            List<Usuario> lista = new List<Usuario>();
            if (!File.Exists(RUTA_USUARIOS))
                return lista;

            return LeerJsonUsuarios(RUTA_USUARIOS);
        }

        /// <summary>
        /// Este método lee del fichero la lista de cuentas cifrada y las trae a la memoria
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        public List<Cuenta> CargarCuentas()
        {
            List<Cuenta> lista = new List<Cuenta>();
            if (!File.Exists(RUTA_CUENTAS))
                return lista;

            return LeerJsonCuentas(RUTA_CUENTAS);
        }

        /// <summary>
        /// Devuelve el objeto usuario deseado de quien coincida con el DNI introducido.
        /// </summary>
        /// <param name="dni">El dni con el cual se encontrará al usuario</param>
        /// <returns>Usuario(acepta nulos)</returns>
        public Usuario? BuscarUsuario(string dni)
        {
            return LUsuarios.FirstOrDefault(x => x.Dni == dni);
        }

        /// <summary>
        /// Guarda el usuario deseado indicado por su DNI y lo guarda en un fichero temporal cifrada para leerlo por separado.
        /// </summary>
        /// <param name="dni">El dni con el cual se encontrará al usuario</param>
        public void SeleccionarUsuario(string dni)
        {
            TmpUsuario = BuscarUsuario(dni);
            if (TmpUsuario == null)
                return;

            string contenidoJson = JsonSerializer.Serialize(TmpUsuario);

            escribirArchivoCifrado(RUTA_TMPUSUARIO, contenidoJson);
        }

        /// <summary>
        /// Carga el usuario guardado en el fichero temporal.
        /// </summary>
        /// <returns>Usuario(acepta nulos)</returns>
        public Usuario? CargarTmpUsuario()
        {
            try
            {
                string jsonDescifrado = leerArchivoCifrado(RUTA_TMPUSUARIO);

                if (!string.IsNullOrWhiteSpace(jsonDescifrado))
                {
                    return JsonSerializer.Deserialize<Usuario>(jsonDescifrado);
                }
            }
            catch (CryptographicException)
            {
                MessageBox.Show(
                    "El archivo ha sido manipulado o la llave es incorrecta",
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }

            return null;
        }

        /// <summary>
        /// Guarda la cuenta ingresada y la guarda en un fichero temporal para leerlo por separado.
        /// </summary>
        /// <param name="c">La cuenta que se guardará</param>
        public void SeleccionarCuenta(Cuenta c)
        {
            string contenidoJson = JsonSerializer.Serialize(c);

            escribirArchivoCifrado(RUTA_TMPCUENTA, contenidoJson);
        }

        /// <summary>
        /// Carga la cuenta guardada en el fichero temporal.
        /// </summary>
        /// <returns>Cuenta(acepta nulos)</returns>
        public Cuenta? CargarTmpCuenta()
        {
            try
            {
                string jsonDescifrado = leerArchivoCifrado(RUTA_TMPCUENTA);

                if (!string.IsNullOrWhiteSpace(jsonDescifrado))
                {
                    return JsonSerializer.Deserialize<Cuenta>(jsonDescifrado);
                }
            }
            catch (CryptographicException)
            {
                MessageBox.Show(
                    "El archivo ha sido manipulado o la llave es incorrecta",
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Error al cargar el archivo:\n{ex}", "ERROR");
            }

            return null;
        }

        /// <summary>
        /// Actualiza los valores de la cuenta en el fichero global.
        /// </summary>
        /// <param name="idCuenta">El id con el cual se encontrará la cuenta</param>
        /// <param name="nuevoSaldo">El nuevo saldo que tendrá la cuenta</param>
        public void ActualizarSaldoCuenta(string idCuenta, double nuevoSaldo)
        {
            var cuenta = LCuentas.FirstOrDefault(c => c.IDCuenta == idCuenta);

            if (cuenta != null)
            {
                cuenta.Saldo = nuevoSaldo;

                GuardarCuentas(LCuentas);

                SeleccionarCuenta(cuenta);
            }
        }

        /// <summary>
        /// Añadimos un cotitular a la cuenta.
        /// </summary>
        /// <param name="idCuenta">El id con el cual se encontrará la cuenta</param>
        /// <param name="dniNuevo">El dni con el cual se encontrará al nuevo cotitular</param>
        public void AñadirCoTitularCuenta(string idCuenta, string dniNuevo)
        {
            var cuenta = LCuentas.FirstOrDefault(c => c.IDCuenta == idCuenta);

            if (cuenta != null)
            {
                if (cuenta.DniCoTitular == null)
                    cuenta.DniCoTitular = new List<string>();

                if (!cuenta.DniCoTitular.Contains(dniNuevo))
                {
                    cuenta.DniCoTitular.Add(dniNuevo);

                    GuardarCuentas(LCuentas);
                }
            }
        }

        /// <summary>
        /// Guarda las cuentas en un fichero json y se cifra
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ruta"></param>
        public static void GuardarJsonCuentas(List<Cuenta> data, string ruta)
        {
            JsonArray arrayCuentas = new JsonArray(
                data.Select(c => JsonSerializer.SerializeToNode(c)).ToArray()
            );

            JsonObject root = new JsonObject
            {
                ["crc"] = CalcularCrc32DeTexto(arrayCuentas.ToJsonString()),
                ["versión"] = VERSION,
                ["cuentas"] = arrayCuentas,
            };

            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            string contenidoJson = root.ToJsonString(opciones);

            escribirArchivoCifrado(ruta, contenidoJson);
        }

        /// <summary>
        /// Guarda las cuentas en un fichero json sin cifrar
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ruta"></param>
        public void GuardarJsonCuentasSinCifrar(List<Cuenta> data, string ruta)
        {
            JsonArray arrayCuentas = new JsonArray(
                data.Select(c => JsonSerializer.SerializeToNode(c)).ToArray()
            );

            JsonObject root = new JsonObject
            {
                ["crc"] = CalcularCrc32DeTexto(arrayCuentas.ToJsonString()),
                ["versión"] = VERSION,
                ["cuentas"] = arrayCuentas,
            };

            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            string contenidoJson = root.ToJsonString(opciones);

            File.WriteAllText(ruta, contenidoJson);
        }

        /// <summary>
        /// Lee el fichero de cuentas cifrado y en formato json
        /// Tambien se maneja el crc y la version
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public List<Cuenta> LeerJsonCuentas(string ruta)
        {
            if (!File.Exists(ruta))
            {
                return new List<Cuenta>();
            }

            // Deserialización
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
            };

            string jsonTexto = leerArchivoCifrado(RUTA_CUENTAS);
            JsonObject root = JsonNode.Parse(jsonTexto)!.AsObject();

            string crc = root["crc"]?.ToString() ?? "";
            string version = root["versión"]?.ToString() ?? "";
            JsonArray? cuentasNodo = root["cuentas"]?.AsArray();

            if (cuentasNodo == null)
                return new List<Cuenta>();

            if (CalcularCrc32DeTexto(cuentasNodo.ToJsonString()) != crc)
            {
                ErrorValidacion();
                return new List<Cuenta>();
            }

            if (version != VERSION)
            {
                int v = Convert.ToInt32(version[1]);
                int ver = Convert.ToInt32(VERSION[1]);

                if (v > ver)
                {
                    MessageBox.Show("Version superior o no valida");
                    ErrorValidacion();
                }
                if (v < ver)
                {
                    try
                    {
                        return cuentasNodo.Deserialize<List<Cuenta>>(opciones)
                            ?? new List<Cuenta>();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return new List<Cuenta>();
                    }
                }
            }

            return cuentasNodo.Deserialize<List<Cuenta>>(opciones) ?? new List<Cuenta>();
        }

        /// <summary>
        /// Guarda los usuarios en un fichero json cifrado
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ruta"></param>
        public static void GuardarJsonUsuarios(List<Usuario> data, string ruta)
        {
            JsonArray arrayUsuarios = new JsonArray(
                data.Select(u => JsonSerializer.SerializeToNode(u)).ToArray()
            );

            JsonObject root = new JsonObject
            {
                ["crc"] = CalcularCrc32DeTexto(arrayUsuarios.ToJsonString()),
                ["versión"] = VERSION,
                ["usuarios"] = arrayUsuarios,
            };

            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            string contenidoJson = root.ToJsonString(opciones);

            escribirArchivoCifrado(ruta, contenidoJson);
        }

        /// <summary>
        /// Guarda las cuentas en un fichero json sin cifrar
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ruta"></param>
        public void GuardarJsonUsuariosSinCifrar(List<Usuario> data, string ruta)
        {
            JsonArray arrayUsuarios = new JsonArray(
                data.Select(u => JsonSerializer.SerializeToNode(u)).ToArray()
            );

            JsonObject root = new JsonObject
            {
                ["crc"] = CalcularCrc32DeTexto(arrayUsuarios.ToJsonString()),
                ["versión"] = VERSION,
                ["usuarios"] = arrayUsuarios,
            };

            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            string contenidoJson = root.ToJsonString(opciones);

            File.WriteAllText(ruta, contenidoJson);
        }

        /// <summary>
        /// Lee el fichero de usuarios cifrado y en formato json.
        /// Tambien se maneja el crc y la version
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public List<Usuario> LeerJsonUsuarios(string ruta)
        {
            if (!File.Exists(ruta))
            {
                return new List<Usuario>();
            }

            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
            };

            string jsonTexto = leerArchivoCifrado(ruta);
            JsonObject root = JsonNode.Parse(jsonTexto)!.AsObject();

            string crc = root["crc"]?.ToString() ?? "";
            string version = root["versión"]?.ToString() ?? "";
            JsonArray? usuariosNodo = root["usuarios"]?.AsArray();

            if (usuariosNodo == null)
                return new List<Usuario>();

            if (CalcularCrc32DeTexto(usuariosNodo.ToJsonString()) != crc)
            {
                ErrorValidacion();
                return new List<Usuario>();
            }

            if (version != VERSION)
            {
                int v = Convert.ToInt32(version[1]);
                int ver = Convert.ToInt32(VERSION[1]);

                if (v > ver)
                {
                    MessageBox.Show("Version superior o no valida");
                    ErrorValidacion();
                }
                if (v < ver)
                {
                    try
                    {
                        return usuariosNodo.Deserialize<List<Usuario>>(opciones)
                            ?? new List<Usuario>();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return new List<Usuario>();
                    }
                }
            }

            return usuariosNodo.Deserialize<List<Usuario>>(opciones) ?? new List<Usuario>();
        }

        public static void ControlDeVersiones(string usuarios_cuentas)
        {
            if (usuarios_cuentas.Equals("usuarios"))
            {
                if (!File.Exists(RUTA_USUARIOS))
                {
                    return;
                }

                string? vv;
                string v;
                try
                {
                    var opciones = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        PropertyNameCaseInsensitive = true,
                    };

                    string jsonTexto = leerArchivoCifrado(RUTA_USUARIOS);
                    JsonObject root = JsonNode.Parse(jsonTexto)!.AsObject();

                    v = root["versión"]!.ToString();
                }
                catch
                {
                    Match match = Match.Empty;
                    using (StringReader sr = new StringReader(leerArchivoCifrado(RUTA_USUARIOS)))
                    {
                        sr.ReadToEnd();
                        vv = sr.ReadLine();
                        match = Regex.Match(vv, @"\bV\d+\b"); //Solo cogerá V_ independientemente de lo que vaya seguido
                        if (!match.Success)
                        {
                            vv = sr.ReadLine(); //Leo la linea siguiente
                        }
                    }
                    // El patrón "V\d+" busca una 'V' seguida de uno o más dígitos
                    match = Regex.Match(vv, @"V\d+");

                    if (match.Success)
                    {
                        v = match.Value;
                        Debug.WriteLine($"Versión encontrada: {v}");
                    }
                    else
                    {
                        v = "V0";
                        Debug.WriteLine("Versión no encontrada: V0");
                    }
                }

                if (v.Equals("V0"))
                {
                    v0aVActual(RUTA_USUARIOS);
                }
                else if (v.Equals("V1"))
                {
                    v1aVActual(RUTA_USUARIOS);
                }
                else if (v.Equals("V2"))
                {
                    v2aVActual(RUTA_USUARIOS);
                }
                else if (v.Equals("V3"))
                {
                    Debug.WriteLine("Version correcta");
                }
                else
                {
                    MessageBox.Show("Imposible actualizar los datos a esta nueva version");
                    ErrorValidacion();
                }

                void v0aVActual(string ruta)
                {
                    List<Usuario> lusuario = new List<Usuario>();
                    string? line;

                    using (var sr = new StringReader(ruta))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                Usuario usuario = JsonSerializer.Deserialize<Usuario>(line)!;
                                lusuario.Add(usuario);
                            }
                        }
                    }
                    GuardarJsonUsuarios(lusuario, RUTA_USUARIOS);
                }

                void v1aVActual(string ruta)
                {
                    List<Usuario> lusuario = new List<Usuario>();
                    string? line;

                    using (var sr = new StringReader(ruta))
                    {
                        sr.ReadLine();
                        sr.ReadLine();
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                Usuario usuario = JsonSerializer.Deserialize<Usuario>(line)!;
                                lusuario.Add(usuario);
                            }
                        }
                    }
                    GuardarJsonUsuarios(lusuario, RUTA_USUARIOS);
                }
                void v2aVActual(string ruta)
                {
                    List<Usuario> lusuario = new List<Usuario>();
                    string? line;

                    using (var sr = new StringReader(ruta))
                    {
                        sr.ReadLine();
                        sr.ReadLine();
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                Usuario usuario = JsonSerializer.Deserialize<Usuario>(line)!;
                                lusuario.Add(usuario);
                            }
                        }
                    }
                    GuardarJsonUsuarios(lusuario, RUTA_USUARIOS);
                }
            }
            else if (usuarios_cuentas.Equals("cuentas"))
            {
                if (!File.Exists(RUTA_CUENTAS))
                {
                    return;
                }

                string? vv;
                string v;
                try
                {
                    var opciones = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        PropertyNameCaseInsensitive = true,
                    };

                    string jsonTexto = leerArchivoCifrado(RUTA_CUENTAS);
                    JsonObject root = JsonNode.Parse(jsonTexto)!.AsObject();

                    v = root["versión"]!.ToString();
                }
                catch
                {
                    Match match = Match.Empty;
                    using (StringReader sr = new StringReader(leerArchivoCifrado(RUTA_CUENTAS)))
                    {
                        sr.ReadLine();
                        vv = sr.ReadLine();
                        match = Regex.Match(vv, @"\bV\d+\b"); //Solo cogerá V_ independientemente de lo que vaya seguido
                        if (!match.Success)
                        {
                            vv = sr.ReadLine(); //Leo la linea siguiente
                        }
                    }
                    // El patrón "V\d+" busca una 'V' seguida de uno o más dígitos
                    match = Regex.Match(vv, @"V\d+");

                    if (match.Success)
                    {
                        v = match.Value;
                        Debug.WriteLine($"Versión encontrada: {v}");
                    }
                    else
                    {
                        v = "V0";
                        Debug.WriteLine("Versión no encontrada: V0");
                    }
                }

                if (v.Equals("V0"))
                {
                    v0aVActual(RUTA_CUENTAS);
                }
                else if (v.Equals("V1"))
                {
                    v1aVActual(RUTA_CUENTAS);
                }
                else if (v.Equals("V2"))
                {
                    v2aVActual(RUTA_CUENTAS);
                }
                else if (v.Equals("V3"))
                {
                    Debug.WriteLine("Version correcta");
                }
                else
                {
                    MessageBox.Show("Imposible actualizar los datos a esta nueva version");
                    ErrorValidacion();
                }

                void v0aVActual(string ruta)
                {
                    List<Cuenta> lcuenta = new List<Cuenta>();
                    string? line;

                    using (var sr = new StringReader(ruta))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                Cuenta cuenta = JsonSerializer.Deserialize<Cuenta>(line)!;
                                lcuenta.Add(cuenta);
                            }
                        }
                    }
                    GuardarJsonCuentas(lcuenta, RUTA_USUARIOS);
                }

                void v1aVActual(string ruta)
                {
                    List<Usuario> lusuario = new List<Usuario>();
                    string? line;

                    using (var sr = new StringReader(ruta))
                    {
                        sr.ReadLine();
                        sr.ReadLine();
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                Usuario usuario = JsonSerializer.Deserialize<Usuario>(line)!;
                                lusuario.Add(usuario);
                            }
                        }
                    }
                    GuardarJsonUsuarios(lusuario, RUTA_USUARIOS);
                }
                void v2aVActual(string ruta)
                {
                    List<Cuenta> lcuenta = new List<Cuenta>();
                    string? line;

                    using (var sr = new StringReader(ruta))
                    {
                        sr.ReadLine();
                        sr.ReadLine();
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                Cuenta cuenta = JsonSerializer.Deserialize<Cuenta>(line)!;
                                lcuenta.Add(cuenta);
                            }
                        }
                    }
                    GuardarJsonCuentas(lcuenta, RUTA_CUENTAS);
                }
            }
            else
            {
                ErrorValidacion();
            }
        }
    }
}
