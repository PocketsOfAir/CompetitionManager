using CompetitionManager.MatchupEngine;
using CompetitionManager.Transport;

var compConfig = JsonUtils.LoadCompetitionDetails();
var matchupGenerator = MatchupStrategyFactory.GetMatchupEngine(compConfig);
matchupGenerator.ExportMatches();

Console.Write("Press <Enter> to exit.");
while (Console.ReadKey().Key != ConsoleKey.Enter) { }
