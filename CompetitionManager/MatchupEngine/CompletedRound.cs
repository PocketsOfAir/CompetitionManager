using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitionManager.MatchupEngine
{
    internal sealed class CompletedRound
    {
        public List<CompletedMatch> Matches { get; set; } = new List<CompletedMatch>();

        public int RoundNumber { get; set; }

        public int RoundCost { get; set; }
    }
}
