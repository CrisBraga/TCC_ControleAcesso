using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wpf_exemplo.Helpers
{
    internal class navegarParaJanela
    {
        private void NavegarParaJanela(Window novaJanela)
        {
            // Sempre desconecta o Arduino ao sair dessa tela para liberar a porta COM
            try { _arduino?.Disconnect(); } catch { }

            novaJanela.WindowState = WindowState.Maximized;
            novaJanela.Show();
            this.Close();
        }
    }
}
