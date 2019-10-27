using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class MultiplayerGame
    {
        public string status;
        public string gameID;
        public long startTime;
        public int position;
        public int ratingChange;
        public bool completedGame;
        public List<Hole> holes;
        
    }
}