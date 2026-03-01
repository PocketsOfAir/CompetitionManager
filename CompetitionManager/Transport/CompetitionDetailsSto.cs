namespace CompetitionManager.Transport
{
    public sealed class CompetitionDetailsSto
    {
        public string CompetitionName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public int GameLength { get; set; } = 0;
        public List<LocationSto> Locations { get; set; } = [];
        public string Mode { get; set; } = string.Empty;
        public int? StartingRound { get; set; }
    }
}
