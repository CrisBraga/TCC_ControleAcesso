using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;

namespace wpf_exemplo
{
    public partial class ManageResids : Window
    {
        private int _moradorId;

        public ManageResids(int moradorId)
        {
            InitializeComponent();
            _moradorId = moradorId;
            CarregarDadosMorador();
        }

        // Botão Voltar
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void CarregarDadosMorador()
        {
            string connStr = "server=localhost;database=SISTEMA;uid=root;pwd=senha;";
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = "SELECT * FROM moradores WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", _moradorId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                TxtNome.Text = reader.GetString("nome_completo");
                TxtBloco.Text = reader.GetString("bloco");
                TxtApartamento.Text = reader.GetString("apartamento");
                TxtTelefone.Text = reader.GetString("telefone");

                TxtFinger1.Text = reader["fingerprint_id_1"]?.ToString();
                TxtFinger2.Text = reader["fingerprint_id_2"]?.ToString();

                ChkAtivo.IsChecked = reader.GetBoolean("ativo");
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            string connStr = "server=localhost;database=SISTEMA;uid=root;pwd=senha;";
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = @"
                UPDATE moradores SET
                    nome_completo = @nome,
                    bloco = @bloco,
                    apartamento = @apto,
                    telefone = @tel,
                    fingerprint_id_1 = @fp1,
                    fingerprint_id_2 = @fp2,
                    ativo = @ativo
                WHERE id = @id";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@nome", TxtNome.Text);
            cmd.Parameters.AddWithValue("@bloco", TxtBloco.Text);
            cmd.Parameters.AddWithValue("@apto", TxtApartamento.Text);
            cmd.Parameters.AddWithValue("@tel", TxtTelefone.Text);

            cmd.Parameters.AddWithValue("@fp1",
                string.IsNullOrWhiteSpace(TxtFinger1.Text) ? DBNull.Value : TxtFinger1.Text);

            cmd.Parameters.AddWithValue("@fp2",
                string.IsNullOrWhiteSpace(TxtFinger2.Text) ? DBNull.Value : TxtFinger2.Text);

            cmd.Parameters.AddWithValue("@ativo", ChkAtivo.IsChecked == true);
            cmd.Parameters.AddWithValue("@id", _moradorId);

            cmd.ExecuteNonQuery();

            MessageBox.Show("Morador atualizado com sucesso!");
        }
    }
}
