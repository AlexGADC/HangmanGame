namespace Hangman.Models
{
    public class SavedGame
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public string OwnerName { get; set; } 
        public DateTime SaveDate { get; set; } 

        public int Level { get; set; }
        public int Mistakes { get; set; }
        public int RemainingTime { get; set; }
        public string Category { get; set; }
        public string TargetWord { get; set; }
        public string WordState { get; set; } 

        public List<char> UsedLetters { get; set; }
    }
}
