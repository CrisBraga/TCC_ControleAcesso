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

        private async void btn_register(object sender, RoutedEventArgs e)
        {
            // 1. Pegando os valores dos campos
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
                // ERRO: Campos vazios
                NotificationHelper.ShowError(msgErroRegistro, "Preencha todos os campos!");
                return;
            }

            // 3. Validação: Verifica se as senhas são iguais
            if (password != confirmPassword)
            {
                // ERRO: Senhas não batem
                NotificationHelper.ShowError(msgErroRegistro, "As senhas não coincidem!");
                return;
            }

            // 4. Salvar no Banco de Dados
            if (DatabaseHelper.RegisterUser(nome, username, password, email))
            {
                // SUCESSO:
                // Aqui passamos 'this' como terceiro argumento.
                // O Helper vai mostrar a mensagem verde, esperar 2 segundos e fechar ESSA janela automaticamente.
                await NotificationHelper.ShowSuccess(msgRegistro, "Usuário criado com sucesso!", this);
            }
            else
            {
                // ERRO: Falha no banco (provavelmente usuário ou email duplicado)
                NotificationHelper.ShowError(msgErroRegistro, "Erro ao cadastrar. Usuário ou E-mail já existem.");
            }
        }

        private void btn_cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}