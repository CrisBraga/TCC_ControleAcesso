using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace wpf_exemplo
{
    public partial class AddMoradorWindow : Window
    {
        public AddMoradorWindow()
        {
            InitializeComponent();
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text))
            {
                MessageBox.Show("Informe o nome do morador.");
                return;
            }

            string connStr = "server=localhost;database=SISTEMA;uid=root;pwd=;";
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = @"
                INSERT INTO moradores
                (nome_completo, bloco, apartamento, telefone,
                 fingerprint_id_1, fingerprint_id_2, ativo)
                VALUES
                (@nome, @bloco, @apto, @tel, @fp1, @fp2, @ativo)";

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

            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Morador cadastrado com sucesso!");
                this.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao cadastrar morador:\n" + ex.Message);
            }
        }
    }
}
