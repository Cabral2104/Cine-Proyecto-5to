using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Cine
{
    internal class Program
    {
        static int idUsuarioAutenticado = -1;
        static void Main(string[] args)
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=CineCH;Integrated Security=True;TrustServerCertificate=True;";

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                Console.WriteLine("Conexión EXITOSA");

                // Llamar al método de autenticación
                if (!AutenticarUsuario(connection))
                {
                    Console.WriteLine("Autenticación fallida. Saliendo...");
                    return;
                }
                connection.Close();
                int opcion;
                do
                {
                    connection.Open();
                    // Menú principal
                    Console.Clear();
                    Console.WriteLine("Bienvenido al sistema de gestión");
                    Console.WriteLine("1. Gestión de Empleados");
                    Console.WriteLine("2. Gestión de Películas");
                    Console.WriteLine("0. Salir");
                    Console.Write("Seleccione una opción: ");

                    // Validar opción
                    if (!int.TryParse(Console.ReadLine(), out opcion))
                    {
                        Console.WriteLine("Opción no válida. Intenta de nuevo.");
                        continue;
                    }

                    switch (opcion)
                    {
                        case 1:
                            GestionEmpleados(connection);  // Submenú de empleados
                            break;
                        case 2:
                            GestionPeliculas(connection);  // Submenú de películas
                            break;
                        case 0:
                            Console.WriteLine("Saliendo...");
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }

                    // Espera antes de continuar
                    Console.WriteLine("\nPresione una tecla para continuar...");
                    Console.ReadKey();

                } while (opcion != 0);
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // Método para autenticar al usuario
        static bool AutenticarUsuario(SqlConnection connection)
        {
           
            Console.WriteLine("Ingrese su nombre de usuario:");
            string username = Console.ReadLine();

            Console.WriteLine("Ingrese su contraseña:");
            string password = Console.ReadLine();

            string query = "SELECT id FROM Usuario WHERE username = @username AND password = @password";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            try
            {
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();  // Leer el primer (y único) registro
                    idUsuarioAutenticado = (int)reader["id"];  // Almacenar el ID del usuario autenticado
                    Console.WriteLine("¡Bienvenido, " + username + "!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Credenciales incorrectas.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al autenticar: " + ex.Message);
                return false;
            }
        }

        // Función para gestionar empleados
        static void GestionEmpleados(SqlConnection connection)
        {
            int opcionEmpleado;
            do
            {
                // Menú de gestión de empleados
                Console.Clear();
                Console.WriteLine("Gestión de Empleados");
                Console.WriteLine("1. Insertar Empleado");
                Console.WriteLine("2. Ver Empleados");
                Console.WriteLine("3. Actualizar Empleado");
                Console.WriteLine("0. Regresar al menú principal");
                Console.Write("Seleccione una opción: ");

                // Validar opción
                if (!int.TryParse(Console.ReadLine(), out opcionEmpleado))
                {
                    Console.WriteLine("Opción no válida. Intenta de nuevo.");
                    continue;
                }

                switch (opcionEmpleado)
                {
                    case 1:
                        InsertarEmpleado(connection);
                        break;
                    case 2:
                        VerEmpleados(connection);
                        break;
                    case 3:
                        ActualizarEmpleado(connection);
                        break;
                    case 0:
                        Console.WriteLine("Regresando al menú principal...");
                        connection.Close();
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }

                // Espera antes de continuar
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();

            } while (opcionEmpleado != 0);
        }

        // Función para gestionar películas
        static void GestionPeliculas(SqlConnection connection)
        {
            int opcionPelicula;
            do
            {
                // Menú de gestión de películas
                Console.Clear();
                Console.WriteLine("Gestión de Películas");
                Console.WriteLine("1. Insertar Película");
                Console.WriteLine("2. Ver Películas");
                Console.WriteLine("3. Actualizar Película");
                Console.WriteLine("0. Regresar al menú principal");
                Console.Write("Seleccione una opción: ");

                // Validar opción
                if (!int.TryParse(Console.ReadLine(), out opcionPelicula))
                {
                    Console.WriteLine("Opción no válida. Intenta de nuevo.");
                    continue;
                }

                switch (opcionPelicula)
                {
                    case 1:
                        InsertarPelicula(connection);
                        break;
                    case 2:
                        VerPeliculas(connection);
                        break;
                    case 3:
                        ActualizarPelicula(connection);
                        break;
                    case 0:
                        Console.WriteLine("Regresando al menú principal...");
                        connection.Close();
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }

                // Espera antes de continuar
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();

            } while (opcionPelicula != 0);
        }
        // Método para insertar un nuevo empleado
        public static void InsertarEmpleado(SqlConnection connection)
        {
            Console.WriteLine("Ingrese los datos del nuevo empleado:");

            // Obtener los datos del empleado
            Console.Write("Nombres: ");
            string nombres = Console.ReadLine();

            Console.Write("Apellidos: ");
            string apellidos = Console.ReadLine();

            Console.Write("RFC (13 caracteres): ");
            string rfc = Console.ReadLine();

            Console.Write("Fecha de nacimiento (yyyy-mm-dd): ");
            DateTime fechaNacimiento = DateTime.Parse(Console.ReadLine());

            Console.Write("Teléfono (10 dígitos): ");
            string telefono = Console.ReadLine().Replace("(", "").Replace(")", "").Replace("-", ""); // Limpiar caracteres no numéricos

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Fecha de ingreso (yyyy-mm-dd hh:mm:ss): ");
            DateTime fechaIngreso = DateTime.Parse(Console.ReadLine());

            Console.Write("Sueldo: ");
            int sueldo = int.Parse(Console.ReadLine());

            // Mostrar opciones de sucursal
            Console.WriteLine("Seleccione la sucursal:");
            Console.WriteLine("1. Cine_Pape");
            Console.WriteLine("2. Cine_Frontera");
            Console.WriteLine("3. Cine_Castaños");
            Console.Write("Opción: ");
            int sucursalId = int.Parse(Console.ReadLine());

            // Validar la opción seleccionada
            if (sucursalId < 1 || sucursalId > 3)
            {
                Console.WriteLine("Opción inválida. Seleccionando sucursal 'Cine_Pape' por defecto.");
                sucursalId = 1;  // Si la opción es inválida, se asigna la sucursal 'Cine_Pape' por defecto
            }

            // Ingresar los datos en la base de datos
            string query = @"INSERT INTO Empleado (nombres, apellidos, RFC, fecha_nacimiento, telefono, email, fecha_ingreso, sueldo, id_sucursal, id_Usuario_Crea)
                             VALUES (@Nombres, @Apellidos, @RFC, @FechaNacimiento, @Telefono, @Email, @FechaIngreso, @Sueldo, @SucursalId, @idUsuario)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Nombres", nombres);
            command.Parameters.AddWithValue("@Apellidos", apellidos);
            command.Parameters.AddWithValue("@RFC", rfc);
            command.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);
            command.Parameters.AddWithValue("@Telefono", telefono);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@FechaIngreso", fechaIngreso);
            command.Parameters.AddWithValue("@Sueldo", sueldo);
            command.Parameters.AddWithValue("@SucursalId", sucursalId);
            command.Parameters.AddWithValue("@idUsuario", idUsuarioAutenticado);

            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} fila(s) insertada(s) exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar: " + ex.Message);
            }
        }

        // Ver todos los empleados
        public static void VerEmpleados(SqlConnection connection)
        {
            string query = "SELECT id, nombres, apellidos, RFC, fecha_nacimiento, telefono, email, fecha_ingreso, sueldo, id_sucursal FROM Empleado";

            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine("\nEmpleados:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}, Nombre: {reader["nombres"]} {reader["apellidos"]}, RFC: {reader["RFC"]}, Fecha Nacimiento: {reader["fecha_nacimiento"]}, Teléfono: {reader["telefono"]}, Email: {reader["email"]}, Fecha Ingreso: {reader["fecha_ingreso"]}, Sueldo: {reader["sueldo"]}, Sucursal: {reader["id_sucursal"]}");
            }

            reader.Close();
        }

        public static void ActualizarEmpleado(SqlConnection connection)
        {
            Console.Write("Ingrese el ID del empleado que desea actualizar: ");
            int idEmpleado = int.Parse(Console.ReadLine());

            Console.WriteLine("¿Qué desea actualizar?");
            Console.WriteLine("1. Sueldo");
            Console.WriteLine("2. Teléfono");
            Console.WriteLine("3. Email");
            Console.WriteLine("4. Sucursal");
            Console.WriteLine("5. Actualizar Status");
            Console.Write("Seleccione una opción: ");
            int opcion = int.Parse(Console.ReadLine());

            string query = string.Empty;
            SqlCommand command = new SqlCommand() { Connection = connection };

            switch (opcion)
            {
                case 1: // Actualizar Sueldo
                    Console.Write("Ingrese el nuevo sueldo: ");
                    int nuevoSueldo = int.Parse(Console.ReadLine());

                    query = "UPDATE Empleado SET sueldo = @nuevoSueldo, id_usuario_edita = @idUsuario WHERE id = @idEmpleado";
                    command.Parameters.AddWithValue("@nuevoSueldo", nuevoSueldo);
                    break;

                case 2: // Actualizar Teléfono
                    Console.Write("Ingrese el nuevo teléfono (10 dígitos): ");
                    string nuevoTelefono = Console.ReadLine().Replace("(", "").Replace(")", "").Replace("-", ""); // Limpiar caracteres no numéricos

                    query = "UPDATE Empleado SET telefono = @nuevoTelefono, id_usuario_edita = @idUsuario WHERE id = @idEmpleado";
                    command.Parameters.AddWithValue("@nuevoTelefono", nuevoTelefono);
                    break;

                case 3: // Actualizar Email
                    Console.Write("Ingrese el nuevo email: ");
                    string nuevoEmail = Console.ReadLine();

                    query = "UPDATE Empleado SET email = @nuevoEmail, id_usuario_edita = @idUsuario WHERE id = @idEmpleado";
                    command.Parameters.AddWithValue("@nuevoEmail", nuevoEmail);
                    break;

                case 4: // Actualizar Sucursal
                    Console.WriteLine("Seleccione la nueva sucursal:");
                    Console.WriteLine("1. Cine_Pape");
                    Console.WriteLine("2. Cine_Frontera");
                    Console.WriteLine("3. Cine_Castaños");
                    Console.Write("Opción: ");
                    int sucursalId = int.Parse(Console.ReadLine());

                    // Validar la opción seleccionada
                    if (sucursalId < 1 || sucursalId > 3)
                    {
                        Console.WriteLine("Opción no válida.");
                        return;
                    }

                    query = "UPDATE Empleado SET id_sucursal = @sucursalId, id_usuario_edita = @idUsuario WHERE id = @idEmpleado";
                    command.Parameters.AddWithValue("@sucursalId", sucursalId);
                    break;

                case 5: // Cambiar el estado de Activo a Inactivo
                    Console.Write("Ingresa el estado del Empleado (1: Activo, 0: Inactivo): ");
                    int estadoEntidad = int.Parse(Console.ReadLine());

                    query = "UPDATE Empleado SET status = @nuevoStatus, id_usuario_edita = @idUsuario WHERE id = @idEmpleado";
                    command.Parameters.AddWithValue("@nuevoStatus", estadoEntidad);
                    break;

                default:
                    Console.WriteLine("Opción no válida.");
                    return;
            }

            // Agregar parámetros comunes
            command.CommandText = query;
            command.Parameters.AddWithValue("@idEmpleado", idEmpleado);
            command.Parameters.AddWithValue("@idUsuario", idUsuarioAutenticado);

            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Empleado actualizado correctamente.");
                }
                else
                {
                    Console.WriteLine("No se encontró el empleado con el ID proporcionado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el empleado: " + ex.Message);
            }
        }


        static void InsertarPelicula(SqlConnection connection)
        {
            Console.Clear();
            Console.WriteLine("Insertar Película");
            Console.Write("Título: ");
            string titulo = Console.ReadLine();
            Console.Write("Duración (HH:MM:SS): ");
            TimeSpan duracion = TimeSpan.Parse(Console.ReadLine());
            Console.Write("Clasificación: ");
            string clasificacion = Console.ReadLine();
            Console.Write("Idioma: ");
            string idioma = Console.ReadLine();
            Console.WriteLine("Selecciona un género:");
            Console.WriteLine("1. Películas de Terror");
            Console.WriteLine("2. Películas de Acción");
            Console.WriteLine("3. Películas de Suspenso");
            Console.WriteLine("4. Películas de Ciencia Ficción");
            Console.WriteLine("5. Películas de Amor");
            Console.WriteLine("6. Películas de Comedia");
            Console.WriteLine("7. Documentales");
            Console.WriteLine("8. Películas Históricas");
            Console.WriteLine("9. Conciertos");
            Console.Write("Ingrese el número del género: ");
            int idGenero = int.Parse(Console.ReadLine());

            string query = "INSERT INTO Pelicula (titulo, duracion, clasificacion, idioma, id_Genero_pelicula, id_usuario_crea) " +
                           "VALUES (@titulo, @duracion, @clasificacion, @idioma, @idGenero, @idUsuarioCreacion)";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@titulo", titulo);
            cmd.Parameters.AddWithValue("@duracion", duracion);
            cmd.Parameters.AddWithValue("@clasificacion", clasificacion);
            cmd.Parameters.AddWithValue("@idioma", idioma);
            cmd.Parameters.AddWithValue("@idGenero", idGenero);
            cmd.Parameters.AddWithValue("@idUsuarioCreacion", idUsuarioAutenticado);  // Aquí se agrega el ID del usuario que crea

            cmd.ExecuteNonQuery();
            Console.WriteLine("Película insertada correctamente.");
        }


        static void VerPeliculas(SqlConnection connection)
        {
            string query = "SELECT id, titulo, duracion, clasificacion, idioma, status, id_Genero_pelicula FROM Pelicula";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            Console.Clear();
            Console.WriteLine("Películas registradas:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}, Título: {reader["titulo"]}, Duración: {reader["duracion"]}, Clasificación: {reader["clasificacion"]}, Idioma: {reader["idioma"]}, Género ID: {reader["id_Genero_pelicula"]}");
            }
            reader.Close();
        }

        static void ActualizarPelicula(SqlConnection connection)
        {
            Console.Clear();
            Console.WriteLine("Actualizar Película");

            // Solicitar ID de la película a actualizar
            Console.Write("ID de la película a actualizar: ");
            int id = int.Parse(Console.ReadLine());

            // Mostrar las opciones de qué actualizar
            Console.WriteLine("¿Qué desea actualizar?");
            Console.WriteLine("1. Título");
            Console.WriteLine("2. Duración");
            Console.WriteLine("3. Clasificación");
            Console.WriteLine("4. Idioma");
            Console.WriteLine("5. Género");
            Console.WriteLine("6. Status");
            Console.Write("Seleccione una opción: ");
            int opcion = int.Parse(Console.ReadLine());

            // Variables para los nuevos valores
            string titulo = null;
            TimeSpan? duracion = null;
            string clasificacion = null;
            string idioma = null;
            int? idGenero = null;
            bool? status = null;

            // Inicializamos la base del query
            string query = "UPDATE Pelicula SET id_usuario_edita = @idUsuario";

            // Agregar el parámetro para id_usuario_edita
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@idUsuario", idUsuarioAutenticado);
            command.Parameters.AddWithValue("@idPelicula", id);

            switch (opcion)
            {
                case 1: // Actualizar Título
                    Console.Write("Nuevo título: ");
                    titulo = Console.ReadLine();
                    query += ", titulo = @titulo";
                    command.Parameters.AddWithValue("@titulo", titulo);
                    break;

                case 2: // Actualizar Duración
                    Console.Write("Nueva duración (HH:MM:SS): ");
                    duracion = TimeSpan.Parse(Console.ReadLine());
                    query += ", duracion = @duracion";
                    command.Parameters.AddWithValue("@duracion", duracion);
                    break;

                case 3: // Actualizar Clasificación
                    Console.Write("Nueva clasificación (A, B, C, D): ");
                    clasificacion = Console.ReadLine();
                    query += ", clasificacion = @clasificacion";
                    command.Parameters.AddWithValue("@clasificacion", clasificacion);
                    break;

                case 4: // Actualizar Idioma
                    Console.Write("Nuevo idioma: ");
                    idioma = Console.ReadLine();
                    query += ", idioma = @idioma";
                    command.Parameters.AddWithValue("@idioma", idioma);
                    break;

                case 5: // Actualizar Género
                    Console.WriteLine("Selecciona un nuevo género:");
                    Console.WriteLine("1. Películas de Terror");
                    Console.WriteLine("2. Películas de Acción");
                    Console.WriteLine("3. Películas de Suspenso");
                    Console.WriteLine("4. Películas de Ciencia Ficción");
                    Console.WriteLine("5. Películas de Amor");
                    Console.WriteLine("6. Películas de Comedia");
                    Console.WriteLine("7. Documentales");
                    Console.WriteLine("8. Películas Históricas");
                    Console.WriteLine("9. Conciertos");
                    Console.Write("Ingrese el número del género: ");
                    idGenero = int.Parse(Console.ReadLine());
                    query += ", id_Genero_pelicula = @idGenero";
                    command.Parameters.AddWithValue("@idGenero", idGenero);
                    break;

                case 6: // Actualizar Status
                    Console.Write("Nuevo status (1 para activo, 0 para inactivo): ");
                    int statusInput = int.Parse(Console.ReadLine());
                    status = (statusInput == 1); // Convertir 1 a true y 0 a false
                    query += ", status = @status";
                    command.Parameters.AddWithValue("@status", status);
                    break;

                default:
                    Console.WriteLine("Opción no válida.");
                    return;
            }

            query += " WHERE id = @idPelicula"; // Finalizamos la consulta con la cláusula WHERE

            // Ejecutar la consulta de actualización
            command.CommandText = query;
            command.ExecuteNonQuery();

            Console.WriteLine("Película actualizada correctamente.");
        }
    }
}
    

