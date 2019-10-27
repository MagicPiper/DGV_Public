using Assets.Scripts.Menu;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Assets.Scripts
{
    public class DiscSelect : MonoBehaviour
    {
        public bool discBagPopulated;
        public bool quickSelectPopulated;
        public UIDisc currentSelection;
        public UIDisc UIdiscPrefab;
        public Transform discHolder;
        public Transform emptySlots;
        public Transform[] extraSlots;
        public PlayerSave playerSave;
        internal Player playerScript;
        public CanvasGroup discPane;
        public CanvasGroup quickSelect;
        public ScrollRect quickSelectScroll;
        public Transform quickSelectDischHolder;
        public List<UIDisc> quickSelectDiscObjects;
        public HorizontalScrollSnap snap;
        private bool showing;

        public void ShowQuickSelect(float distance)
        {               
            FadeInPane(quickSelect, 1f);
            if (!quickSelectPopulated)
            {
                if (Menu.PlayerSave.offline)
                {
                    foreach (Disc disc in playerScript.offlineDiscs)
                    {
                        var discItem = Instantiate(UIdiscPrefab);
                        quickSelectDiscObjects.Add(discItem);
                        discItem.GetComponent<UIDisc>().Populate(disc, UIDisc.Context.InGame);
                        discItem.GetComponent<UIDisc>().DiscSelect = this;
                        discItem.transform.SetParent(quickSelectDischHolder.transform, false);
                    }
                }
                else if (!Menu.PlayerSave.playerGet)
                {
                    foreach (Disc disc in playerScript.testDiscs)
                    {
                        var discItem = Instantiate(UIdiscPrefab);
                        quickSelectDiscObjects.Add(discItem);
                        discItem.GetComponent<UIDisc>().Populate(disc, UIDisc.Context.InGame);
                        discItem.GetComponent<UIDisc>().DiscSelect = this;
                        discItem.transform.SetParent(quickSelectDischHolder.transform, false);
                    }
                }
                else
                {
                    var discBag = playerSave.GetDiscBagContent();
                    foreach (Disc disc in discBag)
                    {
                        var discItem = Instantiate(UIdiscPrefab);
                        quickSelectDiscObjects.Add(discItem);
                        discItem.GetComponent<UIDisc>().Populate(disc, UIDisc.Context.InGame);
                        discItem.GetComponent<UIDisc>().DiscSelect = this;
                        discItem.transform.SetParent(quickSelectDischHolder.transform, false);
                    }
                }
                snap.Init();
                quickSelectPopulated = true;
            }
            quickSelectScroll.horizontalNormalizedPosition = 0f;
            snap.ChangeBulletsInfo(0);
            if (distance < 40f)
            {
                var newOrder = new List<UIDisc>();
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Putter));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Midrange));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Fairway));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Distance));

                SortQuickSelect(newOrder);
            }
            else if (distance < 65f)
            {
                var newOrder = new List<UIDisc>();
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Midrange));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Putter));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Fairway));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Distance));

                SortQuickSelect(newOrder);
            }
            else if (distance < 90f)
            {
                var newOrder = new List<UIDisc>();
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Fairway));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Midrange));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Distance));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Putter));

                SortQuickSelect(newOrder);
            }
            else
            {
                var newOrder = new List<UIDisc>();
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Distance));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Fairway));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Midrange));
                newOrder.AddRange(quickSelectDiscObjects.FindAll(d => d.mould.discType == DiscMould.DiscType.Putter));

                SortQuickSelect(newOrder);
            }
        }

        private void SortQuickSelect(List<UIDisc> newOrder)
        {
            int pos = 0;
            foreach (UIDisc disc in newOrder)
            {
                disc.transform.SetSiblingIndex(pos);
                pos++;
            }
        }

        public void ShowDiscSelect()
        {
            if (showing)
            {
                FadeOutPane(discPane);
                showing = false;
                return;
            }
            showing = true;

            if (currentSelection)
            {
                currentSelection.Deselect();
            }

            FadeInPane(discPane, 0f);

            if (!discBagPopulated)
            {
                bool bigDiscBag = false;
                if (!Menu.PlayerSave.playerGet)
                {
                    foreach (Disc disc in playerScript.testDiscs)
                    {
                        var discItem = Instantiate(UIdiscPrefab);
                        discItem.GetComponent<UIDisc>().Populate(disc, UIDisc.Context.InGame);
                        discItem.GetComponent<UIDisc>().DiscSelect = this;
                        discItem.transform.SetParent(discHolder, false);
                    }
                }
                else
                {
                    if (playerSave.playerStats.statsData.devSupport)
                    {
                        bigDiscBag = true;
                    }
                    var discBag = playerSave.GetDiscBagContent();
                    foreach (Disc disc in discBag)
                    {
                        var discItem = Instantiate(UIdiscPrefab);
                        discItem.GetComponent<UIDisc>().Populate(disc, UIDisc.Context.InGame);
                        discItem.GetComponent<UIDisc>().DiscSelect = this;
                        discItem.transform.SetParent(discHolder, false);

                    }
                }

                if (bigDiscBag)
                {
                    discHolder.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    emptySlots.localScale = new Vector3(0.7f, 0.7f, 0.7f);

                    foreach (Transform slot in extraSlots)
                    {
                        slot.gameObject.SetActive(true);
                    }
                }
                discBagPopulated = true;
            }
        }

        internal void SelectionChange(Disc disc, UIDisc select)
        {
            currentSelection = select;
            playerScript.action.ChangeDisc(disc);
            FadeOutPane(discPane);
            FadeOutPane(quickSelect);
            showing = false;
        }

        public void FadeInPane(CanvasGroup pane, float delay)
        {
            pane.gameObject.SetActive(true);
            pane.alpha = 0f;
            LeanTween.alphaCanvas(pane, 1f, 1.5f).setEaseOutExpo().setDelay(delay);
        }

        public void FadeOutPane(CanvasGroup pane)
        {            
            LeanTween.alphaCanvas(pane, 0f, 0.3f).setEaseInSine().setOnComplete(() =>
            {
                pane.gameObject.SetActive(false);               
            });
        }
    }
}
