using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class PracticeRoundManager : RoundManager
    {
        public PracticeMode.PracticeType type;
        public int energy;
        public int score;
        public int difficultyLevel;
        public List<Score> highScores;
        public bool gotHighScores;

        public void StartPracticeRound(PracticeMode.PracticeType mode)
        {
            type = mode;
            energy = 3;
            score = 0;
            difficultyLevel = 0;
            canRestart = true;
        }

        internal override void EndRound()
        {
            roundComplete = true;
            playerSave.FirebaseManager.SubmitPracticeScore(score, type);
        }

        internal void GetHighscores()
        {
            playerSave.FirebaseManager.GetPracticeHighscores(type, this);
        }

        internal void EnergyChange(int energy)
        {
            this.energy += energy;            
        }

        internal void ScoreChange(int score)
        {
            this.score += score;            
        }

        internal override void Restart()
        {
            SceneManager.LoadScene(type.ToString());
            StartPracticeRound(type);
        }

        internal void GotHighScores(List<Score> s)
        {
            highScores = s;
            gotHighScores = true;
        }
    }
}