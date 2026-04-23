using Hangman.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

public static class UserStorage 
{
    private static string _directoryPath = "Profiles";
    private static string _filePath = Path.Combine(_directoryPath, "users.json");

    public static void Serialize(ObservableCollection<User> users)
    {
        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }

        var options = new JsonSerializerOptions { WriteIndented = true };

        string jsonString = JsonSerializer.Serialize(users, options);

        File.WriteAllText(_filePath, jsonString);
    }
    public static ObservableCollection<User> Deserialize()
    {
        if (!File.Exists(_filePath))
        {
            return new ObservableCollection<User>();
        }

        try
        {
            string jsonString = File.ReadAllText(_filePath);

            var users = JsonSerializer.Deserialize<ObservableCollection<User>>(jsonString);

            return users ?? new ObservableCollection<User>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Reading fail: {ex.Message}");
            return new ObservableCollection<User>();
        }
    }
}