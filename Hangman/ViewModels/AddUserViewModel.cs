using System.Windows;
using System.Windows.Input;
using Hangman.Helpers;

namespace Hangman.ViewModels
{
    public class AddUserViewModel : ObservableObject
    {
        private string _userName;
        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        public ICommand EnterCommand { get; }
        public ICommand CancelCommand { get; }

        public AddUserViewModel()
        {
            EnterCommand = new RelayCommand(execute =>
            {
                Enter();
                if (execute is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }, canExecute => CanEnter());

            CancelCommand = new RelayCommand(execute =>
            {
                Cancel();
                if (execute is Window window)
                {
                    window.DialogResult = false;
                    window.Close();
                }
            });
        }

        private void Enter()
        {
            if (UserName != null)
            {
                UserName = UserName.Trim();
            }
        }

        private bool CanEnter()
        {
            if (string.IsNullOrWhiteSpace(UserName))
                return false;

            return !UserName.Trim().Contains(" ");
        }

        private void Cancel() { }

    }
}