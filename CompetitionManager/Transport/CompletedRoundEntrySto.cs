using CsvHelper.Configuration.Attributes;

namespace CompetitionManager.Transport
{
    [Delimiter(",")]
    internal sealed class CompletedRoundEntrySto
    {
        [Name("home_team")]
        public string HomeTeam { get; set; } = string.Empty;

        [Name("away_team")]
        public string AwayTeam { get; set; } = string.Empty;

        [Name("home_score")]
        public string HomeScore { get; set; } = string.Empty;

        [Name("away_score")]
        public string AwayScore { get; set; } = string.Empty;

        [Name("start_date")]
        public string GameDate { get; set; } = string.Empty;
    }
}
