using Hangman.Helpers;
using Hangman.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace Hangman.ViewModels
{
    public class OpenGameViewModel : ObservableObject
    {
        public ObservableCollection<SavedGame> UserSaves { get; set; }
        public SavedGame SelectedSave { get; set; }
        public ICommand SelectCommand { get; }

        public OpenGameViewModel(string userName)
        {
            var allSaves = GameStorage.Deserialize();
            var filtered = allSaves.Where(s => s.OwnerName == userName).ToList();
            UserSaves = new ObservableCollection<SavedGame>(filtered);

            SelectCommand = new RelayCommand(execute => {
                if (execute is Window win) win.DialogResult = true;
            }, canExecute => SelectedSave != null);
        }
    }
}
