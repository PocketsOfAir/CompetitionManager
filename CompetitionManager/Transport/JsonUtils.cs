using CompetitionManager.MatchupEngine;
using System.Reflection;
using System.Text.Json;

namespace CompetitionManager.Transport
{
    internal static class JsonUtils
    {
        public static CompetitionDetails LoadCompetitionDetails()
        {
            var path = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration", "CompetitionDetails.json");
            var jsonString = File.ReadAllText(path);
            var details = JsonSerializer.Deserialize<CompetitionDetailsSto>(jsonString) ?? throw new InvalidDataException("Failed to find CompetitionDetails.json");
            var output = CompetitionDetails.CreateFromSto(details);

            return output;
        }
    }
}
