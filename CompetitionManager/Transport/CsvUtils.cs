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
            var teamsCsvPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration", "Teams.csv");
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
            var teamsCsvPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration", "Field Preferences.csv");
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
            var csvPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration", "Completed Rounds.csv");
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

        public static void ExportMatches(List<Match> matches, DateTime startDate, int duration, string location, string filename)
        {
            var round = new List<RoundEntrySto>();

            var endDate = startDate.AddMinutes(duration);

            var preferences = LoadFieldPreferences();

            var fieldAllocation = new bool[matches.Count + 1];

            foreach (var match in matches)
            {
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
                            foreach (var entryToReallocate in round)
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

                round.Add(entry);
            }

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Encoding = Encoding.UTF8
            };

            var allText = new StringBuilder();
            foreach (var entry in round.OrderBy(r => r.FieldNumber ))
            {
                allText.AppendLine($"Field {entry.FieldNumber}: {entry.HomeTeam} vs. {entry.AwayTeam}");
            }

            File.WriteAllText(Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration", $"Round {roundNumber} email.txt"), allText.ToString());

            var outputCsvPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration", $"Round {roundNumber}.csv");
            var outputCsvPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration", filename);

            using var writer = new StreamWriter(outputCsvPath);
            using var csvOut = new CsvWriter(writer, csvConfig);
            csvOut.WriteRecords(round);
        }
    }
}
