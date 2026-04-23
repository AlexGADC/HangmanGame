using Hangman.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Hangman.Helpers
{
    public static class GameStorage
    {
        private static string _directoryPath = "Profiles";
        private static string _filePath = Path.Combine(_directoryPath, "games.json");

        public static void Serialize(ObservableCollection<SavedGame> games)
        {
            if (!Directory.Exists(_directoryPath)) Directory.CreateDirectory(_directoryPath);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(games, options);
            File.WriteAllText(_filePath, jsonString);
        }

        public static ObservableCollection<SavedGame> Deserialize()
        {
            if (!File.Exists(_filePath)) return new ObservableCollection<SavedGame>();
            try
            {
                string jsonString = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<ObservableCollection<SavedGame>>(jsonString) ?? new ObservableCollection<SavedGame>();
            }
            catch { return new ObservableCollection<SavedGame>(); }
        }
    }
}
