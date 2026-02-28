using CsvHelper.Configuration.Attributes;

namespace CompetitionManager.Transport
{
    [Delimiter(",")]
    public sealed class TeamSto
    {
        [Index(0)]
        public string Name { get; set; } = string.Empty;
        [Index(1)]
        public int Rating { get; set; } = 0;

        public TeamSto()
        {
        }
    }
}
