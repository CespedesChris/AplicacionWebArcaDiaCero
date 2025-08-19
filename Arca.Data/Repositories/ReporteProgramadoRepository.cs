using Arca.Data.Models;
using Arca.Data.Models;
using Arca.Data.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arca.Data.Repositories;
public class ReporteProgramadoRepository
{
    private readonly string _connectionString;
    public ReporteProgramadoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Insertar(ReporteProgramado r)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        var query = 
            
            @"INSERT INTO ReportesProgramados(NombreReporte, Formato, Frecuencia, Destinatarios, ProximoEnvio, Parametros, FechaCreacion)
        VALUES(@NombreReporte, @Formato, @Frecuencia, @Destinatarios, @ProximoEnvio, @Parametros, @FechaCreacion);
        SELECT SCOPE_IDENTITY(); ";

        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@NombreReporte", r.NombreReporte);
        cmd.Parameters.AddWithValue("@Formato", r.Formato);
        cmd.Parameters.AddWithValue("@Frecuencia", r.Frecuencia);
        cmd.Parameters.AddWithValue("@Destinatarios", r.Destinatarios);
        //cmd.Parameters.AddWithValue("@ProximoEnvio", r.ProximoEnvio ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ProximoEnvio", r.ProximoEnvio);
        cmd.Parameters.AddWithValue("@Parametros", r.Parametros);
        cmd.Parameters.AddWithValue("@FechaCreacion", r.FechaCreacion);
        cmd.ExecuteNonQuery();
    }

    public List<ReporteProgramado> Listar() //Listar()
    {
        var lista = new List<ReporteProgramado>();
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        var query = @"SELECT Id, NombreReporte, Formato, Frecuencia, Destinatarios, ProximoEnvio, Parametros, FechaCreacion
                        FROM ReportesProgramados ORDER BY Id DESC";
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new ReporteProgramado
            {
                Id = (int)reader["Id"],
                NombreReporte = reader["NombreReporte"].ToString(),
                Formato = reader["Formato"].ToString(),
                Frecuencia = reader["Frecuencia"].ToString(),
                Destinatarios = reader["Destinatarios"].ToString(),
                FechaCreacion = (DateTime)reader["FechaCreacion"],
                //ProximoEnvio = reader["ProximoEnvio"] as DateTime?

                // 🚨 Aquí es donde da el error
                ProximoEnvio = reader["ProximoEnvio"] == DBNull.Value
                                 ? (DateTime?)null
                                 : Convert.ToDateTime(reader["ProximoEnvio"]),

                //ProximoEnvio = (DateTime)reader["ProximoEnvio"],
                Parametros = reader.GetString(6),
                //FechaCreacion = reader.GetDataTime(7)
            });
        }
        return lista;
    }

    public ReporteProgramado ObtenerPorId(int id)
    {
        using var cn = new SqlConnection(_connectionString);
        cn.Open();
        var sql = @"SELECT Id, NombreReporte, Formato, Frecuencia, Destinatarios, ProximoEnvio, Parametros, FechaCreacion
                        FROM ReportesProgramados WHERE Id=@Id";
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Id", id);
        using var rd = cmd.ExecuteReader();
        if (!rd.Read()) return null;

        return new ReporteProgramado
        {
            Id = rd.GetInt32(0),
            NombreReporte = rd.GetString(1),
            Formato = rd.GetString(2),
            Frecuencia = rd.GetString(3),
            Destinatarios = rd.GetString(4),
            ProximoEnvio = rd.IsDBNull(5) ? (DateTime?)null : rd.GetDateTime(5),
            Parametros = rd.GetString(6),
            FechaCreacion = rd.GetDateTime(7)
        };
    }

    public void Actualizar(ReporteProgramado r)
    {
        using var cn = new SqlConnection(_connectionString);
        cn.Open();
        var sql = @"
                UPDATE ReportesProgramados
                SET NombreReporte=@NombreReporte, Formato=@Formato, Frecuencia=@Frecuencia, Destinatarios=@Destinatarios,
                       ProximoEnvio=@ProximoEnvio, Parametros=@Parametros
                        WHERE Id=@Id";
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Id", r.Id);
        cmd.Parameters.AddWithValue("@NombreReporte", r.NombreReporte);
        cmd.Parameters.AddWithValue("@Formato", r.Formato);
        cmd.Parameters.AddWithValue("@Frecuencia", r.Frecuencia);
        cmd.Parameters.AddWithValue("@Destinatarios", r.Destinatarios);
        cmd.Parameters.AddWithValue("@ProximoEnvio", (object?)r.ProximoEnvio ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Parametros", r.Parametros);
        cmd.ExecuteNonQuery();
    }

    public void Eliminar(int id)
    {
        using var cn = new SqlConnection(_connectionString);
        cn.Open();
        var sql = "DELETE FROM ReportesProgramados WHERE Id=@Id";
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }
}
