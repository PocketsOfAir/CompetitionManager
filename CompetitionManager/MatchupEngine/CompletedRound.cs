namespace CompetitionManager.MatchupEngine
{
    public sealed class CompletedRound
    {
        public List<CompletedMatch> Matches { get; set; } = [];

        public int RoundNumber { get; set; }

        public bool RatingCalculationRequired { get; set; }
    }
}
