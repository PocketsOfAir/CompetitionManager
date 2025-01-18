﻿using CompetitionManager.MatchupEngine;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
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

            using var reader = new StreamReader(teamsCsvPath);
            using var csv = new CsvReader(reader, csvConfig);
            var csvTeams = csv.GetRecords<TeamSto>().ToList();
            foreach (var team in csvTeams)
            {
                teams.Add(new Team(team.Name, team.Rating));
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

            using var reader = new StreamReader(teamsCsvPath);
            using var csv = new CsvReader(reader, csvConfig);
            var csvTeams = csv.GetRecords<FieldPreferenceSto>().ToList();
            foreach (var pref in csvTeams)
            {
                preferences.Add(new FieldPreference(pref.Team, pref.FieldNumber, pref.Location));
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

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, csvConfig);
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
            return completedRounds;
        }

        private static RoundSto GetRoundAsSto(List<Match> matches, DateTime startDate, int duration, List<FieldDetails> fields)
        {
            if (matches.Count > fields.Count)
            {
                throw new InvalidDataException($"More matches in round ({matches.Count}) than fields defined in competition details ({fields.Count})");
            }

            foreach (var field in fields)
            {
                field.Allocated = false;
            }

            var round = new RoundSto();

            var endDate = startDate.AddMinutes(duration);

            var preferences = LoadFieldPreferences();

            var fieldLookup = new HashSet<FieldDetails>(fields);

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
                };

                var allocated = false;
                foreach (var preference in preferences)
                {
                    if (match.HomeTeam == preference.Team || match.AwayTeam == preference.Team)
                    {
                        entry.FieldNumber = preference.Field.FieldNumber;
                        entry.Location = preference.Field.Location;
                        allocated = true;
                        if (fieldLookup.TryGetValue(preference.Field, out var matchedField))
                        {
                            if (matchedField.Allocated == false)
                            {
                                matchedField.Allocated = true;
                            }
                            else
                            {
                                foreach (var entryToReallocate in round.Matches)
                                {
                                    if (entryToReallocate.FieldNumber == preference.Field.FieldNumber && entryToReallocate.Location == preference.Field.Location)
                                    {
                                        foreach (var field in fields)
                                        {
                                            if (field.Allocated == false)
                                            {
                                                entryToReallocate.FieldNumber = field.FieldNumber;
                                                entryToReallocate.Location = field.Location;
                                                field.Allocated = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidDataException($"Preferred field for team '{preference.Team}' (#{preference.Field.FieldNumber} at {preference.Field.Location}) doesn't exist in configured locations");
                        }
                        break;
                    }
                }

                if (!allocated)
                {
                    foreach (var field in fields)
                    {
                        if (field.Allocated == false)
                        {
                            entry.FieldNumber = field.FieldNumber;
                            entry.Location = field.Location;
                            field.Allocated = true;
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

        public static void ExportRounds(List<List<Match>> rounds, DateTime startDate, int duration, List<FieldDetails> fields, string filename)
        {
            var allMatches = new List<RoundEntrySto>();
            for (var i = 0; i < rounds.Count; i++)
            {
                var round = rounds[i];
                var roundDate = startDate.AddDays(i * 7);
                var matches = GetRoundAsSto(round, roundDate, duration, fields);

                var allText = new StringBuilder();
                foreach (var entry in matches.Matches.OrderBy(r => r.FieldNumber))
                {
                    allText.AppendLine($"Field {entry.FieldNumber}: {entry.HomeTeam} vs. {entry.AwayTeam}");
                }
                if (matches.ByeTeam != string.Empty)
                {
                    allText.AppendLine($"Bye: {matches.ByeTeam}");
                }


                File.WriteAllText(PathUtils.GetOutputFilePath($"{filename} round {i + 1} email.txt"), allText.ToString());

                allMatches.AddRange(matches.Matches);
            }

            WriteMatchesToCSV(allMatches, filename);
        }

        public static void ExportRound(List<Match> round, DateTime startDate, int duration, List<FieldDetails> fields, string filename)
        {
            var matches = GetRoundAsSto(round, startDate, duration, fields);

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
