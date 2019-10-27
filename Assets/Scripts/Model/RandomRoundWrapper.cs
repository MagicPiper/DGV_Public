using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class RandomRoundWrapper
    {
        public List<Hole> holes;
        public List<int> windSeeds;
        public string status;
        public string tournamentKey;
    }
}
