using System.Windows;
using wpf_exemplo.Helpers;
using MaterialDesignThemes.Wpf;

namespace wpf_exemplo
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            RegisterSnackbar.MessageQueue = new SnackbarMessageQueue();
        }

        private void btn_register(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Por favor, preencha todos os campos!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Validação: Verifica se as senhas são iguais
            if (password != confirmPassword)
            {
                MessageBox.Show("As senhas não coincidem!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4. Salvar no Banco de Dados
            // O método RegisterUser deve retornar true se deu certo, ou false se falhou (ex: usuário já existe)
            if (DatabaseHelper.RegisterUser(username, password))
            {
                MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
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