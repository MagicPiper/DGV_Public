using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class DiscTestBehavior : PlayerBehavior
    {       

        public override void PlayerStart()
        { 
           // player.throwUI.DisableWind();
        }

        public override void ReadyToThrow()
        {
            //player.UI.readyToThrowButton.DisablePopOut();
            //Invoke("ReadyDelay", 1f);
        }

        public override void PlayerReady()
        {
            //player.throwUI.ShowThrowUI();
            //player.UI.ShowScoreBar();
            //player.UI.popUpText.ShowText("Select Disc", 3f);
            //player.UI.popUpText2.ShowText("", 0.1f);

            //SetScoreBar();
            //TargetPracticeShot();

            //player.throwUI.shotSelect.SelectShot(ShotSelector.ShotType.BackHand);
        }

        private void TargetPracticeShot()
        {
            //hit = false;            
            //sceneManager.SetTargetPosition(TargetPosition(), TargetSize());
            //player.throwUI.ShowThrowUI();
            //player.DistanceToBasket = Mathf.RoundToInt(Vector3.Distance(player.transform.position, player.sceneManager.Basket.basketPosition.position));
            //player.UI.scoreBar.UpdatePanel("Distance", player.gameState.playerSave.playerSettings.DistanceConverter(player.DistanceToBasket));
            //player.sceneManager.Basket.ShowBasketIcon(player.DistanceToBasketString,player.DistanceToBasket, player.transform);
            //player.discSelect.ShowQuickSelect(player.DistanceToBasket);
        }

        internal override void ChangeDisc(Disc disc)
        {
            if (player.curentDisc)
            {
                player.curentDiscScript.RemoveDisc();
            }

            var d = Instantiate(player.discPrefab, player.discHolder.transform);
            player.curentDisc = d;
            player.curentDiscScript = d.GetComponent<DiscBehavior>();
            player.curentDiscScript.player = player;
            player.curentDiscScript.PopulateDisc(disc);
            player.curentDiscScript.isMenu = true;
            player.curentDiscScript.isDiscTester = true;
        }

        public override void Throw(float power)
        {           
            player.curentDiscScript.Throw(power, 0f);
        }
               
        internal override void CompletedThrow(float throwDistance, Vector3 position)
        {
            var sc = player.sceneManager as DiscTesterSceneManager;
            sc.distanceText.text = "Distance: " + Mathf.Round(throwDistance) + " m";
        }

        public override void ThrowAgain()
        {
            //player.playerCameraScript.DiscCam = false;
            //// player.distanceToBasket = Vector3.Distance(player.transform.position, player.sceneManager.Basket.basketPosition.position);           
            //// player.throwUI.ShowThrowUI();
            ////DiscFlightCamera.isTracking = null;
            //player.throwUI.panPanel.ResetPos();
            //transform.rotation = sceneManager.transform.rotation;

            //player.rotateAroundPoint.localRotation = Quaternion.identity;
            //player.rotateAroundPoint.position = transform.position;

            //player.playerCamera.transform.position = player.mainCam.position;
            //player.playerCamera.transform.rotation = player.mainCam.rotation;

            //player.curentDiscScript.RemoveDisc();
            //player.UI.popUpText.ShowText("Select Disc", 3f);

            //TargetPracticeShot();
        }
    }
}

