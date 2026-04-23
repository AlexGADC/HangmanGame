using Hangman.Helpers;

namespace Hangman.Models
{
    public class AlphabetLetter : ObservableObject
    {
        public char Letter { get; set; }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); }
        }
    }
}
