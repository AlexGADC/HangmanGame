namespace Hangman.Models
{
    public class UserStatistics
    {
        public string UserName { get; set; }
        public int TotalGamesPlayed { get; set; }
        public Dictionary<string, int> CategoryWins { get; set; } = new Dictionary<string, int>();
    }
}
