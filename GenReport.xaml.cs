using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using wpf_exemplo.Helpers;

namespace wpf_exemplo
{
    public partial class GenReport : Window
    {
        private string _usuarioLogado;

        // JÁ AJUSTADO PARA O NOME DO SEU BANCO: SISTEMA
        private string connectionString = "Server=localhost;Database=SISTEMA;Uid=root;Pwd=;";

        public GenReport(string usuario)
        {
            InitializeComponent();
            _usuarioLogado = usuario;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            Window1 principal = new Window1(_usuarioLogado);
            navigationHelper.NavegarParaJanela(this, principal);
        }

        private async void BtnGerarRelatorio_Click(object sender, RoutedEventArgs e)
        {
            if (DtInicio.SelectedDate == null || DtFim.SelectedDate == null)
            {
                NotificationHelper.ShowError(MainSnackbar, "Selecione a Data de Início e Fim.");
                return;
            }
            if (string.IsNullOrWhiteSpace(CmbTipoRelatorio.Text))
            {
                NotificationHelper.ShowError(MainSnackbar, "Selecione o Tipo de Relatório.");
                return;
            }

            try
            {
                string pastaHistorico = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RelatoriosGerados");
                if (!Directory.Exists(pastaHistorico)) Directory.CreateDirectory(pastaHistorico);

                string tipoLimpo = CmbTipoRelatorio.Text.Replace(" ", "").Replace("/", "_");
                string nomeArquivo = $"Rel_{tipoLimpo}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string caminhoCompleto = Path.Combine(pastaHistorico, nomeArquivo);

                // GERA O PDF
                GerarArquivoPDF(caminhoCompleto);

                // SALVA NO BANCO
                SalvarHistoricoNoBanco(nomeArquivo, CmbTipoRelatorio.Text, caminhoCompleto);

                await NotificationHelper.ShowSuccess(MainSnackbar, "Relatório gerado com sucesso!");

                // Abre o PDF
                Process.Start(new ProcessStartInfo
                {
                    FileName = caminhoCompleto,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError(MainSnackbar, $"Erro ao gerar: {ex.Message}");
            }
        }

        private void SalvarHistoricoNoBanco(string nomeArquivo, string tipo, string caminhoCompleto)
        {
            // O INSERT está combinando perfeitamente com a sua tabela HistoricoRelatorios
            string query = @"INSERT INTO HistoricoRelatorios (NomeArquivo, TipoRelatorio, DataCriacao, CaminhoCompleto, UsuarioSolicitante) 
                             VALUES (@Nome, @Tipo, @Data, @Caminho, @User)";

            using (MySqlConnection conexao = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(query, conexao))
            {
                cmd.Parameters.AddWithValue("@Nome", nomeArquivo);
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cmd.Parameters.AddWithValue("@Data", DateTime.Now);
                cmd.Parameters.AddWithValue("@Caminho", caminhoCompleto);
                cmd.Parameters.AddWithValue("@User", _usuarioLogado);

                conexao.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void GerarArquivoPDF(string caminho)
        {
            DateTime dataInicio = DtInicio.SelectedDate.Value;
            DateTime dataFim = DtFim.SelectedDate.Value;
            string tipoRelatorio = CmbTipoRelatorio.Text;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text($"Relatório: {tipoRelatorio}").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium).AlignCenter();

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text($"Período: {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}");
                        col.Item().Text($"Solicitado por: {_usuarioLogado}\n");

                        col.Item().Table(tabela =>
                        {
                            tabela.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Nome
                                columns.RelativeColumn(1.5f); // Bloco e Apt (Aumentei um pouco o espaço)
                                columns.RelativeColumn(2); // Data/Hora
                                columns.RelativeColumn(1); // Tipo (ENTRADA/SAIDA)
                            });

                            tabela.Header(header =>
                            {
                                header.Cell().Element(EstiloCabecalho).Text("Nome do Morador");
                                header.Cell().Element(EstiloCabecalho).Text("Bloco / Apt");
                                header.Cell().Element(EstiloCabecalho).Text("Data/Hora");
                                header.Cell().Element(EstiloCabecalho).Text("Acesso");
                            });

                            // QUERY AJUSTADA PARA O SEU BANCO DE DADOS
                            // Uso os "Alias" (a para acessos, m para moradores) para o código ficar mais limpo
                            // Adicionei m.apartamento e o ORDER BY no final
                            string queryJoin = @"
                                SELECT 
                                    m.nome_completo, 
                                    m.bloco, 
                                    m.apartamento,
                                    a.data_hora,
                                    a.tipo
                                FROM acessos a
                                INNER JOIN moradores m ON (a.fingerprint_id = m.fingerprint_id_1 OR a.fingerprint_id = m.fingerprint_id_2)
                                WHERE a.data_hora >= @DataInicio AND a.data_hora <= @DataFim
                                ORDER BY a.data_hora DESC";

                            using (MySqlConnection conexao = new MySqlConnection(connectionString))
                            using (MySqlCommand cmd = new MySqlCommand(queryJoin, conexao))
                            {
                                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                                cmd.Parameters.AddWithValue("@DataFim", dataFim.AddDays(1).AddSeconds(-1));

                                conexao.Open();
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        tabela.Cell().Element(EstiloLinha).Text(reader["nome_completo"].ToString());

                                        // Junta o Bloco e o Apartamento
                                        string bloco = reader["bloco"].ToString();
                                        string apto = reader["apartamento"].ToString();
                                        tabela.Cell().Element(EstiloLinha).Text($"{bloco} - Apt {apto}");

                                        tabela.Cell().Element(EstiloLinha).Text(Convert.ToDateTime(reader["data_hora"]).ToString("dd/MM/yyyy HH:mm"));

                                        string tipoAcesso = reader["tipo"].ToString();
                                        tabela.Cell().Element(EstiloLinha).Text(tipoAcesso).FontColor(tipoAcesso == "ENTRADA" ? Colors.Green.Medium : Colors.Red.Medium);
                                    }
                                }
                            }
                        });
                    });
                });
            }).GeneratePdf(caminho);
        }

        static IContainer EstiloCabecalho(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
        static IContainer EstiloLinha(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
    }
}