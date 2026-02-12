using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace wpf_exemplo.Helpers
{
    public static class NotificationHelper
    {
        // Método para ERRO
        public static void ShowError(Snackbar snackbar, string message)
        {
            if (snackbar == null) return;

            // Busca a cor vermelha definida no App.xaml
            var errorBrush = Application.Current.FindResource("ErrorBrush") as SolidColorBrush;

            snackbar.Background = errorBrush;
            snackbar.MessageQueue?.Enqueue(message);
        }

        // Método para SUCESSO (com espera opcional antes de fechar a janela)
        public static async Task ShowSuccess(Snackbar snackbar, string message, Window? windowToClose = null)
        {
            if (snackbar == null) return;

            // Busca a cor verde definida no App.xaml
            var successBrush = Application.Current.FindResource("SuccessBrush") as SolidColorBrush;

            snackbar.Background = successBrush;
            snackbar.MessageQueue?.Enqueue(message);

            // Se você passou uma janela para fechar, ele espera e fecha
            if (windowToClose != null)
            {
                await Task.Delay(2000); // Espera 2 segundos
                windowToClose.Close();
            }
        }
    }
}