using CsvHelper.Configuration.Attributes;

namespace CompetitionManager.Transport
{
    [Delimiter(",")]
    internal sealed class FieldPreferenceSto
    {
        [Index(0)]
        public string Team { get; set; } = string.Empty;
        [Index(1)]
        public int FieldNumber { get; set; } = 0;

        public FieldPreferenceSto()
        {
        }
    }
}
