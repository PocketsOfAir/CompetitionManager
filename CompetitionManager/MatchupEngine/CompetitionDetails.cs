using CompetitionManager.Transport;

namespace CompetitionManager.MatchupEngine
{
    internal sealed class CompetitionDetails
    {
        public DateTime StartDateTime { get; set; }

        public DateOnly StartDate { get; set; }

        public List<TimeOnly> Timeslots { get; set; } = [];

        public int GameLength { get; set; }

        public string CompetitionName { get; set; } = string.Empty;

        public CompetitionMode Mode { get; set; }

        public List<FieldDetails> Fields { get; set; } = [];

        public static CompetitionDetails CreateFromSto(CompetitionDetailsSto details)
        {
            var valid = int.TryParse(details.GameLength, out var gameLength);
            valid &= Enum.TryParse<CompetitionMode>(details.Mode, out var mode);

            if (!valid)
            {
                throw new InvalidDataException("Invalid Competition Details");
            }

            var output = new CompetitionDetails
            {
                CompetitionName = details.CompetitionName,
                GameLength = gameLength,
                Mode = mode,
            };
            output.LoadFieldsFromLocations(details.Locations);

            if (details.Timeslots.Count == 0)
            {
                valid &= DateTime.TryParse(details.StartDate, out var startDate);
                output.StartDateTime = startDate;
                output.StartDate = DateOnly.FromDateTime(startDate);
                output.Timeslots.Add(TimeOnly.FromDateTime(startDate));
            }
            else
            {
                valid &= DateTime.TryParse(details.StartDate, out var startDate);
                output.StartDate = DateOnly.FromDateTime(startDate);
                foreach (var timeslot in details.Timeslots)
                {
                    valid &= TimeOnly.TryParse(timeslot, out var startTime);
                    output.Timeslots.Add(startTime);
                }
                output.StartDateTime = new DateTime(output.StartDate, output.Timeslots[0]);
            }

            if (!valid)
            {
                throw new InvalidDataException("Invalid Competition Details");
            }

            return output;
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
