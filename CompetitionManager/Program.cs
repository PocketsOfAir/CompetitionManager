using CompetitionManager.MatchupEngine;
using CompetitionManager.Transport;

var teams = CsvUtils.LoadTeams();
var completedRounds = CsvUtils.LoadCompletedRounds();
var ratingUpdater = new RatingUpdateService(teams);
ratingUpdater.UpdateRatingsForRounds(completedRounds);

Console.WriteLine("");
Console.WriteLine("Current Calculated Ratings");
foreach (var team in teams.OrderByDescending(t => t.Rating))
{
    Console.WriteLine($"    Team: {team.Name}, Rating: {team.Rating}");
}
Console.WriteLine("");

var matchupGenerator = new MatchupEngine(11, teams, completedRounds);
var matches = matchupGenerator.GenerateRound();

var compConfig = JsonUtils.LoadCompetitionDetails();

var competitionStartDate = compConfig.StartDate;
var roundDate = competitionStartDate.AddDays(completedRounds.Count * 7);
CsvUtils.SaveRound(matches, roundDate, compConfig.GameLength, compConfig.Location, completedRounds.Count + 1);


Console.Write("Press <Enter> to exit.");
while (Console.ReadKey().Key != ConsoleKey.Enter) { }
