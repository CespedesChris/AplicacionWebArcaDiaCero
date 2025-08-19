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
    public class EspecieRepository
    {
        private readonly string _connectionString;
        public EspecieRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("La cadena de conexión no puede ser nula o vacía.", nameof(connectionString));

            _connectionString = connectionString;
        }
        // =========================
        // LEER TODAS LAS ESPECIES
        // =========================

        public List<Especie> ObtenerTodas()
        {
            var lista = new List<Especie>();
            try
            {
                using var conexion = new SqlConnection(_connectionString);
                conexion.Open();
                // LEFT JOIN PARA EVITAR ERRORES SI FALTAN DATOS EN ESPECIES O UBICACIONES          
                var sql = @" 
                SELECT IdEspecie, NombreCientifico, NombreComun, Familia, Descripcion 
                FROM Especies";

                using var cmd = new SqlCommand(sql, conexion);
                using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                lista.Add(new Especie
                {
                    IdEspecie = rd.GetInt32(0),
                    NombreCientifico = rd.GetString(1),
                    NombreComun = rd.GetString(2),
                    Familia = rd.IsDBNull(3) ? null : rd.GetString(3),
                    Descripcion = rd.IsDBNull(4) ? null : rd.GetString(4)
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
        // FIN LEER TODAS LAS ESPECIES
        // =========================

        // =========================
        // CREAR ESPECIE
        // =========================
        public void RegistrarEspecie(Especie especie)
        {
            using var conexion = new SqlConnection(_connectionString);

            conexion.Open();

            var sql = @" INSERT INTO Especies (NombreCientifico , NombreComun, Familia, Descripcion) 
                                       VALUES (@Nombre, @NombreComun, @Familia, @Descripcion); SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Nombre", especie.NombreCientifico);
            cmd.Parameters.AddWithValue("@NombreComun", especie.NombreComun);
            cmd.Parameters.AddWithValue("@Familia", especie.Familia);
            cmd.Parameters.AddWithValue("@Descripcion", especie.Descripcion);
            cmd.ExecuteNonQuery();
            
        }
        // =========================
        // LEER ESPECIE POR ID
        // =========================

        public Especie ObtenerPorId(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var sql = @"
                SELECT IdEspecie, NombreCientifico, NombreComun, Familia, Descripcion
                        FROM Especies WHERE IdEspecie=@Id";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new Especie
            {
                IdEspecie = rd.GetInt32(0),
                NombreCientifico = rd.GetString(1),
                NombreComun = rd.GetString(2),
                Familia = rd.IsDBNull(3) ? null : rd.GetString(3),
                Descripcion = rd.IsDBNull(4) ? null : rd.GetString(4)
            };
        }


        // =========================
        // ACTUALIZAR ESPECIE
        // =========================
        public void Actualizar(Especie e)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var sql = @"
                    UPDATE Especies
                    SET NombreCientifico=@NombreCientifico, NombreComun=@NombreComun,
                        Familia=@Familia, Descripcion=@Descripcion
                    WHERE IdEspecie=@Id";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@NombreCientifico", e.NombreCientifico);
            cmd.Parameters.AddWithValue("@NombreComun", e.NombreComun);
            cmd.Parameters.AddWithValue("@Familia", e.Familia);
            cmd.Parameters.AddWithValue("@Descripcion", e.Descripcion);
            cmd.Parameters.AddWithValue("@Id", e.IdEspecie);
            cmd.ExecuteNonQuery();
        }


        // =========================
        // ELIMINAR ESPECIE
        // =========================
        public bool Eliminar(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            // 1. Verificar si hay semillas asociadas
            var checkSql = "SELECT COUNT(*) FROM Semillas WHERE IdEspecie = @Id";
            using (var checkCmd = new SqlCommand(checkSql, conexion))
            {
                checkCmd.Parameters.AddWithValue("@Id", id);
                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    return false; // No se elimina, tiene semillas
                }
            }
            // 2. Eliminar si no hay referencias
            var sql = @"DELETE FROM Especies WHERE IdEspecie=@Id";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
            return true;
        }
        // =========================
        // OBTENER TODAS LAS ESPECIES //AUN NO SE SI LO VOY A NECESITAR
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



    }
}
