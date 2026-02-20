using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using wpf_exemplo.Helpers;

namespace wpf_exemplo
{
    public partial class GenReport : Window
    {
        // Variável para guardar o usuário que veio da tela anterior
        private string _usuarioLogado;

        // 1. ALTERADO: O construtor agora pede o nome do usuário
        public GenReport(string usuario)
        {
            InitializeComponent();
            _usuarioLogado = usuario;

            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ==========================================================
        // 2. LÓGICA DO BOTÃO VOLTAR (Corrigida)
        // ==========================================================
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Agora instanciamos a Window1 (Dashboard) e não a Login
            // E passamos o usuário de volta para ela
            Window1 dashboard = new Window1(_usuarioLogado);

            dashboard.WindowState = WindowState.Maximized;
            dashboard.Show();

            this.Close();
        }

        // ... O RESTANTE DO CÓDIGO DO PDF PERMANECE IGUAL ...
        // (BtnGerarRelatorio_Click e GerarArquivoPDF)
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
                // 1. Cria a pasta oficial do sistema para guardar o histórico
                string pastaHistorico = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RelatoriosGerados");
                if (!Directory.Exists(pastaHistorico))
                {
                    Directory.CreateDirectory(pastaHistorico);
                }

                // 2. Monta o nome do arquivo dinamicamente
                string tipoLimpo = CmbTipoRelatorio.Text.Replace(" ", "").Replace("/", "_");
                string nomeArquivo = $"Rel_{tipoLimpo}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string caminhoCompleto = Path.Combine(pastaHistorico, nomeArquivo);

                // 3. Gera o PDF usando o seu método
                GerarArquivoPDF(caminhoCompleto);

                // 4. Mostra o sucesso com o Toast Flutuante
                await NotificationHelper.ShowSuccess(MainSnackbar, "Relatório gerado e salvo no histórico!");
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError(MainSnackbar, $"Erro ao gerar: {ex.Message}");
            }
        }

        private void GerarArquivoPDF(string caminho)
        {
            // ... (Mantenha seu código do QuestPDF aqui) ...
            // Apenas lembre de copiar o código que você já tinha.
            // Se precisar que eu repita, me avise.
            // Código simplificado para compilar:
            string dtInicio = DtInicio.SelectedDate.Value.ToString("dd/MM/yyyy");
            string dtFim = DtFim.SelectedDate.Value.ToString("dd/MM/yyyy");
            string tipo = CmbTipoRelatorio.Text;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Header().Text($"Relatório: {tipo}").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium).AlignCenter();
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Text($"Período: {dtInicio} até {dtFim}");
                        col.Item().Text($"Solicitado por: {_usuarioLogado}"); // Adicionei quem pediu o relatório
                    });
                });
            }).GeneratePdf(caminho);
        }

        // Estilos auxiliares... (Mantenha os seus)
        static IContainer EstiloCabecalho(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
        static IContainer EstiloLinha(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
    }
}