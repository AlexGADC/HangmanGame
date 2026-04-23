using Hangman.Helpers;
using Hangman.Models;
using System.Collections.ObjectModel;

namespace Hangman.ViewModels
{
    public class CategoryStat
    {
        public string CategoryName { get; set; }
        public int Wins { get; set; }
    }

    public class StatisticsViewModel : ObservableObject
    {
        public ObservableCollection<CategoryStat> Stats { get; set; }
        public int TotalGames { get; }
        public StatisticsViewModel(UserStatistics stats)
        {
            TotalGames = stats.TotalGamesPlayed;
            Stats = new ObservableCollection<CategoryStat>();

            foreach (var entry in stats.CategoryWins)
            {
                Stats.Add(new CategoryStat { CategoryName = entry.Key, Wins = entry.Value });
            }
        }
    }
}
