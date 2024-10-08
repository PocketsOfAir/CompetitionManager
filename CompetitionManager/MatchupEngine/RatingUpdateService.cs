﻿using CompetitionManager.Util;

namespace CompetitionManager.MatchupEngine
{
    internal sealed class RatingUpdateService
    {
        private Dictionary<string, Team> Teams { get; set; } = [];
        public RatingUpdateService(List<Team> teams)
        {
            foreach (var team in teams)
            {
                Teams[team.Name] = team;
            }
        }

        public void UpdateRatingsForRounds(List<CompletedRound> rounds)
        {
            foreach (var round in rounds)
            {
                if (!round.RatingCalculationRequired)
                {
                    continue;
                }
                foreach (var match in round.Matches)
                {
                    var relativeRating = Teams[match.AwayTeam].Rating - Teams[match.HomeTeam].Rating;
                    var homeTeamExpectedScore = 1.0 / (1.0 + Math.Pow(10.0, relativeRating / 400.0));

                    var wager = (Math.Max(match.HomeTeamScore, match.AwayTeamScore) * 2) + 10;
                    var homeTeamWager = (int)Math.Round(homeTeamExpectedScore * wager);
                    var awayTeamWager = wager - homeTeamWager;

                    LoggingService.Instance.Log($"Rating change for game between {match.HomeTeam} (prior rating: {Teams[match.HomeTeam].Rating}) and {match.AwayTeam} (prior rating: {Teams[match.AwayTeam].Rating})");
                    LoggingService.Instance.Log($"\tExpected result is {match.HomeTeam} {Math.Round(homeTeamExpectedScore * 100)}% : {match.AwayTeam} {Math.Round((1 - homeTeamExpectedScore) * 100)}%");
                    LoggingService.Instance.Log($"\tActual result is {match.HomeTeam} {match.HomeTeamScore} : {match.AwayTeam} {match.AwayTeamScore}");

                    if (match.ExcludeFromRatings)
                    {
                        LoggingService.Instance.Log($"\tFORFEIT - RATINGS UNCHANGED: {match.HomeTeam} ({Teams[match.HomeTeam].Rating}) and {match.AwayTeam} ({Teams[match.AwayTeam].Rating})");
                        continue;
                    }

                    if (match.HomeTeamScore > match.AwayTeamScore)
                    {
                        var winnerPot = wager - match.AwayTeamScore;
                        Teams[match.AwayTeam].Rating = Teams[match.AwayTeam].Rating - awayTeamWager + match.AwayTeamScore;
                        Teams[match.HomeTeam].Rating = Teams[match.HomeTeam].Rating - homeTeamWager + winnerPot;
                    }
                    else if (match.AwayTeamScore > match.HomeTeamScore)
                    {
                        var winnerPot = wager - match.HomeTeamScore;
                        Teams[match.HomeTeam].Rating = Teams[match.HomeTeam].Rating - homeTeamWager + match.HomeTeamScore;
                        Teams[match.AwayTeam].Rating = Teams[match.AwayTeam].Rating - awayTeamWager + winnerPot;
                    }
                    else //draw
                    {
                        Teams[match.HomeTeam].Rating = Teams[match.HomeTeam].Rating - homeTeamWager + wager / 2;
                        Teams[match.AwayTeam].Rating = Teams[match.AwayTeam].Rating - awayTeamWager + wager / 2;
                    }
                    LoggingService.Instance.Log($"\tNew Ratings: {match.HomeTeam} ({Teams[match.HomeTeam].Rating}) and {match.AwayTeam} ({Teams[match.AwayTeam].Rating})");
                }
            }
        }
    }
}
