using Hangman.Helpers; 

namespace Hangman.ViewModels
{
    public class AboutViewModel : ObservableObject
    {
        public string AuthorName { get; } = "Gindea Alexandru - Daniel";
        public string GroupName { get; } = "Grupa 10LF242"; 
        public string Specialization { get; } = "Informatica"; 
    }
}