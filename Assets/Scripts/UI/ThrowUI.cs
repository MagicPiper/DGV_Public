using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ThrowUI : MonoBehaviour
    {
        public ShotSelector shotSelect;
        public Button discBagButton;
        public ReachBack reachbackPanel;
        public TouchLook panPanel;
        public WindIndicator windIndicator;
        internal Player playerScript;

        private bool disableWind=false;

        private void Start()
        {
            if (shotSelect != null)
            {
                shotSelect.gameObject.SetActive(false);
                discBagButton.gameObject.SetActive(false);
                panPanel.gameObject.SetActive(false);
                reachbackPanel.gameObject.SetActive(false);
                windIndicator.gameObject.SetActive(false);
                if (Menu.PlayerSave.playerGet && playerScript.gameState.playerSave.playerSettings.settingsData.leftHandedMode)
                {
                    shotSelect.SetLeftyMode();
                }
            }
        }

        internal void DisableWind()
        {
            disableWind = true;
        }

        public void HideThrowUI()
        {
            shotSelect.GetComponent<ButtonBehavior>().SlideOut(ButtonBehavior.Direction.left, 1f, 0.5f);
            discBagButton.GetComponent<ButtonBehavior>().SlideOut(ButtonBehavior.Direction.left, 1f, 0.5f);
            panPanel.GetComponent<ButtonBehavior>().SlideOut(ButtonBehavior.Direction.right, 1f, 0.5f);
            windIndicator.GetComponent<ButtonBehavior>().SlideOut(ButtonBehavior.Direction.right, 1f, 0.5f);
        }

        public void ShowThrowUI()
        {
            shotSelect.GetComponent<ButtonBehavior>().SlideIn(ButtonBehavior.Direction.left, 1f, 0.5f);
            discBagButton.GetComponent<ButtonBehavior>().SlideIn(ButtonBehavior.Direction.left, 1f, 0.5f);
            panPanel.GetComponent<ButtonBehavior>().SlideIn(ButtonBehavior.Direction.right, 1f, 0.5f);
            if(!disableWind)windIndicator.GetComponent<ButtonBehavior>().SlideIn(ButtonBehavior.Direction.right, 1f, 0.5f);
        }

        public void ShowDiscSelect()
        {
            playerScript.discSelect.ShowDiscSelect();
        }
    }  
}