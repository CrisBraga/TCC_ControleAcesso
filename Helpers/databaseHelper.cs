using MySql.Data.MySqlClient;
using System;
using System.Windows;
using wpf_exemplo.Helpers;

namespace wpf_exemplo.Helpers
{
    public static class DatabaseHelper
    {
        // Ajuste conforme seu ambiente
        private const string connectionString = "Server=localhost;Database=SISTEMA;Uid=root;Pwd='';";

        public static bool RegisterUser(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("Usuário e senha não podem estar vazios.");
                }

                string hashedPassword = PasswordHelper.HashPassword(password);

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO porteiros (username, password_hash) VALUES (@u, @p)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", hashedPassword);

                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Códigos mais comuns:
                // 1062 → Duplicate entry (usuário já existe)
                if (ex.Number == 1062)
                {
                    MessageBox.Show("Este nome de usuário já está cadastrado.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        public static bool ValidateLogin(string username, string password)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT password_hash FROM porteiros WHERE username = @u";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader.GetString("password_hash");
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
    }
}
