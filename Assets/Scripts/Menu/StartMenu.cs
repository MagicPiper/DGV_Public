using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class StartMenu : MenuPane
    {
        public GameObject newUpdatePane;
        public PlayerSave playerSave;

        public ChallengeMap challengeMap;
        public CanvasGroup tournamentPanel;
        public LoginPanel signInPanel;
        public CanvasGroup practicePanel;
        public MenuPane offlinePanel;
        public MultiplayerPanel multiplayerPanel;
        public ProTourPanel proTourPanel;
        public FriendlyGamePanel friendlyPanel;

        public RectTransform titleText;
        public TMP_Text titleText1;
        public TMP_Text titleText2;

        public TMP_Text taptostartText;
        public GameObject tapToStartpanel;

        public GameState gameState;
        public RewardPanel rewardPanel;

        public TMP_Text loadingText;

       // public RewardNotification rewardNotification;
        public TMP_Text versionNumber;

        public List<RectTransform> buttons;
        public ButtonBehavior feedbackButton;
        public ButtonBehavior storeButton;
        private float buttonDelay;
        public DiscLauncher launcher;
        private int loading;

        void Start()
        {
            versionNumber.text = "v" + Application.version;

            currentPane = this;
            playerSave.start = this;

            if (gameState.roundManager != null)
            {
                ReturnFromRound();
            }
            else
            {
                signInPanel.Show();
            }
        }

        public void SignInUser()
        {
            StartCoroutine(playerSave.GetPlayer());

            signInPanel.FadeOutPane(signInPanel.GetComponent<CanvasGroup>());
            loadingText.gameObject.SetActive(true);

            loading = LeanTween.value(gameObject, 0.9f, 0.02f, 1f).setEase(LeanTweenType.easeOutSine).setOnUpdate((float val) =>
            {
                TextAlpha(val, loadingText);
            })
           .setLoopPingPong().id;
        }

        public void AuthFailed()
        {
            signInPanel.Show();
            signInPanel.connectionErrorText.text = "Authentication Failed";
        }

        public void AuthCanceled()
        {
            signInPanel.Show();
            loadingText.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                currentPane.Back();
            }
        }

        public override void Back()
        {
            Debug.Log("quit");
            Application.Quit();
        }

        public void PlayOffline()
        {
            playerSave.PlayOffline();
            offlinePanel.Show();
            signInPanel.Hide();
        }

        public void IntroAnimation()
        {
            titleText.GetComponent<CanvasGroup>().alpha = 1;

            loadingText.gameObject.SetActive(false);
            LeanTween.cancel(loading);
            AudioListener.volume = playerSave.playerSettings.settingsData.volume;
            if (playerSave.playerSettings.settingsData.musicOff)
            {
                MusicManager.Instance.DisableMusic();
            }
            MusicManager.Instance.PlayIntroMusic();

            SetGFXLevel();

            StartDiscLauncher();
            titleText.gameObject.SetActive(true);
            taptostartText.gameObject.SetActive(true);

            LeanTween.value(gameObject, 0f, 1f, 3f).setEase(LeanTweenType.easeInCubic).setOnUpdate((float val) =>
            {
                TextAlpha(val, titleText1);
            });

            LeanTween.value(gameObject, 0f, 1f, 0.5f).setEase(LeanTweenType.easeInCubic).setOnUpdate((float val) =>
            {
                TextAlpha(val, titleText2);
                TextGlow(val, titleText2);
            }).setOnComplete(() =>
            {
                LeanTween.value(gameObject, 1f, 0f, 0.5f).setEase(LeanTweenType.easeOutCubic).setOnUpdate((float val) =>
                {
                    TextGlow(val, titleText2);
                });
                tapToStartpanel.SetActive(true);
            })
            .setDelay(3f);

            LeanTween.value(gameObject, 0, 1, 2f).setEase(LeanTweenType.easeInCubic).setOnUpdate((float val) =>
            {
                TextAlpha(val, taptostartText);
            })
            .setDelay(5f).setOnComplete(() =>
            {
                LeanTween.value(gameObject, 0.9f, 0.02f, 1f).setEase(LeanTweenType.easeOutSine).setOnUpdate((float val) =>
                {
                    TextAlpha(val, taptostartText);
                })
               .setLoopPingPong();
            });
        }

        private void SetGFXLevel()
        {
            QualitySettings.SetQualityLevel(playerSave.playerSettings.settingsData.gfxLevel, true);
        }

        void StartDiscLauncher()
        {
            launcher.StartLaunching();
        }

        void TextAlpha(float val, TMP_Text text)
        {
            text.alpha = val;
        }

        void TextGlow(float val, TMP_Text text)
        {
            text.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, val);
        }

        public void TapToStart()
        {
            taptostartText.gameObject.SetActive(false);
            tapToStartpanel.SetActive(false);

            WelcomeOkButton();
        }

        public void WelcomeOkButton()
        {
            foreach (RectTransform button in buttons)
            {
                var position = button.anchoredPosition;
                buttonDelay += 0.2f;
                button.anchoredPosition = new Vector2(-1000, position.y);
                button.gameObject.SetActive(true);
                LeanTween.move(button, position, 0.3f).setDelay(buttonDelay).setEase(LeanTweenType.easeOutCubic);
            }
            LeanTween.move(titleText, new Vector2(0, 142), 0.5f).setEase(LeanTweenType.easeInCubic);
            feedbackButton.EnablePopIn();
            //storeButton.EnablePopIn();
            // CheckForRewards();

            var currentVersion = float.Parse(Application.version, CultureInfo.InvariantCulture);

            if (playerSave.LatestVersion > currentVersion + 0.002)
            {
                Debug.Log("new version available. current " + currentVersion + " latest: " + playerSave.LatestVersion);
                newUpdatePane.SetActive(true);
            }
            else
            {
                Debug.Log("client up to date. current " + currentVersion + " latest: " + playerSave.LatestVersion);
                newUpdatePane.SetActive(false);
            }
        }

        //public void CheckForRewards()
        //{
        //    playerSave.GetOpenReward();
        //}

        public void OpenPlayStore()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Application.OpenURL("https://play.google.com/store/apps/details?id=com.Per.DiscGolf");
            }
            else
            {
                Application.OpenURL("https://www.apple.com/lae/ios/app-store/");
            }
        }

        public override void Show()
        {
            currentPane = this;
            titleText.gameObject.SetActive(true);
            titleText.GetComponent<CanvasGroup>().alpha = 1;
            titleText1.alpha = 1f;
            titleText2.alpha = 1f;

            foreach (RectTransform button in buttons)
            {
                button.gameObject.SetActive(true);
            }
            feedbackButton.EnablePopIn();
           // storeButton.EnablePopIn();

            var currentVersion = float.Parse(Application.version, CultureInfo.InvariantCulture);

            if (playerSave.LatestVersion > currentVersion + 0.002)
            {
                Debug.Log("new version available. current " + currentVersion + " latest: " + playerSave.LatestVersion);
               // FadeInPane(newUpdatePane);
            }
        }

        public void Feedback()
        {
            Application.OpenURL("https://www.reddit.com/r/discgolfvalley/");
        }

        public override void Hide()
        {
            foreach (RectTransform button in buttons)
            {
                button.gameObject.SetActive(false);
            }
            titleText.gameObject.SetActive(false);
            feedbackButton.DisablePopOut();
           // storeButton.DisablePopOut();
           // FadeOutPane(newUpdatePane);
        }

        public void ReturnFromRound()
        {
            if (!playerSave.FirebaseManager.playerAuthenticated)
            {
                offlinePanel.Show();
                return;
            }

            playerSave.FirebaseManager.GetCurrentVersion();

            titleText.GetComponent<CanvasGroup>().alpha = 0;
            titleText.anchoredPosition = new Vector2(0, 142);
            titleText1.alpha = 1f;
            titleText2.alpha = 1f;

            taptostartText.gameObject.SetActive(false);
            tapToStartpanel.SetActive(false);
            loadingText.gameObject.SetActive(false);

            if (gameState.roundManager is TournamentRoundManager)
            {
                FadeInPane(tournamentPanel);
                var trm = (TournamentRoundManager)gameState.roundManager;
                if (!trm.retiredRun && trm.currentTournament.type != Tournament.TournamentType.Open)
                {
                    //UnityAction reset = gameState.ResetRound;
                    rewardPanel.XPReward(trm.XPreward, trm.position, trm.rewardText);
                }
            }
            else if (gameState.roundManager is PracticeRoundManager)
            {
                FadeInPane(practicePanel);
                gameState.ResetRound();
              //  CheckForRewards();
            }
            else if (gameState.roundManager is MultiplayerRoundManager)
            {
                multiplayerPanel.Return(gameState.roundManager.roundID);

                gameState.ResetRound();
            }

            else if (gameState.roundManager is ProTourRoundManager)
            {
                proTourPanel.Return();

                gameState.ResetRound();
            }
            else if (gameState.roundManager is FriendlyRoundManager)
            {
                var frm = (FriendlyRoundManager)gameState.roundManager;
                friendlyPanel.Return(frm.gameID);

                //gameState.ResetRound();
            }

            else
            {
                FadeInPane(challengeMap.GetComponent<CanvasGroup>());
                challengeMap.Show(true);
            }
        }
    }
}
