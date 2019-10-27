using UnityEngine;

namespace Assets.Scripts
{
    public class PuttingPracticeSceneManager : DGSceneManager
    {
        [SerializeField] public Transform[] puttingBasketPositions;
        [SerializeField] public GameState gameState;
        [SerializeField] public PracticeMode testMode;            
        [SerializeField] public int startingDistance;
        [SerializeField] public bool testing;

        protected override void SetPlayerBehavior(GameObject player)
        {
            if (testing)
            {
                gameState.NewPracticeRound().StartPracticeRound(testMode.type); //FOR TEST
            }

            playerScript.action = player.AddComponent<PuttingPracticeBehavior>();
        }

        public void SetBasketPosition(Transform position)
        {
            basket.transform.position = position.position;
        }

        protected override void InitScene()
        {
            if (hasWater)
            {
                playerScript.waterDepthScript.Set();
            }

            Wind();
           
            playerScript.BirbController.StartAmbienceSounds(currentWind, birbs);
            playerScript.InitPlayer(currentWind, windDirection);
        }
    }
}
