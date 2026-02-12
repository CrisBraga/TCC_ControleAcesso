using System;
using System.Threading.Tasks;
using System.Windows;
using wpf_exemplo.Helpers;

namespace wpf_exemplo
{
    public partial class EsqueceuSenhaWindow : Window
    {
        public EsqueceuSenhaWindow()
        {
            InitializeComponent();
        }

        private async void BtnRedefinir_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text;
            string email = txtEmail.Text;
            string novaSenha = txtNovaSenha.Password;
            string confirmarSenha = txtConfirmarSenha.Password;

            // 1. Validações básicas (Campos Vazios)
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(novaSenha))
            {
                NotificationHelper.ShowError(MySnackbar, "Preencha todos os campos.");
                return;
            }

            // 2. Validação de Senha Igual
            if (novaSenha != confirmarSenha)
            {
                NotificationHelper.ShowError(MySnackbar, "As senhas não coincidem.");
                return;
            }

            // 3. Tenta atualizar no banco
            // Assumindo que você criou o método UpdatePassword no DatabaseHelper conforme conversamos antes
            bool sucesso = DatabaseHelper.UpdatePassword(usuario, email, novaSenha);

            if (sucesso)
            {
                // SUCESSO:
                // Passamos 'this' para que ele feche a janela automaticamente após 2 segundos
                await NotificationHelper.ShowSuccess(MySnackbar, "Senha atualizada com sucesso!", this);
            }
            else
            {
                // ERRO:
                // Substituímos o MessageBox antigo por um aviso bonito no Snackbar vermelho
                NotificationHelper.ShowError(MySnackbar, "Usuário ou E-mail incorretos.");
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}