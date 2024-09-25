using CompetitionManager.MatchupEngine;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace CompetitionManager.Transport
{
    internal static class CsvUtils
    {
        public static List<Team> LoadTeams()
        {
            var teamsCsvPath = PathUtils.GetConfigFilePath("Teams.csv");
            var teams = new List<Team>();
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Encoding = Encoding.UTF8
            };

            using (var reader = new StreamReader(teamsCsvPath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var csvTeams = csv.GetRecords<TeamSto>().ToList();
                foreach (var team in csvTeams)
                {
                    teams.Add(new Team(team.Name, team.Rating));
                }
            }
            return teams;
        }

        public static List<FieldPreference> LoadFieldPreferences()
        {
            var teamsCsvPath = PathUtils.GetConfigFilePath("Field Preferences.csv");
            var preferences = new List<FieldPreference>();
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Encoding = Encoding.UTF8
            };

            using (var reader = new StreamReader(teamsCsvPath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var csvTeams = csv.GetRecords<FieldPreferenceSto>().ToList();
                foreach (var pref in csvTeams)
                {
                    preferences.Add(new FieldPreference(pref.Team, pref.FieldNumber));
                }
            }
            return preferences;
        }

        public static List<CompletedRound> LoadCompletedRounds()
        {
            var csvPath = PathUtils.GetConfigFilePath("Completed Rounds.csv");
            var completedRounds = new List<CompletedRound>();
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Encoding = Encoding.UTF8
            };

            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                var csvTeams = csv.GetRecords<CompletedRoundEntrySto>().ToList();
                var roundStos = csvTeams.GroupBy(r => r.GameDate);
                var i = 1;
                foreach (var roundSto in roundStos)
                {
                    var round = new CompletedRound
                    {
                        RatingCalculationRequired = true,
                        RoundNumber = i,
                    };
                    foreach (var matchSto in roundSto)
                    {
                        var match = CompletedMatch.CreateFromSto(matchSto);
                        round.Matches.Add(match);
                    }
                    completedRounds.Add(round);
                    i++;
                }
            }
            return completedRounds;
        }

        private static RoundSto GetRoundAsSto(List<Match> matches, DateTime startDate, int duration, string location)
        {
            var round = new RoundSto();

            var endDate = startDate.AddMinutes(duration);

            var preferences = LoadFieldPreferences();

            var fieldAllocation = new bool[matches.Count + 1];

            foreach (var match in matches)
            {
                if (match.IsBye)
                {
                    round.ByeTeam = match.HomeTeam + match.AwayTeam;
                    continue;
                }
                var entry = new RoundEntrySto
                {
                    HomeTeam = match.HomeTeam,
                    AwayTeam = match.AwayTeam,
                    StartDateText = startDate.ToString("dd/MM/yyyy"),
                    StartTimeText = startDate.ToString("HH:mm"),
                    EndDateText = endDate.ToString("dd/MM/yyyy"),
                    EndTimeText = endDate.ToString("HH:mm"),
                    Location = location,
                };

                var allocated = false;
                foreach (var preference in preferences)
                {
                    if (match.HomeTeam == preference.Team || match.AwayTeam == preference.Team)
                    {
                        entry.FieldNumber = preference.FieldNumber;
                        allocated = true;
                        if (fieldAllocation[entry.FieldNumber] == true)
                        {
                            foreach (var entryToReallocate in round.Matches)
                            {
                                if (entryToReallocate.FieldNumber == preference.FieldNumber)
                                {
                                    for (var i = 1; i <= fieldAllocation.Length; i++)
                                    {
                                        var field = fieldAllocation[i];
                                        if (field == false)
                                        {
                                            entryToReallocate.FieldNumber = i;
                                            fieldAllocation[i] = true;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        fieldAllocation[entry.FieldNumber] = true;
                        break;
                    }
                }

                if (!allocated)
                {
                    for (var i = 1; i <= fieldAllocation.Length; i++)
                    {
                        var field = fieldAllocation[i];
                        if (field == false)
                        {
                            entry.FieldNumber = i;
                            fieldAllocation[i] = true;
                            break;
                        }
                    }
                }

                round.Matches.Add(entry);
            }

            return round;
        }

        private static void WriteMatchesToCSV(List<RoundEntrySto> matches, string filename)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Encoding = Encoding.UTF8
            };

            var outputCsvPath = PathUtils.GetOutputFilePath($"{filename}.csv");

            using var writer = new StreamWriter(outputCsvPath);
            using var csvOut = new CsvWriter(writer, csvConfig);
            csvOut.WriteRecords(matches);
        }

        public static void ExportRounds(List<List<Match>> rounds, DateTime startDate, int duration, string location, string filename)
        {
            var allMatches = new List<RoundEntrySto>();
            for (var i = 0; i < rounds.Count; i++)
            {
                var round = rounds[i];
                var roundDate = startDate.AddDays(i * 7);
                var matches = GetRoundAsSto(round, roundDate, duration, location);

                var allText = new StringBuilder();
                foreach (var entry in matches.Matches.OrderBy(r => r.FieldNumber))
                {
                    allText.AppendLine($"Field {entry.FieldNumber}: {entry.HomeTeam} vs. {entry.AwayTeam}");
                }
                if (matches.ByeTeam != string.Empty)
                {
                    allText.AppendLine($"Bye: {matches.ByeTeam}");
                }


                File.WriteAllText(PathUtils.GetOutputFilePath($"{filename} round {i+1} email.txt"), allText.ToString());

                allMatches.AddRange(matches.Matches);
            }

            WriteMatchesToCSV(allMatches, filename);
        }

        public static void ExportRound(List<Match> round, DateTime startDate, int duration, string location, string filename)
        {
            var matches = GetRoundAsSto(round, startDate, duration, location);

            var allText = new StringBuilder();
            foreach (var entry in matches.Matches.OrderBy(r => r.FieldNumber))
            {
                allText.AppendLine($"Field {entry.FieldNumber}: {entry.HomeTeam} vs. {entry.AwayTeam}");
            }
            if (matches.ByeTeam != string.Empty)
            {
                allText.AppendLine($"Bye: {matches.ByeTeam}");
            }

            File.WriteAllText(PathUtils.GetOutputFilePath($"{filename} email.txt"), allText.ToString());

            WriteMatchesToCSV(matches.Matches, filename);
        }
    }
}
