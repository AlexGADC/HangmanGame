using System.Diagnostics;
using System.IO;
using System.Text.Json;

public static class WordProvider
{
    private static Random _random = new Random();

    private static Dictionary<string, List<string>> _allWords;
    private static List<string> _categories;

    private static Dictionary<string, List<string>> _availableWords = new Dictionary<string, List<string>>();

    private static string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Words", "words.json");

    static WordProvider()
    {
        if (!File.Exists(_filePath))
        {
            Debug.WriteLine($"Error: File not found at path: {Path.GetFullPath(_filePath)}");
            _allWords = new Dictionary<string, List<string>>();
            _categories = new List<string>();
            return;
        }

        try
        {
            string jsonString = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                Debug.WriteLine("Error: JSON empty!");
                _allWords = new Dictionary<string, List<string>>();
                _categories = new List<string>();
                return;
            }

            _allWords = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonString) ?? new Dictionary<string, List<string>>();

            if (_allWords != null)
            {
                _categories = _allWords.Keys.ToList();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error at JSON processing: {ex.Message}");
            _allWords = new Dictionary<string, List<string>>();
            _categories = new List<string>();
        }
    }
    public static string GetRandomWord(string category)
    {
        if (_allWords == null || _allWords.Count == 0)
        {
            throw new InvalidOperationException("No words are loaded. Check that words.json exists and contains categories.");
        }

        string targetCategory = category;

        if (string.Equals(category, "All Categories", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(category))
        {
            var keys = _allWords.Keys.ToList();
            if (keys.Count ==0)
            {
                throw new InvalidOperationException("No categories available in words.json.");
            }
            targetCategory = keys[_random.Next(keys.Count)];
        }

        if (!_allWords.ContainsKey(targetCategory) || _allWords[targetCategory] == null || _allWords[targetCategory].Count ==0)
        {
            throw new ArgumentException($"Category '{targetCategory}' does not exist or has no words.");
        }

        if (!_availableWords.ContainsKey(targetCategory) || _availableWords[targetCategory] == null || _availableWords[targetCategory].Count ==0)
        {
            _availableWords[targetCategory] = _allWords[targetCategory];
        }

        var wordList = _availableWords[targetCategory];
        if (wordList.Count ==0)
        {
            throw new InvalidOperationException($"No available words left for category '{targetCategory}'.");
        }

        int index = _random.Next(wordList.Count);
        string pickedWord = wordList[index];

        wordList.RemoveAt(index);

        return pickedWord.ToUpperInvariant();
    }
}