namespace CompetitionManager.MatchupEngine
{
    public sealed class FieldDetails(string location, int fieldNumber) : IEquatable<FieldDetails>
    {
        public string Location { get; } = location;
        public int FieldNumber { get; } = fieldNumber;
        public bool Allocated { get; private set; } = false;
        public bool AllocatedToManualMatch { get; private set; } = false;

        public void Allocate()
        {
            Allocated = true;
        }

        public void AllocateToManualMatch()
        {
            AllocatedToManualMatch = true;
        }

        public void Deallocate()
        {
            if (AllocatedToManualMatch == false)
            {
                Allocated = false;
            }
        }

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
