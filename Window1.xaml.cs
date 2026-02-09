using System;
using System.Collections.Generic;
using System.Windows;
using wpf_exemplo.Services;
using wpf_exemplo.Models;
using System.Threading.Tasks;


namespace wpf_exemplo
{
    public partial class Window1 : Window 
    {
        private string nomeUser;

        private SerialArduinoService _arduino;
        private AccessControlService _accessControl;

        public Window1(string username)
        {
            InitializeComponent();

            // Usuário logado
            nomeUser = username.Replace("Olá, ", "");
            txtUsuarioNome.Text = $"Olá, {nomeUser}";

            // Serviços
            _arduino = new SerialArduinoService();
            _accessControl = new AccessControlService();

            // Eventos do Arduino
            _arduino.OnArduinoReady += ArduinoReady;
            _arduino.OnFingerprintDetected += FingerprintDetected;
            _arduino.OnNoMatchDetected += NoMatchDetected;


            // Conectar no Arduino
            Loaded += (s, e) =>
            {
                try
                {
                    _arduino.Connect("COM3");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erro ao conectar no Arduino:\n{ex.Message}",
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            };

            // Carrega dashboard inicial
            AtualizarDashboard();
        }

        // ========================
        // EVENTOS DO ARDUINO
        // ========================

        private void ArduinoReady()
        {
            Dispatcher.Invoke(() =>
            {
                Console.WriteLine("Arduino pronto");
            });
        }

        private async void MostrarAlertaNaoAutorizado()
        {
            AlertNaoAutorizado.Visibility = Visibility.Visible;

            await Task.Delay(2500);

            AlertNaoAutorizado.Visibility = Visibility.Collapsed;
        }
        private void NoMatchDetected()
        {
            Dispatcher.Invoke(() =>
            {
                MostrarAlertaNaoAutorizado();
            });
        }

        private void FingerprintDetected(int fingerprintId)
        {
            Dispatcher.Invoke(() =>
            {
                bool autorizado = _accessControl.ValidateFingerprint(fingerprintId);

                if (autorizado)
                {
                    _accessControl.RegisterAccess(fingerprintId);
                    _arduino.SendCommand("OPEN");

                    AtualizarDashboard();

                    Console.WriteLine($"Acesso liberado: {fingerprintId}");
                }
                else
                {
                    Console.WriteLine($"Acesso NEGADO: {fingerprintId}");
                    MostrarAlertaNaoAutorizado();
                }
            });
        }

        // ========================
        // DASHBOARD
        // ========================

        private void AtualizarDashboard()
        {
            List<AcessoDashboard> entradas = _accessControl.GetUltimasEntradas();
            List<AcessoDashboard> saidas = _accessControl.GetUltimasSaidas();

            LvEntradas.ItemsSource = entradas;
            LvSaidas.ItemsSource = saidas;
        }

        // ========================
        // BOTÕES / MENU
        // ========================

        private void MenuBtn_click(object sender, RoutedEventArgs e)
            => MainDrawerHost.IsLeftDrawerOpen = false;

        private void MenuRelatorioBtn_Click(object sender, RoutedEventArgs e)
            => MainDrawerHost.IsLeftDrawerOpen = false;

        private void MenuMoradorBtn_Click(object sender, RoutedEventArgs e)
            => MainDrawerHost.IsLeftDrawerOpen = false;

        private void SairBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogoPrincipal.IsOpen = true;
        }

        private void SairConfirmaBtn_Click(object sender, RoutedEventArgs e)
        {
            _arduino.Disconnect();

            MainWindow telaLogin = new MainWindow();
            telaLogin.WindowState = WindowState.Maximized;
            telaLogin.Show();
            Close();
        }

        private void GerenciarBtn_Click(object sender, RoutedEventArgs e)
        {
            _arduino.Disconnect();

            ListaMoradores listaMoradores = new ListaMoradores(nomeUser);
            listaMoradores.WindowState = WindowState.Maximized;
            listaMoradores.Show();
            Close();
        }

        private void MenuHistoricoBtn_Click(object sender, RoutedEventArgs e)
        {
            _arduino.Disconnect();

            históricoRelatorio telaHistorico = new históricoRelatorio(nomeUser);
            telaHistorico.WindowState = WindowState.Maximized;
            telaHistorico.Show();
            Close();
        }

        // ========================
        // FECHAMENTO LIMPO
        // ========================

        protected override void OnClosed(EventArgs e)
        {
            _arduino?.Disconnect();
            base.OnClosed(e);
        }
    }
}
