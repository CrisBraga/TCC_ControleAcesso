using System.Windows;
using wpf_exemplo.Helpers;
using System.Windows.Media;

namespace wpf_exemplo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Conectado ao botão "ENTRAR" do XAML
        private async void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsuario.Text;
            string password = txtSenha.Password;

            // 1. Validação de campos vazios
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                msgErro.Background = new SolidColorBrush(Color.FromRgb(255, 30, 58));
                msgErro.MessageQueue?.Enqueue("Preencha todos os campos!");
                return;
            }


            // 2. Validação no Banco de Dados
            if (DatabaseHelper.ValidateLogin(username, password))
            {
                // LOGIN SUCESSO:

                msgLogin.MessageQueue?.Enqueue($"Login bem-sucedido");

                await Task.Delay(2000);

                // Cria a janela principal (o Dashboard que fizemos com menu lateral)
                Window1 dashboard = new Window1(username);

                // Abre o Dashboard
                dashboard.Show();

                // Fecha a tela de Login atual para não ficar aberta no fundo
                this.Close();
            }
            else
            {
                msgErro.Background = new SolidColorBrush(Color.FromRgb(255, 30, 58));
                // LOGIN FALHOU:
                msgErro.MessageQueue?.Enqueue("Usuário ou senha invalidos!");
            }
        }

        // Conectado ao botão "Criar uma conta" do XAML
        private void BtnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            // Abre a tela de registro
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog(); // ShowDialog trava a tela de trás até fechar o cadastro
        }
    }
}