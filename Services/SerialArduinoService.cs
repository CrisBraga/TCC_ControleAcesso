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
        public event Action<int> OnEnrollSuccess;
        public event Action<string> OnEnrollStatus;


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
                else if (line.StartsWith("ENROLL_OK:"))
                {
                    int id = int.Parse(line.Substring(10));
                    OnEnrollSuccess?.Invoke(id);
                }
                else if (line == "ENROLL_FAIL")
                {
                    OnEnrollStatus?.Invoke("Falha ao cadastrar digital");
                }
                else if (line == "PLACE_FINGER")
                {
                    OnEnrollStatus?.Invoke("Coloque o dedo no sensor");
                }
                else if (line == "REMOVE_FINGER")
                {
                    OnEnrollStatus?.Invoke("Remova o dedo");
                }
                else if (line == "PLACE_FINGER_AGAIN")
                {
                    OnEnrollStatus?.Invoke("Coloque o dedo novamente");
                }
            }
            catch
            {
                // ignora erros
            }
        }
    }
}
 

