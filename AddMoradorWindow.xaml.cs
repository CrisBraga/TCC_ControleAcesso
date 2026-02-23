using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks; // <-- CORREÇÃO 1: Necessário para o tempo de espera do Snackbar
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf; // <-- CORREÇÃO 2: Necessário para configurar o Snackbar
using wpf_exemplo.Helpers;
using wpf_exemplo.Services;

namespace wpf_exemplo
{
    public partial class AddMoradorWindow : Window
    {
        private SerialArduinoService _arduino;
        private string _usuarioLogado;

        // controla qual dedo está sendo cadastrado
        private int _fingerIdEmCadastro = 0;
        private int _fingerSlotAtual = 1; // 1 ou 2

        public AddMoradorWindow(string username)
        {
            InitializeComponent();
            _usuarioLogado = username;

            // <-- CORREÇÃO 3: Conserta o "quadrado branco" inicializando a fila do Snackbar
            if (MainSnackbar.MessageQueue == null)
            {
                MainSnackbar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            }

            _arduino = new SerialArduinoService();
            _arduino.OnEnrollSuccess += OnEnrollSuccess;
            _arduino.OnEnrollStatus += AtualizarStatusBiometria;

            Loaded += async (s, e) =>
            {
                try
                {
                    // 2. Aguarda meio segundo para garantir que a tela anterior liberou o leitor
                    await Task.Delay(500);

                    // 3. Conecta ao Arduino
                    _arduino.Connect("COM4");
                    AtualizarStatusBiometria("Sensor conectado. Escolha um dedo para cadastrar.");
                }
                catch (Exception ex)
                {
                    AtualizarStatusBiometria($"Erro no sensor: {ex.Message}");
                }
            };
        }

        // =========================
        // GERAR ID LIVRE
        // =========================
        private int GerarNovoFingerprintId()
        {
            string connStr = "server=localhost;database=SISTEMA;uid=root;pwd=;";
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = @"
                SELECT IFNULL(MAX(id),0) + 1 FROM (
                    SELECT fingerprint_id_1 AS id FROM moradores WHERE fingerprint_id_1 IS NOT NULL
                    UNION
                    SELECT fingerprint_id_2 FROM moradores WHERE fingerprint_id_2 IS NOT NULL
                ) t";

            using var cmd = new MySqlCommand(sql, conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // =========================
        // CADASTRAR DIGITAL 1
        // =========================
        private void BtnCadastrarFinger1_Click(object sender, RoutedEventArgs e)
        {
            _fingerSlotAtual = 1;
            _fingerIdEmCadastro = GerarNovoFingerprintId();

            // <-- CORREÇÃO 4: Removemos o popup chato e colocamos direto no texto da tela!
            TxtStatusBiometria.Text = $"Posicione o dedo no sensor... (ID: {_fingerIdEmCadastro})";
            TxtStatusBiometria.Foreground = Brushes.Yellow;

            _arduino.SendCommand($"ENROLL:{_fingerIdEmCadastro}");
        }

        // =========================
        // CADASTRAR DIGITAL 2
        // =========================
        private void BtnCadastrarFinger2_Click(object sender, RoutedEventArgs e)
        {
            _fingerSlotAtual = 2;
            _fingerIdEmCadastro = GerarNovoFingerprintId();

            // <-- CORREÇÃO 4: Mensagem direta na tela
            TxtStatusBiometria.Text = $"Posicione o dedo no sensor... (ID: {_fingerIdEmCadastro})";
            TxtStatusBiometria.Foreground = Brushes.Yellow;

            _arduino.SendCommand($"ENROLL:{_fingerIdEmCadastro}");
        }

        // =========================
        // RESPOSTA DO ARDUINO
        // =========================
        private void OnEnrollSuccess(int fingerId)
        {
            Dispatcher.Invoke(() =>
            {
                if (_fingerSlotAtual == 1)
                    TxtFinger1.Text = fingerId.ToString();
                else
                    TxtFinger2.Text = fingerId.ToString();

                TxtStatusBiometria.Text = "Digital cadastrada com sucesso!";
                TxtStatusBiometria.Foreground = Brushes.LightGreen;
            });
        }

        // =========================
        // SALVAR MORADOR
        // =========================
        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                string.IsNullOrWhiteSpace(TxtBloco.Text) ||
                string.IsNullOrWhiteSpace(TxtApartamento.Text) ||
                string.IsNullOrWhiteSpace(TxtTelefone.Text))
            {
                NotificationHelper.ShowError(MainSnackbar, "Preencha todos os campos: Nome, Bloco, Apto e Telefone.");
                return;
            }

            string connStr = "server=localhost;database=SISTEMA;uid=root;pwd=;";
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = @"
                INSERT INTO moradores
                (nome_completo, bloco, apartamento, telefone,
                 fingerprint_id_1, fingerprint_id_2, ativo)
                VALUES
                (@nome, @bloco, @apto, @tel, @fp1, @fp2, @ativo)";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@nome", TxtNome.Text);
            cmd.Parameters.AddWithValue("@bloco", TxtBloco.Text);
            cmd.Parameters.AddWithValue("@apto", TxtApartamento.Text);
            cmd.Parameters.AddWithValue("@tel", TxtTelefone.Text);

            cmd.Parameters.AddWithValue("@fp1",
                string.IsNullOrWhiteSpace(TxtFinger1.Text) ? DBNull.Value : TxtFinger1.Text);

            cmd.Parameters.AddWithValue("@fp2",
                string.IsNullOrWhiteSpace(TxtFinger2.Text) ? DBNull.Value : TxtFinger2.Text);

            cmd.Parameters.AddWithValue("@ativo", ChkAtivo.IsChecked == true);

            try
            {
                cmd.ExecuteNonQuery();

                NotificationHelper.ShowSuccess(MainSnackbar, "Morador cadastrado com sucesso!");

                // Agora não dá erro porque adicionamos o using System.Threading.Tasks
                await Task.Delay(1500);

                BtnVoltar_Click(sender, e);
            }
            catch (MySqlException ex)
            {
                NotificationHelper.ShowError(MainSnackbar, $"Erro ao cadastrar: {ex.Message}");
            }
        }

        private void AtualizarStatusBiometria(string mensagem)
        {
            Dispatcher.Invoke(() =>
            {
                TxtStatusBiometria.Text = mensagem;
                TxtStatusBiometria.Foreground = Brushes.LightBlue;
            });
        }

        // =========================
        // VOLTAR / FECHAR
        // =========================
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            _arduino?.Disconnect();

            Window1 dashboard = new Window1(_usuarioLogado);

            // <-- CORREÇÃO 5: Usando o nosso Helper de navegação padrão
            navigationHelper.NavegarParaJanela(this, dashboard);
        }

        protected override void OnClosed(EventArgs e)
        {
            _arduino?.Disconnect();
            base.OnClosed(e);
        }
    }
}