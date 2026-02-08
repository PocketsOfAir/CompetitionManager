using CompetitionManager.Transport;

namespace CompetitionManager.MatchupEngine.Strategies
{
    internal sealed class RoundRobinMatchupStrategy : IMatchupStrategy
    {
        private List<Team> Teams { get; set; }
        private CompetitionDetails CompetitionDetails { get; set; }
        private MatchupsMatrix MatchupsMatrix { get; set; }

        public RoundRobinMatchupStrategy(CompetitionDetails competitionDetails)
        {
            CompetitionDetails = competitionDetails;
            Teams = CsvUtils.LoadTeams();

            if (Teams.Count % 2 != 0)
            {
                Teams.Add(Team.CreateBye());
            }
            MatchupsMatrix = new MatchupsMatrix(Teams);
        }

        public void ExportMatches()
        {
            var allMatches = new List<List<Match>>();
            var totalRounds = Teams.Count - 1;
            for (var i = 0; i < totalRounds; i++)
            {
                var nextRound = GenerateRound(i);
                allMatches.Add(nextRound);
            }
            MatchupsMatrix.CheckMissingMatchups();
            CsvUtils.ExportRounds(allMatches, CompetitionDetails);
        }

        private List<Match> GenerateRound(int roundIndex)
        {
            var output = new List<Match>();
            var matchArray = new Team[Teams.Count];
            //Set the anchor team
            matchArray[0] = Teams[0];
            for (var i = 1; i < Teams.Count; i++)
            {
                var nextTeam = i + roundIndex;
                //Exclude the anchor team from the offset
                if (nextTeam >= Teams.Count)
                {
                    nextTeam = (nextTeam % Teams.Count) + 1;
                }
                matchArray[i] = Teams[nextTeam];
            }
            for (var i = 0; i < Teams.Count / 2; i++)
            {
                var j = Teams.Count - i - 1;
                var homeTeam = matchArray[i];
                var awayTeam = matchArray[j];

                MatchupsMatrix.AddMatchup(homeTeam.Name, awayTeam.Name);

                var nextMatch = new Match()
                {
                    HomeTeam = homeTeam.Name,
                    AwayTeam = awayTeam.Name,
                    IsBye = homeTeam.IsBye || awayTeam.IsBye,
                };

                output.Add(nextMatch);
            }
            return output;
        }
    }
}
