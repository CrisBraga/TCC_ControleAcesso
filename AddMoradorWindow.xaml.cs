using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Media;
using wpf_exemplo.Helpers;
using wpf_exemplo.Services;

namespace wpf_exemplo
{
    public partial class AddMoradorWindow : Window
    {
        private SerialArduinoService _arduino;

        // 1. Variável para guardar o usuário e não perder o login
        private string _usuarioLogado;

        // controla qual dedo está sendo cadastrado
        private int _fingerIdEmCadastro = 0;
        private int _fingerSlotAtual = 1; // 1 ou 2

        // 2. Construtor alterado para receber o username
        public AddMoradorWindow(string username)
        {
            InitializeComponent();

            _usuarioLogado = username; // Guarda o nome do usuário

            _arduino = new SerialArduinoService();
            _arduino.OnEnrollSuccess += OnEnrollSuccess;
            _arduino.OnEnrollStatus += AtualizarStatusBiometria;


            Loaded += (s, e) =>
            {
                try
                {
                    _arduino.Connect("COM3"); // ajuste se necessário
                }
                catch (Exception)
                {
                    // CHAMA SÓ A SUA FUNÇÃO PRONTA AQUI!
                    NotificationHelper.ShowError(MainSnackbar, "⚠️ Não foi possível conectar ao leitor biométrico.");
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

            MessageBox.Show(
                $"Posicione o dedo no sensor.\nID: {_fingerIdEmCadastro}",
                "Cadastro de Digital",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            _arduino.SendCommand($"ENROLL:{_fingerIdEmCadastro}");
        }

        // =========================
        // CADASTRAR DIGITAL 2
        // =========================
        private void BtnCadastrarFinger2_Click(object sender, RoutedEventArgs e)
        {
            _fingerSlotAtual = 2;
            _fingerIdEmCadastro = GerarNovoFingerprintId();

            MessageBox.Show(
                $"Posicione o dedo no sensor.\nID: {_fingerIdEmCadastro}",
                "Cadastro de Digital",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

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
        // 1. Adicionamos o 'async' aqui na declaração do método
        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // Valida se NOME, BLOCO, APARTAMENTO ou TELEFONE estão vazios
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                string.IsNullOrWhiteSpace(TxtBloco.Text) ||
                string.IsNullOrWhiteSpace(TxtApartamento.Text) ||
                string.IsNullOrWhiteSpace(TxtTelefone.Text))
            {
                NotificationHelper.ShowError(MainSnackbar, "Preencha todos os campos: Nome, Bloco, Apto e Telefone.");
                return; // Para a execução aqui e não salva no banco
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

                // 2. Trocamos o MessageBox pelo Snackbar de SUCESSO
                // Não passamos 'this' aqui, porque queremos abrir a Window1 antes de fechar
                await NotificationHelper.ShowSuccess(MainSnackbar, "Morador cadastrado com sucesso!");

                // 3. Aguarda 1.5 segundos para o usuário ler a mensagem na tela
                await Task.Delay(1500);

                // 4. Volta para o Dashboard
                BtnVoltar_Click(sender, e);
            }
            catch (MySqlException ex)
            {
                // Trocamos o MessageBox pelo Snackbar de ERRO
                NotificationHelper.ShowError(MainSnackbar, $"Erro ao cadastrar morador: {ex.Message}");
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
            // Desconecta o Arduino desta tela
            _arduino?.Disconnect();

            // 4. Reabre a Window1 passando o usuário logado
            Window1 dashboard = new Window1(_usuarioLogado);
            dashboard.WindowState = WindowState.Maximized;
            dashboard.Show();

            // Fecha a tela de cadastro
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _arduino?.Disconnect();
            base.OnClosed(e);
        }
    }
}