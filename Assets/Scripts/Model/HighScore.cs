using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class ScoreCollection
    {        
        public List<Score> scores;
    }

    [Serializable]
    public class Score
    {
        public int score;
        public string userName;
        public string userid;
        public int playerIcon;
        public int currentRating;
    }

    [Serializable]
    public class OpenScore
    {
        public int score;
        public int parDiff;
        public string userName;
        public string userid;
        public int icon;
        public int proRating;        
    }

    [Serializable]
    public class StatsScore
    {
        public int score;
        public string roundID;
    }

    [Serializable]
    public class RoundRating
    {
        public float rating;
        public float weight;
    }
}