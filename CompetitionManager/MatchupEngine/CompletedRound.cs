namespace CompetitionManager.MatchupEngine
{
    internal sealed class CompletedRound
    {
        public List<CompletedMatch> Matches { get; set; } = new List<CompletedMatch>();

        public int RoundNumber { get; set; }

        public bool RatingCalculationRequired { get; set; }
    }
}
