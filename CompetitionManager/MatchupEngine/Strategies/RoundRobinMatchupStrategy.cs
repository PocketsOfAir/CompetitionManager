namespace CompetitionManager.MatchupEngine.Strategies
{
    internal sealed class RoundRobinMatchupStrategy(CompetitionDetails competitionDetails) : IMatchupStrategy
    {
        private CompetitionDetails CompetitionDetails { get; set; } = competitionDetails;

        public void ExportMatches()
        {
            throw new NotImplementedException();
        }
    }
}
