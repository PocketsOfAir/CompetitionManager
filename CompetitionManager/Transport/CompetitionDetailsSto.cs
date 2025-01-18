namespace CompetitionManager.Transport
{
    internal sealed class CompetitionDetailsSto
    {
        public string CompetitionName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string GameLength { get; set; } = string.Empty;
        public List<LocationSto> Locations { get; set; } = [];
        public string Mode { get;set; } = string.Empty;
    }
}
