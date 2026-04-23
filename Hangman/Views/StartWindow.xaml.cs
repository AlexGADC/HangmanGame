using Hangman.ViewModels;
using System.Windows;


namespace Hangman.Views
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            DataContext = new StartViewModel();
        }
    }
}