using CompetitionManager.Transport;
using CompetitionManager.Util;
using System.Diagnostics;

namespace CompetitionManager.MatchupEngine.Strategies
{
    internal sealed class DynamicMatchupStrategy : IMatchupStrategy
    {
        public int ReplayThreshold { get; private set; } = int.MaxValue;
        public int CurrentRound { get; private set; } = 1;
        private CostsMatrix Costs { get; set; }
        private List<Team> Teams { get; set; }
        private List<CompletedRound> PreviousRounds { get; set; }
        private CompetitionDetails CompetitionDetails { get; set; }

        public DynamicMatchupStrategy(CompetitionDetails competitionDetails)
        {
            Teams = CsvUtils.LoadTeams();
            PreviousRounds = CsvUtils.LoadCompletedRounds();

            if (Teams.Count % 2 != 0)
            {
                Teams.Add(Team.CreateBye());
            }

            Costs = new CostsMatrix(Teams);
            CurrentRound = PreviousRounds.Count + 1;
            CompetitionDetails = competitionDetails;
        }

        public void ExportMatches()
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var ratingUpdater = new RatingUpdateService(Teams);
            ratingUpdater.UpdateRatingsForRounds(PreviousRounds);

            LoggingService.Instance.Log("");
            LoggingService.Instance.Log("Calculating Ratings");
            foreach (var team in Teams.OrderByDescending(t => t.Rating))
            {
                LoggingService.Instance.Log($"    Team: {team.Name}, Rating: {team.Rating}");
            }
            LoggingService.Instance.Log("");

            LoggingService.Instance.Log("Generating costs and preventing rematches");
            Costs.GenerateCosts(Teams);
            Costs.PreventRematches(PreviousRounds, CurrentRound, ReplayThreshold);

            stopwatch.Stop();
            LoggingService.Instance.Log($"Costs generated after {stopwatch.ElapsedMilliseconds}ms. Generating rounds");
            stopwatch.Start();

            var roundGenerator = new RoundGenerationService(Costs, Teams, MatchupMode.BestTotalScore);

            var nextRound = roundGenerator.FindBestRound();

            stopwatch.Stop();
            LoggingService.Instance.Log($"Round generated after {stopwatch.ElapsedMilliseconds}ms.");

            LoggingService.Instance.Log($"Next round generated. Round score is {nextRound.RoundCost}");

            var output = new List<Match>();

            foreach (var match in nextRound.Matches)
            {
                if (match.IsBye)
                {
                    LoggingService.Instance.Log($"\t {match.HomeTeam} BYE");
                }
                else
                {
                    LoggingService.Instance.Log($"\t {match.HomeTeam} vs. {match.AwayTeam} (cost: {match.Cost})");
                }
                output.Add(match);
            }

            var competitionStartDate = CompetitionDetails.StartDate;
            var roundDate = competitionStartDate.AddDays(PreviousRounds.Count * 7);
            CsvUtils.ExportRound(output, roundDate, CompetitionDetails.GameLength, CompetitionDetails.Fields, $"{CompetitionDetails.CompetitionName} Round {PreviousRounds.Count + 1}");
        }
    }
}
