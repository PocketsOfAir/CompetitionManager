namespace CompetitionManager.MatchupEngine
{
    public sealed class FieldPreference(string team, int fieldNumber, string location)
    {
        public string Team { get; set; } = team;
        public FieldDetails Field { get; set; } = new FieldDetails(location, fieldNumber);
    }
}
