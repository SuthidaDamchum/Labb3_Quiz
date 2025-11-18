using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Labb3_Quiz.Models;
using Labb3_Quiz.ViewModels;

namespace Labb3_Quiz.Views
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        private MainWindowViewModel mainViewModel;
        public MenuView()
        {
            InitializeComponent();
            mainViewModel = ((MainWindowViewModel)DataContext);
        }

        private bool _isFullscreen = false;
        private WindowState _previousWindowState;
        private WindowStyle _previousWindowStyle;

        public WindowState WindowState { get; private set; }
        public WindowStyle WindowStyle { get; private set; }

        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                ToggleFullscreen();
                e.Handled = true;
            }
        }

        private void ToggleFullscreen()
        {
            if (!_isFullscreen)
            {
                _previousWindowState = this.WindowState;
                _previousWindowStyle = this.WindowStyle;

                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Normal;
                _isFullscreen = true;
            }
            else
            {
                WindowStyle = _previousWindowStyle;
                WindowState = _previousWindowState;
                _isFullscreen = false;
            }
        }

        private void AddQuestionPack(object sender, RoutedEventArgs e)
        {
            var newQuestionPackViewModel = new QuestionPackViewModel(new QuestionPack("<PackName>"));
            var dialog = new AddNewQuestionDialog(newQuestionPackViewModel);

            if (dialog.ShowDialog() == true)
            {
                mainViewModel.ActivePack = newQuestionPackViewModel;
            }
        }
    }
}