using System.Configuration;
using System.Data;
using System.Windows;
using QuestPDF.Infrastructure;

namespace wpf_exemplo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            QuestPDF.Settings.License = LicenseType.Community;
        }
    }
}