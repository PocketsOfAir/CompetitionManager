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
                teams[i].CostsMatrixIndex = i;
            }
        }

        public int GetCost(Team team1, Team team2)
        {
            return MatchupCosts[team1.CostsMatrixIndex, team2.CostsMatrixIndex];
        }

        public void SetCost(string team1, string team2, int cost)
        {
            var team1Index = TeamIdLookup[team1];
            var team2Index = TeamIdLookup[team2];
            //set both t1:t2 and t2:t1 so that retrieval is order-agnostic
            MatchupCosts[team1Index, team2Index] = cost;
            MatchupCosts[team2Index, team1Index] = cost;
        }

        public void CalculateCost(Team team1, Team team2)
        {
            var cost = Math.Abs(team1.Rating - team2.Rating);
            if (team1.IsBye || team2.IsBye)
            {
                //all teams are equally valid for byes
                cost = 0;
            }
            var team1Index = TeamIdLookup[team1.Name];
            var team2Index = TeamIdLookup[team2.Name];

            MatchupCosts[team1Index, team2Index] = cost;
            MatchupCosts[team2Index, team1Index] = cost;
        }
    }
}
