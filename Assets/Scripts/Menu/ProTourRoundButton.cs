using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class ProTourRoundButton : MonoBehaviour
    {
        public Image sprite;
        public Transform lockPane;
        public TMP_Text scoreText;
        public TMP_Text playText;
        public TMP_Text unlockTimeText;
        public Color playColor;
        public Color lockColor;
        public Color playedColor;
        public Button button;

        internal void HasPlayed()
        {
            playText.gameObject.SetActive(true);
            playText.text = "Done";
            lockPane.gameObject.SetActive(false);

            sprite.color = playedColor;
            button.interactable = false;
        }

        internal void Unlock()
        {
            sprite.color = playColor;
            lockPane.gameObject.SetActive(false);
            playText.gameObject.SetActive(true);
            playText.text = "Play!";
            button.interactable = true;
        }

        internal void Lock(int daysLeft)
        {
            button.interactable = false;
            lockPane.gameObject.SetActive(true);

            sprite.color = lockColor;
            unlockTimeText.text = daysLeft == 1 ? "1 Day Left" : daysLeft.ToString() + " Days Left";            
        }
    }
}