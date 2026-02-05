using System.Windows;

namespace wpf_exemplo
{
    public partial class Window1 : Window
    {
        private string nomeUser;

        public Window1(string username)
        {
            InitializeComponent();

            nomeUser = username.Replace("Olá, ", "");

            txtUsuarioNome.Text = $"Olá, {nomeUser}";
        }
        private void MenuBtn_click(object sender, RoutedEventArgs e) => MainDrawerHost.IsLeftDrawerOpen = false;
        private void MenuRelatorioBtn_Click(object sender, RoutedEventArgs e) => MainDrawerHost.IsLeftDrawerOpen = false;
        private void MenuMoradorBtn_Click(object sender, RoutedEventArgs e) => MainDrawerHost.IsLeftDrawerOpen = false;

        private void SairBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogoPrincipal.IsOpen = true;
        }

        private void SairConfirmaBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow telaLogin = new MainWindow();
            telaLogin.WindowState = WindowState.Maximized;
            telaLogin.Show();
            this.Close();
        }

        private void GerenciarBtn_Click(object sender, RoutedEventArgs e)
        {

            ListaMoradores listaMoradores = new ListaMoradores(nomeUser);

            listaMoradores.WindowState = WindowState.Maximized;
            listaMoradores.Show();
            this.Close();
        }

        private void MenuHistoricoBtn_Click(object sender, RoutedEventArgs e)
        {
            MainDrawerHost.IsLeftDrawerOpen = false;

            históricoRelatorio telaHistorico = new históricoRelatorio(nomeUser);

            telaHistorico.WindowState = WindowState.Maximized;

            telaHistorico.Show();
            this.Close();

        }
    }
}