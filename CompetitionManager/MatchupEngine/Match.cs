namespace CompetitionManager.MatchupEngine
{
    internal struct Match
    {
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public int Cost { get; set; } = 0;
        public Match() { }
    }
}
