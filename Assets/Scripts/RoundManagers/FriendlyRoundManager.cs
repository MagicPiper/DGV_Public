using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Firebase.Database;

namespace Assets.Scripts
{
    public class FriendlyRoundManager : RoundManager
    {
        public Tournament currentTournament;
        public List<ScoreCard> scoreCards;        
        public string gameID;
        private ScoreScreen scoreScreenReturn;
        
        public void StartFriendlyRound(Tournament tournament, string gameID)
        {
            currentTournament = tournament;
            FinalHole = false;
            currentHole = currentTournament.holes[0];
            currentHoleNumber = 1;
            gotHoleStats = false;
            holeBest = 0;
            holes = currentTournament.holes;
            this.gameID = gameID;

            scoreCard = new ScoreCard()
            {
                ownerName = playerSave.PlayerDisplayName(),
                playerID = playerSave.FirebaseManager.playerID,
                
                scores = new int[currentTournament.holes.Count],
                diffFromPar = new int[currentTournament.holes.Count],
                multiplayerStatus = "ig",
                icon = playerSave.playerSettings.settingsData.icon               
            };
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
              
                playerSave.FirebaseManager.FriendlyFunctions.FriendlySaveScorecard(scoreCard, gameID);
            }
        }

        public void GetScoredelayed(ScoreScreen scoreScreen)
        {
            playerSave.FirebaseManager.FriendlyFunctions.FriendlyGetScore(this, gameID);
            scoreScreenReturn = scoreScreen;
        }

        internal override void NextHole()
        {
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
            //playerSave.FirebaseManager.FriendlySaveScorecard(scoreCard, gameID);
           // roundRating= RatingFunctions.CalculateRating(currentTournament.holes, scoreCard.score, 2f);
        }

        internal override void Retire()
        {
            scoreCard.multiplayerStatus = "r";
            playerSave.FirebaseManager.FriendlyFunctions.FriendlySaveScorecard(scoreCard, gameID);  
            
            base.Retire();
        }

        internal void UpdateScore(List<ScoreCard> cards)
        {
            Debug.Log("RoundManager got new cards");

            scoreCards = cards;
            scoreScreenReturn.NewScoresAvailable(cards);
        }
    }
}