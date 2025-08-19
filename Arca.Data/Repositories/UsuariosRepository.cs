using Arca.Data.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Arca.Data.Repositories
{
    public class UsuariosRepository
    {
        private readonly string _connectionString;
        public UsuariosRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("La cadena de conexión no puede ser nula o vacía.", nameof(connectionString));

            _connectionString = connectionString;
        }
        // ============================================================================================================================
        // -----------------------*   LEER TODAS LOS USUARIOS
        // ============================================================================================================================
        public List<Usuarios> ObtenerTodas()
        {
            var lista = new List<Usuarios>();
            try
            {
                using var conexion = new SqlConnection(_connectionString);
                conexion.Open();
                // LEFT JOIN PARA EVITAR ERRORES SI FALTAN DATOS EN ESPECIES O UBICACIONES          
                var sql = @" 
                        SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, IdRol FROM Usuarios";
                using var cmd = new SqlCommand(sql, conexion);
                using var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    lista.Add(new Usuarios
                    {
                        IdUsuario = rd.GetInt32(0),
                        Nombre = rd.GetString(1),
                        Apellido = rd.GetString(2),
                        Email = rd.GetString(3),
                        PasswordHash = rd.GetString(4),
                        IdRol = rd.GetInt32(5)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener todas los usuarios: " + ex.Message, ex);
            }
            return lista;
        }
        // ============================================================================================================================
        // -----------------------*  FIN LEER TODAS LOS USUARIOS
        // ============================================================================================================================

        // ============================================================================================================================
        // -----------------------*  CREAR USUARIO
        // ============================================================================================================================
        public void RegistrarUsuarios(Usuarios usuarios)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            var sql = @"INSERT INTO Usuarios (Nombre, Apellido, Email, PasswordHash, IdRol) 
                          VALUES (@Nombre, @Apellido, @Email, @PasswordHash, @IdRol); SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Nombre", usuarios.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", usuarios.Apellido);
            cmd.Parameters.AddWithValue("@Email", usuarios.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", usuarios.PasswordHash);
            cmd.Parameters.AddWithValue("@IdRol", usuarios.IdRol);
            cmd.ExecuteNonQuery();
        }
        // ============================================================================================================================
        // -----------------------*  FIN CREAR USUARIO
        // ============================================================================================================================


        // ============================================================================================================================
        // -----------------------------* LEER USUARIO POR ID
        //  ============================================================================================================================
        public Usuarios ObtenerPorId(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var sql = @"
                        SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, IdRol 
                          FROM Usuarios WHERE IdUsuario = @IdUsuario";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@IdUsuario", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new Usuarios
            {
                    IdUsuario = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    Apellido = rd.GetString(2),
                    Email = rd.GetString(3),
                    PasswordHash = rd.GetString(4),
                    IdRol = rd.GetInt32(5)
            };
        }
        // ============================================================================================================================
        // -----------------------*  FIN LEER USUARIO
        // ============================================================================================================================

        // ============================================================================================================================
        // ----------------------------------* ACTUALIZAR USUARIO
        // ============================================================================================================================
        public void Actualizar(Usuarios usu)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var sql = @"
                        UPDATE Usuarios 
                          SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email, IdRol = @IdRol
                          WHERE IdUsuario = @IdUsuario";


            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Nombre", usu.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", usu.Apellido);
            cmd.Parameters.AddWithValue("@Email", usu.Email);
            cmd.Parameters.AddWithValue("@IdRol", usu.IdRol);
            cmd.Parameters.AddWithValue("@IdUsuario", usu.IdUsuario);
            cmd.ExecuteNonQuery();
        }
        // ============================================================================================================================
        // ----------------------------------* FIN ACTUALIZAR USUARIO
        // ============================================================================================================================

        // ============================================================================================================================
        // -----------------------------------* ELIMINAR USUARIO
        // ============================================================================================================================
        public bool Eliminar(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var sql = @"DELETE FROM Usuarios WHERE IdUsuario = @IdUsuario";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@IdUsuario", id);
            cmd.ExecuteNonQuery();
            return true;
        }
        // ============================================================================================================================
        // -----------------------------------* FIN ELIMINAR USUARIO
        // ============================================================================================================================


        // =========================
        // LEER USUARIO POR EMAIL
        // =========================
        public Usuarios ObtenerPorEmail(string email)
        {
            Usuarios usuario = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, IdRol FROM Usuarios WHERE Email = @Email";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);


                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuarios
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                Email = reader.GetString(3),
                                PasswordHash = reader.GetString(4),
                                IdRol = reader.GetInt32(5)
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        // =========================
        // OBTENER LOS ROLES
        // =========================
        public List<Roles> ObtenerTodosRoles()
        {
            var lista = new List<Roles>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM Roles", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Roles
                {
                    IdRol = (int)reader["IdRol"],
                    NombreRol = reader["NombreRol"].ToString()
                });
            }
            return lista;
        }


        // ===========================================================================
        // -----------*  PARA VALIDAR USUARIOS PARA EL LOGIN
        // ===========================================================================
        public Usuarios? Login(string email, string password)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            var sql = "SELECT * FROM Usuarios WHERE Email = @Email AND PasswordHash = @PasswordHash";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@PasswordHash", password); // aquí después podemos encriptar

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Usuarios
                {
                    IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                    Nombre = reader["Nombre"].ToString(),
                    Apellido = reader["Apellido"].ToString(),
                    Email = reader["Email"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString(),
                    IdRol = Convert.ToInt32(reader["IdRol"])
                };
            }
            return null;
        }
        // ===========================================================================
        // -----------*  FIN PARA VALIDAR USUARIOS PARA EL LOGIN
        // ===========================================================================


    }
}
