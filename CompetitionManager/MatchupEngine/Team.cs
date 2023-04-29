namespace CompetitionManager.MatchupEngine
{
    internal sealed class Team
    {
        public int Rating { get; set; }
        public string Name { get; set; }
        public int CostsMatrixIndex { get; set; } = 0;

        public Team(string name, int rating)
        {
            Rating = rating;
            Name = name;
        }
    }
}
