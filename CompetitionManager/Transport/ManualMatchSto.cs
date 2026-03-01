namespace CompetitionManager.Transport
{
    public sealed class ManualMatchSto
    {
        public string HomeTeam { get; set; } = "";
        public string AwayTeam { get; set; } = "";
        public string Location { get; set; } = "";
        public int FieldNumber { get; set; }
    }
}
