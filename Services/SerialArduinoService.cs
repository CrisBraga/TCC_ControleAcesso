using System;
using System.IO.Ports;
using System.Threading;

namespace wpf_exemplo.Services
{
    public class SerialArduinoService
    {
        private SerialPort _serial;

        public event Action OnArduinoReady;
        public event Action<int> OnFingerprintDetected;
        public event Action OnNoMatchDetected;


        public bool IsConnected => _serial != null && _serial.IsOpen;

        // ========================
        // CONECTAR
        // ========================
        public void Connect(string portName)
        {
            if (IsConnected)
                return;

            _serial = new SerialPort(portName, 9600);
            _serial.NewLine = "\n";
            _serial.DataReceived += Serial_DataReceived;
            _serial.Open();
        }

        // ========================
        // DESCONECTAR
        // ========================
        public void Disconnect()
        {
            if (_serial != null)
            {
                _serial.DataReceived -= Serial_DataReceived;

                if (_serial.IsOpen)
                    _serial.Close();

                _serial.Dispose();
                _serial = null;
            }
        }

        // ========================
        // ENVIAR COMANDOS AO ARDUINO
        // ========================
        public void SendCommand(string command)
        {
            if (!IsConnected)
                return;

            _serial.WriteLine(command);
        }

        // ========================
        // RECEBER DADOS DO ARDUINO
        // ========================
        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string line = _serial.ReadLine().Trim();

                if (line == "READY")
                {
                    OnArduinoReady?.Invoke();
                }
                else if (line.StartsWith("MATCH:"))
                {
                    int id = int.Parse(line.Substring(6));
                    OnFingerprintDetected?.Invoke(id);
                }
                else if (line == "NO_MATCH")
                {
                    OnNoMatchDetected?.Invoke();
                }

            }
            catch
            {
                // ignora erros de leitura
            }
        }
    }
}
