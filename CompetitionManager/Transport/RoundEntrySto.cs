using CsvHelper.Configuration.Attributes;

namespace CompetitionManager.Transport
{
    [Delimiter(",")]
    public sealed class RoundEntrySto
    {
        [Index(0)]
        public string HomeTeam { get; set; } = string.Empty;

        [Index(1)]
        public string AwayTeam { get; set; } = string.Empty;

        [Index(2)]
        public string StartDateText { get; set; } = string.Empty;

        [Index(3)]
        public string StartTimeText { get; set; } = string.Empty;

        [Index(4)]
        public string EndDateText { get; set; } = string.Empty;

        [Index(5)]
        public string EndTimeText { get; set; } = string.Empty;

        [Index(6)]
        public string Location { get; set; } = string.Empty;

        [Index(7)]
        public int FieldNumber { get; set; } = 0;
    }
}
