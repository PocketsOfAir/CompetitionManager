using CompetitionManager.MatchupEngine;
using System.Text.Json;

namespace CompetitionManager.Transport
{
    public static class JsonUtils
    {
        public static CompetitionDetails LoadCompetitionDetails()
        {
            var path = PathUtils.GetConfigFilePath("CompetitionDetails.json");
            var jsonString = File.ReadAllText(path);
            var details = JsonSerializer.Deserialize<CompetitionDetailsSto>(jsonString) ?? throw new InvalidDataException("Failed to find CompetitionDetails.json");
            var output = CompetitionDetails.CreateFromSto(details);

            return output;
        }

        public static ManualMatchesSto LoadManualMatches()
        {
            var manualMatchesSto = new ManualMatchesSto();
            var manualMatchesPath = PathUtils.GetConfigFilePath("ManualMatches.json");
            if (File.Exists(manualMatchesPath))
            {
                var manualMatchesJsonString = File.ReadAllText(manualMatchesPath);
                manualMatchesSto = JsonSerializer.Deserialize<ManualMatchesSto>(manualMatchesJsonString) ?? throw new InvalidDataException("Failed to load ManualMatches.json");
            }

            return manualMatchesSto;
        }
    }
}
