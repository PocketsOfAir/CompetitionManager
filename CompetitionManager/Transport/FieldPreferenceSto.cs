using CsvHelper.Configuration.Attributes;

namespace CompetitionManager.Transport
{
    [Delimiter(",")]
    public sealed class FieldPreferenceSto
    {
        [Index(0)]
        public string Team { get; set; } = string.Empty;
        [Index(1)]
        public int FieldNumber { get; set; } = 0;
        [Index(2)]
        public string Location { get; set; } = string.Empty;
    }
}
