﻿namespace CompetitionManager.Transport
{
    internal sealed class RoundSto
    {
        public List<RoundEntrySto> Matches { get; set; } = [];
        public string ByeTeam { get; set; } = string.Empty;
    }
}
