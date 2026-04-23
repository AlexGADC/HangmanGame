using Hangman.Helpers;
using Hangman.Models;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System;
using System.Linq;

namespace Hangman.ViewModels
{
    public class PlayViewModel : ObservableObject
    {
        private ObservableCollection<User> _allUsers;

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(); 
            }
        }

        private bool _isGameStarted = false;
        public bool IsGameStarted
        {
            get => _isGameStarted;
            set
            {
                _isGameStarted = value;
                OnPropertyChanged();
            }
        }

        private string _currentCategory;
        public string CurrentCategory
        {
            get => _currentCategory;
            set
            {
                _currentCategory = value;
                OnPropertyChanged();
                (NewGameCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<char> _hiddenWordLetters;
        public ObservableCollection<char> HiddenWordLetters
        {
            get => _hiddenWordLetters;
            set 
            {  
                _hiddenWordLetters = value; 
                OnPropertyChanged(); 
            }
        }

        private int _currentLevel;
        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> _livesProgress;
        public ObservableCollection<string> LivesProgress
        {
            get => _livesProgress;
            set 
            { 
                _livesProgress = value; 
                OnPropertyChanged(); 
            }
        }

        private int _wrongGuesses;
        public int WrongGuesses
        {
            get => _wrongGuesses;
            set
            {
                _wrongGuesses = value;
                OnPropertyChanged();
                UpdateHangmanImage();
            }
        }

        private string _currentImage;
        public string CurrentImage
        {
            get => _currentImage;
            set
            {
                _currentImage = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<AlphabetLetter> Alphabet { get; set; }

        private int _remainingTime;
        public int RemainingTime
        {
            get => _remainingTime;
            set 
            { 
                _remainingTime = value; 
                OnPropertyChanged(); 
            }
        }

        private DispatcherTimer _gameTimer;

        private SavedGame? _openedSave;

        private string _targetWord;
        public ICommand NewGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand StatisticsCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand GuessLetterCommand { get; }
        public ICommand HandleKeyDownCommand { get; }
        public ICommand CleanupCommand { get; }
        public ICommand AboutCommand { get; }

        public PlayViewModel(User selectedUser, ObservableCollection<User> allUsers, SavedGame loadedGame = null)
        {
            CurrentUser = selectedUser;
            _allUsers = allUsers;

            if (loadedGame != null)
            {
                _openedSave = loadedGame;

                CurrentCategory = loadedGame.Category;
                CurrentLevel = loadedGame.Level;
                WrongGuesses = loadedGame.Mistakes;
                RemainingTime = loadedGame.RemainingTime;
                _targetWord = loadedGame.TargetWord;

                HiddenWordLetters = new ObservableCollection<char>(loadedGame.WordState.ToCharArray());

                InitializeLives();
                for (int i =0; i < WrongGuesses; i++)
                {
                    if (i < LivesProgress.Count) LivesProgress[i] = "X";
                }

                IsGameStarted = true;
                _gameTimer.Start();
            }

            NewGameCommand = new RelayCommand(execute => NewGame(), canExecute => { return CurrentCategory != null; });
            OpenGameCommand = new RelayCommand(execute => OpenGame());
            SaveGameCommand = new RelayCommand(execute => SaveGame());
            StatisticsCommand = new RelayCommand(execute => Statistics());
            CancelCommand = new RelayCommand(execute => {
                if (execute is Window window)
                {
                    window.Close(); 
                }
            });

            SelectCategoryCommand = new RelayCommand(execute =>
            {
                CurrentCategory = execute.ToString();
            });

            LivesProgress = new ObservableCollection<string> { " ", " ", " ", " ", " ", " ", " " };

            Alphabet = new ObservableCollection<AlphabetLetter>();
            foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            {
                Alphabet.Add(new AlphabetLetter { Letter = c });
            }

            GuessLetterCommand = new RelayCommand(execute =>
            {
                if (execute is AlphabetLetter letterItem)
                {
                    ExecuteGuess(letterItem);
                }
            });

            HandleKeyDownCommand = new RelayCommand(execute =>
            {
                if (execute is KeyEventArgs e)
                {
                    if (e.Key >= Key.A && e.Key <= Key.Z)
                    {
                        char pressedChar = (char)('A' + (e.Key - Key.A));
                        HandleKeyPress(pressedChar);
                    }
                }
            });

            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1); 
            _gameTimer.Tick += GameTimer_Tick;

            CleanupCommand = new RelayCommand(execute => {
                _gameTimer?.Stop();
            });

            AboutCommand = new RelayCommand(execute => OpenAbout());
        }
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (RemainingTime > 0)
            {
                RemainingTime--;
            }
            else
            {
                _gameTimer.Stop();
                EndGame(false); 
            }
        }
        private void NewGame()
        {
            CurrentLevel = 1;

            IsGameStarted = true;

            StartLevel();
        }
        private void StartLevel()
        {
            _targetWord = WordProvider.GetRandomWord(CurrentCategory).ToUpper();

            WrongGuesses = 0;

            InitializeWord(_targetWord);

            InitializeLives();

            ResetKeyboard();

            RemainingTime = 30;

            _gameTimer.Start();
        }
        private void InitializeWord(string word)
        {
            HiddenWordLetters = new ObservableCollection<char>();
            foreach (var c in word)
            {
                HiddenWordLetters.Add(char.IsLetter(c) ? '_' : c);
            }
        }
        private void OnWordGuessed()
        {
            if (CurrentLevel < 3)
            {
                CurrentLevel++; 
                StartLevel();   
            }
            else
            {
                EndGame(true); 
            }
        }
        private void UpdatePersistentStatistics(bool won)
        {
            var allStats = StatisticsStorage.LoadAllStats();
            var userStat = allStats.FirstOrDefault(s => s.UserName == CurrentUser.UserName);

            if (userStat == null)
            {
                userStat = new UserStatistics { UserName = CurrentUser.UserName };
                allStats.Add(userStat);
            }

            userStat.TotalGamesPlayed++; 

            if (won)
            {
                if (!userStat.CategoryWins.ContainsKey(CurrentCategory))
                {
                    userStat.CategoryWins[CurrentCategory] = 0;
                }
                userStat.CategoryWins[CurrentCategory]++;
            }

            StatisticsStorage.SaveStats(allStats);
        }

        private void EndGame(bool won)
        {
            _gameTimer.Stop();

            if (CurrentUser != null)
            {
                if (won)
                {
                    MessageBox.Show("Nice work! You won!");
                }
                else
                {
                    MessageBox.Show("Try again because you lost...");
                }

                UpdatePersistentStatistics(won);
            }

            if (_openedSave != null)
            {
                DeleteSave(_openedSave);
                _openedSave = null;
            }

            IsGameStarted = false;
        }

        private void OpenGame()
        {
            var vm = new OpenGameViewModel(CurrentUser.UserName);

            var dialog = new Views.OpenGameWindow
            {
                DataContext = vm
            };

            if (dialog.ShowDialog() == true && vm.SelectedSave != null)
            {
                _openedSave = vm.SelectedSave;
                ApplySave(vm.SelectedSave);
            }
        }

        private static void DeleteSave(SavedGame save)
        {
            var allSaves = GameStorage.Deserialize();

            var match = allSaves.FirstOrDefault(s =>
                s.OwnerName == save.OwnerName &&
                s.SaveDate == save.SaveDate);

            if (match != null)
            {
                allSaves.Remove(match);
                GameStorage.Serialize(allSaves);
            }
        }

        private void ApplySave(SavedGame save)
        {
            _gameTimer.Stop(); 

            CurrentLevel = save.Level;
            WrongGuesses = save.Mistakes;
            RemainingTime = save.RemainingTime;
            _targetWord = save.TargetWord;
            CurrentCategory = save.Category;

            HiddenWordLetters = new ObservableCollection<char>(save.WordState.ToCharArray());

            InitializeLives();
            for (int i = 0; i < WrongGuesses; i++)
            {
                if (i < LivesProgress.Count) LivesProgress[i] = "X";
            }

            ResetKeyboard();

            IsGameStarted = true;
            _gameTimer.Start(); 
        }
        private void SaveGame()
        {
            if (!IsGameStarted) return;

            var newSave = new SavedGame
            {
                OwnerName = CurrentUser.UserName,
                Level = CurrentLevel,
                Mistakes = WrongGuesses,
                RemainingTime = RemainingTime,
                Category = CurrentCategory,
                TargetWord = _targetWord,

                WordState = new string(HiddenWordLetters.ToArray()),
                SaveDate = DateTime.Now
            };

            var allSaves = GameStorage.Deserialize();
            allSaves.Add(newSave);
            GameStorage.Serialize(allSaves);

            MessageBox.Show("Game saved successfully!");
        }
        private void Statistics()
        {
            var allStats = StatisticsStorage.LoadAllStats();
            var myStats = allStats.FirstOrDefault(s => s.UserName == CurrentUser.UserName);

            if (myStats == null) myStats = new UserStatistics { UserName = CurrentUser.UserName };

            var vm = new StatisticsViewModel(myStats);

            var statsWin = new Views.StatisticsWindow
            {
                DataContext = vm,
                Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)
            };

            statsWin.ShowDialog();
        }
        private void OpenAbout()
        {
            var vm = new AboutViewModel();
            var aboutWin = new Views.AboutWindow
            {
                DataContext = vm,
                Owner = System.Windows.Application.Current.MainWindow
            };
            aboutWin.ShowDialog();
        }
        private void ExecuteGuess(AlphabetLetter letterItem)
        {
            letterItem.IsEnabled = false;
            char guessedChar = letterItem.Letter;

            bool found = false;

            for (int i =0; i < _targetWord.Length; i++)
            {
                if (_targetWord[i] == guessedChar)
                {
                    HiddenWordLetters[i] = guessedChar;
                    found = true;
                }
            }

            if (found)
            {
                if (!HiddenWordLetters.Contains('_'))
                {
                    _gameTimer.Stop();
                    OnWordGuessed();
                }
            }
            else
            {
                if (WrongGuesses < LivesProgress.Count)
                {
                    LivesProgress[WrongGuesses] = "X";
                }

                WrongGuesses++;

                if (WrongGuesses >=7)
                {
                    EndGame(false);
                }
            }
        }

        public void HandleKeyPress(char pressedChar)
        {
            char upperChar = char.ToUpper(pressedChar);

            var letterItem = Alphabet.FirstOrDefault(l => l.Letter == upperChar);

            if (letterItem != null && letterItem.IsEnabled)
            {
                ExecuteGuess(letterItem);
            }
        }

        private void InitializeLives()
        {
            LivesProgress = new ObservableCollection<string> { "", "", "", "", "", "", "" };
        }

        private void ResetKeyboard()
        {
            if (Alphabet == null) return;

            foreach (var letter in Alphabet)
            {
                letter.IsEnabled = true;
            }
        }

        private void UpdateHangmanImage()
        {
            CurrentImage = $"/Assets/HangmanPoses/HangmanMistakes{WrongGuesses}.jpg";
        }
    }
}