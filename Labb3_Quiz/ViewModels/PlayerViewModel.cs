using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Labb3_Quiz.Commend;
using Labb3_Quiz.Models;

namespace Labb3_Quiz.ViewModels
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private DispatcherTimer _timer;
        private bool _answeredCurrentQuestion = false;
        public QuestionPackViewModel? ActivePack => _mainWindowViewModel?.ActivePack;

        private int _currentIndex;
        private int _score;

        public int Score
        {
            get => _score;
            set { _score = value; RaisePropertyChanged(); }
        }

        private int _secondsLeft;
        public int SecondsLeft
        {
            get => _secondsLeft;
            set { _secondsLeft = value; RaisePropertyChanged(); }
        }

        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set { _currentQuestion = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<Question> PlayQuestions { get; private set; }

        private ObservableCollection<Alternative> _alternatives = new ObservableCollection<Alternative>();
        public ObservableCollection<Alternative> Alternatives
        {
            get => _alternatives;
            set { _alternatives = value; RaisePropertyChanged(); }
        }

        public DelegateCommand CheckAnswerCommand { get; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            CheckAnswerCommand = new DelegateCommand(param => CheckAnswer((Alternative?)param));

            SetupTimer();
        }
        public void StartQuiz()
        {
            if (ActivePack == null)
                return;

            Score = 0;

            PlayQuestions = new ObservableCollection<Question>(
                ActivePack.Questions.OrderBy(q => Guid.NewGuid())
            );

            _currentIndex = 0;
            ShowNextQuestion();
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (SecondsLeft > 0)
                SecondsLeft--;
            else
            {
                _timer.Stop();
                ShowCorrectAnswer();
                ContinueAfterDelay();
            }
        }
        public void StopTimer()
        {
            _timer.Stop();
        }

        private void ShowNextQuestion()
        {
            if (_currentIndex >= PlayQuestions.Count)
            {
                EndQuiz();
                return;
            }

            CurrentQuestion = PlayQuestions[_currentIndex];
            BuildAlternatives(CurrentQuestion);
            RaisePropertyChanged(nameof(CurrentQuestionText));
            SecondsLeft = ActivePack.TimeLimitInSeconds;
            _timer.Start();
        }
        private void BuildAlternatives(Question q)
        {
            var alts = new[]
            {
                new Alternative(){ Answer = q.CorrectAnswer, IsCorrect = true, Asset="pack://application:,,,/Assets/correct.png"},
                new Alternative(){ Answer = q.IncorrectAnswers[0], IsCorrect = false, Asset="pack://application:,,,/Assets/incorrect.png"},
                new Alternative(){ Answer = q.IncorrectAnswers[1], IsCorrect = false, Asset="pack://application:,,,/Assets/incorrect.png"},
                new Alternative(){ Answer = q.IncorrectAnswers[2], IsCorrect = false, Asset="pack://application:,,,/Assets/incorrect.png"},
            };

            Alternatives = new ObservableCollection<Alternative>(
                alts.OrderBy(x => Guid.NewGuid())
            );
        }
        public string CurrentQuestionText
        {
            get

            {
                if (PlayQuestions == null || CurrentQuestion == null)
                    return "Question 0  of 0";
                int CurrentNumber = _currentIndex + 1;
                int total = PlayQuestions.Count;
                return $"Question {CurrentNumber} of {total}";
            }
        }
        private async void CheckAnswer(Alternative? alt)
        {
            if (alt == null) return;

            if (_answeredCurrentQuestion) return;

            _answeredCurrentQuestion = true;

            _timer.Stop();

            foreach (var a in Alternatives)
            {
                a.ShowCorrectIndicator = false;
                a.ShowIncorrectIndicator = false;
                a.PickedAlternative = false;
            }

            alt.PickedAlternative = true;

            if (alt.IsCorrect)
            {
                Score++;
                alt.ShowCorrectIndicator = true;
            }
            else
            {
                alt.ShowIncorrectIndicator = true;
                Alternatives.First(a => a.IsCorrect).ShowCorrectIndicator = true;
            }

            RaisePropertyChanged(nameof(Alternatives));

            await Task.Delay(2000);
            _currentIndex++;
            _answeredCurrentQuestion = false; // reset for next question
            ShowNextQuestion();
        }

        private void ShowCorrectAnswer()
        {
            foreach (var a in Alternatives)
            {
                if (a.IsCorrect)
                    a.ShowCorrectIndicator = true;
            }
        }
        private async void ContinueAfterDelay()
        {
            await Task.Delay(2000);
            _currentIndex++;
            ShowNextQuestion();
        }
        private void EndQuiz()
        {
            _timer.Stop();

            var vm = new PlayerPointViewModel(_mainWindowViewModel!, Score, PlayQuestions.Count);
            _mainWindowViewModel!.CurrentView = vm;
        }

        public class Alternative : INotifyPropertyChanged
        {
            public string Answer { get; set; }
            public bool IsCorrect { get; set; }
            public string Asset { get; set; }

            private bool picked;
            public bool PickedAlternative
            {
                get => picked;
                set { picked = value; OnPropertyChanged(); }
            }

            private bool correct;
            public bool ShowCorrectIndicator
            {
                get => correct;
                set { correct = value; OnPropertyChanged(); }
            }

            private bool incorrect;
            public bool ShowIncorrectIndicator
            {
                get => incorrect;
                set { incorrect = value; OnPropertyChanged(); }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string? p = null)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}









