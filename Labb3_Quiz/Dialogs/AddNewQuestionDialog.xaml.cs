using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Labb3_Quiz.ViewModels;

namespace Labb3_Quiz.Views
{
    /// <summary>
    /// Interaction logic for AddNewQuestionDialog.xaml
    /// </summary>
    public partial class AddNewQuestionDialog : Window
    {
        public AddNewQuestionDialog(QuestionPackViewModel questionPackViewModel)
        {
            DataContext = questionPackViewModel;
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
