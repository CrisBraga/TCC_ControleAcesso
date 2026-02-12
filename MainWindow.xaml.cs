using System;
using System.Threading.Tasks; // Necessário para o Task.Delay
using System.Windows;
using wpf_exemplo.Helpers;

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
                NotificationHelper.ShowError(msgErro, "Preencha todos os campos!");
                return;
            }

            // 2. Validação no Banco de Dados
            if (DatabaseHelper.ValidateLogin(username, password))
            {
                // LOGIN SUCESSO:
                // Chamamos o helper para mostrar a mensagem verde
                await NotificationHelper.ShowSuccess(msgLogin, "Login bem-sucedido");

                // =================================================================
                // CORREÇÃO AQUI: Adicionamos o delay manual para dar tempo de ver
                // =================================================================
                await Task.Delay(2000);

                // Abre o Dashboard
                Window1 dashboard = new Window1(username);
                dashboard.Show();

                // Fecha a tela de Login
                this.Close();
            }
            else
            {
                // LOGIN FALHOU:
                NotificationHelper.ShowError(msgErro, "Usuário ou senha inválidos!");
            }
        }

        // Conectado ao botão "Criar uma conta" do XAML
        private void BtnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }

        // Conectado ao botão "Esqueceu a senha"
        private void BtnEsqueceuSenha_Click(object sender, RoutedEventArgs e)
        {
            EsqueceuSenhaWindow janela = new EsqueceuSenhaWindow();
            janela.ShowDialog();
        }
    }
}