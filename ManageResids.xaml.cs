using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // Necessário para o botão de Voltar

namespace wpf_exemplo
{
    public partial class ManageResids : Page
    {
        public ManageResids()
        {
            InitializeComponent();
        }

        // Botão de Voltar (Seta no canto superior)
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Verifica se tem como voltar na navegação
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                // Se não tiver histórico, limpa o frame para mostrar o fundo da janela principal
                NavigationService.Content = null;
            }
        }

        // Botão Cadastrar
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder: Apenas uma mensagem de teste
            MessageBox.Show("Botão [Cadastrar] clicado com sucesso!", "Teste", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Botão Editar
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder: Apenas uma mensagem de teste
            MessageBox.Show("Botão [Editar] clicado com sucesso!", "Teste", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Botão Excluir
        private void BtnExclude_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder: Apenas uma mensagem de teste
            MessageBox.Show("Botão [Excluir] clicado com sucesso!", "Teste", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}