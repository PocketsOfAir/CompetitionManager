namespace CompetitionManager.MatchupEngine
{
    internal sealed class Round
    {
        public Stack<Match> Matches { get; set; } = new Stack<Match>();

        public int RoundCost { get; set; } = 0;
    }
}
