using System; // Para o Exception
using System.Threading.Tasks; // Para o Task.Delay
using System.Windows;
using System.Windows.Media;
using wpf_exemplo.Helpers; // Certifique-se que o DatabaseHelper está aqui

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
            // 1. Pegando os valores dos campos (Agora incluindo Nome e Email)
            // Certifique-se que criou o x:Name="txtNome" no seu XAML também!
            string nome = txtNome.Text; 
            string email = txtEmail.Text;
            string username = txtUsuario.Text;
            string password = txtSenha.Password;
            string confirmPassword = txtConfirmarSenha.Password;

            // 2. Validação: Verifica se algum campo está vazio
            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) ||
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
            // Atualizamos o método para receber nome e email também
            if (DatabaseHelper.RegisterUser(nome, username, password, email))
            {
                msgRegistro.MessageQueue?.Enqueue("Usuário criado com sucesso!");

                // Espera 2 segundos para o usuário ler a mensagem
                await Task.Delay(2000);

                this.Close(); 
            }
            else
            {
                // Se falhou, provavelmente o usuário ou email já existem
                MessageBox.Show("Erro ao cadastrar.\nUsuário ou E-mail já existentes.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}