using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient; // Alterado para MySQL
using wpf_exemplo.Helpers;

namespace wpf_exemplo
{
    public class RelatorioItem
    {
        public string NomeArquivo { get; set; }
        public string TipoRelatorio { get; set; }
        public string DataCriacao { get; set; }
        public string CaminhoCompleto { get; set; }
    }

    public partial class históricoRelatorio : Window
    {
        private string _nomeUser;
        private List<RelatorioItem> _todosRelatorios;

        // SUA STRING DE CONEXÃO DO MYSQL
        private string connectionString = "server=localhost;database=SISTEMA;uid=root;pwd=;";

        public históricoRelatorio(string nomeUser)
        {
            InitializeComponent();
            _nomeUser = nomeUser;
            CarregarRelatoriosDoBanco();
        }

        private void CarregarRelatoriosDoBanco()
        {
            _todosRelatorios = new List<RelatorioItem>();

            // SELECT DO MYSQL
            string query = "SELECT NomeArquivo, TipoRelatorio, DataCriacao, CaminhoCompleto FROM HistoricoRelatorios ORDER BY DataCriacao DESC";

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(connectionString))
                using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                {
                    conexao.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _todosRelatorios.Add(new RelatorioItem
                            {
                                NomeArquivo = reader["NomeArquivo"].ToString(),
                                TipoRelatorio = reader["TipoRelatorio"].ToString(),
                                DataCriacao = Convert.ToDateTime(reader["DataCriacao"]).ToString("dd/MM/yyyy HH:mm"),
                                CaminhoCompleto = reader["CaminhoCompleto"].ToString()
                            });
                        }
                    }
                }

                LvHistoricos.ItemsSource = _todosRelatorios;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar histórico: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            Window1 principal = new Window1(_nomeUser);
            navigationHelper.NavegarParaJanela(this, principal);
        }

        private void LvHistoricos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LvHistoricos.SelectedItem is RelatorioItem selecionado)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = selecionado.CaminhoCompleto,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Não foi possível abrir o arquivo. {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TxtpesqHistorico_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = TxtpesqHistorico.Text.ToLower();

            if (string.IsNullOrWhiteSpace(filtro))
            {
                LvHistoricos.ItemsSource = _todosRelatorios;
            }
            else
            {
                var listaFiltrada = _todosRelatorios.Where(r =>
                    r.NomeArquivo.ToLower().Contains(filtro) ||
                    r.TipoRelatorio.ToLower().Contains(filtro) ||
                    r.DataCriacao.Contains(filtro)).ToList();

                LvHistoricos.ItemsSource = listaFiltrada;
            }
        }

        private void LvHistRelatorio_MouseDoubleClick(object sender, MouseButtonEventArgs e) { }
    }
}