using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{

    public class PracticeScoreScreen : MonoBehaviour
    {
        public CanvasGroup screen;
        public ScoreLine scoreLinePrefab;
        public Transform highScoreHolder;
        public TMP_Text practiceModeText;

        public GameState gameState;
        public PracticeRoundManager roundManager;

        [SerializeField] private TMP_Text scoreText;
        private bool highScoresPopulated = false;

        public void ShowPracticeScoreScreen()
        {
            roundManager = (PracticeRoundManager)gameState.roundManager;
            PracticeScore();
        }

        private void PracticeScore()
        {
            screen.alpha = 0;
            LeanTween.alphaCanvas(screen, 1f, 1.5f).setEaseOutExpo().setDelay(0.2f);
            practiceModeText.text = gameState.roundManager.roundName;
            scoreText.text = roundManager.score.ToString();
        }

        private void Update()
        {
            if (!highScoresPopulated && roundManager.gotHighScores)
            {
                highScoresPopulated = true;
                int pos = 1;
                roundManager.highScores.Reverse();

                foreach (Score score in roundManager.highScores)
                {
                    var isMe = false;
                    var s = Instantiate(scoreLinePrefab, highScoreHolder);
                    if (score.userid == gameState.playerSave.FirebaseManager.playerID)
                    {
                        isMe = true;
                    }
                    s.Populate(pos, score.userName, score.score, isMe, 0, "", score.playerIcon);
                    pos++;
                }
            }
        }

        public void TryAgain()
        {
            gameState.NewPracticeRound().StartPracticeRound(roundManager.type);

            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadSceneAsync(scene.name);
        }

        public void Retire()
        {
            gameState.roundManager.Retire();
        }
    }
}
