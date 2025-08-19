using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Arca.Data.Models;

namespace Arca.Data.Repositories
{
    public class UsuarioRepository
    {
        private readonly string _connectionString;
        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // =========================
        // CREAR USUARIO
        // =========================
        public void RegistrarUsuario(Usuario usuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            var query = @"INSERT INTO Usuarios (Nombre, Apellido, Email, PasswordHash, IdRol) 
                          VALUES (@Nombre, @Apellido, @Email, @PasswordHash, @IdRol)";

            using var comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            comando.Parameters.AddWithValue("@Apellido", usuario.Apellido);
            comando.Parameters.AddWithValue("@Email", usuario.Email);
            comando.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
            comando.Parameters.AddWithValue("@IdRol", usuario.IdRol);

            comando.ExecuteNonQuery();
        }

        // =========================
        // LEER TODOS LOS USUARIOS
        // =========================
        public List<Usuario> ObtenerTodosUsuarios()
        {
            var usuarios = new List<Usuario>();
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var query = "SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, IdRol FROM Usuarios";
            using var comando = new SqlCommand(query, conexion);
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                usuarios.Add(new Usuario
                {
                    IdUsuario = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Email = reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    IdRol = reader.GetInt32(5)
                });
            }

            return usuarios;
        }

        // =========================
        // LEER USUARIO POR ID
        // =========================
        public Usuario ObtenerUsuarioPorId(int idUsuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            var query = @"SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, IdRol 
                          FROM Usuarios WHERE IdUsuario = @IdUsuario";

            using var comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using var reader = comando.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    IdUsuario = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Email = reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    IdRol = reader.GetInt32(5)
                };
            }

            return null;
        }

        // =========================
        // LEER USUARIO POR EMAIL
        // =========================
        public Usuario ObtenerPorEmail(string email)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            var query = @"SELECT IdUsuario, Nombre, Apellido, Email, PasswordHash, IdRol 
                          FROM Usuarios WHERE Email = @Email";

            using var comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@Email", email);

            using var reader = comando.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    IdUsuario = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Email = reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    IdRol = reader.GetInt32(5)
                };
            }

            return null;
        }

        // =========================
        // ACTUALIZAR USUARIO
        // =========================
        public void ActualizarUsuario(Usuario usuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            var query = @"UPDATE Usuarios 
                          SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email, IdRol = @IdRol
                          WHERE IdUsuario = @IdUsuario";

            using var comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            comando.Parameters.AddWithValue("@Apellido", usuario.Apellido);
            comando.Parameters.AddWithValue("@Email", usuario.Email);
            comando.Parameters.AddWithValue("@IdRol", usuario.IdRol);
            comando.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);

            comando.ExecuteNonQuery();
        }

        // =========================
        // ELIMINAR USUARIO
        // =========================
        public void EliminarUsuario(int idUsuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            var query = "DELETE FROM Usuarios WHERE IdUsuario = @IdUsuario";

            using var comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

            comando.ExecuteNonQuery();
        }
    }
}