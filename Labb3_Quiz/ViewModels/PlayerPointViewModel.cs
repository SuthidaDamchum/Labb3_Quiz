using Labb3_Quiz.Commend;

namespace Labb3_Quiz.ViewModels
{
    class PlayerPointViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public int Score { get; }
        public int TotalQuestions { get; }

        public string PointText => $"You got {Score} out of {TotalQuestions}!";

        public DelegateCommand RestartQuizCommand { get; }

        public PlayerPointViewModel(MainWindowViewModel mainWindowViewModel, int score, int totalQuestions)
        {
            _mainWindowViewModel = mainWindowViewModel;
            Score = score;
            TotalQuestions = totalQuestions;

            RestartQuizCommand = new DelegateCommand(RestartQuiz);

        }
        private void RestartQuiz(object? arg)
        {
            var playerVm = _mainWindowViewModel.PlayerViewModel;
            playerVm.StartQuiz();
            _mainWindowViewModel.CurrentView = playerVm;
        }
    }
}