using MaterialDesignColors;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Runtime.Intrinsics.X86;
using System.Windows;

namespace wpf_exemplo.Helpers
{
    public static class DatabaseHelper
    {
        // Ajuste conforme seu ambiente
        private const string connectionString = "Server=localhost;Database=SISTEMA;Uid=root;Pwd='';";

        // MÉTODO ATUALIZADO: Agora recebe nome e email
        public static bool RegisterUser(string nome, string username, string password, string email)
        {
            try
            {
                // 1. Validação de todos os campos
                if (string.IsNullOrWhiteSpace(nome) ||
                    string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(email))
                {
                    // Apenas retorna false, a janela já validou isso antes de chamar aqui
                    return false;
                }

                // 2. Hash da senha
                string hashedPassword = PasswordHelper.HashPassword(password);

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO porteiros (nome_completo, username, email, password_hash) VALUES (@n, @u, @e, @p)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@n", nome);
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.Parameters.AddWithValue("@p", hashedPassword);

                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Erro de SQL (ex: Duplicado, Sem conexão)
                // Isso imprime o erro no painel "Saída" do Visual Studio (para você ver), mas não abre janela pro usuário
                System.Diagnostics.Debug.WriteLine($"Erro SQL: {ex.Message}");
                return false;
            }
            catch (ArgumentException ex)
            {
                // Erro de Argumento
                System.Diagnostics.Debug.WriteLine($"Erro Argumento: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Erro Genérico
                System.Diagnostics.Debug.WriteLine($"Erro Geral: {ex.Message}");
                return false;
            }
        }

        // Mantive o ValidateLogin igual (login continua sendo via Username)
        public static bool ValidateLogin(string username, string password)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Busca a senha criptografada baseada no usuario
                    string query = "SELECT password_hash FROM porteiros WHERE username = @u";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader.GetString("password_hash");
                                // Compara a senha digitada com o hash do banco
                                return PasswordHelper.VerifyPassword(password, storedHash);
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao validar login:\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool UpdatePassword(string username, string email, string newPassword)
        {
            using (var conn = GetConnection())
            {
                try
                {
                    conn.Open();

                    // 1. CORREÇÃO: Tabela 'porteiros', colunas 'password_hash', 'username' e 'email'
                    // 2. Usamos o TRIM() para garantir que espaços em branco não atrapalhem
                    string sql = "UPDATE porteiros SET password_hash = @senha WHERE username = @usuario AND email = @email";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        // 3. CORREÇÃO: Gerar o HASH da nova senha (fundamental!)
                        string hashedPassword = PasswordHelper.HashPassword(newPassword);

                        cmd.Parameters.AddWithValue("@usuario", username.Trim());
                        cmd.Parameters.AddWithValue("@email", email.Trim());
                        cmd.Parameters.AddWithValue("@senha", hashedPassword);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Se rowsAffected > 0, significa que o par usuário+email existia e foi atualizado
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erro ao atualizar senha: " + ex.Message);
                    return false;
                }
            }
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}