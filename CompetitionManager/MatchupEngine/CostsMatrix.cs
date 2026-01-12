namespace CompetitionManager.MatchupEngine
{
    internal sealed class CostsMatrix
    {
        private Dictionary<string, int> TeamIdLookup { get; set; } = [];

        private int[,] MatchupCosts { get; set; }

        private int TeamCount { get; set; } = 0;

        public CostsMatrix(List<Team> teams)
        {
            TeamCount = teams.Count;
            MatchupCosts = new int[TeamCount, TeamCount];

            for (int i = 0; i < TeamCount; i++)
            {
                TeamIdLookup[teams[i].Name] = i;
                teams[i].MatrixIndex = i;
            }
        }

        public void GenerateCosts(List<Team> teams)
        {
            for (int i = 0; i < teams.Count; i++)
            {
                var team1 = teams[i];
                for (int j = i + 1; j < teams.Count; j++)
                {
                    var team2 = teams[j];
                    var cost = Math.Abs(team1.Rating - team2.Rating);
                    if (team1.IsBye || team2.IsBye)
                    {
                        if (team1.PreventByes || team2.PreventByes)
                        {
                            cost = int.MaxValue;
                        }
                        else
                        {
                            cost = 0;
                        }
                    }
                    SetCost(team1.Name, team2.Name, cost);
                }
            }
        }

        public void PreventRematches(List<CompletedRound> previousRounds, int currentRound, int replayThreshold)
        {
            foreach (var round in previousRounds)
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
            var team1Index = TeamIdLookup[team1];
            var team2Index = TeamIdLookup[team2];
            //set both t1:t2 and t2:t1 so that retrieval is order-agnostic
            MatchupCosts[team1Index, team2Index] = cost;
            MatchupCosts[team2Index, team1Index] = cost;
        }
    }
}
