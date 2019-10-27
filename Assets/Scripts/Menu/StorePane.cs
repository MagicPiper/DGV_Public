using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class StorePane : MenuPane
    {
        public GameState gameState;
        public Button devSupportPurchaseButton;

        public override void Show()
        {
            base.Show();

            if (!gameState.playerSave.playerStats.statsData.devSupport)
            {
                devSupportPurchaseButton.enabled = true;
            }
            else
            {
                devSupportPurchaseButton.enabled = false;
            }
        }

        public void OnPurchaseDevSupport()
        {
            gameState.playerSave.playerStats.DevSupportPurchased();
            devSupportPurchaseButton.enabled = false;
        }
    }
}
