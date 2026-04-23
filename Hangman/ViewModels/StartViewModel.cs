using Hangman.Helpers;
using Hangman.Models;
using Hangman.Constants;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace Hangman.ViewModels
{
    public class StartViewModel : ObservableObject
    {
        public ObservableCollection<User> Users { get; set; }
        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                FilterSavesForCurrentUser();
            }
        }

        private SavedGame _selectedSavedGame;
        public SavedGame SelectedSavedGame
        {
            get => _selectedSavedGame;
            set { _selectedSavedGame = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SavedGame> _allLoadedGames;
        public ObservableCollection<SavedGame> SavedGames { get; set; }
        public ICommand NewUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand CancelCommand { get; }
        public StartViewModel()
        {
            Users = UserStorage.Deserialize();

            _allLoadedGames = GameStorage.Deserialize();

            SavedGames = new ObservableCollection<SavedGame>();

            NewUserCommand = new RelayCommand(execute => AddNewUser());
            DeleteUserCommand = new RelayCommand(execute => DeleteUser(), canExecute => IsUserSelected());
            PreviousImageCommand = new RelayCommand(execute => ChangeImage(-1), canExecute => IsUserSelected());
            NextImageCommand = new RelayCommand(execute => ChangeImage(1), canExecute => IsUserSelected());
            PlayCommand = new RelayCommand(execute => Play(), canExecute => IsUserSelected());
            CancelCommand = new RelayCommand(execute => Cancel());
        }
        private void FilterSavesForCurrentUser()
        {
            if (SelectedUser == null) return;

            var filtered = _allLoadedGames.Where(g => g.OwnerName == SelectedUser.UserName).ToList();

            SavedGames.Clear();
            foreach (var game in filtered)
            {
                SavedGames.Add(game);
            }
        }
        private bool IsUserSelected() => SelectedUser != null;
        private void AddNewUser()
        {
            var vm = new AddUserViewModel();

            var dialog = new Views.AddUserWindow
            {
                DataContext = vm
            };

            if (dialog.ShowDialog() == true)
            {
                string inputName = vm.UserName;

                if (!string.IsNullOrWhiteSpace(inputName))
                {
                    User newUser = new User
                    {
                        UserName = inputName.Trim(),
                        ImagePath = "/Assets/Avatars/ArcherAvatar.jpg"
                    };

                    Users.Add(newUser);

                    UserStorage.Serialize(Users);

                    SelectedUser = newUser;
                }
            }
        }
        private void DeleteUser()
        {
            if (IsUserSelected())
            {
                string userNameToDelete = SelectedUser.UserName;

                Users.Remove(SelectedUser);
                UserStorage.Serialize(Users);

                var allStats = StatisticsStorage.LoadAllStats();
                var userStat = allStats.FirstOrDefault(s => s.UserName == userNameToDelete);
                if (userStat != null)
                {
                    allStats.Remove(userStat);
                    StatisticsStorage.SaveStats(allStats);
                }

                var allGames = GameStorage.Deserialize();
                var userGames = allGames.Where(g => g.OwnerName == userNameToDelete).ToList();

                if (userGames.Any())
                {
                    foreach (var game in userGames)
                    {
                        allGames.Remove(game);
                    }
                    GameStorage.Serialize(allGames);
                }

                SavedGames.Clear();
            }
        }
        private void ChangeImage(int direction)
        {
            if (!IsUserSelected()) return;

            var avatars = ImageProvider.Avatars;
            string initialImage = SelectedUser.ImagePath;

            int currentIndex = avatars.IndexOf(initialImage);

            int newIndex = (currentIndex + direction + avatars.Count) % avatars.Count;

            SelectedUser.ImagePath = avatars[newIndex];

            if (SelectedUser.ImagePath != initialImage)
            {
                UserStorage.Serialize(Users);
            }
        }
        private void Play()
        {
            var vm = new PlayViewModel(SelectedUser, Users, SelectedSavedGame);

            var dialog = new Views.PlayWindow
            {
                DataContext = vm
            };

            Application.Current.MainWindow.Hide();

            dialog.ShowDialog();

            Application.Current.MainWindow.Show();
        }
        private void Cancel()
        {
            Application.Current.Shutdown();
        }
    }
}
