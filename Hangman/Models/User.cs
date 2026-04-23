using Hangman.Helpers;

namespace Hangman.Models
{
    public class User : ObservableObject
    {
        private string _userName;
        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }
    }
}

