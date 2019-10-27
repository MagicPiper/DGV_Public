using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class MultiplayerPanel : MenuPane
    {
        public Color completeGameColor;
        public TMP_Text gameExpiredText;
        public Button claimRewardButton;
        public GameObject lookingForGamePane;
        public Button findGameButton;
        public TMP_Text loadingText;
        public TMP_Text welcomeText;

        public GameObject gamePanel;
        public ScoreScreen scoreScreen;

        public GameState gameState;

        public List<MultiplayerGame> multiplayerGames;
        public string loadGamesStatus;

        public List<MultiplayerUIGame> games;
        public MultiplayerUIGame gamePrefab;
        public Transform gamesHolder;

        public string lookForGameStatus;
        internal RandomRoundWrapper tournamentWrapper;
        internal bool hasHoles;

        public Tournament tournament;
        private MultiplayerUIGame currentGame;
        public RewardPanel rewardPanel;
        internal long startTime;

        public MenuPlayerBehavior menuPlayer;
        private string lastGameID;

        public string friendlyGameID;

        public TMP_Text friendlyGameText;

        public override void Show()
        {
            base.Show();
            findGameButton.gameObject.SetActive(false);

            var currentVersion = float.Parse(Application.version, CultureInfo.InvariantCulture);

            if (gameState.playerSave.LatestVersion > currentVersion + 0.002)
            {
                Debug.Log("not on latest version, disabling multiplayer.");
                welcomeText.text = "Update to latest version to play Multiplayer!";
            }
            else
            {
                welcomeText.text = "Play a round with other players.";
                Invoke("ShowFindGameButton", 1f);
                if (loadGamesStatus != "hasGames")
                {
                    GetMultiplayerGames();
                    StartCoroutine(LoadMultiplayerGames());
                }
                else
                {
                    SelectLatestGame();
                }
            }
        }

        internal void ShowFindGameButton()
        {
            findGameButton.gameObject.SetActive(true);
        }

        internal void ShowRewardButton(bool v)
        {
            claimRewardButton.gameObject.SetActive(v);
        }

        private void GetMultiplayerGames()
        {
            loadGamesStatus = "pending";
            gameState.playerSave.FirebaseManager.MultiPlayerFunctions.MultiplayerGetOpenGames(this);
        }

        private IEnumerator LoadMultiplayerGames()
        {
            while (loadGamesStatus == "pending")
            {
                yield return new WaitForSeconds(0.2f);
            }

            PopulatePreviousGames();
        }

        internal void SelectGame(MultiplayerUIGame UIgame)
        {
            foreach (MultiplayerUIGame game in games)
            {
                game.Deselect();
            }
            if (UIgame != null)
            {
                currentGame = UIgame;
                welcomeText.gameObject.SetActive(false);
            }
            else
            {
                welcomeText.gameObject.SetActive(true);
                scoreScreen.ClearScores();
                scoreScreen.timeLeftText.text = "";
                scoreScreen.countdown = false;
            }
        }

        private void PopulatePreviousGames()
        {
            games = new List<MultiplayerUIGame>();
            foreach (MultiplayerGame game in multiplayerGames)
            {
                var b = Instantiate(gamePrefab, gamesHolder);
                games.Add(b);
                b.Populate(game, this);
            }
            SelectLatestGame();
        }

        public void SelectLatestGame()
        {
            if (games.Count > 0)
            {
                var game = games[games.Count - 1];
                game.SelectGame();
            }
            else
            {
                SelectGame(null);
            }
        }

        public void Return(string gameID)
        {
            lastGameID = gameID;
            Invoke("Show", 3f);
        }

        [ContextMenu("queue test player")]
        public void QueueTestPlayers()
        {
            gameState.playerSave.FirebaseManager.QueueTestPlayer();
        }

        public void FindGame()
        {
            lookingForGamePane.SetActive(true);
            FadeOutPaneDontDeactive(GetComponent<CanvasGroup>());
            lookForGameStatus = "wfg";
            StartCoroutine(WaitForGame());
            StartCoroutine(AnimateText());
            gameState.playerSave.FirebaseManager.MultiPlayerFunctions.MultiplayerQueuePlayer(this, lastGameID);

            menuPlayer.PlayerStart();
        }

        public void CancelFindGame()
        {
            gameState.playerSave.FirebaseManager.MultiPlayerFunctions.MultiplayerUnqueue(this);
        }

        public void ClaimReward()
        {
            gameState.playerSave.FirebaseManager.MultiPlayerFunctions.MultplayerClaimReward(currentGame.game.gameID);
            var xpGain = 100 + (50 / currentGame.game.position);
            var text = "Multiplayer Game Reward";

            if (!currentGame.game.completedGame)
            {
                xpGain = 0;
                text = "Abandoned Game";
            }

            rewardPanel.XPReward(xpGain, currentGame.game.position, text);

            ShowRewardButton(false);
            games.Remove(currentGame);
            Destroy(currentGame.gameObject);
            SelectLatestGame();
        }

        private IEnumerator WaitForGame()
        {
            while (lookForGameStatus == "wfg")
            {
                yield return new WaitForSeconds(1f);
            }
            gameState.playerSave.FirebaseManager.MultiPlayerFunctions.StopWaitingForGameListener();

            if (lookForGameStatus == "nq")
            {
                loadingText.alpha = 1f;
                lookingForGamePane.SetActive(false);
                menuPlayer.End();
                FadeInPane(GetComponent<CanvasGroup>());
            }
            else
            {
                StartCoroutine(StartGame());
            }
        }

        private IEnumerator StartGame()
        {
            gameState.playerSave.FirebaseManager.MultiPlayerFunctions.MultiplayerGetHoles(this, lookForGameStatus);

            while (!hasHoles)
            {
                yield return new WaitForSeconds(1f);
            }

            var rm = gameState.NewMultiplayerRound();
            tournament.holes = tournamentWrapper.holes;
            tournament.windSeed = tournamentWrapper.windSeeds;

            var s = DateTimeOffset.FromUnixTimeMilliseconds(startTime).DateTime;
            var gameEndTime = s.AddMinutes(15);
            rm.StartMultiplayerRound(tournament, lookForGameStatus, gameEndTime);
            Debug.Log("GameID" + lookForGameStatus);

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(gameState.roundManager.currentHole.holeScene);
        }

        private IEnumerator AnimateText()
        {
            while (lookForGameStatus == "wfg")
            {
                loadingText.alpha = Mathf.PingPong(Time.time, 1f);
                yield return null;
            }
        }
    }
}