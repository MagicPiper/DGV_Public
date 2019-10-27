using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class MenuPlayerBehavior : PlayerBehavior
    {     
        private Disc menuDisc;
        public Transform basket;
        public Transform[] basketPositions;
        private int counter;
        private int basketIndex;
        private Transform cameraStart;
        public TMP_Text waitText;
        public Vector3 startPos;
        public Quaternion startRot;

        public override void PlayerStart()
        {
            startPos = player.playerCamera.transform.position;
            startRot = player.playerCamera.transform.rotation;
            waitText.gameObject.SetActive(true);
            Invoke("HideText", 3f);
            player.action = this;
            transform.LookAt(basket);
            player.playerCameraScript.MoveToLieCam();
            menuDisc = player.testDiscs[0];
            player.DistanceToBasket = Vector3.Distance(transform.position, basket.position);
            player.throwUI.playerScript = this.player;
            player.throwUI.gameObject.SetActive(true);
            player.throwUI.panPanel.GetComponent<ButtonBehavior>().SlideIn(ButtonBehavior.Direction.right, 1f, 0.5f);
            counter = 0;
            basketIndex = 0;
            Putt();
        }

        private void HideText()
        {
            waitText.gameObject.SetActive(false);
        }

        public void Putt()
        {
            counter++;
            if (counter > 4)
            {
                counter = 0;
                basketIndex = basketIndex > 1 ? basketIndex = 0 : basketIndex + 1;

                basket.position = basketPositions[basketIndex].position;
                player.playerCamera.transform.SetParent(null);

                transform.LookAt(basket);
                player.throwUI.panPanel.ResetPos();
                player.DistanceToBasket = Vector3.Distance(transform.position, basket.position);

                player.playerCameraScript.MoveToLieCam();
            }
            //Debug.Log("putt counter: " + counter + " basket index " + basketIndex);
            var d = Instantiate(player.discPrefab, player.discHolder.transform);
            player.curentDisc = d;
            player.curentDiscScript = d.GetComponent<DiscBehavior>();
            player.curentDiscScript.player = player;
            player.curentDiscScript.PopulateMenuDisc(menuDisc, false);
           // d.transform.SetParent(null, true);

            player.curentDiscScript.ShowPuttingGuide(true);
            player.curentDiscScript.puttingBehavior.menu = true;
        }

        public override void ThrowAgain()
        {
            Invoke("Putt", 3f);
        }

        public void End()
        {
            player.playerCamera.transform.SetParent(null);
            player.playerCamera.transform.position = startPos;
            player.playerCamera.transform.rotation = startRot;
            // player.playerCameraScript.MoveToLieCam();
            basket.position = basketPositions[0].position;
            player.throwUI.gameObject.SetActive(false);
            Destroy(player.curentDisc);

        }
    }
}

