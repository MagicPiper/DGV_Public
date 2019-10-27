using UnityEngine;
using TMPro;

namespace Assets.Scripts.Menu
{
    public class PlayerProfilePane : MenuPane
    {
        public TMP_Text playerName;
        public TMP_Text discsAquired;
        public TMP_Text longestPutt;
        public TMP_Text longestDrive;
        public TMP_Text aces;
        public TMP_Text eagles;
        public TMP_Text albatrosses;
        public TMP_Text playerRating;
        public TMP_Text division;
        public TMP_Text playerLevel;
        public PlayerSave playerSave;
        
        public override void Show()
        {
            currentPane = this;
            var stats = playerSave.playerStats.statsData;
            FadeInPane(GetComponent<CanvasGroup>());
            playerName.text = playerSave.FirebaseManager.playerDisplayName;
            discsAquired.text = playerSave.GetDiscCount().ToString();
            longestDrive.text = playerSave.playerSettings.DistanceConverter(stats.longestDrive);
            longestPutt.text = playerSave.playerSettings.DistanceConverter(stats.longestPutt);
            aces.text = stats.aces.ToString();
            eagles.text = stats.eagles.ToString();
            albatrosses.text = stats.albatrosses.ToString();
            playerRating.text = stats.playerRating.ToString();
            division.text = stats.division.ToString();
            playerLevel.text = stats.playerLevel.ToString();
        }
    }
}
