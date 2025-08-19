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
    public class UbicacionRepository
    {
        private readonly string _connectionString;
       // public UbicacionRepository(string connectionString) => _connectionString = connectionString;


        public UbicacionRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("La cadena de conexión no puede ser nula o vacía.", nameof(connectionString));

            _connectionString = connectionString;
        }
        // =========================
        // LEER TODAS LAS UBICACIONES
        // =========================

        public List<Ubicacion> ObtenerTodas()
        {
            var lista = new List<Ubicacion>(); 
            try
            {
                using var conexion = new SqlConnection(_connectionString);
                conexion.Open();
                // LEFT JOIN PARA EVITAR ERRORES SI FALTAN DATOS EN ESPECIES O UBICACIONES          
                var sql = @"SELECT IdUbicacion, Nombre, Descripcion, Condiciones FROM Ubicaciones";

                using var cmd = new SqlCommand(sql, conexion);
                using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                lista.Add(new Ubicacion
                {
                    IdUbicacion = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    Descripcion = rd.IsDBNull(2) ? null : rd.GetString(2),
                    Condiciones = rd.IsDBNull(3) ? null : rd.GetString(3)
                });
            }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener todas las ubicaciones: " + ex.Message, ex);
            }
            return lista;
        }
        // =========================
        // FIN LEER TODAS LAS UBICACIONES
        // =========================


        // =========================
        // CREAR UBICACIONES
        // =========================
        public void RegistrarUbicacion(Ubicacion ubicacion)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            var sql = @" INSERT INTO Ubicaciones (Nombre , Descripcion, Condiciones) 
                                       VALUES (@Nombre, @Descripcion, @Condiciones); SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Nombre", ubicacion.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", ubicacion.Descripcion);
            cmd.Parameters.AddWithValue("@Condiciones", ubicacion.Condiciones);
            cmd.ExecuteNonQuery();
        }
        // =========================
        // FIN DE CREAR UBICACIONES
        // =========================

        // =========================
        // LEER UBICACION POR ID
        // =========================

        public Ubicacion ObtenerPorId(int id)
        {
            using var cn = new SqlConnection(_connectionString);
            cn.Open();
            var sql = @"SELECT IdUbicacion, Nombre, Descripcion, Condiciones
                        FROM Ubicaciones WHERE IdUbicacion=@Id";
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var rd = cmd.ExecuteReader();
            return rd.Read()
                ? new Ubicacion
                {
                    IdUbicacion = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    Descripcion = rd.IsDBNull(2) ? null : rd.GetString(2),
                    Condiciones = rd.IsDBNull(3) ? null : rd.GetString(3)
                }
                : null;
        }
        // =========================
        // FIN DE LEER UBICACION POR ID
        // =========================

        // =========================
        // ACTUALIZAR UBICACION
        // =========================
        public void Actualizar(Ubicacion u)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();
            var sql = @"
                    UPDATE Ubicaciones
                    SET Nombre=@Nombre, Descripcion=@Descripcion,
                        Condiciones=@Condiciones
                    WHERE IdUbicacion=@Id";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Nombre", u.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", u.Descripcion);
            cmd.Parameters.AddWithValue("@Condiciones", u.Condiciones);
            cmd.Parameters.AddWithValue("@Id", u.IdUbicacion);
            cmd.ExecuteNonQuery();
        }
        // =========================
        // FIN DE ACTUALIZAR UBICACION
        // =========================

        // =========================
        // ELIMINAR UBICACION
        // =========================
        public bool Eliminar(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            // 1. Verificar si hay especies o algo asociado
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

            var sql = @"DELETE FROM Ubicaciones WHERE IdUbicacion = @IdUbicacion";
            using var cmd = new SqlCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@IdUbicacion", id);
            cmd.ExecuteNonQuery();
            return true;
        }
        // =========================
        // FIN DE ELIMINAR UBICACION
        // =========================




    }
}