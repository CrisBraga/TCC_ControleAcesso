using System.Windows; // Adicione isso no topo

namespace wpf_exemplo
{
    public partial class Window1 : Window
    {

        public Window1(string username)
        {
            InitializeComponent();

            txtUsuarioNome.Text = $"Olá, {username}";

        }

        private void MenuBtn_click(object sender, RoutedEventArgs e)
        {

            MainDrawerHost.IsLeftDrawerOpen = false;
        }

        private void MenuRelatorioBtn_Click(object sender, RoutedEventArgs e)
        {

            MainDrawerHost.IsLeftDrawerOpen = false;
        }

        private void MenuMoradorBtn_Click(object sender, RoutedEventArgs e)
        {

            MainDrawerHost.IsLeftDrawerOpen = false;
        }

        private void SairBtn_Click(object sender, RoutedEventArgs e)
        {
            // Apenas abre o visual bonito que criamos no XAML
            DialogoPrincipal.IsOpen = true;
        }

        // Esse é o clique do botão "SIM, SAIR" de dentro da caixinha
        private void SairConfirmaBtn_Click(object sender, RoutedEventArgs e)
        {
            // 1. Instancia a tela de login
            MainWindow telaLogin = new MainWindow();

            // 2. Maximiza
            telaLogin.WindowState = WindowState.Maximized;

            // 3. Mostra
            telaLogin.Show();

            // 4. Fecha a janela atual
            this.Close();
        }

        private void GerenciarBtn_Click(object sender, RoutedEventArgs e)
        {

            string nomeAtual = txtUsuarioNome.Text;

            ListaMoradores listaMoradores = new ListaMoradores(nomeAtual);

            listaMoradores.WindowState = WindowState.Maximized;

            listaMoradores.Show();

            this.Close();

        }
    }
}