using CompetitionManager.MatchupEngine;
using CompetitionManager.Transport;
using CompetitionManager.Util;

try
{
    var compConfig = JsonUtils.LoadCompetitionDetails();
    var matchupGenerator = MatchupStrategyFactory.GetMatchupEngine(compConfig);
    matchupGenerator.ExportMatches();
}
catch (Exception e)
{
    LoggingService.Instance.Log($"Unexpected error generating matches: {e.Message}");
}

Console.Write("Press <Enter> to exit.");
while (Console.ReadKey().Key != ConsoleKey.Enter) { }
