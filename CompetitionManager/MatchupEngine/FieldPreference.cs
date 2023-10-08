namespace CompetitionManager.MatchupEngine
{
    internal sealed class FieldPreference
    {
        public string Team { get; set; }
        public int FieldNumber { get; set; }

        public FieldPreference(string team, int fieldNumber)
        {
            Team = team;
            FieldNumber = fieldNumber;
        }
    }
}
