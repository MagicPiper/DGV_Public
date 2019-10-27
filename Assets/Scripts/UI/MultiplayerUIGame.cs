using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Menu;
using Firebase.Database;

namespace Assets.Scripts
{
    public class MultiplayerUIGame : MonoBehaviour
    {
        public MultiplayerGame game;
        private string currentGameStatus;
        private MultiplayerPanel panel;
        public PlayerSave playerSave;

        public TMP_Text gameStatustext;
        public Image buttonImage;
        public Button button;

        public Color completedGameColor;
        public Color onGoingGameColor;
        public Color selectedGameColor;

        private float firebaseUpdateCounter = 11f;
        internal List<ScoreCard> scoreCards;
        public bool newScore;

        public bool selected;
        public bool oldGame;
        public DateTime gameEndTime;
        internal EventHandler<ValueChangedEventArgs> statusHandler;
        internal DatabaseReference statusRef;

        internal EventHandler<ValueChangedEventArgs> scoreHandler;
        internal DatabaseReference scoreRef;

        public void UpdateGame()
        {
            if(selected)UpdateScoreScreen();

            if (game.status == "c" && scoreCards != null)
            {
                currentGameStatus = "c";
                buttonImage.color = Color();
                gameStatustext.text = game.status + " game";
                if (selected)panel.ShowRewardButton(true);
                statusRef.ValueChanged -= statusHandler;
                scoreRef.ValueChanged -= scoreHandler;                
            }
        }

        private void OnDestroy()
        {
            if (statusRef != null && statusHandler != null)
            {
                statusRef.ValueChanged -= statusHandler;
            }

            if (scoreRef != null && scoreHandler != null)
            {
                scoreRef.ValueChanged -= scoreHandler;
            }
        }

        public void Populate(MultiplayerGame game, MultiplayerPanel panel)
        {
            this.panel = panel;
            this.game = game;

            var statusText = game.status == "o" ? "ongoing" : "closed";
            gameStatustext.text = statusText + " game";

            buttonImage.color = Color();
            currentGameStatus = game.status;

            var startTime = DateTimeOffset.FromUnixTimeMilliseconds(game.startTime).DateTime;
            gameEndTime = startTime.AddMinutes(15);

            if (DateTime.UtcNow > startTime.AddDays(1))
            {
                oldGame = true;
                Debug.Log("old game");

                if (game.status != "c") //broken game
                {
                    Debug.Log("broken game");
                    playerSave.FirebaseManager.MultiPlayerFunctions.MultplayerClaimReward(game.gameID);
                    panel.games.Remove(this);
                    Destroy(gameObject);
                }                
            }
            else
            {
                playerSave.FirebaseManager.MultiPlayerFunctions.MultiplayerGetGameStatus(this, game.gameID);
                playerSave.FirebaseManager.MultiPlayerFunctions.MultiplayerGetScore(this, game.gameID);
            }
        }

        public void SelectGame()
        {
            Debug.Log("select game" +  game.gameID);

            panel.SelectGame(this);
            selected = true;
            buttonImage.color = selectedGameColor;
            button.interactable = false;

            if (scoreCards != null && scoreCards.Count > 0)
            {
                UpdateScoreScreen();
            }
            else
            {
                Debug.Log("no scorecards");
                panel.scoreScreen.ClearScores();
            }
            panel.ShowRewardButton(game.status == "c");
        }

        private void UpdateScoreScreen()
        {
            Debug.Log("Update Scorescreen");

            panel.scoreScreen.ClearScores();
            panel.scoreScreen.scoreCards = scoreCards;
            panel.scoreScreen.holeData = game.holes;
            panel.scoreScreen.PopulateScore();

            panel.scoreScreen.endTime = gameEndTime;
            panel.scoreScreen.countdown = true;
        }

        private Color Color()
        {
            if (selected)
            {
                return selectedGameColor;
            }
            else
            {
                if (game.status == "c")
                {
                    return completedGameColor;
                }
                else
                {
                    return onGoingGameColor;
                }
            }
        }

        internal void Deselect()
        {
            selected = false;
            buttonImage.color = Color();
            button.interactable = true;
        }
    }
}