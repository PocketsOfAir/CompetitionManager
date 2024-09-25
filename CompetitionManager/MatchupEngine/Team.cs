namespace CompetitionManager.MatchupEngine
{
    internal sealed class Team(string name, int rating)
    {
        public string Name { get; set; } = name;
        public int Rating { get; set; } = rating;
        public bool IsBye { get; set; } = false;
        public int CostsMatrixIndex { get; set; } = 0;

        public static Team CreateBye()
        {
            var result = new Team("Bye", 0)
            {
                IsBye = true
            };
            return result;
        }
    }
}
