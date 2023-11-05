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

var matchupGenerator = new MatchupEngine(11, teams, completedRounds);
var matches = matchupGenerator.GenerateRound();

var compConfig = JsonUtils.LoadCompetitionDetails();

var competitionStartDate = compConfig.StartDate;
var roundDate = competitionStartDate.AddDays(completedRounds.Count * 7);
CsvUtils.SaveRound(matches, roundDate, compConfig.GameLength, compConfig.Location, completedRounds.Count + 1);


Console.Write("Press <Enter> to exit.");
while (Console.ReadKey().Key != ConsoleKey.Enter) { }
