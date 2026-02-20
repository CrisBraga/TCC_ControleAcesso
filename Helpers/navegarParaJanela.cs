using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using wpf_exemplo.Services;
using wpf_exemplo.Models;
using MaterialDesignThemes.Wpf;
using wpf_exemplo.Helpers;

namespace wpf_exemplo.Helpers
{
    public static class navigationHelper
    {
        public static void NavegarParaJanela(Window janelaAtual,Window novaJanela)
        {
            novaJanela.WindowState = WindowState.Maximized;
            novaJanela.Show();
            janelaAtual.Close();
        }
    }
}
