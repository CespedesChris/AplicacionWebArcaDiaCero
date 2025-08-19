using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Arca.Data.Models;


using System.Data;

namespace Arca.Data.Repositories
{
    public class SemillaRepository
    {
        private readonly string _connectionString;
        public SemillaRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("La cadena de conexión no puede ser nula o vacía.", nameof(connectionString));

            _connectionString = connectionString;
        }


        // =========================
        // LEER TODAS LAS SEMILLAS
        // =========================
        public List<Semilla> ObtenerTodas()
        {
            var lista = new List<Semilla>();
            try
            { 
                 using var conexion = new SqlConnection(_connectionString);
                 conexion.Open();

                 // LEFT JOIN PARA EVITAR ERRORES SI FALTAN DATOS EN ESPECIES O UBICACIONES
                 var sql = @"
                 SELECT  
                    s.IdSemilla, s.Nombre, s.IdEspecie, s.IdUbicacion, s.Cantidad, s.FechaAlmacenamiento,
                    e.IdEspecie, e.NombreCientifico, e.NombreComun, e.Familia, e.Descripcion,
                    u.IdUbicacion, u.Nombre, u.Descripcion, u.Condiciones
                FROM Semillas s
                 LEFT JOIN Especies e ON s.IdEspecie = e.IdEspecie
                 LEFT JOIN Ubicaciones u ON s.IdUbicacion = u.IdUbicacion
                 ORDER BY s.IdSemilla DESC";

                using var cmd = new SqlCommand(sql, conexion);
                using var rd = cmd.ExecuteReader();

                 while (rd.Read())
                 {
                    lista.Add(new Semilla
                    {
                        IdSemilla = rd.GetInt32(0),
                        Nombre = rd.GetString(1),
                        IdEspecie = rd.GetInt32(2),
                        IdUbicacion = rd.GetInt32(3),
                        Cantidad = rd.GetInt32(4),
                        FechaAlmacenamiento = rd.GetDateTime(5),
                        Especie = new Especie
                        {
                            IdEspecie = rd.IsDBNull(6) ? 0 : rd.GetInt32(6),
                            NombreCientifico = rd.IsDBNull(7) ? null : rd.GetString(7),
                            NombreComun = rd.IsDBNull(8) ? null : rd.GetString(8),
                            Familia = rd.IsDBNull(9) ? null : rd.GetString(9),
                            Descripcion = rd.IsDBNull(10) ? null : rd.GetString(10)
                        },
                        Ubicacion = new Ubicacion
                        {
                            IdUbicacion = rd.IsDBNull(11) ? 0 : rd.GetInt32(11),
                            Nombre = rd.IsDBNull(12) ? null : rd.GetString(12),
                            Descripcion = rd.IsDBNull(13) ? null : rd.GetString(13),
                            Condiciones = rd.IsDBNull(14) ? null : rd.GetString(14)
                        }
                    });
                 }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener todas las semillas: " + ex.Message, ex);
            }


            return lista;
        }

        // =========================
        // FIN LEER TODAS LAS SEMILLAS
        // =========================


        // =========================
        // CREAR SEMILLA
        // =========================
        public int RegistrarSemilla(Semilla semilla)
        {
            using var conexion = new SqlConnection(_connectionString);

            conexion.Open();

            var sql = @" INSERT INTO Semillas (Nombre, IdEspecie, IdUbicacion, Cantidad, FechaAlmacenamiento) VALUES (@Nombre, @IdEspecie, @IdUbicacion, @Cantidad, @Fecha); SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Nombre", semilla.Nombre);
            cmd.Parameters.AddWithValue("@IdEspecie", semilla.IdEspecie);
            cmd.Parameters.AddWithValue("@IdUbicacion", semilla.IdUbicacion);
            cmd.Parameters.AddWithValue("@Cantidad", semilla.Cantidad);
            cmd.Parameters.AddWithValue("@Fecha", semilla.FechaAlmacenamiento);
            //cmd.ExecuteNonQuery();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // =========================
        // LEER SEMILLA POR ID
        // =========================
        public Semilla ObtenerPorId(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var sql = @"
                SELECT  s.IdSemilla, s.Nombre, s.IdEspecie, s.IdUbicacion, s.Cantidad, s.FechaAlmacenamiento,
                        e.IdEspecie, e.NombreCientifico, e.NombreComun, e.Familia, e.Descripcion,
                        u.IdUbicacion, u.Nombre, u.Descripcion, u.Condiciones
                FROM Semillas s
                INNER JOIN Especies   e ON s.IdEspecie   = e.IdEspecie
                INNER JOIN Ubicaciones u ON s.IdUbicacion = u.IdUbicacion
                WHERE s.IdSemilla = @Id";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Semilla
            {
                IdSemilla = rd.GetInt32(0),
                Nombre = rd.GetString(1),
                IdEspecie = rd.GetInt32(2),
                IdUbicacion = rd.GetInt32(3),
                Cantidad = rd.GetInt32(4),
                FechaAlmacenamiento = rd.GetDateTime(5),
                Especie = new Especie
                {
                    IdEspecie = rd.GetInt32(6),
                    NombreCientifico = rd.GetString(7),
                    NombreComun = rd.GetString(8),
                    Familia = rd.IsDBNull(9) ? null : rd.GetString(9),
                    Descripcion = rd.IsDBNull(10) ? null : rd.GetString(10)
                },
                Ubicacion = new Ubicacion
                {
                    IdUbicacion = rd.GetInt32(11),
                    Nombre = rd.GetString(12),
                    Descripcion = rd.IsDBNull(13) ? null : rd.GetString(13),
                    Condiciones = rd.IsDBNull(14) ? null : rd.GetString(14)
                }
            };
        }

        // =========================
        // ACTUALIZAR SEMILLA
        // =========================
        public void Actualizar(Semilla s)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var sql = @"
                UPDATE Semillas
                SET Nombre=@Nombre, IdEspecie=@IdEspecie, IdUbicacion=@IdUbicacion,
                    Cantidad=@Cantidad, FechaAlmacenamiento=@Fecha
                WHERE IdSemilla=@Id";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Nombre", s.Nombre);
            cmd.Parameters.AddWithValue("@IdEspecie", s.IdEspecie);
            cmd.Parameters.AddWithValue("@IdUbicacion", s.IdUbicacion);
            cmd.Parameters.AddWithValue("@Cantidad", s.Cantidad);
            cmd.Parameters.AddWithValue("@Fecha", s.FechaAlmacenamiento);
            cmd.Parameters.AddWithValue("@Id", s.IdSemilla);
            cmd.ExecuteNonQuery();
        }

        // =========================
        // ELIMINAR SEMILLA
        // =========================
        public void Eliminar(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var sql = @"DELETE FROM Semillas WHERE IdSemilla=@Id";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
        // =========================
        // OBTENER TODAS LAS ESPECIES
        // =========================
        public List<Especie> ObtenerTodasEspecies()
        {
            // Lógica ADO.NET para traer todas las especies
            var lista = new List<Especie>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM Especies", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Especie
                {
                    IdEspecie = (int)reader["IdEspecie"],
                    NombreComun = reader["NombreComun"].ToString(),
                    NombreCientifico = reader["NombreCientifico"].ToString()
                });
            }
            return lista;
        }
        // =========================
        // OBTENER LAS UBICACIONES
        // =========================
        public List<Ubicacion> ObtenerTodasUbicaciones()
        {
            var lista = new List<Ubicacion>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT * FROM Ubicaciones", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Ubicacion
                {
                    IdUbicacion = (int)reader["IdUbicacion"],
                    Nombre = reader["Nombre"].ToString()
                });
            }
            return lista;
        }

        // =====================================================================================================================
        // *----------------------------------------------* OBTENER LAS SEMILLAS CON NOMBRE DE ESPECIES
        // =====================================================================================================================
        public List<SemillaViewModel> ObtenerSemillasConNombres()
        {
            var lista = new List<SemillaViewModel>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"
        SELECT s.IdSemilla, s.Nombre, s.Cantidad, s.FechaAlmacenamiento,
               s.IdEspecie, e.NombreComun AS NombreEspecie,
               s.IdUbicacion, u.Nombre AS NombreUbicacion
        FROM Semillas s
        INNER JOIN Especies e ON s.IdEspecie = e.IdEspecie
        INNER JOIN Ubicaciones u ON s.IdUbicacion = u.IdUbicacion
    ", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new SemillaViewModel
                {
                    IdSemilla = (int)reader["IdSemilla"],
                    Nombre = reader["Nombre"].ToString(),
                    IdEspecie = (int)reader["IdEspecie"],
                    NombreEspecie = reader["NombreEspecie"].ToString(),
                    IdUbicacion = (int)reader["IdUbicacion"],
                    NombreUbicacion = reader["NombreUbicacion"].ToString(),
                    Cantidad = (int)reader["Cantidad"],
                    FechaAlmacenamiento = (DateTime)reader["FechaAlmacenamiento"]
                });
            }
            return lista;
        }
        // =====================================================================================================================
        // *----------------------------------------------* FIN OBTENER LAS SEMILLAS CON NOMBRE DE ESPECIES
        // =====================================================================================================================


    }
}
