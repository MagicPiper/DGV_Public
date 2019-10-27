using System;

namespace Assets.Scripts
{
    [Serializable]
    public class MultiPlayerScoreCard
    {
        public string on;  //ownerName    
        public int i;    //icon
        public int[] s;    //scores
        public int t;  //total Score
        public string ms; //Multiplayer Status
    }
}
