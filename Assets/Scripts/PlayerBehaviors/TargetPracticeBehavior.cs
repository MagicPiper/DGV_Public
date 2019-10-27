using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class TargetPracticeBehavior : PlayerBehavior
    {
        private PracticeRoundManager roundManager;
        private float currentSize = 1.1f;
        private int targetIndex = 0;
        private int sizeIndex = 0;
        private TargetPracticeSceneManager sceneManager;
        public bool hit;

        public override void PlayerStart()
        {
            roundManager = (PracticeRoundManager)player.gameState.roundManager;
            sceneManager = (TargetPracticeSceneManager)player.sceneManager;
            player.UI.readyToThrowButton.buttonText.text = "Ready!";            
            player.sceneManager.MiniMap.HideMiniMap();
            player.throwUI.DisableWind();
        }

        public override void ReadyToThrow()
        {
            player.UI.readyToThrowButton.DisablePopOut();
            Invoke("ReadyDelay", 1f);
        }

        public override void PlayerReady()
        {
            player.throwUI.ShowThrowUI();
            player.UI.ShowScoreBar();
            player.UI.popUpText.ShowText("Select Disc", 3f);
            player.UI.popUpText2.ShowText("", 0.1f);

            SetScoreBar();
            TargetPracticeShot();

            player.throwUI.shotSelect.SelectShot(ShotSelector.ShotType.BackHand);
        }

        private void TargetPracticeShot()
        {
            hit = false;            
            sceneManager.SetTargetPosition(TargetPosition(), TargetSize());
            player.throwUI.ShowThrowUI();
            player.DistanceToBasket = Mathf.RoundToInt(Vector3.Distance(player.transform.position, player.sceneManager.Basket.basketPosition.position));
            player.UI.scoreBar.UpdatePanel("Distance", player.gameState.playerSave.playerSettings.DistanceConverter(player.DistanceToBasket));
            player.sceneManager.Basket.ShowBasketIcon(player.DistanceToBasketString,player.DistanceToBasket, player.transform);
            player.discSelect.ShowQuickSelect(player.DistanceToBasket);
        }

        private float TargetSize()
        {
            if (sizeIndex % 7 == 0)
            {
                currentSize *= 0.8f;
            }

            sizeIndex++;
            return currentSize;
        }

        private Vector3 TargetPosition()
        {
            if (targetIndex >= sceneManager.targetPracticeAreas.Length)
            {
                targetIndex = 0;
            }
            var target = sceneManager.targetPracticeAreas[targetIndex];
            targetIndex++;

            var random = UnityEngine.Random.insideUnitCircle * 10;

            Vector3 targetPos = new Vector3(random.x, 0, random.y) + target.position;
            var terrainHeight = player.sceneManager.Terrain.terrain.SampleHeight(targetPos);
            Vector3 targetPosFinal = new Vector3(targetPos.x, terrainHeight, targetPos.z);

            return targetPosFinal;
        }

        internal void HitTarget()
        {
            hit = true;
        }

        private int ShotDistance()
        {
            return roundManager.difficultyLevel + 5;
        }

        internal override void CompletedThrow(float throwDistance, Vector3 position)
        {
            StopCoroutine(replayRecorder);

            if (hit)
            {
                player.UI.popUpText2.ShowText("Score+1", 2f);
                player.UI.popUpText.ShowText("Energy+1", 2f, Color.green);

                roundManager.ScoreChange(1);
                roundManager.EnergyChange(1);
            }
            else
            {
                player.UI.popUpText2.ShowText("Energy-1", 2f, Color.red);
                roundManager.EnergyChange(-1);
            }
            if (roundManager.energy < 1)
            {
                roundManager.EndRound();
                StartCoroutine("EndRound");
            }
            else
            {
                Invoke("ThrowAgain", 1.5f);
            }

            player.UI.scoreBar.UpdatePanel("Score", "Score:", roundManager.score.ToString());
            player.UI.scoreBar.UpdatePanel("Energy", "Energy:", roundManager.energy.ToString());
        }

        private IEnumerator EndRound()
        {
            yield return new WaitForSeconds(2f);
            roundManager.GetHighscores();           
            player.ShowPracticeScoreScreen();
            yield return null;
        }

        public override void ThrowAgain()
        {
            player.playerCameraScript.DiscCam = false;
            // player.distanceToBasket = Vector3.Distance(player.transform.position, player.sceneManager.Basket.basketPosition.position);           
            // player.throwUI.ShowThrowUI();
            //DiscFlightCamera.isTracking = null;
            player.throwUI.panPanel.ResetPos();
            transform.rotation = sceneManager.transform.rotation;

            player.rotateAroundPoint.localRotation = Quaternion.identity;
            player.rotateAroundPoint.position = transform.position;

            player.playerCamera.transform.position = player.mainCam.position;
            player.playerCamera.transform.rotation = player.mainCam.rotation;

            player.curentDiscScript.RemoveDisc();
            player.UI.popUpText.ShowText("Select Disc", 3f);

            TargetPracticeShot();
        }

        public override void SetScoreBar()
        {
            player.UI.scoreBar.AddPanel("Energy", "Energy:", "3");
            player.UI.scoreBar.AddPanel("Score", "Score:", "0");
            player.UI.scoreBar.AddPanel("Distance", "Distance to\n Basket:", 50 + "m", true);
        }
    }
}

