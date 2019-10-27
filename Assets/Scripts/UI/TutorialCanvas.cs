using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts
{

    public class TutorialCanvas : MonoBehaviour
    {
        [SerializeField] private CanvasGroup selectDiscPane;
        [SerializeField] private CanvasGroup beforeThrowPane;
        [SerializeField] private CanvasGroup duringThrowPane;
        [SerializeField] private CanvasGroup duringPuttPane;
        [SerializeField] private GameState gameState;

        public void SelectDisc()
        {
            FadeInPane(selectDiscPane, 4f);           
        }

        public void SelectDiscComplete()
        {
            FadeOutPane(selectDiscPane);
            gameState.playerSave.GetTutorialCheck().seenFirstDiscSelect = true;           
        }

        public void BeforeThrow()
        {            
            FadeInPane(beforeThrowPane, 1f);
        }

        public void DuringThrow()
        {
            FadeOutPane(beforeThrowPane);
            FadeInPane(duringThrowPane, 1f);
        }

        public void ThrowComplete()
        {
            FadeOutPane(duringThrowPane);
            gameState.playerSave.GetTutorialCheck().seenFirstThrow = true;
        }

        public void FadeInPane(CanvasGroup pane, float delay)
        {
            pane.gameObject.SetActive(true);
            pane.alpha = 0f;

            LeanTween.alphaCanvas(pane, 1f, 1.5f).setEaseOutExpo().setDelay(delay);
        }

        public void FadeOutPane(CanvasGroup pane)
        {
            LeanTween.alphaCanvas(pane, 0f, 0.5f).setEaseInSine().setOnComplete(() =>
            {
                pane.gameObject.SetActive(false);
            });
        }

        internal void BeforePutt()
        {
            FadeInPane(duringPuttPane, 2f);
        }

        internal void PuttComplete()
        {
            FadeOutPane(duringPuttPane);
            gameState.playerSave.GetTutorialCheck().seenFirstPutt = true;
        }
    }
}