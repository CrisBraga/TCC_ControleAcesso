using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace wpf_exemplo
{
    public partial class históricoRelatorio : Window
    {
        // Variável para guardar o nome do Porteiro logado
        private string _nomeUser;

        public históricoRelatorio(string nomeUser)
        {
            InitializeComponent();
            _nomeUser = nomeUser;

            // DICA: Aqui no futuro você vai carregar a lista de arquivos PDF 
            // salvos na pasta de relatórios de entrada/saída.
        }

        // --- BOTÃO VOLTAR (Corrigido) ---
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Volta para a tela principal (Window1), devolvendo o nome do porteiro
            Window1 telaPrincipal = new Window1(_nomeUser);

            telaPrincipal.WindowState = WindowState.Maximized;
            telaPrincipal.Show();

            this.Close(); // Fecha a tela de histórico
        }

        // --- EVENTOS DA LISTA E PESQUISA ---

        // Quando o porteiro der 2 cliques num item da lista (Ex: "Relatório 05/02 a 10/02")
        private void LvHistoricos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // FUTURO: Código para abrir o PDF selecionado na tela
            // Exemplo: Process.Start("explorer.exe", "CaminhoDoArquivo.pdf");
        }

        // Pesquisar relatórios (Ex: o porteiro digita uma data específica)
        private void TxtpesqHistorico_TextChanged(object sender, TextChangedEventArgs e)
        {
            // FUTURO: Código para filtrar a lista visualmente
        }

        // (Este método apareceu duplicado no seu código anterior, mantive vazio para não dar erro)
        private void LvHistRelatorio_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
    }
}