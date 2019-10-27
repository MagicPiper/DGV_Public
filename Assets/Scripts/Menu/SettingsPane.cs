using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class SettingsPane : MenuPane
    {
        public PlayerSave playerSave;
        public PlayerIconData iconData;

        public Toggle leftyToggle;
        public Slider volume;
        public TMP_Dropdown metersToFeet;
        public TMP_Dropdown gfxQuality;
        public TMP_Dropdown playerIconDropdown;
        public TMP_InputField displayNameInput;
        public Toggle disableMusic;        
        public bool populated;

        public override void Show()
        {
            currentPane = this;
            FadeInPane(GetComponent<CanvasGroup>());
            var settings = playerSave.playerSettings.settingsData;
            leftyToggle.isOn = settings.leftHandedMode;
            volume.value = settings.volume;
            metersToFeet.value = settings.metersToFeet ? 1 : 0;
            gfxQuality.value = settings.gfxLevel;
            disableMusic.isOn = settings.musicOff;
            displayNameInput.text = playerSave.PlayerDisplayName();
            if (!populated)
            {
                PopulateIcons();
            }
            playerIconDropdown.value = settings.icon;
        }

        private void PopulateIcons()
        {
            var options = new List<TMP_Dropdown.OptionData>();
            foreach(Sprite icon in iconData.AllIcons())
            {
                var option = new TMP_Dropdown.OptionData
                {
                    image = icon,
                    text = ""
                };
                options.Add(option);
            }

            playerIconDropdown.AddOptions(options);
            populated = true;
        }

        public void OnVolumeChange(float volume)
        {
            playerSave.playerSettings.settingsData.volume = volume;
            AudioListener.volume = volume;
        }

        public void OnLeftyChange(bool lefty)
        {
            playerSave.playerSettings.settingsData.leftHandedMode = lefty;
        }

        public void OnMetersToFeetChange(int change)
        {
            playerSave.playerSettings.settingsData.metersToFeet = change == 0 ? false : true;
        }

        public void OnDisableMusicChange(bool musicOff)
        {
            playerSave.playerSettings.settingsData.musicOff = musicOff;
            if (musicOff)
            {
                MusicManager.Instance.DisableMusic();
            }
            else
            {
                MusicManager.Instance.EnableMusic();
            }
        }

        public void OnGFXQualityChange(int gfx)
        {
            playerSave.playerSettings.settingsData.gfxLevel = gfx;
            QualitySettings.SetQualityLevel(gfx, true);
            Debug.Log("Current QUality Level: " + QualitySettings.GetQualityLevel());
            playerSave.playerSettings.SaveSettings();
        }

        public void OnIconChange(int icon)
        {
            playerSave.playerSettings.settingsData.icon = icon;           
        }

        public void OnDisplayNameCHange(string displayName)
        {
            playerSave.playerSettings.settingsData.displayName = displayName;                
        }

        public override void Back()
        {
            playerSave.playerSettings.SaveSettings();
            Hide();
            backPane.Show();
        }
    }
}
