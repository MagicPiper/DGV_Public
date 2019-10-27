using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Firebase.Database;

namespace Assets.Scripts
{
    //  [CreateAssetMenu(fileName = "RoundManager", menuName = "DiscGolf/Round Manager", order = 7)]
    public class MultiplayerRoundManager : RoundManager
    {
        public Tournament currentTournament;
        public List<ScoreCard> scoreCards;
        public DateTime endTime;
        private ScoreScreen scoreScreenReturn;

        //internal EventHandler<ValueChangedEventArgs> scoreHandler;
        //internal DatabaseReference scoreRef;

        public void StartMultiplayerRound(Tournament tournament, string id, DateTime endTime)
        {
            roundID = id;
            currentTournament = tournament;
            FinalHole = false;
            currentHole = currentTournament.holes[0];
            currentHoleNumber = 1;
            gotHoleStats = false;
            holeBest = 0;
            holes = currentTournament.holes;
            this.endTime = endTime;

            scoreCard = new ScoreCard()
            {
                ownerName = playerSave.PlayerDisplayName(),
                playerID = playerSave.FirebaseManager.playerID,
                //isPlayer = false,
                scores = new int[currentTournament.holes.Count],
                diffFromPar = new int[currentTournament.holes.Count],
                multiplayerStatus = "ig",
                icon = playerSave.playerSettings.settingsData.icon
                //rating = (int)playerSave.playerStats.statsData.playerRating
            };
            
            playerSave.playerStats.NewRoundRating(RatingFunctions.AbandonedRound());            
        }

        internal override void HoleComplete(int strokes, string holeID)
        {
            if (scoreCard.scores[currentHoleNumber - 1] == 0)
            {
                scoreCard.scores[currentHoleNumber - 1] = strokes;
                var diff = strokes - currentHole.par;
                scoreCard.diffFromPar[currentHoleNumber - 1] = diff;
                scoreCard.score = scoreCard.score + strokes;
                scoreCard.totalDiffFromPar += diff;
                
                playerSave.FirebaseManager.MultiPlayerFunctions.MultiPlayerSaveScorecard(scoreCard, roundID);
            }
        }

        public void GetScoredelayed(ScoreScreen scoreScreen)
        {
            playerSave.FirebaseManager.MultiPlayerFunctions.MultiplayerGetScore(this, roundID);
            scoreScreenReturn = scoreScreen;
        }

        internal override void NextHole()
        {
            UnsubscribeScores();

            currentHole = currentTournament.holes[currentHoleNumber];
            currentHoleNumber++;

            if (currentHoleNumber >= holes.Count)
            {
                FinalHole = true;
            }
            SceneManager.LoadScene(currentHole.holeScene);
            MusicManager.Instance.FadeOutMusic();

            gotHoleStats = false;           
        }

        internal override void EndRound()
        {
            roundComplete = true;
            scoreCard.multiplayerStatus = "rc";
            playerSave.FirebaseManager.MultiPlayerFunctions.MultiPlayerSaveScorecard(scoreCard, roundID);
            roundRating= RatingFunctions.CalculateRating(currentTournament.holes, scoreCard.score, 2f);
            playerSave.playerStats.MultiplayerOverwrite(roundRating);
            UnsubscribeScores();
        }        

        internal override void Retire()
        {
            scoreCard.multiplayerStatus = "r";
            playerSave.FirebaseManager.MultiPlayerFunctions.MultiPlayerSaveScorecard(scoreCard, roundID, true);            
            
            UnsubscribeScores();
            base.Retire();
        }

        internal void UnsubscribeScores()
        {
            //if (scoreHandler != null)
            //{
            //    scoreRef.ValueChanged -= scoreHandler;
            //}
        }

        internal void UpdateScore(List<ScoreCard> cards)
        {
            scoreCards = cards;
            scoreScreenReturn.NewScoresAvailable(cards);
            Debug.Log("RoundManager got new cards");
        }
    }
}