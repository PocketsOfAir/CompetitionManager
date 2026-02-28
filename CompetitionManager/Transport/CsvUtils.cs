using CompetitionManager.MatchupEngine;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace CompetitionManager.Transport
{
    public static class CsvUtils
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

            var adminTeams = new HashSet<string>(LoadFieldPreferences().Select(t => t.Team));

            foreach (var csvTeam in csvTeams)
            {
                var team = new Team(csvTeam.Name, csvTeam.Rating);
                if (adminTeams.Contains(team.Name))
                {
                    team.PreventByes = true;
                }
                teams.Add(team);
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

            var unallocatedMatches = new List<RoundEntrySto>();

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
                            matchedField.Allocated = true;
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
                    unallocatedMatches.Add(entry);
                }

                round.Matches.Add(entry);
            }

            var rng = new Random();

            foreach (var field in fields)
            {
                if (unallocatedMatches.Count == 0)
                {
                    break;
                }
                if (field.Allocated)
                {
                    continue;
                }
                var matchIndex = rng.Next(unallocatedMatches.Count);
                var entry = unallocatedMatches[matchIndex];
                entry.FieldNumber = field.FieldNumber;
                entry.Location = field.Location;
                unallocatedMatches.RemoveAt(matchIndex);
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

        public static void ExportRounds(List<List<Match>> rounds, CompetitionDetails details)
        {
            var allMatches = new List<RoundEntrySto>();
            for (var i = 0; i < rounds.Count; i++)
            {
                var round = rounds[i];
                var roundDate = details.StartDate.AddDays(i * 7);
                var matches = GetRoundAsSto(round, roundDate, details.GameLength, details.Fields);

                WriteMatchesToText(matches, $"{details.CompetitionName} round {i + details.StartingRound} email");

                allMatches.AddRange(matches.Matches);
            }

            var csvFilename = $"{details.CompetitionName} rounds {details.StartingRound} to {details.StartingRound + rounds.Count - 1}";

            WriteMatchesToCSV(allMatches, csvFilename);
        }

        public static void ExportRound(List<Match> round, DateTime startDate, int duration, List<FieldDetails> fields, string filename)
        {
            var matches = GetRoundAsSto(round, startDate, duration, fields);

            WriteMatchesToText(matches, $"{filename} email");

            WriteMatchesToCSV(matches.Matches, filename);
        }

        private static void WriteMatchesToText(RoundSto matches, string filename)
        {
            var allText = new StringBuilder();
            var groupedLocations = matches.Matches.GroupBy(r => r.Location);
            foreach (var location in groupedLocations)
            {
                if (groupedLocations.Count() > 1)
                {
                    allText.AppendLine($"{location.Key}:");
                    foreach (var entry in location.OrderBy(r => r.FieldNumber))
                    {
                        allText.AppendLine($"\tField {entry.FieldNumber}: {entry.HomeTeam} vs. {entry.AwayTeam}");
                    }
                }
                else
                {
                    foreach (var entry in location.OrderBy(r => r.FieldNumber))
                    {
                        allText.AppendLine($"Field {entry.FieldNumber}: {entry.HomeTeam} vs. {entry.AwayTeam}");
                    }
                }
            }
            if (matches.ByeTeam != string.Empty)
            {
                allText.AppendLine($"Bye: {matches.ByeTeam}");
            }

            File.WriteAllText(PathUtils.GetOutputFilePath($"{filename}.txt"), allText.ToString());
        }
    }
}
