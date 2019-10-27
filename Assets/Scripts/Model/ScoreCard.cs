using System;

namespace Assets.Scripts
{
    [Serializable]
    public class ScoreCard
    {
        public string ownerName;
        public string playerID;
        public bool isPlayer;
        public int icon;
        //public int courseID;
        public int[] scores;
        public int[] diffFromPar;
        public int score;
        public string multiplayerStatus;
        //public int rating;
        public int totalDiffFromPar;
        //public bool evenPar;
    }
}
