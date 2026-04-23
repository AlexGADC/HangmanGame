namespace Hangman.Constants
{
    public static class ImageProvider
    {
        private static readonly List<string> _avatars = new List<string>
    {
        "/Assets/Avatars/ArcherAvatar.jpg",
        "/Assets/Avatars/AssassinAvatar.jpg",
        "/Assets/Avatars/ChineseSwordsmanAvatar.jpg",
        "/Assets/Avatars/MaskedManAvatar.jpg",
        "/Assets/Avatars/SamuraiAvatar.jpg",
        "/Assets/Avatars/SamuraiWonderingAvatar.jpg",
        "/Assets/Avatars/StaffMasterAvatar.jpg",
        "/Assets/Avatars/SwordMaster2Avatar.jpg",
        "/Assets/Avatars/SwordMasterAvatar.jpg",
        "/Assets/Avatars/TaiChiMasterAvatar.jpg"
    };

        public static List<string> Avatars => _avatars;
    }
}
