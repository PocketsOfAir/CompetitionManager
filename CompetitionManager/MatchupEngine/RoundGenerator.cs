namespace CompetitionManager.MatchupEngine
{
    internal sealed class RoundGenerator
    {
        private CostsMatrix Costs { get; set; }
        private List<Team> Teams { get; set; }
        private Round NextRound { get; set; }
        private long PermutationsConsidered { get; set; }
        private long EarlyExits{ get; set; }

        public RoundGenerator(CostsMatrix costs, List<Team> teams)
        {
            NextRound = new Round
            {
                RoundCost = int.MaxValue,
            };
            Costs = costs;
            Teams = teams;
        }

        public Round FindBestRound()
        {
            var roundCandidates = new Team[Teams.Count];
            for (int j = 0; j < Teams.Count; j++)
            {
                roundCandidates[j] = Teams[j];
            }
            for (int i = 1; i < Teams.Count; i++)
            {
                CaluclateAllRoundsRecursively(roundCandidates, i, 0);
            }

            Console.WriteLine($"Search complete. {PermutationsConsidered} distinct rounds generated and {EarlyExits} early exits");
            Console.WriteLine($"Search complete. {PermutationsConsidered} distinct rounds generated and {EarlyExits} early exits");
            return NextRound;
        }

        private bool CaluclateAllRoundsRecursively(Team[] teams, int nextMatchIndex, int currentScore)
        {
            var homeTeam = teams[0];
            var awayTeam = teams[nextMatchIndex];
            var cost = Costs.GetCost(homeTeam, awayTeam);

            //rematch - early exit
            if (cost == int.MaxValue)
            {
                PermutationsConsidered++;
                return false;
            }
            currentScore += cost;

            //Current round score is worse than our best - early exit
            if (currentScore >= NextRound.RoundCost)
            {
                PermutationsConsidered++;
                return false;
            }

            var match = new Match
            {
                HomeTeam = homeTeam.Name,
                AwayTeam = awayTeam.Name,
            };

            if (teams.Length > 2)
            {
                //Create a new teams array containing all remaining teams other than the two we've just added as a match.
                var teamsToRecur = new Team[teams.Length - 2];
                var j = 0;
                for (int i = 1; i < teams.Length; i++)
                {
                    if (i == nextMatchIndex)
                    {
                        continue;
                    }
                    teamsToRecur[j] = teams[i];
                    j++;
                }

                var isBestRound = false;

                for (int i = 1; i < teamsToRecur.Length; i++)
                {
                    isBestRound = CaluclateAllRoundsRecursively(teamsToRecur, i, currentScore) || isBestRound;
                }

                if (isBestRound)
                {
                    NextRound.Matches.Push(match);
                }

                return isBestRound;
            }
            else
            {
                PermutationsConsidered++;
                Console.WriteLine($"Valid round found with score {currentScore}");
                NextRound.RoundCost = currentScore;
                NextRound.Matches.Clear();
                NextRound.Matches.Push(match);
                return true;
            }
        }
    }
}
