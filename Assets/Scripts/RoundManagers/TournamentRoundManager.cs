using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class TournamentRoundManager : RoundManager
    {
        public Tournament currentTournament;
        public List<ScoreCard> opponentScoreCards;
        internal string rewardText;
        public int XPreward;
        public int position;
        public int week;

        public void StartTournamentRound(Tournament tournament, int week)
        {
            roundID = System.Guid.NewGuid().ToString();
            currentTournament = tournament;
            FinalHole = false;
            currentHole = currentTournament.holes[0];
            currentHoleNumber = 1;
            rewardUnlocks = new List<Disc>();
            canRestart = true;
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
                icon = playerSave.playerSettings.settingsData.icon
                //courseID = 0
            };
           
            opponentScoreCards = new List<ScoreCard>();

            foreach (AIOpponent opp in currentTournament.opponents)
            {
                opponentScoreCards.Add(new ScoreCard()
                {
                    ownerName = opp.opponentName,
                    //isPlayer = false,
                    scores = new int[currentTournament.holes.Count],
                    diffFromPar = new int[currentTournament.holes.Count],
                   // courseID = 0
                });
            }
            SetAIScores();
        }

        private void SetAIScores()
        {
            foreach (ScoreCard card in opponentScoreCards)
            {
                var score = currentTournament.AIScore(currentHole.par);
                card.scores[currentHoleNumber - 1] = score;

                var diff = score - currentHole.par;
                card.diffFromPar[currentHoleNumber - 1] = diff;

                card.score += score;                

                card.totalDiffFromPar += diff;
            }
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
            
            SetAIScores();
        }

        internal override void Restart()
        {
            SceneManager.LoadScene(currentTournament.holes[0].holeScene);
            StartTournamentRound(currentTournament, week);
        }

        internal override void EndRound()
        {
            roundComplete = true;

            if (currentTournament.type != Tournament.TournamentType.Open)
            {
                List<ScoreCard> scoreCards = new List<ScoreCard>();
                scoreCards.Add(scoreCard);
                scoreCards.AddRange(opponentScoreCards);
                scoreCards.Sort((x, y) => x.score.CompareTo(y.score));

                position = scoreCards.IndexOf(scoreCard) + 1;

                Debug.Log("Player final position= " + position);
                var calc = currentTournament.XPReward * (1.1f - (position * 0.1f));
                XPreward = (int)calc;
                rewardText = "Tournament Reward";
                //roundRating = RatingFunctions.CalculateRating(currentTournament.holes, scoreCard.score, 1f);
                //playerSave.playerStats.NewRoundRating(roundRating);
            }
            else
            {
                playerSave.FirebaseManager.SubmitOpenScore(scoreCard.score, scoreCard.totalDiffFromPar, playerSave.playerStats.statsData.division, week);
            }
        }
    }
}