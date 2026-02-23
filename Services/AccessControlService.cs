using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using wpf_exemplo.Helpers;
using wpf_exemplo.Models;

namespace wpf_exemplo.Services
{
    public class AccessControlService
    {
        public bool ValidateFingerprint(int fingerprintId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"
                SELECT id FROM moradores
                WHERE ativo = TRUE
                AND (fingerprint_id_1 = @id OR fingerprint_id_2 = @id)
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", fingerprintId);

            using var reader = cmd.ExecuteReader();
            return reader.Read();
        }

        public void RegisterAccess(int fingerprintId)
        {
            string ultimoTipo = GetLastAccessType(fingerprintId);

            // Se o último foi entrada, agora é saída. Se não tem registro ou foi saída, agora é entrada.
            string novoTipo = (ultimoTipo == "ENTRADA") ? "SAIDA" : "ENTRADA";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"
                INSERT INTO acessos (fingerprint_id, data_hora, tipo)
                VALUES (@id, NOW(), @tipo)
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", fingerprintId);
            cmd.Parameters.AddWithValue("@tipo", novoTipo);

            cmd.ExecuteNonQuery();
        }

        public List<AcessoDashboard> GetUltimasEntradas()
        {
            var lista = new List<AcessoDashboard>();

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            // ATUALIZADO: Adicionado m.bloco e m.apartamento
            string sql = @"
                SELECT 
                    m.nome_completo,
                    m.bloco,
                    m.apartamento,
                    a.data_hora
                FROM acessos a
                JOIN moradores m 
                  ON (a.fingerprint_id = m.fingerprint_id_1 OR a.fingerprint_id = m.fingerprint_id_2)
                WHERE a.tipo = 'ENTRADA'
                ORDER BY a.data_hora DESC
                LIMIT 5;";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AcessoDashboard
                {
                    Nome = reader["nome_completo"].ToString(),
                    DataHora = Convert.ToDateTime(reader["data_hora"]).ToString("dd/MM/yyyy HH:mm"),
                    Bloco = reader["bloco"].ToString(),             // Tem que preencher o Bloco
                    Apartamento = reader["apartamento"].ToString()  // Tem que preencher o Apartamento
                });
            }

            return lista;
        }

        public List<AcessoDashboard> GetUltimasSaidas()
        {
            var lista = new List<AcessoDashboard>();

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            // ATUALIZADO: Adicionado m.bloco e m.apartamento
            string sql = @"
                SELECT 
                    m.nome_completo,
                    m.bloco,
                    m.apartamento,
                    a.data_hora
                FROM acessos a
                JOIN moradores m 
                  ON (a.fingerprint_id = m.fingerprint_id_1 OR a.fingerprint_id = m.fingerprint_id_2)
                WHERE a.tipo = 'SAIDA'
                ORDER BY a.data_hora DESC
                LIMIT 5;";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AcessoDashboard
                {
                    Nome = reader["nome_completo"].ToString(),
                    Bloco = reader["bloco"].ToString(),             // Preenche o Bloco
                    Apartamento = reader["apartamento"].ToString(), // Preenche o Apto
                    DataHora = Convert.ToDateTime(reader["data_hora"]).ToString("dd/MM/yyyy HH:mm")
                });
            }

            return lista;
        }

        public string GetLastAccessType(int fingerprintId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"
                SELECT tipo 
                FROM acessos
                WHERE fingerprint_id = @id
                ORDER BY data_hora DESC
                LIMIT 1
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", fingerprintId);

            var result = cmd.ExecuteScalar();
            return result?.ToString();
        }
    }
}