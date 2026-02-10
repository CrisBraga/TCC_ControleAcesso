using System;
using System.Windows;
using wpf_exemplo.Services;
using MaterialDesignThemes.Wpf; // Necessário para o DialogHost

namespace wpf_exemplo
{
    public partial class Window1 : Window
    {
        private SerialArduinoService _arduino;
        private string _usuarioLogado;

        public Window1(string nomeUser)
        {
            InitializeComponent();

            _usuarioLogado = nomeUser;

            // Preenche o nome do usuário no topo da tela (conforme seu XAML)
            if (txtUsuarioNome != null)
                txtUsuarioNome.Text = nomeUser;

            // Configuração do Arduino
            _arduino = new SerialArduinoService();
            _arduino.OnFingerprintDetected += OnFingerprintDetected;

            Loaded += (s, e) =>
            {
                try
                {
                    _arduino.Connect("COM3"); // Ajuste sua porta COM aqui
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro Arduino: " + ex.Message);
                }
            };
        }

        // ============================================
        // LÓGICA DO ARDUINO
        // ============================================
        private void OnFingerprintDetected(int fingerId)
        {
            Dispatcher.Invoke(() =>
            {
                // Aqui você pode adicionar na lista de entradas (LvEntradas)
                // Por enquanto, mostra um aviso
                MessageBox.Show($"Digital ID {fingerId} detectada!");
            });
        }

        // ============================================
        // BOTÕES DO MENU LATERAL
        // ============================================

        // 1. Botão Dashboard (Entrada/Saída)
        private void MenuBtn_click(object sender, RoutedEventArgs e)
        {
            // Já estamos na Dashboard, só fecha o menu
            MainDrawerHost.IsLeftDrawerOpen = false;
        }

        // 2. Botão Adicionar Moradores (O IMPORTANTE)
        // OBS: Você precisa adicionar Click="AddBtn_Click" no seu XAML neste botão
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            NavegarParaJanela(new AddMoradorWindow(_usuarioLogado));
        }

        // 3. Botão Gerenciar Moradores
        private void GerenciarBtn_Click(object sender, RoutedEventArgs e)
        {
            // Se você ainda não criou a Window de Gerenciar, deixe comentado ou crie a janela
            NavegarParaJanela(new ListaMoradores(_usuarioLogado));
        }

        // 4. Botão Relatórios
        private void MenuRelatorioBtn_Click(object sender, RoutedEventArgs e)
        {
            NavegarParaJanela(new GenReport(_usuarioLogado));
        }

        // 5. Botão Histórico
        private void MenuHistoricoBtn_Click(object sender, RoutedEventArgs e)
        {
            // Exemplo se tiver a janela de histórico
            NavegarParaJanela(new históricoRelatorio(_usuarioLogado));
        }

        // ============================================
        // LOGOUT / SAIR
        // ============================================

        // Abre o Dialog de confirmação
        private void SairBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogoPrincipal.IsOpen = true;
        }

        // Confirma a saída (Botão SIM do Dialog)
        private void SairConfirmaBtn_Click(object sender, RoutedEventArgs e)
        {
            try { _arduino?.Disconnect(); } catch { }

            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        // ============================================
        // FUNÇÃO AUXILIAR PARA TROCAR DE TELA
        // ============================================
        private void NavegarParaJanela(Window novaJanela)
        {
            try { _arduino?.Disconnect(); } catch { } // Libera o Arduino

            novaJanela.WindowState = WindowState.Maximized;
            novaJanela.Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _arduino?.Disconnect();
            base.OnClosed(e);
        }
    }
}