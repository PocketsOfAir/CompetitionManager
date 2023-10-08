namespace CompetitionManager.MatchupEngine
{
    internal sealed class RoundGenerationService
    {
        private CostsMatrix Costs { get; set; }
        private List<Team> Teams { get; set; }
        private Round NextRound { get; set; }
        private MatchupMode Mode { get; set; }
        private long PermutationsConsidered { get; set; }

        public RoundGenerationService(CostsMatrix costs, List<Team> teams, MatchupMode mode)
        {
            NextRound = new Round
            {
                RoundCost = int.MaxValue,
            };
            Costs = costs;
            Teams = teams;
            Mode = mode;
        }

        public Round FindBestRound()
        {
            if(Teams.Count % 2 != 0)
            {
                var error = "Can't generate round: there's an odd number of teams.";
                Console.WriteLine(error);
                throw new InvalidDataException(error);
            }
            var roundCandidates = new Team[Teams.Count];
            for (int j = 0; j < Teams.Count; j++)
            {
                roundCandidates[j] = Teams[j];
            }
            for (int i = 1; i < Teams.Count; i++)
            {
                CaluclateAllRoundsRecursively(roundCandidates, i, 0);
            }

            Console.WriteLine($"Search complete. {PermutationsConsidered} paths considered");
            return NextRound;
        }

        private bool CaluclateAllRoundsRecursively(Team[] teams, int nextMatchIndex, int currentScore)
        {
            PermutationsConsidered++;

            var homeTeam = teams[0];
            var awayTeam = teams[nextMatchIndex];
            var cost = Costs.GetCost(homeTeam, awayTeam);

            //rematch - early exit
            if (cost == int.MaxValue)
            {
                return false;
            }

            if (Mode == MatchupMode.LeastWorstSingleScore)
            {
                if (cost > currentScore)
                {
                    currentScore = cost;
                }
            }
            else
            {
                currentScore += cost;
            }

            //Current round score is worse than our best - early exit
            if (currentScore >= NextRound.RoundCost)
            {
                return false;
            }

            var match = new Match
            {
                HomeTeam = homeTeam.Name,
                AwayTeam = awayTeam.Name,
                Cost = cost,
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
                Console.WriteLine($"Valid round found with score {currentScore}");
                NextRound.RoundCost = currentScore;
                NextRound.Matches.Clear();
                NextRound.Matches.Push(match);
                return true;
            }
        }
    }
}
