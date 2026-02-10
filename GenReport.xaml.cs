using System;
using System.Windows;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
        private void BtnGerarRelatorio_Click(object sender, RoutedEventArgs e)
        {
            // ... (mesmo código que você já tem) ...
            // Vou omitir aqui para economizar espaço, mantenha o seu código de PDF.
            if (DtInicio.SelectedDate == null || DtFim.SelectedDate == null)
            {
                MessageBox.Show("Selecione a Data de Início e Fim.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Arquivo PDF (*.pdf)|*.pdf",
                FileName = $"Relatorio_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    GerarArquivoPDF(dialog.FileName);
                    MessageBox.Show("PDF gerado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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