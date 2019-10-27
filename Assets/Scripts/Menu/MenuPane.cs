using System.Collections.Generic;
using UnityEngine;
using System.Collections;

using TMPro;
using UnityEngine.Events;

namespace Assets.Scripts.Menu
{
    public class MenuPane : MonoBehaviour
    {
        public static MenuPane currentPane;        
        public MenuPane backPane;

        public virtual void Back()
        {
            Hide();
            backPane.Show();
        }

        public virtual void Show()
        {
            currentPane = this;
            FadeInPane(GetComponent<CanvasGroup>());
        }

        public virtual void Hide()
        {
            FadeOutPane(GetComponent<CanvasGroup>());
        }

        public void FadeInPane(CanvasGroup pane)
        {
            pane.gameObject.SetActive(true);
            pane.blocksRaycasts = true;
            pane.alpha = 0f;

            LeanTween.alphaCanvas(pane, 1f, 1.5f).setEaseOutExpo().setDelay(0.2f);
        }

        public void FadeOutPane(CanvasGroup pane)
        {
            LeanTween.alphaCanvas(pane, 0f, 0.5f).setEaseInSine().setOnComplete(() =>
            {
                pane.gameObject.SetActive(false);
            });
        }

        public void FadeOutPaneDontDeactive(CanvasGroup pane)
        {
            LeanTween.alphaCanvas(pane, 0f, 0.5f).setEaseInSine();
            pane.blocksRaycasts = false;

        }
    }
}
