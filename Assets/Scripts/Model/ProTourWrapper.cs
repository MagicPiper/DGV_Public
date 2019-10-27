using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class ProTourWrapper
    {
        public enum Division { Recreational = 0, Advanced = 1, Pro = 2, Intermediate = 3, Test = 99 }

        public int week;
        public Division division;
        public List<ProTourRound> rounds;
        public bool closed;
        public bool prizeWeek;
    }
}
