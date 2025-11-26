using System.Windows;
using wpf_exemplo.Helpers;

namespace wpf_exemplo
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btn_register(object sender, RoutedEventArgs e)
        {
            string username = txtUsuario.Text;
            string password = txtSenha.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Preencha todos os campos!");
                return;
            }

            if (DatabaseHelper.RegisterUser(username, password))
            {
                MessageBox.Show("Usuário cadastrado com sucesso!");
                this.Close();
            }
        }

        private void btn_cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
