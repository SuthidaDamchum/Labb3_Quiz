using System.Windows;
using Labb3_Quiz.Models;
using Labb3_Quiz.ViewModels;

namespace Labb3_Quiz;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            if (vm.ExitCommand.CanExecute(null))
                vm.ExitCommand.Execute(null);
        }
    }

    private async Task Window_Opened(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            await vm.ReloadPacks();
        }
    }
}