namespace CompetitionManager.MatchupEngine
{
    internal sealed class FieldPreference(string team, int fieldNumber)
    {
        public string Team { get; set; } = team;
        public int FieldNumber { get; set; } = fieldNumber;
    }
}
