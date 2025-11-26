using System.Windows; // Adicione isso no topo

namespace wpf_exemplo
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

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
    }
}