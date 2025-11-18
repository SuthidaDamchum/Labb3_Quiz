using System.Collections.ObjectModel;
using System.Windows.Input;
using Labb3_Quiz.Commend;
using Labb3_Quiz.Models;

namespace Labb3_Quiz.ViewModels
{
    class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }


        private Question? _activeQuestion;
        public Question? ActiveQuestion

        {
            get => _activeQuestion;

            set
            {
                _activeQuestion = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Difficulty> DifficultyOptions { get; } =
            new ObservableCollection<Difficulty> { Difficulty.Easy, Difficulty.Medium, Difficulty.Hard };

        public ICommand SavePackCommand { get; }
        public ICommand CreateNewPackCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand? NewQuestionCommand { get => _mainWindowViewModel?.NewQuestionCommand; }
        public ICommand? RemoveQuestionCommand { get => _mainWindowViewModel?.RemoveQuestionCommand; }

        public ConfigurationViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel
                                   ?? throw new ArgumentNullException(nameof(mainWindowViewModel));

            _mainWindowViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainWindowViewModel.ActivePack))
                {
                    RaisePropertyChanged(nameof(ActivePack));
                }
            };
            SavePackCommand = new DelegateCommand(SavePack, CanSavePack);
        }

        private void SavePack(object? obj)
        {

            if (ActivePack != null)
            {
                System.Diagnostics.Debug.WriteLine($"Saved pack: {ActivePack.Name}");
            }
        }

        private bool CanSavePack(object? obj)
        {
            return ActivePack != null && !string.IsNullOrWhiteSpace(ActivePack.Name);
        }
    }
}