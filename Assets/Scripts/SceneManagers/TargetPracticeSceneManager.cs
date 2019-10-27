using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class TargetPracticeSceneManager : DGSceneManager
    {
        [SerializeField] public Transform[] targetPracticeAreas;
        [SerializeField] public GameState gameState;
        [SerializeField] public PracticeMode testMode;            
        [SerializeField] public bool testing;
        [SerializeField] public TargetPracticeTarget targetSphere;

        protected override void SetPlayerBehavior(GameObject player)
        {
            if (testing)
            {
                gameState.NewPracticeRound().StartPracticeRound(testMode.type); //FOR TEST
            }

            playerScript.action = player.AddComponent<TargetPracticeBehavior>();           
        }
              

        internal void TargetHit()
        {
            var script = (TargetPracticeBehavior)playerScript.action;
            script.HitTarget();
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

        internal void SetTargetPosition(Vector3 position, float size)
        {
            basket.transform.position = position;
            targetSphere.gameObject.SetActive(true);
            targetSphere.ResetColor();
            targetSphere.transform.localScale = new Vector3(10,10,10) * size;
        }
    }
}
