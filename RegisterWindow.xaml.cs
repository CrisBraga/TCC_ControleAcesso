using System.Windows;
using wpf_exemplo.Helpers;
using System.Windows.Media;

namespace wpf_exemplo
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void btn_register(object sender, RoutedEventArgs e)
        {
            // 1. Pegando os valores dos campos definidos no XAML
            string username = txtUsuario.Text;
            string password = txtSenha.Password;
            string confirmPassword = txtConfirmarSenha.Password;

            // 2. Validação: Verifica se algum campo está vazio
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                msgErroRegistro.Background = new SolidColorBrush(Color.FromRgb(255, 30, 58));
                msgErroRegistro.MessageQueue?.Enqueue("Preencha todos os campos!");
                return;
            }

            // 3. Validação: Verifica se as senhas são iguais
            if (password != confirmPassword)
            {
                msgErroRegistro.Background = new SolidColorBrush(Color.FromRgb(255, 30, 58));
                msgErroRegistro.MessageQueue?.Enqueue("As senhas não coincidem!");
                return;
            }

            // 4. Salvar no Banco de Dados
            // O método RegisterUser deve retornar true se deu certo, ou false se falhou (ex: usuário já existe)
            if (DatabaseHelper.RegisterUser(username, password))
            {
                msgRegistro.MessageQueue?.Enqueue("Usuário criado com sucesso!");

                await Task.Delay(2000);

                this.Close(); // Fecha a janela e volta para o login
            }
            else
            {
                MessageBox.Show("Erro ao cadastrar. Verifique se o usuário já existe.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_cancel(object sender, RoutedEventArgs e)
        {
            // Fecha a janela sem fazer nada
            this.Close();
        }
    }
}