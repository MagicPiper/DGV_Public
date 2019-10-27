using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class FriendlyGamePanel : MenuPane
    {
        public GameState gameState;
        public ScoreScreen scoreScreen;
        internal bool hasHoles;
        public Tournament tournament;
        public RandomRoundWrapper roundWrapper;

        public string friendlyGameID;

        public TMP_Text friendlyGameIDText;
        public GameObject joinGamePrompt;
        public GameObject lobbyPane;
        public TMP_Text joinGamePromptText;
        public TMP_InputField joinGameInputID;

        public Button newFriendlyButton;
        public Button joinFriendlyButton;
        public Button joinPromptButton;
        public Button cancelPromptButton;

        public Button playAgainButton;

        public Button startRoundButton;
        public Button leaveGroupButton;

        internal EventHandler<ValueChangedEventArgs> scoreHandler;
        internal DatabaseReference scoreRef;
        public List<ScoreCard> scoreCards;
        public bool isLobbyOwner;

        public TMP_Text countdownText;
        internal EventHandler<ValueChangedEventArgs> holesHandler;
        internal DatabaseReference holesRef;
        private bool startingNewRound;

        public override void Show()
        {
            base.Show();
            lobbyPane.SetActive(false);
            joinGamePrompt.SetActive(false);
            newFriendlyButton.interactable = true;
            joinFriendlyButton.interactable = true;
        }

        public void NewFriendly()
        {
            Debug.Log("new friendly");
            newFriendlyButton.interactable = false;
            joinFriendlyButton.interactable = false;

            tournament.Generate();
            roundWrapper = new RandomRoundWrapper()
            {
                holes = tournament.holes,
                status = "fresh",
                windSeeds = tournament.windSeed
            };
            gameState.playerSave.FirebaseManager.FriendlyFunctions.NewFriendlyGame(this, roundWrapper);

            friendlyGameID = "pending";
           
            joinPromptButton.interactable = true;
            cancelPromptButton.interactable = true;
            isLobbyOwner = true;

            StartCoroutine(WaitForFriendlyID());
        }

        public void JoinFriendly()
        {
            joinGamePromptText.text = "Enter Game ID!";
            joinGamePrompt.SetActive(true);
            isLobbyOwner = false;
        }

        public void JoinFriendlyWithID()
        {
            Debug.Log("join friendly");

            if(joinGameInputID.text.Length > 3)
            {
                joinPromptButton.interactable = false;
                cancelPromptButton.interactable = false;

                friendlyGameID = "pending";
                roundWrapper = null;
                gameState.playerSave.FirebaseManager.FriendlyFunctions.JoinFriendlyGame(this, joinGameInputID.text.ToUpper());
                gameState.playerSave.FirebaseManager.FriendlyFunctions.SubscribeToHoles(this, joinGameInputID.text.ToUpper());

                StartCoroutine(WaitForFriendlyID());
            }
            else
            {
                joinGamePromptText.text = "Enter a valid ID";
            }
        }

        public void CancelJoinFriendlyGame()
        {
            Show();
        }

        private IEnumerator WaitForFriendlyID()
        {
            while (friendlyGameID == "pending" || roundWrapper == null)
            {
                yield return new WaitForSeconds(1);
            }
            if (friendlyGameID == "missing")
            {
                joinGamePromptText.text = "ID not found!";
            }
            else
            {
                if (holesRef == null && holesHandler == null)
                {
                    gameState.playerSave.FirebaseManager.FriendlyFunctions.SubscribeToHoles(this, friendlyGameID);
                }
                friendlyGameIDText.text = friendlyGameID;
                joinGamePrompt.SetActive(false);
                ShowLobby();
            }

            joinPromptButton.interactable = true;
            cancelPromptButton.interactable = true;
        }

        public void ShowLobby()
        {
            lobbyPane.SetActive(true);
            tournament.holes = roundWrapper.holes;
            tournament.windSeed = roundWrapper.windSeeds;
            var rm = gameState.NewFriendlyRound();
            rm.StartFriendlyRound(tournament, friendlyGameID);
            startRoundButton.gameObject.SetActive(true);
            leaveGroupButton.gameObject.SetActive(true);

            gameState.playerSave.FirebaseManager.FriendlyFunctions.FriendlySubscribeToScore(this, friendlyGameID);
        }

        public void StartRound()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(gameState.roundManager.currentHole.holeScene);
        }

        public void LeaveGroup()
        {
            UnsubscribeScore();
            UnsubscribeHoles();
            gameState.playerSave.FirebaseManager.FriendlyFunctions.LeaveFriendlyGame(friendlyGameID);
            Show();
        }

        internal void UpdateScoreCards()
        {
            Debug.Log("Update Scorescreen");

            scoreScreen.ClearScores();
            scoreScreen.scoreCards = scoreCards;
            scoreScreen.PopulateScore();

            var rc = IsRoundComplete(scoreCards);

            if (rc)
            {
                scoreScreen.HighlightWinner();
            }

            if (rc && isLobbyOwner && !startingNewRound)
            {
                Debug.Log("Resetting round as admin");
                ResetFriendly();
            }
            if (rc && !startingNewRound)
            {
                startingNewRound = true;
                NewRoundStarting();
            }
        }

        public void ResetFriendly()
        {
            tournament.Generate();
            roundWrapper = new RandomRoundWrapper()
            {
                holes = tournament.holes,
                status = "closed",
                windSeeds = tournament.windSeed
            };

            gameState.playerSave.FirebaseManager.FriendlyFunctions.ResetFriendlyGame(this, roundWrapper, friendlyGameID);
        }

        private IEnumerator WaitForNewRound()
        {
            while (friendlyGameID == "pending" || roundWrapper == null)
            {
                yield return new WaitForSeconds(1);
                Debug.Log(roundWrapper == null);
                Debug.Log("gameID " + friendlyGameID);
            }

            tournament.holes = roundWrapper.holes;
            tournament.windSeed = roundWrapper.windSeeds;
            var rm = gameState.NewFriendlyRound();
            rm.StartFriendlyRound(tournament, friendlyGameID);
            startRoundButton.gameObject.SetActive(true);
            leaveGroupButton.gameObject.SetActive(true);           
        }

        private bool IsRoundComplete(List<ScoreCard> scorecards)
        {
            bool roundComplete = true;

            foreach (ScoreCard card in scoreCards)
            {
               if(card.multiplayerStatus != "rc" && card.multiplayerStatus != "r")
                {
                    roundComplete = false;
                }
            }

            Debug.Log("round complete = " + roundComplete);
            return roundComplete;
        }

        internal void Return(string gameID)
        {
            Show();
            roundWrapper = null;
            gameState.playerSave.FirebaseManager.FriendlyFunctions.panelRef = this;
            gameState.playerSave.FirebaseManager.FriendlyFunctions.FriendlySaveScorecard(gameState.roundManager.scoreCard, gameID);
            
            startRoundButton.gameObject.SetActive(false);
            friendlyGameID = gameID;

            friendlyGameIDText.text = friendlyGameID;

            lobbyPane.SetActive(true);

            gameState.roundManager.EndRound();
            gameState.playerSave.FirebaseManager.FriendlyFunctions.FriendlySubscribeToScore(this, friendlyGameID); 
        }

        private void OnDestroy()
        {
            UnsubscribeScore();
        }

        private void UnsubscribeScore()
        {
            Debug.Log("stop subrcribing to scores");
            if (scoreRef != null && scoreHandler != null)
            {
                scoreRef.ValueChanged -= scoreHandler;
            }
        }

        private void UnsubscribeHoles()
        {
            Debug.Log("stop subrcribing to scores");
            if (holesRef != null && holesHandler != null)
            {
                holesRef.ValueChanged -= holesHandler;
            }
        }

        internal void NewRoundStarting()
        {           
            leaveGroupButton.gameObject.SetActive(false);            
            //UnsubscribeHoles();
            StartCoroutine(NewRoundCountdown());
        }

        private IEnumerator NewRoundCountdown()
        {
            for (int i = 20; i > 0; i--)
            {
                countdownText.text = "new game in " + i;
                yield return new WaitForSeconds(1);
            }
            countdownText.text = "";
            var id = friendlyGameID;
            friendlyGameID = "pending";

            gameState.playerSave.FirebaseManager.FriendlyFunctions.JoinFriendlyGame(this, id);
            StartCoroutine(WaitForNewRound());
        }
    }
}