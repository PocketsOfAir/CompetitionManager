using CompetitionManager.Util;

namespace CompetitionManager.MatchupEngine
{
    internal sealed class MatchupsMatrix
    {
        private Dictionary<string, int> TeamIdLookup { get; set; } = [];

        private bool[,] Matchups { get; set; }

        private List<Team> Teams { get; set; }

        public MatchupsMatrix(List<Team> teams)
        {
            Teams = teams;
            Matchups = new bool[Teams.Count, Teams.Count];

            for (int i = 0; i < Teams.Count; i++)
            {
                TeamIdLookup[teams[i].Name] = i;
                teams[i].MatrixIndex = i;
            }
        }

        public void AddMatchup(string team1, string team2)
        {
            var team1Index = TeamIdLookup[team1];
            var team2Index = TeamIdLookup[team2];
            if (Matchups[team1Index, team2Index] || Matchups[team2Index, team1Index])
            {
                LoggingService.Instance.Log($"Rematch detected between {team1} and {team2}");
            }
            else
            {
                Matchups[team1Index, team2Index] = true;
                Matchups[team2Index, team1Index] = true;
            }
        }

        public void CheckMissingMatchups()
        {
            for (var i = 0; i < Teams.Count; i++)
            {
                for (var j = i + 1; j < Teams.Count; j++)
                {
                    if (!Matchups[j, i])
                    {
                        var homeTeam = Teams[j];
                        var awayTeam = Teams[i];
                        if (homeTeam.IsBye || awayTeam.IsBye)
                        {
                            LoggingService.Instance.Log($"Missing bye for {homeTeam.Name + awayTeam.Name}");
                        }
                        else
                        {
                            LoggingService.Instance.Log($"Missing match between {homeTeam.Name} and {awayTeam.Name}");
                        }
                    }
                }
            }
        }
    }
}
