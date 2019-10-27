using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class ProTourRoundManager : RoundManager
    {
        public Tournament currentTournament;
        private ProTourWrapper.Division division;
        private int round;
        private int week;

        public void StartProTourRound(Tournament tournament, ProTourWrapper.Division division, int round, int week)
        {
            roundID = System.Guid.NewGuid().ToString();
            currentTournament = tournament;
            FinalHole = false;
            currentHole = currentTournament.holes[0];
            currentHoleNumber = 1;
            this.division = division;
            this.round = round;
            this.week = week;

            gotHoleStats = false;
            holeBest = 0;
            holes = currentTournament.holes;
            
            scoreCard = new ScoreCard()
            {
                ownerName = playerSave.PlayerDisplayName(),
               // isPlayer = true,
                scores = new int[currentTournament.holes.Count],
                diffFromPar = new int[currentTournament.holes.Count],
                icon = playerSave.playerSettings.settingsData.icon,

                // courseID = 0
            };
            playerSave.FirebaseManager.SubmitProTourScore(999, 0, division, round, week);
        }

        internal override void NextHole()
        {
            currentHole = currentTournament.holes[currentHoleNumber];
            currentHoleNumber++;

            if (currentHoleNumber >= holes.Count)
            {
                FinalHole = true;
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentHole.holeScene);
            gotHoleStats = false;                        
        }
        
        internal override void EndRound()
        {
            roundComplete = true;
            playerSave.FirebaseManager.SubmitProTourScore(scoreCard.score, scoreCard.totalDiffFromPar, division, round, week);
            roundRating= RatingFunctions.CalculateRating(currentTournament.holes, scoreCard.score, 4f);
            playerSave.playerStats.NewRoundRating(roundRating);
        }
    }
}