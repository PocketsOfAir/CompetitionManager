using CompetitionManager.Transport;
using System.Data;

namespace CompetitionManager.MatchupEngine
{
    internal sealed class CompletedMatch
    {
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }

        public CompletedMatch(string homeTeam, int homeTeamScore, string awayTeam, int awayTeamScore)
        {
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            HomeTeamScore = homeTeamScore;
            AwayTeamScore = awayTeamScore;
        }

        public static CompletedMatch CreateFromSto(CompletedRoundEntrySto sto)
        {
            var valid = false;
            float homeScore;
            float awayScore;

            if (sto.HomeScore.ToLower() == "win" && sto.AwayScore.ToLower() == "loss")
            {
                valid = true;
                homeScore = 1;
                awayScore = 0;
            }
            else if (sto.AwayScore.ToLower() == "win" && sto.HomeScore.ToLower() == "loss")
            {
                valid = true;
                homeScore = 0;
                awayScore = 1;
            }
            else if (float.TryParse(sto.HomeScore, out homeScore) & float.TryParse(sto.AwayScore, out awayScore))
            {
                valid = true;
            }

            if (!valid)
            {
                throw new DataException($"Invalid serialised game between {sto.HomeTeam} and {sto.AwayTeam}");
            }

            return new CompletedMatch(sto.HomeTeam, (int)Math.Floor(homeScore), sto.AwayTeam, (int)Math.Floor(awayScore));
        }
    }
}
