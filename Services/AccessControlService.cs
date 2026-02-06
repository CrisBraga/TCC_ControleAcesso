using MySql.Data.MySqlClient;
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
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"
                INSERT INTO acessos (fingerprint_id, data_hora)
                VALUES (@id, NOW())
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", fingerprintId);
            cmd.ExecuteNonQuery();
        }

        public List<AcessoDashboard> GetUltimasEntradas()
        {
            var lista = new List<AcessoDashboard>();

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"
        SELECT 
            m.nome_completo,
            a.data_hora
        FROM acessos a
        JOIN moradores m 
          ON a.fingerprint_id = m.fingerprint_id_1
          OR a.fingerprint_id = m.fingerprint_id_2
        WHERE a.tipo = 'ENTRADA'
        ORDER BY a.data_hora DESC
        LIMIT 5;";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AcessoDashboard
                {
                    Nome = reader.GetString("nome_completo"),
                    DataHora = reader.GetDateTime("data_hora").ToString("dd/MM/yyyy HH:mm")
                });
            }

            return lista;
        }

        public List<AcessoDashboard> GetUltimasSaidas()
        {
            var lista = new List<AcessoDashboard>();

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"
        SELECT 
            m.nome_completo,
            a.data_hora
        FROM acessos a
        JOIN moradores m 
          ON a.fingerprint_id = m.fingerprint_id_1
          OR a.fingerprint_id = m.fingerprint_id_2
        WHERE a.tipo = 'SAIDA'
        ORDER BY a.data_hora DESC
        LIMIT 5;
        ";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AcessoDashboard
                {
                    Nome = reader.GetString("nome_completo"),
                    DataHora = reader.GetDateTime("data_hora").ToString("dd/MM/yyyy HH:mm")
                });
            }

            return lista;
        }
    }
}
