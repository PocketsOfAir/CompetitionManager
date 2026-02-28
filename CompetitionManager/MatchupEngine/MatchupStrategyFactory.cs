using CompetitionManager.MatchupEngine.Strategies;

namespace CompetitionManager.MatchupEngine
{
    public static class MatchupStrategyFactory
    {
        public static IMatchupStrategy GetMatchupEngine(CompetitionDetails competitionDetails)
        {
            return competitionDetails.Mode switch
            {
                CompetitionMode.DynamicRounds => new DynamicMatchupStrategy(competitionDetails),
                CompetitionMode.RoundRobin => new RoundRobinMatchupStrategy(competitionDetails),
                _ => throw new ArgumentException($"Invalid competition mode '{competitionDetails.Mode}'"),
            };
        }
    }
}
