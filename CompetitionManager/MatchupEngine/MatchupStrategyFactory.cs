using CompetitionManager.MatchupEngine.Strategies;

namespace CompetitionManager.MatchupEngine
{
    internal static class MatchupStrategyFactory
    {
        public static IMatchupStrategy GetMatchupEngine(CompetitionDetails competitionDetails)
        {
            switch (competitionDetails.Mode)
            {
                case CompetitionMode.DynamicRounds:
                    var matchupGenerator = new DynamicMatchupStrategy(competitionDetails);
                    return matchupGenerator;
                case CompetitionMode.None:
                default:
                    throw new ArgumentException($"Invalid competition mode '{competitionDetails.Mode}'");
            }
        }
    }
}
