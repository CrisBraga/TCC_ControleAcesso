using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using wpf_exemplo.Models;

namespace wpf_exemplo
{
    public partial class ListaMoradores : Window
    {
        public ListaMoradores()
        {
            InitializeComponent();
            CarregarMoradores();
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CarregarMoradores()
        {
            var moradores = new List<Morador>();

            string connStr = "server=localhost;database=SISTEMA;uid=root;pwd=senha;";
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = @"
                SELECT id, nome_completo, bloco, apartamento,
                       telefone, fingerprint_id_1, fingerprint_id_2, ativo
                FROM moradores";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                moradores.Add(new Morador
                {
                    Id = reader.GetInt32("id"),
                    NomeCompleto = reader.GetString("nome_completo"),
                    Bloco = reader.GetString("bloco"),
                    Apartamento = reader.GetString("apartamento"),
                    Telefone = reader.GetString("telefone"),
                    FingerprintId1 = reader["fingerprint_id_1"] == DBNull.Value ? null : reader.GetInt32("fingerprint_id_1"),
                    FingerprintId2 = reader["fingerprint_id_2"] == DBNull.Value ? null : reader.GetInt32("fingerprint_id_2"),
                    Ativo = reader.GetBoolean("ativo")
                });
            }

            LvMoradores.ItemsSource = moradores;
        }

        private void LvMoradores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LvMoradores.SelectedItem is Morador morador)
            {
                var editar = new ManageResids(morador.Id);
                editar.ShowDialog();
                CarregarMoradores();
            }
        }
    }
}
