using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class ProTourResult
    {
        public int position;
        public int score;
        public int participants;
        public bool completeRound;
        public ProTourTop3[] top3;
        public int parDiff;
        public ProTourWrapper.Division division;
    }

    [Serializable]
    public class ProTourTop3
    {
        public string playerName;
        public int score;
    }
}
