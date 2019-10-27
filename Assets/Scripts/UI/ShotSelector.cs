using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ShotSelector : MonoBehaviour
    {
        [SerializeField]
        private GameObject forehandButton;
        [SerializeField]
        private GameObject backhandButton;
        [SerializeField]
        private GameObject puttButton;

        [SerializeField]
        private ThrowUI throwUI;

        [SerializeField] private Sprite leftArrowSprite;
        [SerializeField] private Sprite rightArrowSprite;

        [SerializeField] private Image backHand;
        [SerializeField] private Image foreHand;

        public enum ShotType { None, ForeHand, BackHand, Putt };

        [SerializeField]
        private ShotType selectedShot;
        public ShotType SelectedShot
        {
            get
            {
                return selectedShot;
            }
        }
        
        private ShotType previousShotType=ShotType.BackHand;

        public void SetLeftyMode()
        {
            backHand.sprite = rightArrowSprite;
            foreHand.sprite = leftArrowSprite;
        }
        
        public void Clicked(int shot)
        {
            if (selectedShot != ShotType.None)
            {
                previousShotType = selectedShot;
                selectedShot = ShotType.None;
                forehandButton.SetActive(true);
                backhandButton.SetActive(true);
                puttButton.SetActive(true);
            }
            else
            {
                SelectShot((ShotType)shot);
            }
        }

        public void SelectShot(ShotType shot)
        {           
            selectedShot = shot;
            throwUI.playerScript.action.UpdateReachBackPanel();

            switch (shot)
            {
                case ShotType.Putt:
                    puttButton.SetActive(true);
                    puttButton.transform.SetSiblingIndex(0);
                    backhandButton.SetActive(false);
                    forehandButton.SetActive(false);                    
                    break;
                case ShotType.BackHand:
                    backhandButton.SetActive(true);
                    backhandButton.transform.SetSiblingIndex(0);
                    puttButton.SetActive(false);
                    forehandButton.SetActive(false);
                    break;
                case ShotType.ForeHand:
                    forehandButton.SetActive(true);
                    forehandButton.transform.SetSiblingIndex(0);
                    puttButton.SetActive(false);
                    backhandButton.SetActive(false);
                    break;                
                default:                    
                    selectedShot = ShotType.BackHand;
                    backhandButton.SetActive(true);
                    backhandButton.transform.SetSiblingIndex(0);
                    puttButton.SetActive(false);
                    forehandButton.SetActive(false);
                    break;
            }
            throwUI.playerScript.action.UpdateReachBackPanel();
        }

        internal void LastShotType()
        {
            SelectShot(previousShotType);
        }
    }
}
