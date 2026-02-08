using CompetitionManager.Transport;
using CompetitionManager.Util;
using System.Diagnostics;

namespace CompetitionManager.MatchupEngine.Strategies
{
    internal sealed class DynamicMatchupStrategy : IMatchupStrategy
    {
        private int ReplayThreshold { get; } = int.MaxValue;
        private int CurrentRound { get; } = 1;
        private List<Team> Teams { get; }
        private List<CompletedRound> PreviousRounds { get; }
        private CompetitionDetails CompetitionDetails { get; }
        private Dictionary<string, Team> TeamLookup { get; } = [];
        private int[,] MatchupCosts { get; }
        private Round NextRound { get; }
        private MatchupMode Mode { get; } = MatchupMode.BestTotalScore;
        private long PermutationsConsidered { get; set; } = 0;

        public DynamicMatchupStrategy(CompetitionDetails competitionDetails)
        {
            Teams = CsvUtils.LoadTeams();
            PreviousRounds = CsvUtils.LoadCompletedRounds();

            if (Teams.Count % 2 != 0)
            {
                Teams.Add(Team.CreateBye());
            }

            CurrentRound = PreviousRounds.Count + 1;
            CompetitionDetails = competitionDetails;
            MatchupCosts = new int[Teams.Count, Teams.Count];

            for (int i = 0; i < Teams.Count; i++)
            {
                TeamLookup[Teams[i].Name] = Teams[i];
                Teams[i].MatrixIndex = i;
            }

            NextRound = new Round
            {
                RoundCost = int.MaxValue,
            };
        }

        public void ExportMatches()
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var ratingUpdater = new RatingUpdateService(Teams);
            ratingUpdater.UpdateRatingsForRounds(PreviousRounds);

            LoggingService.Instance.Log("");
            LoggingService.Instance.Log("Calculating Ratings");
            foreach (var team in Teams.OrderByDescending(t => t.Rating))
            {
                LoggingService.Instance.Log($"\tTeam: {team.Name}, Rating: {team.Rating}");
            }
            LoggingService.Instance.Log("");

            LoggingService.Instance.Log("Generating costs and preventing rematches");
            GenerateCosts();
            PreventRematches(ReplayThreshold);

            stopwatch.Stop();
            LoggingService.Instance.Log($"Costs generated after {stopwatch.ElapsedMilliseconds}ms. Generating rounds");
            stopwatch.Start();

            FindBestRound();

            stopwatch.Stop();
            LoggingService.Instance.Log($"Round generated after {stopwatch.ElapsedMilliseconds}ms.");

            LoggingService.Instance.Log($"Next round generated. Round score is {NextRound.RoundCost}");

            var output = new List<Match>();

            foreach (var match in NextRound.Matches)
            {
                if (match.IsBye)
                {
                    LoggingService.Instance.Log($"\t{match.HomeTeam} BYE");
                }
                else
                {
                    LoggingService.Instance.Log($"\t{match.HomeTeam} vs. {match.AwayTeam} (cost: {match.Cost})");
                }
                output.Add(match);
            }

            var competitionStartDate = CompetitionDetails.StartDate;
            var roundDate = competitionStartDate.AddDays(PreviousRounds.Count * 7);
            CsvUtils.ExportRound(output, roundDate, CompetitionDetails.GameLength, CompetitionDetails.Fields, $"{CompetitionDetails.CompetitionName} Round {PreviousRounds.Count + 1}");
        }

        private void GenerateCosts()
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
                LoggingService.Instance.Log($"The following teams are candidates for the bye:\n\t{string.Join("\n\t", byeCandidates.Select(b => b.Name))}");
                var byeIndex = new Random().Next(byeCandidates.Count);
                var teamReceivingBye = byeCandidates[byeIndex];
                SetCost(teamReceivingBye.Name, byeTeam.Name, 0);
                LoggingService.Instance.Log($"Team '{teamReceivingBye.Name}' was allocated the bye.");
            }
        }

        private void PreventRematches(int replayThreshold)
        {
            foreach (var round in PreviousRounds)
            {
                var roundsSinceMatch = CurrentRound - round.RoundNumber;
                if (roundsSinceMatch < replayThreshold)
                {
                    foreach (var match in round.Matches)
                    {
                        SetCost(match.HomeTeam, match.AwayTeam, int.MaxValue);
                    }
                }
            }
        }

        private int GetCost(Team team1, Team team2)
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

        private Round FindBestRound()
        {
            if (Teams.Count % 2 != 0)
            {
                var error = "Can't generate round: there's an odd number of teams.";
                LoggingService.Instance.Log(error);
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

            LoggingService.Instance.Log($"Search complete. {PermutationsConsidered} paths considered");
            return NextRound;
        }

        private bool CaluclateAllRoundsRecursively(Team[] teams, int nextMatchIndex, int currentScore)
        {
            PermutationsConsidered++;

            var homeTeam = teams[0];
            var awayTeam = teams[nextMatchIndex];
            var cost = GetCost(homeTeam, awayTeam);

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
                IsBye = homeTeam.IsBye || awayTeam.IsBye,
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
                LoggingService.Instance.Log($"Valid round found with score {currentScore}");
                NextRound.RoundCost = currentScore;
                NextRound.Matches.Clear();
                NextRound.Matches.Push(match);
                return true;
            }
        }
    }
}
