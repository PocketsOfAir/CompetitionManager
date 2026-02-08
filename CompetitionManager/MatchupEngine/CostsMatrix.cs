namespace CompetitionManager.MatchupEngine
{
    internal sealed class CostsMatrix
    {
        private List<Team> Teams { get; }
        private Dictionary<string, Team> TeamLookup { get; } = [];

        private List<CompletedRound> PreviousRounds { get; }

        private int[,] MatchupCosts { get; }

        public CostsMatrix(List<Team> teams, List<CompletedRound> previousRounds)
        {
            Teams = teams;
            PreviousRounds = previousRounds;
            MatchupCosts = new int[Teams.Count, Teams.Count];

            for (int i = 0; i < Teams.Count; i++)
            {
                TeamLookup[teams[i].Name] = teams[i];
                teams[i].MatrixIndex = i;
            }
        }

        public void GenerateCosts()
        {
            var hasBye = false;
            Team? byeTeam = null;
            var matchCount = new Dictionary<string, int>();
            foreach (var team in TeamLookup.Values)
            {
                if (team.IsBye)
                {
                    hasBye = true;
                    byeTeam = team;
                }
                matchCount[team.Name] = 0;
            }

            var maxGamesPlayed = 0;

            if (hasBye)
            {
                foreach (var round in PreviousRounds)
                {
                    foreach (var match in round.Matches)
                    {
                        TeamLookup[match.HomeTeam].MatchesPlayed++;
                        TeamLookup[match.AwayTeam].MatchesPlayed++;
                    }
                }
                maxGamesPlayed = matchCount.Select(m => m.Value).Max();
            }

            List<Team> byeCandidates = [];

            for (int i = 0; i < Teams.Count; i++)
            {
                var team1 = Teams[i];
                for (int j = i + 1; j < Teams.Count; j++)
                {
                    var team2 = Teams[j];
                    var cost = Math.Abs(team1.Rating - team2.Rating);
                    if (team1.IsBye || team2.IsBye)
                    {
                        cost = int.MaxValue;
                        if (team1.PreventByes || team2.PreventByes)
                        {
                        }
                        else if (team1.IsBye && matchCount[team2.Name] == maxGamesPlayed)
                        {
                            byeCandidates.Add(team2);
                        }
                        else if (team2.IsBye && matchCount[team1.Name] == maxGamesPlayed)
                        {
                            byeCandidates.Add(team1);
                        }
                    }
                    SetCost(team1.Name, team2.Name, cost);
                }
            }

            if (byeTeam != null && byeCandidates.Count > 0)
            {
                Console.WriteLine($"The following teams are candidates for the bye:\n\t{string.Join("\n\t", byeCandidates.Select(b => b.Name))}");
                var byeIndex = new Random().Next(byeCandidates.Count);
                var teamReceivingBye = byeCandidates[byeIndex];
                SetCost(teamReceivingBye.Name, byeTeam.Name, 0);
                Console.WriteLine($"Team '{teamReceivingBye.Name}' was allocated the bye.");
            }
        }

        public void PreventRematches(int replayThreshold)
        {
            var currentRound = PreviousRounds.Count + 1;
            foreach (var round in PreviousRounds)
            {
                var roundsSinceMatch = currentRound - round.RoundNumber;
                if (roundsSinceMatch < replayThreshold)
                {
                    foreach (var match in round.Matches)
                    {
                        SetCost(match.HomeTeam, match.AwayTeam, int.MaxValue);
                    }
                }
            }
        }

        public int GetCost(Team team1, Team team2)
        {
            return MatchupCosts[team1.MatrixIndex, team2.MatrixIndex];
        }

        private void SetCost(string team1, string team2, int cost)
        {
            var team1Index = TeamLookup[team1].MatrixIndex;
            var team2Index = TeamLookup[team2].MatrixIndex;
            //set both t1:t2 and t2:t1 so that retrieval is order-agnostic
            MatchupCosts[team1Index, team2Index] = cost;
            MatchupCosts[team2Index, team1Index] = cost;
        }
    }
}
