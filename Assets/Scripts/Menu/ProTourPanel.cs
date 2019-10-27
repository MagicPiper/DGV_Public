using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class ProTourPanel : MenuPane
    {
        public GameState gameState;
        public bool hasTourData;
        public Transform roundButtonPane;
        private ProTourWrapper tour;
        private List<ProTourScore> scores;
        private bool hasScoreData;
        public TMP_Text divisionText;
        public TMP_Text weekText;
        public GameObject closedPane;
        public ProTourRoundButton[] roundButtons;
        public ProScoreLine scoreLinePrefab;
        public List<ProScoreLine> scoreLines;
        public Transform scoreLineHolder;
        private ProTourScoreSortable playerScore;
        internal RandomRoundWrapper tournamentWrapper;
        internal bool hasHoles;
        public Transform confirmPlayPane;

        public Transform prizeDetailsPane;
        public GameObject prizeButton;
        public TMP_InputField prizeEmailTextbox;

        public GameObject playButton;
        public GameObject cancelButton;
        private int currentRoundNumber;
        public Tournament tournament;
        internal string hasResult;
        internal ProTourResult result;
        public ResultPanel resultPanel;
        public RewardPanel rewardPanel;
        private ProTourScore playerScoreObject;
        private bool hasPlayerScore;

        private void OnEnable()
        {
            if (scoreLines != null)
            {
                foreach (ProScoreLine line in scoreLines)
                {
                    Destroy(line.gameObject);
                }
            }
            scoreLines = new List<ProScoreLine>();

            hasResult = "pending";
            hasTourData = false;
            hasScoreData = false;
            roundButtonPane.gameObject.SetActive(false);

            gameState.playerSave.FirebaseManager.GetProTour(this, gameState.playerSave.playerStats.statsData.division);
            StartCoroutine(LoadProTour());
        }

        private IEnumerator LoadProTour()
        {
            while (!hasTourData || !hasScoreData || hasResult == "pending" || !hasPlayerScore)
            {
                yield return new WaitForSeconds(1);
            }

            PopulateTour();
            hasResult = "pending";
            hasTourData = false;
            hasScoreData = false;

            yield return null;
        }

        private void PopulateTour()
        {
            if (hasResult == "true")
            {
                if (result.completeRound)
                {
                    resultPanel.gameObject.SetActive(true);
                    var diff = gameState.playerSave.playerStats.UpdateRating();
                    resultPanel.PopulateProTour(result, (int)gameState.playerSave.playerStats.statsData.playerRating, diff);
                }
                else
                {
                    gameState.playerSave.playerStats.UpdateRating();
                }
            }

            divisionText.text = gameState.playerSave.playerStats.statsData.division.ToString() + " Division";
            weekText.text = "Pro Tour Week " + tour.week;
            if (scores != null)
            {
                var sortedScores = new List<ProTourScoreSortable>();
                foreach (ProTourScore score in scores)
                {
                    int totalscore = 0;
                    int validRounds = 0;
                    for (int iter = 0; iter < score.scores.Length; iter++)
                    {
                        totalscore += score.scores[iter];
                        if (score.scores[iter] < 999 && score.scores[iter] > 0) validRounds++;
                    }

                    var s = new ProTourScoreSortable()
                    {
                        scores = score.scores,
                        pars = score.pars,
                        playerID = score.playerID,
                        playerName = score.playerName,
                        totalScore = totalscore,
                        completedRounds = validRounds,
                        icon = score.icon
                    };
                    sortedScores.Add(s);
                }
                List<ProTourScoreSortable> things = sortedScores.OrderByDescending(s => s.completedRounds).ThenBy(s => s.totalScore).ToList<ProTourScoreSortable>();

                int counter = 0;
                foreach (ProTourScoreSortable score in things)
                {
                    // Debug.Log("playerName " + score.playerName + " CompletedRounds " + score.completedRounds + "total " + score.totalScore);

                    var scoreObject = Instantiate(scoreLinePrefab, scoreLineHolder);
                    scoreLines.Add(scoreObject);
                    if (score.playerID == gameState.playerSave.FirebaseManager.playerID)
                    {
                        playerScore = score;
                        scoreObject.Populate(score, true, counter);
                    }
                    else
                    {
                        scoreObject.Populate(score, false, counter);
                    }
                    counter++;
                }
                if(playerScoreObject != null)
                {
                    var scoreObject = Instantiate(scoreLinePrefab, scoreLineHolder);
                    scoreLines.Add(scoreObject);

                    int ptotalscore = 0;
                    int pvalidRounds = 0;
                    for (int iter = 0; iter < playerScoreObject.scores.Length; iter++)
                    {
                        ptotalscore += playerScoreObject.scores[iter];
                        if (playerScoreObject.scores[iter] < 999 && playerScoreObject.scores[iter] > 0) pvalidRounds++;
                    }

                    var p = new ProTourScoreSortable()
                    {
                        scores = playerScoreObject.scores,
                        pars = playerScoreObject.pars,
                        playerID = playerScoreObject.playerID,
                        playerName = playerScoreObject.playerName,
                        totalScore = ptotalscore,
                        completedRounds = pvalidRounds,
                        icon = playerScoreObject.icon
                    };
                    playerScore = p;
                    scoreObject.Populate(p, true, counter);
                }
            }

            if (!tour.closed)
            {
                closedPane.SetActive(false);
                roundButtonPane.gameObject.SetActive(true);

                int i = 0;
                int daysLeft = 1;
                foreach (ProTourRoundButton buttin in roundButtons)
                {
                    if (tour.rounds[i].unlocked)
                    {
                        //Debug.Log("button " + i + " is unlocked and player score is " + playerScore.scores[i]);
                        if (playerScore != null && playerScore.scores != null && playerScore.scores[i] > 0)
                        {
                            roundButtons[i].HasPlayed();
                        }
                        else
                        {
                            roundButtons[i].Unlock();
                        }
                    }
                    else
                    {
                        roundButtons[i].Lock(daysLeft);
                        daysLeft++;
                        //buttin.button.interactable = false;
                    }
                    i++;
                }

                if (tour.prizeWeek)
                {
                    prizeButton.SetActive(true);
                }

            }
            else
            {                
                closedPane.SetActive(true);
                roundButtonPane.gameObject.SetActive(false);
            }
        }

        public void StartTourRound(int roundNumber)
        {
            gameState.playerSave.FirebaseManager.ProTourGetHoles(this, tour.rounds[roundNumber].holesID);
            hasHoles = false;
            StartCoroutine(StartTourRoundCR(roundNumber));
            confirmPlayPane.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
        }

        public IEnumerator StartTourRoundCR(int roundNumber)
        {
            while (!hasHoles)
            {
                yield return new WaitForSeconds(1f);
            }
            currentRoundNumber = roundNumber;
            playButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
        }

        public void ShowPrizeDetails()
        {
            prizeDetailsPane.gameObject.SetActive(true);
            prizeEmailTextbox.text = gameState.playerSave.PlayerEmail();
        }

        public void HidePrizeDetails()
        {
            gameState.playerSave.playerStats.SaveEmail(prizeEmailTextbox.text);
            prizeDetailsPane.gameObject.SetActive(false);
        }

        public void ConfirmStartRound()
        {
            var rm = gameState.NewProTourRound();
            tournament.holes = tournamentWrapper.holes;
            tournament.windSeed = tournamentWrapper.windSeeds;

            rm.StartProTourRound(tournament, gameState.playerSave.playerStats.statsData.division, currentRoundNumber, tour.week);

            MusicManager.Instance.FadeOutMusic();

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(gameState.roundManager.currentHole.holeScene);
        }

        public void CancelStartRound()
        {
            hasHoles = false;
            currentRoundNumber = 0;
            playButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            confirmPlayPane.gameObject.SetActive(false);
        }

        //[ContextMenu("Create Tour Data")]
        //private void SetTourData()
        //{
        //    gameState.playerSave.FirebaseManager.CreateProTour();
        //}

        [ContextMenu("Set Tour Score")]
        private void SetTourScore()
        {
            gameState.playerSave.FirebaseManager.SubmitProTourScore(10, 0, ProTourWrapper.Division.Recreational, 0, 0);
        }

        internal void GotTourData(ProTourWrapper proTour)
        {
            this.tour = proTour;
            hasTourData = true;
        }


        public int CalculateXPGain(int position)
        {
            var xp = 375 + ((26 - position) * 25);
            return xp < 500 ? 500 : xp;
        }

        public void CloseResult()
        {
            gameObject.SetActive(false);
            resultPanel.gameObject.SetActive(false);

            rewardPanel.XPReward(CalculateXPGain(result.position), result.position, "Pro Tour Reward");

            Return();
        }

        internal void Return()
        {
            Invoke("Show", 3f);
        }

        internal void GotScoreData(List<ProTourScore> scores)
        {
            this.scores = scores;
            hasScoreData = true;
        }

        internal void GotPlayerScore(ProTourScore playerScoreObject)
        {
            Debug.Log("got player score");
            this.playerScoreObject = playerScoreObject;
            hasPlayerScore = true;
        }

        internal void GotPlayerScore()
        {
            Debug.Log("got player score");            
            hasPlayerScore = true;
        }
    }
}
