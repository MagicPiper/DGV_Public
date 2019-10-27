using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class UI : MonoBehaviour
    {
        public PopUpText popUpText;
        public PopUpText popUpText2;
        public ScoreBar scoreBar;
        public ButtonBehavior retireButton;
        public GameObject restartButton;
        public ButtonBehavior readyToThrowButton;
        public ButtonBehavior moveToLieButton;
        public ButtonBehavior throwAgainButton;
        public ButtonBehavior replayButton;
        public ButtonBehavior tapInButton;
        public CanvasGroup backPane;
        public TMP_Text retireText;
        private bool backPaneOpen;
        public Player playerScript;
        public GameState gameState;

        // [SerializeField]private GameObject debugGraphs;
        private TutorialCanvas tutorialCanvas;
        [SerializeField]private TutorialCanvas tutorialCanvasPrefab;

        internal void ShowScoreBar()
        {
            scoreBar.GetComponent<ButtonBehavior>().SlideIn(ButtonBehavior.Direction.up, 1f, 0.5f);
            retireButton.GetComponent<ButtonBehavior>().SlideIn(ButtonBehavior.Direction.up, 1f, 0.5f);

        }

        private void Start()
        {
            scoreBar.gameObject.SetActive(false);
            retireButton.gameObject.SetActive(false);
            throwAgainButton.gameObject.SetActive(false);
            tapInButton.gameObject.SetActive(false);
            moveToLieButton.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (backPaneOpen)
                {
                    Retire();
                }
                else
                {
                    ShowBackPane();
                }
            }
        }

        public void ShowBackPane()
        {
            var text = "Retire";
            if(gameState.roundManager is MultiplayerRoundManager)
            {
                text = "Retire (-10 rating)";
            }

            if (gameState.roundManager.canRestart)
            {
                restartButton.SetActive(true);              
            }
            else
            {
                restartButton.SetActive(false);
            }

            retireText.text = text;

            backPane.gameObject.SetActive(true);
            backPane.alpha = 0f;

            LeanTween.alphaCanvas(backPane, 1f, 1.5f).setEaseOutExpo().setDelay(0.2f);
            backPane.transform.SetAsLastSibling();
        }

        public void HideBackPane()
        {
            LeanTween.alphaCanvas(backPane, 0f, 0.5f).setEaseInSine().setOnComplete(() =>
            {
                backPane.gameObject.SetActive(false);
            });
        }

        public void Retire()
        {
            gameState.roundManager.Retire();
        }

        public void Restart()
        {
            gameState.roundManager.Restart();
        }

        public TutorialCanvas Tutorial()
        {
            if (tutorialCanvas == null)
            {
               tutorialCanvas= Instantiate(tutorialCanvasPrefab);
            }
            return tutorialCanvas;
        }

        public void ReadyToThrow()
        {
            playerScript.action.ReadyToThrow();
        }

        public void MoveToLie()
        {
            playerScript.MoveToLie();            
        }

        public void ThrowAgain()
        {
            playerScript.ThrowAgain();
        }

        public void TapIn()
        {
            tapInButton.enabled = false;
            playerScript.TapIn();
        }

        public void PlayReplay()
        {
            playerScript.PlayReplay();
        }
    }
}