using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class PuttingPracticeBehavior : PlayerBehavior
    {
        private PracticeRoundManager roundManager;
        private int currentDistance;
        private int basketIndex = 0;
        private PuttingPracticeSceneManager sceneManager;

        public override void PlayerStart()
        {
            roundManager = (PracticeRoundManager)player.gameState.roundManager;
            sceneManager = (PuttingPracticeSceneManager)player.sceneManager;
            player.UI.readyToThrowButton.buttonText.text = "Ready!";
            player.sceneManager.MiniMap.HideMiniMap();
            player.throwUI.DisableWind();
        }

        public override void ReadyToThrow()
        {
            player.UI.readyToThrowButton.DisablePopOut();
            Invoke("ReadyDelay", 1f);
        }

        public override void ReadyDelay()
        {
            PlayerReady();
        }

        public override void PlayerReady()
        {
            player.throwUI.ShowThrowUI();
            player.UI.ShowScoreBar();
            player.UI.popUpText.ShowText("Select Disc", 3f);
            player.UI.popUpText2.ShowText("", 0.1f);

            SetScoreBar();
            PuttingRound();

            // player.distanceToBasket = Vector3.Distance(player.transform.position, player.sceneManager.Basket.basketPosition.position);

            player.throwUI.shotSelect.SelectShot(ShotSelector.ShotType.Putt);
        }

        public void PuttingRound()
        {
            if (player.curentDiscScript != null)
            {
                player.action.ChangeDisc(player.curentDiscScript.disc);
            }

            currentDistance = PuttDistance();
            sceneManager.SetBasketPosition(PuttBasket());

            var pos = sceneManager.Basket.basketPosition.position;
            var random = UnityEngine.Random.insideUnitCircle.normalized * currentDistance;

            Vector3 puttingPos = new Vector3(random.x, 0, random.y) + pos;

            var terrainHeight = player.sceneManager.Terrain.terrain.SampleHeight(puttingPos);

            Vector3 puttingPosFinal = new Vector3(puttingPos.x, terrainHeight, puttingPos.z);

            player.transform.position = puttingPosFinal;

            player.transform.LookAt(pos);
            player.throwUI.ShowThrowUI();
            player.throwUI.panPanel.ResetPos();
            player.rotateAroundPoint.localRotation = Quaternion.identity;
            player.rotateAroundPoint.position = transform.position;

            player.DistanceToBasket = Mathf.RoundToInt(Vector3.Distance(player.transform.position, player.sceneManager.Basket.basketPosition.position));
            player.UI.scoreBar.UpdatePanel("Distance", player.gameState.playerSave.playerSettings.DistanceConverter(player.DistanceToBasket));
        }

        public override void Throw(float power)
        {
            base.Throw(power);
            player.playerCameraScript.DiscCam = false;
        }

        private Transform PuttBasket()
        {
            Transform basket = sceneManager.puttingBasketPositions[basketIndex];

            if (basketIndex >= sceneManager.puttingBasketPositions.Length - 1)
            {
                basketIndex = 0;
                roundManager.difficultyLevel += 1;
                roundManager.EnergyChange(+1);
                player.UI.popUpText.ShowText("Energy+1", 2f, Color.green);
                player.UI.scoreBar.UpdatePanel("Energy", "Energy:", roundManager.energy.ToString());
            }
            else
            {
                basketIndex++;
            }

            return basket;
        }

        private int PuttDistance()
        {
            return roundManager.difficultyLevel + sceneManager.startingDistance;
        }

        internal override void CompletedThrow(float throwDistance, Vector3 position)
        {
            StopCoroutine(replayRecorder);

            var hit = false;
            if (player.sceneManager.Basket.insideBasketCollider.bounds.Contains(position))
            {
                player.UI.popUpText2.ShowText("Score+1", 2f, Color.yellow);

                roundManager.ScoreChange(1);
                hit = true;
            }
            else
            {
                player.UI.popUpText2.ShowText("Energy-1", 2f, Color.red);
                roundManager.EnergyChange(-1);
                player.lie = position;
                player.DistanceToBasket = Vector3.Distance(player.lie, sceneManager.Basket.basketPosition.position);
            }

            player.UI.scoreBar.UpdatePanel("Score", "Score:", roundManager.score.ToString());
            player.UI.scoreBar.UpdatePanel("Energy", "Energy:", roundManager.energy.ToString());

            if (roundManager.energy < 1)
            {
                roundManager.EndRound();
                StartCoroutine("EndRound");
            }
            else if (hit)
            {
                Invoke("PuttingRound", 1.5f);
            }

            else if (player.DistanceToBasket < 2f)
            {
                player.UI.popUpText.ShowText("That's parked. Just tap it in.", 3f);
                player.UI.tapInButton.EnablePopIn();
            }
            else
            {
                player.UI.moveToLieButton.EnablePopIn();
                player.UI.throwAgainButton.EnablePopIn();
            }
        }

        private IEnumerator EndRound()
        {
            yield return new WaitForSeconds(2f);
            roundManager.GetHighscores();
            player.ShowPracticeScoreScreen();
            yield return null;
        }

        public override void TapIn()
        {
            player.UI.tapInButton.DisablePopOut();
            player.UI.popUpText2.ShowText("Score+1", 2f, Color.yellow);
            roundManager.ScoreChange(1);

            Invoke("PuttingRound", 1.5f);
        }

        public override void SetScoreBar()
        {
            player.UI.scoreBar.AddPanel("Energy", "Energy:", "3");
            player.UI.scoreBar.AddPanel("Score", "Score:", "0");
            player.UI.scoreBar.AddPanel("Distance", "Distance to\n Basket:", currentDistance.ToString() + "m", true);
        }
    }
}

