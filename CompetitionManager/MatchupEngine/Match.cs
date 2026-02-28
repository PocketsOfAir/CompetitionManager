namespace CompetitionManager.MatchupEngine
{
    public struct Match
    {
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public int Cost { get; set; }
        public bool IsBye { get; set; }
        public Match()
        {
        }
    }
}
