using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using Labb3_Quiz.Commend;
using Labb3_Quiz.Dialogs;
using Labb3_Quiz.Helpers;
using Labb3_Quiz.Models;
using Labb3_Quiz.Views;

namespace Labb3_Quiz.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public PlayerViewModel Player { get; }
        private JsonService _jsonService = new JsonService();

        public ICommand ToggleFullscreenCommand { get; }
        public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();

        public bool HasMoreThanOneItem => Packs?.Count > 1;

        public bool CanPlay
            => ActivePack != null && ActivePack.Questions != null && ActivePack.Questions.Count > 0 && CurrentView != PlayerViewModel;

        private QuestionPackViewModel _selectedPack;
        public QuestionPackViewModel selectPack
        {
            get => _selectedPack;
            set
            {
                _selectedPack = value;
                ActivePack = value; 
                RaisePropertyChanged();
            }
        }
        public DelegateCommand SelectPackCommand => new DelegateCommand(param =>
        {
            CurrentView = ConfigurationViewModel;
            if (ConfigurationViewModel == null)
            {
                CurrentView = new ConfigurationViewModel(this);
            }
            if (param is QuestionPackViewModel pack)
                ActivePack = pack;
        });

        private QuestionPackViewModel _activePack;
        public QuestionPackViewModel ActivePack
        {
            get => _activePack;
            set
            {
                if (_activePack == value) return;

                if (_activePack?.Questions != null)
                    _activePack.Questions.CollectionChanged -= Questions_CollectionChanged;

                _activePack = value;
                if (_activePack?.Questions != null)
                    _activePack.Questions.CollectionChanged += Questions_CollectionChanged;

                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanPlay));
                PlayerViewModel?.RaisePropertyChanged(nameof(ActivePack));
            }
        }

        private void Questions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(CanPlay));
        }

        public bool HasQuestions => ActivePack?.Questions.Count > 0;
        public bool? Play { get; set; }
        public PlayerViewModel? PlayerViewModel { get; set; }
        public ConfigurationViewModel? ConfigurationViewModel { get; set; }
        public DelegateCommand? NewQuestionCommand { get; }
        public DelegateCommand? RemoveQuestionCommand { get; }
        public DelegateCommand NewPackCommand { get; }
        public DelegateCommand SavePackCommand { get; }
        public DelegateCommand LoadPacksCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand PlayCommand { get; }
        public DelegateCommand EditCommand { get; }

        public DelegateCommand OpenPackOptionsCommand { get; }
        public MainWindowViewModel()
        {
            Player = new PlayerViewModel(this);
            _jsonService = new JsonService();
            ReloadPacks();

            ToggleFullscreenCommand = new DelegateCommand(_ => ToggleFullscreen());

            NewPackCommand = new DelegateCommand(_ => CreateNewPack());

            SavePackCommand = new DelegateCommand(_ => SaveAllPacks());

            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this);

            PlayCommand = new DelegateCommand(PlayView);

            EditCommand = new DelegateCommand(_ => EditPack());

            OpenPackOptionsCommand = new DelegateCommand(_ => OpenPackOptions());

            CurrentView = ConfigurationViewModel;
            NewQuestionCommand = new DelegateCommand(_ =>
            {
                var question = new Question("New Question", "", new[] { "", "", "" });
                ActivePack.Questions.Add(question);
                ConfigurationViewModel.ActiveQuestion = question;
            }
            );

            RemoveQuestionCommand = new DelegateCommand(question =>
            {
                if (question != null)
                    ActivePack.Questions.Remove((Question)question);
                ConfigurationViewModel.ActiveQuestion = null;
            });


            ExitCommand = new DelegateCommand(_ =>
            {
                _jsonService.AddToFile(Packs.Select(x => x.Model).ToArray());
                Application.Current.Shutdown();
            });
        }
        private void OpenPackOptions()
        {
            if (ActivePack == null) return;

            var dialog = new PackOptionDialog
            {
                DataContext = ActivePack
            };

            dialog.ShowDialog();

            RaisePropertyChanged(nameof(ActivePack));
        }

        private bool _hasStartedPlay;
        public bool HasStartedPlay
        {
            get => _hasStartedPlay;
            set
            {
                _hasStartedPlay = value;
                RaisePropertyChanged();
            }
        }
        private void EditPack()
        {
            CurrentView = ConfigurationViewModel;
            HasStartedPlay = false;
            RaisePropertyChanged(nameof(CanPlay));
        }

        private void PlayView(object args)
        {
            PlayerViewModel.StartQuiz();
            CurrentView = PlayerViewModel;
            HasStartedPlay = true;
            RaisePropertyChanged(nameof(CanPlay));
        }

        public ICommand RemovePackCommand => new DelegateCommand(param =>
        {
            MessageBoxResult result = MessageBox.Show($"Are you sure want to delete {ActivePack.Name}?", 
            "Delete question pack?", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                var packToRemove = param as QuestionPackViewModel;

                if (packToRemove == null) return;

                if (Packs.Count == 1)
                {
                    MessageBox.Show("You can not delete the last pack!");
                    return;
                }
                int index = Packs.IndexOf(packToRemove);
                Packs.Remove(packToRemove);

                if (Packs.Count > 0)
                {
                    int newIndex = Math.Min(index, Packs.Count - 1);
                    ActivePack = Packs[newIndex];
                }
                if (ConfigurationViewModel == null)
                {
                    CurrentView = new ConfigurationViewModel(this);
                }
                else
                {
                    CurrentView = ConfigurationViewModel;
                }
            }
        });

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set
            {
                if (value == ConfigurationViewModel && _currentView == PlayerViewModel)
                {
                    PlayerViewModel?.StopTimer();
                }
                _currentView = value;
                RaisePropertyChanged(nameof(CurrentView));
            }
        }

        private void CreateNewPack()
        {
            if (ConfigurationViewModel == null)
            {
                CurrentView = new ConfigurationViewModel(this);
            }
            var newQuestionPackViewModel = new QuestionPackViewModel(new QuestionPack("<PackName>"));
            var dialog = new AddNewQuestionDialog(newQuestionPackViewModel);

            if (dialog.ShowDialog() == true)
            {
                ActivePack = newQuestionPackViewModel;
                Packs.Add(newQuestionPackViewModel);
                CurrentView = ConfigurationViewModel;
            }
        }

        private void SaveAllPacks()
        {
            var allModels = Packs.Select(p => p.Model).ToArray();
            _jsonService.AddToFile(allModels);
        }

        private void ReloadPacks()
        {
            Packs.Clear();

            var loadedPacks = _jsonService.ReadFile();

            if (loadedPacks.Length == 0)
            {
                var defaultPack = new QuestionPack("Default Pack");
                var defaultPackVM = new QuestionPackViewModel(defaultPack);

                Packs.Add(defaultPackVM);

                MessageBox.Show("No packs found. Default pack created.");
            }

            foreach (var pack in loadedPacks)
            {
                Packs.Add(new QuestionPackViewModel(pack));
            }

            ActivePack = Packs[Packs.Count - 1];
        }

        private void ToggleFullscreen()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null) return;

            if (mainWindow.WindowStyle == WindowStyle.None)
            {
                mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                mainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.WindowState = WindowState.Maximized;
            }
        }
    }
}









