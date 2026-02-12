using System;
using System.Threading.Tasks;
using System.Windows;
using wpf_exemplo.Helpers; // Importante para chamar o DatabaseHelper

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

            // 1. Validações básicas
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(novaSenha))
            {
                MySnackbar.MessageQueue?.Enqueue("Preencha todos os campos.");
                return;
            }

            if (novaSenha != confirmarSenha)
            {
                MySnackbar.MessageQueue?.Enqueue("As senhas não coincidem.");
                return;
            }

            // 2. Tenta atualizar no banco
            bool sucesso = DatabaseHelper.UpdatePassword(usuario, email, novaSenha);

            if (sucesso)
            {
                MySnackbar.MessageQueue?.Enqueue("Senha atualizada com sucesso!");
                await Task.Delay(2000); // Espera um pouco para o usuário ler
                this.Close(); // Fecha a janela
            }
            else
            {
                // Se falhou, é porque o Usuário + Email não bateram com nenhum registro
                MessageBox.Show("Usuário ou E-mail incorretos.\nNão foi possível alterar a senha.",
                                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}