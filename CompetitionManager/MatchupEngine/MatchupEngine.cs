using System.Diagnostics;

namespace CompetitionManager.MatchupEngine
{
    internal sealed class MatchupEngine
    {
        public int ReplayThreshold { get; private set; }
        public int CurrentRound { get; private set; } = 1;

        private CostsMatrix Costs { get; set; }

        private List<Team> Teams { get; set; }
        private List<CompletedRound> PreviousRounds { get; set; }

        public MatchupEngine(int replayThreshold, List<Team> teams, List<CompletedRound> rounds)
        {
            Teams = teams;
            PreviousRounds = rounds;
            Costs = new CostsMatrix(Teams);
            ReplayThreshold = replayThreshold;
            CurrentRound = PreviousRounds.Count + 1;
        }

        public List<Match> GenerateRound()
        {
            var stopwatch = new Stopwatch();
            Console.WriteLine("Generating costs and preventing rematches");
            stopwatch.Start();
            
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

            return output;
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
