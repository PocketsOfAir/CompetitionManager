using CompetitionManager.Transport;

namespace CompetitionManager.MatchupEngine
{
    internal sealed class CompetitionDetails
    {
        public DateTime StartDate { get; set; }

        public int GameLength { get; set; }
        public int StartingRound { get; set; }

        public string CompetitionName { get; set; } = string.Empty;

        public CompetitionMode Mode { get; set; }

        public List<FieldDetails> Fields { get; set; } = [];

        public static CompetitionDetails CreateFromSto(CompetitionDetailsSto details)
        {
            var valid = DateTime.TryParse(details.StartDate, out var startDate);
            valid &= Enum.TryParse<CompetitionMode>(details.Mode, out var mode);
            var startingRound = details.StartingRound ?? 1;
            if (startingRound > 1)
            {
                startDate = startDate.AddDays(7 * (startingRound - 1));
            }

            if (valid)
            {
                var output = new CompetitionDetails
                {
                    CompetitionName = details.CompetitionName,
                    StartDate = startDate,
                    GameLength = details.GameLength,
                    Mode = mode,
                    StartingRound = startingRound,
                };
                output.LoadFieldsFromLocations(details.Locations);
                return output;
            }
            else
            {
                throw new InvalidDataException("Invalid Competition Details");
            }
        }

        private void LoadFieldsFromLocations(List<LocationSto> locations)
        {
            foreach (var location in locations)
            {
                foreach (var field in location.Fields)
                {
                    Fields.Add(new FieldDetails(location.Name, field));
                }
            }
        }
    }
}
