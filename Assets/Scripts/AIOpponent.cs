using System;

namespace Assets.Scripts
{
    [Serializable]
    public class AIOpponent
    {
        public string opponentName;
        public ScoreCard opponentScoreCard;
                
        public AIOpponent()
        {
            opponentName = RandomNameGenerator.NameGenerator.GenerateFirstName();
            opponentScoreCard = new ScoreCard();
        }
    }
}