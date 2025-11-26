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

        private void btn_login(object sender, RoutedEventArgs e)
        {
            string username = txtUsuario.Text;
            string password = txtSenha.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Preencha todos os campos!");
                return;
            }

            if (DatabaseHelper.ValidateLogin(username, password))
            {
                MessageBox.Show("Login bem-sucedido!");
                }
            else
            {
                MessageBox.Show("Nome de usuário ou senha incorretos.");
            }
        }

        private void btn_open_register(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}
