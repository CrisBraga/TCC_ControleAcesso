using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using wpf_exemplo.Services;
using wpf_exemplo.Models;
using MaterialDesignThemes.Wpf; // Necessário para o DialogHost e DrawerHost

namespace wpf_exemplo
{
    public partial class Window1 : Window
    {
        // Variáveis globais
        private string nomeUser; // Nome tratado para exibição
        private string _usuarioLogado; // Nome original para passar entre telas

        // Serviços
        private SerialArduinoService _arduino;
        private AccessControlService _accessControl;

        public Window1(string username)
        {
            InitializeComponent();

            // 1. Tratamento do Nome do Usuário
            _usuarioLogado = username;
            nomeUser = username.Replace("Olá, ", ""); // Garante que não duplique o "Olá"

            if (txtUsuarioNome != null)
                txtUsuarioNome.Text = $"Olá, {nomeUser}";

            // 2. Inicialização dos Serviços
            _arduino = new SerialArduinoService();
            _accessControl = new AccessControlService();

            // 3. Eventos do Arduino (Onde a mágica acontece)
            _arduino.OnFingerprintDetected += FingerprintDetected; // Seu método original
            _arduino.OnNoMatchDetected += NoMatchDetected;         // Seu método original

            // 4. Conectar Arduino e Carregar Dados ao abrir a janela
            Loaded += async (s, e) =>
            {
                AtualizarDashboard(); // Carrega as listas do banco

                await Task.Delay(500); // Pequeno delay para a interface carregar antes de conectar
                try
                {
                    _arduino.Connect("COM3"); // Verifique se é COM3 no seu Windows
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao conectar Arduino: {ex.Message}");
                    // Opcional: MessageBox.Show("Erro ao conectar no Arduino.");
                }
            };
        }

        // ============================================
        // LÓGICA DE RECONHECIMENTO (O CORAÇÃO DO SISTEMA)
        // ============================================

        private void FingerprintDetected(int fingerprintId)
        {
            // O Arduino roda em outra thread, precisamos do Dispatcher para mexer na tela
            Dispatcher.Invoke(() =>
            {
                // 1. Valida se o ID existe e está ativo no banco
                bool autorizado = _accessControl.ValidateFingerprint(fingerprintId);

                if (autorizado)
                {
                    // 2. Registra o acesso no banco (Entrada)
                    _accessControl.RegisterAccess(fingerprintId);

                    // 3. Manda o comando para o Arduino abrir a porta
                    _arduino.SendCommand("OPEN");

                    // 4. Atualiza a lista na tela para mostrar quem acabou de entrar
                    AtualizarDashboard();

                    Console.WriteLine($"Acesso LIBERADO: ID {fingerprintId}");
                }
                else
                {
                    // 5. Se não autorizado, mostra alerta e loga erro
                    Console.WriteLine($"Acesso NEGADO: ID {fingerprintId}");
                    MostrarAlertaNaoAutorizado();
                }
            });
        }

        private void NoMatchDetected()
        {
            Dispatcher.Invoke(() =>
            {
                Console.WriteLine("Digital desconhecida/não cadastrada no sensor.");
                MostrarAlertaNaoAutorizado();
            });
        }

        // ============================================
        // FUNÇÕES VISUAIS E AUXILIARES
        // ============================================

        private async void MostrarAlertaNaoAutorizado()
        {
            // Exibe o alerta visual vermelho (se existir no XAML)
            if (AlertNaoAutorizado != null)
            {
                AlertNaoAutorizado.Visibility = Visibility.Visible;
                await Task.Delay(2500); // Espera 2.5 segundos
                AlertNaoAutorizado.Visibility = Visibility.Collapsed;
            }
        }

        private void AtualizarDashboard()
        {
            try
            {
                // Restaurei as DUAS listas que você tinha (Entradas e Saídas)
                List<AcessoDashboard> entradas = _accessControl.GetUltimasEntradas();
                List<AcessoDashboard> saidas = _accessControl.GetUltimasSaidas();

                if (LvEntradas != null) LvEntradas.ItemsSource = entradas;
                if (LvSaidas != null) LvSaidas.ItemsSource = saidas;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar listas: " + ex.Message);
            }
        }

        // ============================================
        // BOTÕES DO MENU E NAVEGAÇÃO
        // ============================================

        // Abre/Fecha o menu lateral
        private void MenuBtn_click(object sender, RoutedEventArgs e)
        {
            if (MainDrawerHost != null)
                MainDrawerHost.IsLeftDrawerOpen = !MainDrawerHost.IsLeftDrawerOpen;
        }

        // Ir para Adicionar Morador
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            MainDrawerHost.IsLeftDrawerOpen = false; // Fecha o menu
            NavegarParaJanela(new AddMoradorWindow(_usuarioLogado));
        }

        // Ir para Lista de Moradores (Gerenciar)
        private void GerenciarBtn_Click(object sender, RoutedEventArgs e)
        {
            MainDrawerHost.IsLeftDrawerOpen = false;
            NavegarParaJanela(new ListaMoradores(_usuarioLogado));
        }

        // Ir para Relatórios
        private void MenuRelatorioBtn_Click(object sender, RoutedEventArgs e)
        {
            MainDrawerHost.IsLeftDrawerOpen = false;
            NavegarParaJanela(new GenReport(_usuarioLogado));
        }

        // Ir para Histórico (Você mencionou "históricoRelatorio")
        private void MenuHistoricoBtn_Click(object sender, RoutedEventArgs e)
        {
            // ATENÇÃO: Verifique se o nome da classe é 'históricoRelatorio' ou 'HistoricoRelatorio' (maiúscula/acento)
            // No seu código colado estava 'históricoRelatorio'.
            // Se der erro, confira o nome do arquivo .xaml.cs dessa tela.
            MainDrawerHost.IsLeftDrawerOpen = false;
            // NavegarParaJanela(new históricoRelatorio(_usuarioLogado)); 

            // Vou deixar comentado para não quebrar se a classe não existir ainda, 
            // mas é só descomentar a linha de cima.
            MessageBox.Show("Funcionalidade de Histórico pronta para ser ligada.");
        }

        // ============================================
        // LOGOUT / SAIR DO SISTEMA
        // ============================================

        private void SairBtn_Click(object sender, RoutedEventArgs e)
        {
            // Abre o modal de confirmação
            if (DialogoPrincipal != null)
                DialogoPrincipal.IsOpen = true;
        }

        private void SairConfirmaBtn_Click(object sender, RoutedEventArgs e)
        {
            // Desconecta antes de sair
            try { _arduino?.Disconnect(); } catch { }

            MainWindow telaLogin = new MainWindow();
            telaLogin.WindowState = WindowState.Maximized;
            telaLogin.Show();
            this.Close();
        }

        // ============================================
        // MÉTODO GENÉRICO PARA NAVEGAR (LIMPEZA)
        // ============================================
        private void NavegarParaJanela(Window novaJanela)
        {
            // Sempre desconecta o Arduino ao sair dessa tela para liberar a porta COM
            try { _arduino?.Disconnect(); } catch { }

            novaJanela.WindowState = WindowState.Maximized;
            novaJanela.Show();
            this.Close();
        }

        // Garante que desconecte se fechar pelo 'X' da janela
        protected override void OnClosed(EventArgs e)
        {
            try { _arduino?.Disconnect(); } catch { }
            base.OnClosed(e);
        }
    }
}