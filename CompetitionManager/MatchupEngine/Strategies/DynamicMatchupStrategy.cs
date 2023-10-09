using CompetitionManager.Transport;
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

            Console.WriteLine("");
            Console.WriteLine("Calculating Ratings");
            foreach (var team in Teams.OrderByDescending(t => t.Rating))
            {
                Console.WriteLine($"    Team: {team.Name}, Rating: {team.Rating}");
            }
            Console.WriteLine("");

            Console.WriteLine("Generating costs and preventing rematches");

            GenerateCosts();
            PreventRematches();

            stopwatch.Stop();
            Console.WriteLine($"Costs generated after {stopwatch.ElapsedMilliseconds}ms. Generating rounds");
            stopwatch.Start();

            var roundGenerator = new RoundGenerationService(Costs, Teams, MatchupMode.BestTotalScore);

            var nextRound = roundGenerator.FindBestRound();

            stopwatch.Stop();
            Console.WriteLine($"Round generated after {stopwatch.ElapsedMilliseconds}ms.");

            Console.WriteLine($"Next round generated. Round score is {nextRound.RoundCost}");

            var output = new List<Match>();

            foreach (var match in nextRound.Matches)
            {
                output.Add(match);
                Console.WriteLine($"\t {match.HomeTeam} vs. {match.AwayTeam} (cost: {match.Cost})");
            }

            var competitionStartDate = CompetitionDetails.StartDate;
            var roundDate = competitionStartDate.AddDays(PreviousRounds.Count * 7);
            CsvUtils.ExportMatches(output, roundDate, CompetitionDetails.GameLength, CompetitionDetails.Location, $"Round {PreviousRounds.Count + 1}.csv");
        }

        private void GenerateCosts()
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                var team1 = Teams[i];
                for (int j = i + 1; j < Teams.Count; j++)
                {
                    var team2 = Teams[j];
                    Costs.CalculateCost(team1, team2);
                }
            }
        }

        private void PreventRematches()
        {
            foreach (var round in PreviousRounds)
            {
                var roundsSinceMatch = CurrentRound - round.RoundNumber;
                if (roundsSinceMatch < ReplayThreshold)
                {
                    foreach (var match in round.Matches)
                    {
                        Costs.SetCost(match.HomeTeam, match.AwayTeam, int.MaxValue);
                    }
                }
            }
        }
    }
}
