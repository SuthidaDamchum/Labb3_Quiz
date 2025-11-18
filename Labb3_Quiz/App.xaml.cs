using System.Configuration;
using System.Data;
using System.Windows;
using Labb3_Quiz.ViewModels;

namespace Labb3_Quiz
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainViewModel = new MainWindowViewModel();
            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
            await mainViewModel.ReloadPacks();

            mainWindow.Show();
        }
    }

}
