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
                    throw new ArgumentException("Todos os campos são obrigatórios.");
                }

                // 2. Hash da senha (mantendo sua lógica de segurança)
                // Certifique-se de que a classe PasswordHelper existe no seu projeto
                string hashedPassword = PasswordHelper.HashPassword(password);

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // 3. SQL Atualizado para incluir nome_completo e email
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
                // Códigos mais comuns:
                // 1062 → Duplicate entry (usuário OU email já existem)
                if (ex.Number == 1062)
                {
                    MessageBox.Show("Este usuário ou e-mail já estão cadastrados.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Erro no banco de dados:\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return false;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Entrada inválida: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado:\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                            // Atualiza a senha APENAS se o usuário E o e-mail forem encontrados na mesma linha
                            string sql = "UPDATE usuarios SET senha = @senha WHERE usuario = @usuario AND email = @email";

                            using (var cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@usuario", username);
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Parameters.AddWithValue("@senha", newPassword); // Idealmente, use hash aqui!

                                // ExecuteNonQuery retorna o número de linhas afetadas.
                                // Se for maior que 0, significa que encontrou o usuário e atualizou.
                                int rowsAffected = cmd.ExecuteNonQuery();

                                return rowsAffected > 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro ao atualizar senha: " + ex.Message);
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