namespace CompetitionManager.MatchupEngine
{
    public sealed class StaticMatch(Team homeTeam, Team awayTeam, FieldDetails location)
    {
        public Team HomeTeam { get; } = homeTeam;
        public Team AwayTeam { get; } = awayTeam;
        public FieldDetails Location { get; } = location;
    }
}
