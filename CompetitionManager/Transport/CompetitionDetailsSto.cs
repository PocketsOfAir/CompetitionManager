namespace CompetitionManager.Transport
{
    internal sealed class CompetitionDetailsSto
    {
        public string CompetitionName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string GameLength { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Mode { get;set; } = string.Empty;
    }
}
