using System.CodeDom;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using Labb3_Quiz.Commend;
using Labb3_Quiz.Models;

namespace Labb3_Quiz.ViewModels
{
    public class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack _model;       
        public Array DifficultyOptions => Enum.GetValues(typeof(Difficulty));
        public ObservableCollection<Question> Questions { get; set; }
        public QuestionPackViewModel(QuestionPack model)
        {
            _model = model;
            Questions = new ObservableCollection<Question>(_model.Questions);
            Questions.CollectionChanged += Questions_CollectionChanged;
            Difficulty = Difficulty.Medium;    
        }

        public string Name
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                RaisePropertyChanged();
            }
        }

        public Difficulty Difficulty
        {
            get => Model.Difficulty;
            set
            {
                Model.Difficulty = value;
                RaisePropertyChanged();
            }
        }

        public int TimeLimitInSeconds
        {
            get => Model.TimeLimitInSeconds;
            set
            {
                Model.TimeLimitInSeconds = value;
                RaisePropertyChanged();
            }
        }

        public QuestionPack Model => _model;

        private void Questions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
                foreach (Question q in e.NewItems) _model.Questions.Add(q);

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
                foreach (Question q in e.OldItems) _model.Questions.Remove(q);

            if (e.Action == NotifyCollectionChangedAction.Replace && e.OldItems != null && e.NewItems != null)
                _model.Questions[e.OldStartingIndex] = (Question)e.NewItems[0]!;

            if (e.Action == NotifyCollectionChangedAction.Reset)
                _model.Questions.Clear();
        }
    }
}
