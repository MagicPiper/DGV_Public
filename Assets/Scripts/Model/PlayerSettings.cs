using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class PlayerSettings
    {
        public PlayerSettingsData settingsData;
        public PlayerSave playerSave;

        public string DistanceConverter(float distance)
        {
            if (Menu.PlayerSave.playerGet && settingsData.metersToFeet)
            {
                return Mathf.Round((distance * 3.28084f)) + " ft";
            }
            else
            {
                return Mathf.Round(distance) + " M";
            }
        }

        public void SaveSettings()
        {
            playerSave.SavePlayerSettings(settingsData);
        }

        internal void NewSettings()
        {
            Debug.Log("newstats");
            settingsData = new PlayerSettingsData()
            {
                leftHandedMode = false,
                volume = 1f,
                metersToFeet = false,
                musicOff = false,
                gfxLevel = 0
                
            };
            SaveSettings();
        }
    }
}
