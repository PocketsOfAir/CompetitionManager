namespace CompetitionManager.MatchupEngine
{
    internal sealed class FieldDetails(string location, int fieldNumber) : IEquatable<FieldDetails>
    {
        public string Location { get; set; } = location;
        public int FieldNumber { get; set; } = fieldNumber;
        public bool Allocated { get; set; } = false;

        public bool Equals(FieldDetails? other)
        {
            if (other == null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Location == other.Location && FieldNumber == other.FieldNumber;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as FieldDetails);
        }

        public override int GetHashCode()
        {
            var locationHash = Location.GetHashCode();
            var intHash = FieldNumber.GetHashCode();
            return locationHash ^ intHash;
        }
    }
}
