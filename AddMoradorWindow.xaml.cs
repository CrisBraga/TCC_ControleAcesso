using MySql.Data.MySqlClient;
using System;
using System.Windows;
using wpf_exemplo.Services;

namespace wpf_exemplo
{
    public partial class AddMoradorWindow : Window
    {
        private SerialArduinoService _arduino;

        // controla qual dedo está sendo cadastrado
        private int _fingerIdEmCadastro = 0;
        private int _fingerSlotAtual = 1; // 1 ou 2

        public AddMoradorWindow()
        {
            InitializeComponent();

            _arduino = new SerialArduinoService();
            _arduino.OnEnrollSuccess += OnEnrollSuccess;

            Loaded += (s, e) =>
            {
                try
                {
                    _arduino.Connect("COM3"); // ajuste se necessário
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

                MessageBox.Show(
                    "Digital cadastrada com sucesso!",
                    "Sucesso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            });
        }

        // =========================
        // SALVAR MORADOR
        // =========================
        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text))
            {
                MessageBox.Show("Informe o nome do morador.");
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
                MessageBox.Show("Morador cadastrado com sucesso!");
                Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao cadastrar morador:\n" + ex.Message);
            }
        }

        // =========================
        // VOLTAR / FECHAR
        // =========================
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            _arduino?.Disconnect();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _arduino?.Disconnect();
            base.OnClosed(e);
        }
    }
}
