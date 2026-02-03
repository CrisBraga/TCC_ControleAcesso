using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using wpf_exemplo.Models;

namespace wpf_exemplo
{
    public partial class ListaMoradores : Window
    {
        private string nomePorteiro;

        // 1. Criamos essa lista para guardar os dados na memória
        private List<Morador> _listaOriginal = new List<Morador>();

        public ListaMoradores(string porteiroLogado)
        {
            InitializeComponent();
            nomePorteiro = porteiroLogado;
            CarregarMoradores();
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1(nomePorteiro);
            window1.WindowState = WindowState.Maximized;
            window1.Show();
            this.Close();
        }

        private void CarregarMoradores()
        {
            // Limpa a lista antes de carregar
            _listaOriginal.Clear();

            string connStr = "server=localhost;database=SISTEMA;uid=root;pwd=;";
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
                // 2. Adiciona na lista da memória (_listaOriginal) em vez de uma variável local
                _listaOriginal.Add(new Morador
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

            // Exibe a lista completa no começo
            LvMoradores.ItemsSource = _listaOriginal;
        }

        private static readonly char[] separator = [' '];

        // 3. A Lógica da Pesquisa (Adicione este método)
        private void TxtPesquisa_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textoDigitado = ((TextBox)sender).Text.ToLower();

            if (string.IsNullOrWhiteSpace(textoDigitado))
            {
                LvMoradores.ItemsSource = _listaOriginal;
                return;
            }

            // 1. QUEBRA O TEXTO EM PALAVRAS SEPARADAS PELA TECLA ESPAÇO
            // Exemplo: Se digitar "Joao A", vira uma lista com {"joao", "a"}
            var termos = textoDigitado.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            // 2. FILTRO INTELIGENTE
            var listaFiltrada = _listaOriginal.Where(m =>
                // Para o morador aparecer, ele precisa satisfazer TODOS (All) os termos digitados
                termos.All(termo =>
                    (m.NomeCompleto?.ToLower().Contains(termo) ?? false) ||
                    (m.Bloco?.ToLower().Contains(termo) ?? false) ||
                    (m.Apartamento?.ToLower().Contains(termo) ?? false)
                )
            ).ToList();

            LvMoradores.ItemsSource = listaFiltrada;
        }
        /*
        private void FiltrarMoradores(object sender, TextChangedEventArgs e)
        {
            // 1. Pega os textos das 3 caixas (e garante que não sejam nulos)
            string filtroNome = TxtNome.Text?.ToLower() ?? "";
            string filtroBloco = TxtBloco.Text?.ToLower() ?? "";
            string filtroApto = TxtApto.Text?.ToLower() ?? "";

            // 2. Verifica se a lista original foi carregada
            if (_listaOriginal == null) return;

            // 3. Aplica o filtro "E" (AND). O morador tem que bater com TUDO que foi digitado.
            var listaFiltrada = _listaOriginal.Where(m =>
                (string.IsNullOrEmpty(filtroNome) || (m.NomeCompleto?.ToLower().Contains(filtroNome) ?? false)) &&
                (string.IsNullOrEmpty(filtroBloco) || (m.Bloco?.ToLower().Contains(filtroBloco) ?? false)) &&
                (string.IsNullOrEmpty(filtroApto) || (m.Apartamento?.ToLower().Contains(filtroApto) ?? false))
            ).ToList();

            // 4. Atualiza a tela
            LvMoradores.ItemsSource = listaFiltrada;
        }*/

        private void LvMoradores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LvMoradores.SelectedItem is Morador morador)
            {
                var editar = new ManageResids(morador.Id);
                editar.ShowDialog();

                // Recarrega do banco para pegar as alterações feitas na edição
                CarregarMoradores();
            }
        }
    }
}