using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class ProTourScore
    {
        public string playerName;
        public string playerID;
        public int icon;
        public int[] scores;
        public int[] pars;       
    }

    [Serializable]
    public class ProTourScoreSortable
    {
        public string playerName;
        public string playerID;
        public int icon;
        public int[] scores;
        public int[] pars;
        public int completedRounds;
        public int totalScore;
    }
}
