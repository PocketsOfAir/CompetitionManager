using CompetitionManager.MatchupEngine;
using CompetitionManager.Transport;
using CompetitionManager.Util;

var teams = CsvUtils.LoadTeams();
var completedRounds = CsvUtils.LoadCompletedRounds();
var ratingUpdater = new RatingUpdateService(teams);
ratingUpdater.UpdateRatingsForRounds(completedRounds);

LoggingService.Instance.Log("");
LoggingService.Instance.Log("Current Calculated Ratings");
foreach (var team in teams.OrderByDescending(t => t.Rating))
{
    LoggingService.Instance.Log($"    Team: {team.Name}, Rating: {team.Rating}");
}
LoggingService.Instance.Log("");

var compConfig = JsonUtils.LoadCompetitionDetails();
var matchupGenerator = MatchupStrategyFactory.GetMatchupEngine(compConfig);
matchupGenerator.ExportMatches();

Console.Write("Press <Enter> to exit.");
while (Console.ReadKey().Key != ConsoleKey.Enter) { }
