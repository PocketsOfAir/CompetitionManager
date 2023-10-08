using CompetitionManager.Transport;

namespace CompetitionManager.MatchupEngine
{
    internal class CompetitionDetails
    {
        public DateTime StartDate { get; set; }

        public int GameLength { get; set; }

        public string Location { get; set; } = string.Empty;

        public static CompetitionDetails CreateFromSto(CompetitionDetailsSto details)
        {
            var valid = int.TryParse(details.GameLength, out var gameLength);
            valid &= DateTime.TryParse(details.StartDate, out var startDate);
            if (valid)
            {
                var output = new CompetitionDetails
                {
                    Location = details.Location,
                    StartDate = startDate,
                    GameLength = gameLength,
                };
                return output;
            }
            else
            {
                throw new InvalidDataException("Invalid Competition Details");
            }
        }
    }
}
