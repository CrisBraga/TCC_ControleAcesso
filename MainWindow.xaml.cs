using System.Windows;
using wpf_exemplo.Helpers; // Certifique-se que o namespace do seu DatabaseHelper está correto

namespace wpf_exemplo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Conectado ao botão "ENTRAR" do XAML
        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsuario.Text;
            string password = txtSenha.Password;

            // 1. Validação de campos vazios
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Preencha todos os campos!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            // 2. Validação no Banco de Dados
            if (DatabaseHelper.ValidateLogin(username, password))
            {
                // LOGIN SUCESSO:

                // Cria a janela principal (o Dashboard que fizemos com menu lateral)
                Window1 dashboard = new Window1(username);

                // Abre o Dashboard
                dashboard.Show();

                // Fecha a tela de Login atual para não ficar aberta no fundo
                this.Close();
            }
            else
            {
                // LOGIN FALHOU:
                MessageBox.Show("Nome de usuário ou senha incorretos.", "Erro de Acesso", MessageBoxButton.OK, MessageBoxImage.Error);
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