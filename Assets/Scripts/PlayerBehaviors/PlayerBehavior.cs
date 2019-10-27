using Assets.Scripts.Menu;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerBehavior : MonoBehaviour
    {
        public Player player;
        private HoleSceneManager holeManager;
        public Coroutine replayRecorder;
        private bool replayPlaying;

        public virtual void PlayerStart()
        {
            holeManager = (HoleSceneManager)player.sceneManager;
            player.playerCameraScript.StartDroneCam(holeManager);
        }

        public virtual void PlayerReady()
        {
            player.throwUI.ShowThrowUI();
            player.UI.ShowScoreBar();
            player.sceneManager.MiniMap.ShowMiniMap();

            player.UI.popUpText.ShowText("Select Disc", 3f);
            player.UI.popUpText2.ShowText("", 0.1f);

            player.DistanceToBasket = Vector3.Distance(player.transform.position, player.sceneManager.Basket.basketPosition.position);
            player.sceneManager.Basket.ShowBasketIcon(player.DistanceToBasketString, player.DistanceToBasket, player.transform);
            player.discSelect.ShowQuickSelect(player.DistanceToBasket);

            SetScoreBar();
            if (Menu.PlayerSave.playerGet)
            {
                var prefSHot = player.gameState.roundManager.currentHole.preferedShot;
                if (player.gameState.playerSave.playerSettings.settingsData.leftHandedMode)
                {
                    if (prefSHot == ShotSelector.ShotType.BackHand)
                    {
                        prefSHot = ShotSelector.ShotType.ForeHand;
                    }
                    else if (prefSHot == ShotSelector.ShotType.ForeHand)
                    {
                        prefSHot = ShotSelector.ShotType.BackHand;
                    }
                }

                player.throwUI.shotSelect.SelectShot(prefSHot);
                if (!player.gameState.playerSave.GetTutorialCheck().seenFirstDiscSelect)
                {
                    player.UI.Tutorial().SelectDisc();
                }
            }
            else
            {
                player.throwUI.shotSelect.SelectShot(ShotSelector.ShotType.BackHand);
                // player.UI.Tutorial().SelectDisc(player.throwUI.discBagButton);
            }
        }

        public virtual void SetScoreBar()
        {
            if (Menu.PlayerSave.playerGet) player.UI.scoreBar.AddPanel("HoleNumber", "Hole", player.gameState.roundManager.currentHoleNumber.ToString());
            if (Menu.PlayerSave.playerGet) player.UI.scoreBar.AddPanel("Par", "Par", player.gameState.roundManager.currentHole.par.ToString());
            player.UI.scoreBar.AddPanel("Strokes", "Strokes:", "0");
            player.UI.scoreBar.AddPanel("Distance", "Distance to\n Basket:", player.DistanceToBasketString, true);
        }

        public virtual void ReadyToThrow()
        {
            player.UI.readyToThrowButton.DisablePopOut();
            player.playerCameraScript.SkipFlight = true;
            Invoke("ReadyDelay", 1f);
        }

        public virtual void ReadyDelay()
        {
            player.playerCameraScript.DroneCam = false;
            PlayerReady();
        }

        internal virtual void ChangeDisc(Disc disc)
        {

            if (PlayerSave.playerGet && !player.gameState.playerSave.GetTutorialCheck().seenFirstDiscSelect)
            {
                player.UI.Tutorial().SelectDiscComplete();
            }

            if (player.curentDisc)
            {
                player.curentDiscScript.RemoveDisc();
            }

            var d = Instantiate(player.discPrefab, player.discHolder.transform);
            player.curentDisc = d;
            player.curentDiscScript = d.GetComponent<DiscBehavior>();
            player.curentDiscScript.player = player;
            player.curentDiscScript.PopulateDisc(disc);

            if (player.throwUI.shotSelect.SelectedShot == ShotSelector.ShotType.None)
            {
                player.throwUI.shotSelect.LastShotType();
            }
            player.sceneManager.MiniMap.playerDisc = player.curentDisc;
            player.action.UpdateReachBackPanel();

            if (Menu.PlayerSave.playerGet)
            {
                if (player.throwUI.shotSelect.SelectedShot == ShotSelector.ShotType.Putt)
                {
                    if (!player.gameState.playerSave.GetTutorialCheck().seenFirstPutt)
                    {
                        player.UI.Tutorial().BeforePutt();
                    }
                }
                else
                {
                    if (!player.gameState.playerSave.GetTutorialCheck().seenFirstThrow)
                    {
                        player.UI.Tutorial().BeforeThrow();
                    }
                }
            }
        }

        internal void PlayReplay()
        {
            if (player.discInBasket)
            {
                player.scoreScreen.gameObject.SetActive(false);
            }
            player.playerCameraScript.DiscCam = false;
            //DiscFlightCamera.isTracking = null;
            player.playerCamera.transform.position = player.mainCam.position;
            player.playerCamera.transform.rotation = player.mainCam.rotation;

            player.playerCamera.fieldOfView = 60;
            player.curentDiscScript.RemoveDisc();

            var d = Instantiate(player.discPrefab, player.discHolder.transform);
            player.curentDisc = d;
            player.curentDiscScript = d.GetComponent<DiscBehavior>();
            player.curentDiscScript.player = player;
            player.curentDiscScript.PopulateReplayDisc(player.replayThrow.disc);
            player.curentDisc.transform.rotation = player.replayThrow.rotation[0];

            player.sceneManager.MiniMap.playerDisc = player.curentDisc;
            player.sceneManager.MiniMap.UpdateDiscPosition(transform.position);

            player.sceneManager.MiniMap.trackFlight = true;
            player.UI.replayButton.DisablePopOut();
            StartCoroutine("ReplayThrow");
        }

        private IEnumerator ReplayThrow()
        {
            yield return new WaitForSeconds(1f);
            replayPlaying = true;
            player.curentDiscScript.throwSwish.pitch = 1f;
            player.curentDiscScript.throwSwish.volume = 0.6f;
            player.curentDiscScript.throwSwish.Play();

            if (player.throwUI.shotSelect.SelectedShot != ShotSelector.ShotType.Putt)
            {
                player.playerCameraScript.StartDiscFollowCam();
                player.curentDiscScript.speedLine.enabled = true;
            }
            int iter = 0;
            int frame = 0;
            int totalFrame = 0;
            Vector3 fromPos = player.replayThrow.position[frame];
            Vector3 toPos = player.replayThrow.position[frame + 1];
            Quaternion fromRot = player.replayThrow.rotation[frame];
            Quaternion toRot = player.replayThrow.rotation[frame + 1];
            Debug.Log("Start replay throw");
            player.curentDiscScript.isThrown = true;

            while (replayPlaying)
            {
                fromPos = player.replayThrow.position[frame];
                toPos = player.replayThrow.position[frame + 1];
                fromRot = player.replayThrow.rotation[frame];
                toRot = player.replayThrow.rotation[frame + 1];
                float lerp = (float)iter / 10f;

                if (iter >= 10)
                {

                    iter = 0;
                    frame++;
                }
                iter++;
                totalFrame++;

                player.curentDiscScript.rb.MovePosition(Vector3.Lerp(fromPos, toPos, lerp));
                player.curentDiscScript.rb.MoveRotation(Quaternion.Lerp(fromRot, toRot, lerp));
                player.curentDiscScript.rb.velocity = player.replayThrow.velocity[frame];

                if (frame <= player.replayThrow.position.Count - 3)
                {
                    yield return new WaitForFixedUpdate();
                }
                else
                {
                    Debug.Log("replay done");
                    player.UI.replayButton.EnablePopIn();
                    player.sceneManager.MiniMap.trackFlight = false;

                    replayPlaying = false;

                    if (player.discInBasket)
                    {
                        player.scoreScreen.gameObject.SetActive(true);
                    }
                }
            }
        }

        public virtual void Throw(float power)
        {
            player.sceneManager.Basket.HideBasketIcon();
            player.curentDiscScript.Throw(power, 0f);
            player.throwUI.reachbackPanel.ShowReachBackPanel(false);
            player.sceneManager.MiniMap.trackFlight = true;
            player.playerCameraScript.StartDiscFollowCam();
            StartReplayRecord();

            if (Menu.PlayerSave.playerGet)
            {
                if (!player.gameState.playerSave.GetTutorialCheck().seenFirstThrow)
                {
                    player.UI.Tutorial().ThrowComplete();
                }
            }
            //else
            //{
            //    player.UI.Tutorial().ThrowComplete();
            //}
        }

        internal void StartReplayRecord()
        {
            player.replayThrow = new ActionReplayThrow();

            replayRecorder = StartCoroutine("RecordReplay");
        }

        public IEnumerator RecordReplay()
        {
            var iter = 0;
            player.replayThrow.disc = player.curentDiscScript.disc;
            Debug.Log("Start Recording");
            player.replayThrow.position = new System.Collections.Generic.List<Vector3>();
            player.replayThrow.velocity = new System.Collections.Generic.List<Vector3>();
            player.replayThrow.rotation = new System.Collections.Generic.List<Quaternion>();
            player.replayThrow.position.Add(player.curentDisc.transform.position);
            player.replayThrow.rotation.Add(player.curentDisc.transform.rotation);
            player.replayThrow.velocity.Add(player.curentDiscScript.rb.velocity);

            while (true)
            {
                if (iter >= 10)
                {
                    player.replayThrow.position.Add(player.curentDisc.transform.position);
                    player.replayThrow.rotation.Add(player.curentDisc.transform.rotation);
                    player.replayThrow.velocity.Add(player.curentDiscScript.rb.velocity);

                    iter = 0;
                }
                iter++;
                yield return new WaitForFixedUpdate();
            }
        }

        internal virtual void CompletedThrow(float throwDistance, Vector3 position)
        {
            Debug.Log("stop recorder");
            StopCoroutine(replayRecorder);

            player.strokes++;
            player.UI.scoreBar.UpdatePanel("Strokes", player.strokes.ToString());
            player.sceneManager.MiniMap.trackFlight = false;
            if (player.missedMando)
            {
                player.lie = MissedMando();
                player.strokes++;
                player.UI.scoreBar.UpdatePanel("Strokes", player.strokes.ToString());
            }
            else if (player.outOfBounds || player.outOfBoundsWater)
            {
                player.lie = OutOfBounds();
                player.strokes++;
                player.UI.scoreBar.UpdatePanel("Strokes", player.strokes.ToString());
            }
            else
            {
                player.lie = position;
                if (Menu.PlayerSave.playerGet && player.gameState.playerSave.playerStats.LongestDrive((int)throwDistance))
                {
                    player.UI.popUpText2.ShowText("New Longest Drive!", 3f, Color.yellow);
                }
            }

            if (player.sceneManager.Basket.insideBasketCollider.bounds.Contains(position))
            {
                DiscInBasket();
            }
            else if (Vector3.Distance(player.lie, player.sceneManager.Basket.basketPosition.position) < 3f)
            {
                player.UI.popUpText.ShowText("That's parked. Just tap it in.", 3f);
                player.UI.tapInButton.EnablePopIn();
                player.UI.replayButton.EnablePopIn();
            }
            else
            {
                if (player.missedMando)
                {
                    player.UI.popUpText2.ShowText("Missed the Mando!", 3f, Color.red);
                    player.UI.popUpText.ShowText(player.gameState.playerSave.playerSettings.DistanceConverter(throwDistance), 3f);
                    player.outOfBounds = false;
                    player.outOfBoundsWater = false;
                    player.missedMando = false;
                }
                else if (player.outOfBounds || player.outOfBoundsWater)
                {
                    player.UI.popUpText2.ShowText("Out of Bounds!", 3f, Color.red);
                    player.UI.popUpText.ShowText(player.gameState.playerSave.playerSettings.DistanceConverter(throwDistance), 3f);
                    player.outOfBounds = false;
                    player.outOfBoundsWater = false;
                }
                else
                {
                    player.UI.popUpText.ShowText(ShotString(throwDistance), 3f);
                }

                player.UI.moveToLieButton.EnablePopIn();
                player.UI.throwAgainButton.EnablePopIn();
                player.UI.replayButton.EnablePopIn();
            }
        }

        private string ShotString(float throwDistance)
        {
            var shotString = "";
            if (player.DistanceToBasket < 15f)
            {
                shotString = player.missedPutt[UnityEngine.Random.Range(0, player.missedPutt.Length)];
            }
            else if (throwDistance > 90f)
            {
                shotString = player.goodLongShot[UnityEngine.Random.Range(0, player.goodLongShot.Length)] + " " + player.gameState.playerSave.playerSettings.DistanceConverter(throwDistance) + "!";

            }
            else if (Vector3.Distance(player.lie, player.sceneManager.Basket.basketPosition.position) < player.DistanceToBasket)
            {
                shotString = player.goodShot[UnityEngine.Random.Range(0, player.goodShot.Length)] + " " + player.gameState.playerSave.playerSettings.DistanceConverter(throwDistance);
            }
            else
            {
                player.gameState.playerSave.playerSettings.DistanceConverter(throwDistance);
            }

            return shotString;
        }

        private Vector3 OutOfBounds()
        {
            var obPosition = new Vector3();
            if (player.sceneManager.OBDropZone)
            {
                if (player.sceneManager.AltDropZoneposition != null &&
                    Vector3.Distance(player.outOfBoundsPosition, player.sceneManager.AltDropZoneposition.position) < Vector3.Distance(player.outOfBoundsPosition, player.sceneManager.OBDropZoneposition.position))
                {
                    obPosition = player.sceneManager.AltDropZoneposition.position;
                }
                else
                {
                    obPosition = player.sceneManager.OBDropZoneposition.position;
                }

                player.sceneManager.MiniMap.UpdateDiscPosition(obPosition);
            }
            else
            {
                var yPos = player.sceneManager.Terrain.terrain.SampleHeight(player.outOfBoundsPosition);
                obPosition = new Vector3(player.outOfBoundsPosition.x, yPos + 1, player.outOfBoundsPosition.z);
                player.sceneManager.MiniMap.UpdateDiscPosition(obPosition);
            }

            return obPosition;
        }

        private Vector3 MissedMando()
        {
            var yPos = player.sceneManager.Terrain.terrain.SampleHeight(player.mandoPosition);
            var mandoP = new Vector3(player.mandoPosition.x, yPos + 1, player.mandoPosition.z);
            player.sceneManager.MiniMap.UpdateDiscPosition(mandoP);

            return mandoP;
        }

        public virtual void TapIn()
        {
            player.DistanceToBasket = 0f;
            player.UI.tapInButton.DisablePopOut();
            player.strokes++;
            player.UI.scoreBar.UpdatePanel("Strokes", player.strokes.ToString());
            DiscInBasket();
        }

        public virtual void DiscInBasket()
        {
            var score = 3;
            if (Menu.PlayerSave.playerGet)
            {

                score = player.strokes - player.gameState.roundManager.currentHole.par;
            }
            string text1;

            var text2 = score.ToString();
            Color color = Color.white;

            switch (score)
            {
                case -3:
                    text1 = "Albatross! Seriously?!?";
                    color = Color.blue;
                    if (Menu.PlayerSave.playerGet) player.gameState.playerSave.playerStats.Albatross();
                    break;
                case -2:
                    text1 = "Nice Eagle!";
                    color = Color.blue;
                    if (Menu.PlayerSave.playerGet) player.gameState.playerSave.playerStats.Eagle();
                    break;
                case -1:
                    text1 = "Nice Birdie!";
                    color = Color.green;
                    break;
                case 0:
                    text1 = "In for Par!";
                    color = Color.white;
                    text2 = "";
                    break;
                case 1:
                    text1 = "Bogey";
                    color = Color.red;
                    text2 = "+" + text2;
                    break;
                case 2:
                    text1 = "Double Bogey";
                    color = Color.red;
                    text2 = "+" + text2;
                    break;
                case 3:
                    text1 = "Triple Bogey";
                    color = Color.red;
                    text2 = "+" + text2;
                    break;
                default:
                    text1 = "";
                    color = Color.red;
                    text2 = "+" + text2;
                    break;
            }

            if (Menu.PlayerSave.playerGet && player.strokes == 1)
            {
                player.gameState.playerSave.playerStats.Ace();
                text2 = "ACE!";
                color = Color.yellow;
            }

            if (Menu.PlayerSave.playerGet && player.throwUI.shotSelect.SelectedShot == ShotSelector.ShotType.Putt && player.gameState.playerSave.playerStats.LongestPutt((int)Math.Round(player.DistanceToBasket)))
            {
                text2 = "New Longest Putt! " + player.gameState.playerSave.playerSettings.DistanceConverter(player.DistanceToBasket);
                color = Color.yellow;
            }

            player.UI.popUpText.ShowText(text1, 2.5f);
            player.UI.popUpText2.ShowText(text2, 2.5f, color);

            if (Menu.PlayerSave.playerGet && !player.discInBasket)
            {
                player.gameState.roundManager.HoleComplete(player.strokes, player.gameState.roundManager.currentHole.holeScene);
            }
            player.discInBasket = true;

            Invoke("ShowScoreScreen", 2.5f);
        }

        private void ShowScoreScreen()
        {
            player.UI.replayButton.EnablePopIn();
            
            player.ShowScoreScreen();
        }

        public void MoveToLie()
        {
            replayPlaying = false;
            player.sceneManager.MiniMap.trackFlight = false;
            player.DistanceToBasket = Vector3.Distance(player.lie, player.sceneManager.Basket.basketPosition.position);
            player.playerCamera.transform.SetParent(null);
            player.playerCameraScript.DiscCam = false;
            player.UI.moveToLieButton.DisablePopOut();
            player.UI.throwAgainButton.DisablePopOut();
            player.UI.replayButton.DisablePopOut();
            player.throwUI.ShowThrowUI();
            player.throwUI.panPanel.ResetPos();
            player.rotateAroundPoint.localRotation = Quaternion.identity;
            player.rotateAroundPoint.position = transform.position;

            transform.position = player.lie;
            transform.LookAt(player.sceneManager.Basket.basketPosition);
            player.playerCameraScript.MoveToLieCam();
            player.curentDiscScript.RemoveDisc();
            player.curentDisc = null;
            player.UI.scoreBar.UpdatePanel("Distance", player.gameState.playerSave.playerSettings.DistanceConverter(player.DistanceToBasket));
            player.windManager.UpdateWindPosition();
            player.throwUI.windIndicator.UpdateRotation(transform.rotation);

            ClearObstructions(player.mainCam.transform.position);

            if (player.DistanceToBasket < 15f)
            {
                player.UI.popUpText.ShowText("You've got a Putt!\n Select a disc.", 3f);
                player.throwUI.shotSelect.SelectShot(ShotSelector.ShotType.Putt);
            }
            else
            {
                player.sceneManager.Basket.ShowBasketIcon(player.DistanceToBasketString, player.DistanceToBasket, player.transform);
                player.UI.popUpText.ShowText("Select Disc", 3f);
                player.throwUI.shotSelect.SelectShot(ShotSelector.ShotType.BackHand);
            }
            player.discSelect.ShowQuickSelect(player.DistanceToBasket);
        }

        private void ClearObstructions(Vector3 pos)
        {
            player.sceneManager.Terrain.HideTrees(pos);
        }

        public virtual void ThrowAgain()
        {
            replayPlaying = false;
            player.sceneManager.MiniMap.trackFlight = false;
            player.playerCameraScript.DiscCam = false;
            player.DistanceToBasket = Vector3.Distance(player.transform.position, player.sceneManager.Basket.basketPosition.position);
            player.UI.moveToLieButton.DisablePopOut();
            player.UI.throwAgainButton.DisablePopOut();
            player.UI.replayButton.DisablePopOut();
            player.throwUI.ShowThrowUI();
            //DiscFlightCamera.isTracking = null;
            player.playerCamera.transform.position = player.mainCam.position;
            player.playerCamera.transform.rotation = player.mainCam.rotation;
            player.playerCamera.fieldOfView = 60;
            player.curentDiscScript.RemoveDisc();
            player.UI.popUpText.ShowText("Select Disc", 3f);
            player.discSelect.ShowQuickSelect(player.DistanceToBasket);
            player.sceneManager.MiniMap.UpdateDiscPosition(transform.position);

            if (player.DistanceToBasket > 15f)
            {
                player.sceneManager.Basket.ShowBasketIcon(player.DistanceToBasketString, player.DistanceToBasket, player.transform);
            }
        }

        public void UpdateReachBackPanel()
        {
            if (player.curentDisc != null && player.throwUI.shotSelect.SelectedShot != ShotSelector.ShotType.Putt)
            {
                player.curentDiscScript.ShowPuttingGuide(false);
                player.throwUI.reachbackPanel.ShowReachBackPanel(true);
                player.throwUI.reachbackPanel.reachBackDisc.SetColor(player.curentDiscScript.colors.baseColor);
            }
            else if (player.curentDisc != null && player.throwUI.shotSelect.SelectedShot == ShotSelector.ShotType.Putt)
            {
                player.throwUI.reachbackPanel.ShowReachBackPanel(false);
                player.curentDiscScript.ShowPuttingGuide(true);
            }
            else
            {
                player.throwUI.reachbackPanel.ShowReachBackPanel(false);
            }
        }

        [ContextMenu("Update Rotation")]
        public void UpdateRotation()
        {
            player.throwUI.windIndicator.UpdateRotation(transform.rotation);
        }

        internal void SideStep(float posX)
        {
            player.rotateAroundPoint.position = transform.position + (transform.right * (posX / 100f));
        }
    }
}
