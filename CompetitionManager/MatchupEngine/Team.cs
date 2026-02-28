namespace CompetitionManager.MatchupEngine
{
    public sealed class Team(string name, int rating)
    {
        public string Name { get; set; } = name;
        public int Rating { get; set; } = rating;
        public bool IsBye { get; set; } = false;
        //Flag to prevent admin teams from getting the bye
        public bool PreventByes { get; set; } = false;
        public int MatchesPlayed { get; set; } = 0;
        public int MatrixIndex { get; set; } = 0;

        public static Team CreateBye()
        {
            var result = new Team(string.Empty, 0)
            {
                IsBye = true
            };
            return result;
        }
    }
}
