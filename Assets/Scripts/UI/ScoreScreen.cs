using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;
namespace Assets.Scripts
{

    public class ScoreScreen : MonoBehaviour
    {
        public CanvasGroup screen;

        public ScoreScreenRow scoreScreenRowPrefab;
        public ScoreScreenCell scoreScreenCellPrefab;
        public RectTransform scoreCardRect;
        public Transform holeHolder;
        public TMP_Text courseName;
        public GameObject retireButton;
        public TMP_Text retireButtonText;
        public GameObject restartButton;
        public TMP_Text restartButtonText;
        public GameObject nextHoleButton;
        public GameObject finishRoundButton;
        public GameState gameState;
        public PlayerIconData iconData;

        [SerializeField] private Color birdieColor;
        [SerializeField] private Color bogeyColor;
        [SerializeField] private Color parColor;
        [SerializeField] private Color diffColor;

        [SerializeField] private TMP_Text holeText;
        [SerializeField] private TMP_Text parText;
        [SerializeField] private TMP_Text strokesText;
        [SerializeField] private TMP_Text holeBestText;
        [SerializeField] private TMP_Text holeRatingText;

        private int strokes;
        private bool confirmRetire;
        private bool confirmRestart;

        public List<ScoreCard> scoreCards = new List<ScoreCard>();
        private bool newScoresAvailable;
        public List<ScoreScreenRow> scoreLines = new List<ScoreScreenRow>();
        //public bool hasMultiScore;
        //public Coroutine waitForScores;

        public TMP_Text timeLeftText;
        public bool countdown;
        public DateTime endTime;
        // internal EventHandler<ValueChangedEventArgs> scoreHandler;
        //internal DatabaseReference scoreRef;
        private float nextUpdate;
        internal List<Hole> holeData;

        private void Update()
        {
            if (countdown)
            {
                var now = DateTime.UtcNow;

                if (endTime < now)
                {
                    timeLeftText.text = "Time left: <color=yellow>00:00";
                    if (nextHoleButton != null)
                    {
                        nextHoleButton.SetActive(false);
                    }
                    countdown = false;
                }
                else
                {
                    var timeLeft = (endTime - now).ToString(@"mm\:ss");
                    timeLeftText.text = "Time left: <color=yellow>" + timeLeft;
                }                
            }
            if (Time.time >= nextUpdate)
            {
                // Change the next update (current second+1)
                nextUpdate = Mathf.FloorToInt(Time.time) + 1;
                // Call your fonction
                // Debug.Log("update scorescreen tick");

                if (newScoresAvailable)
                {
                    UpdateScore();
                }
            }
        }

        public void ShowScoreScreen(int strokes)
        {
            this.strokes = strokes;
            Score();
        }

        private void Score()
        {
            scoreCards.Add(gameState.roundManager.scoreCard);

            int height = 800;
            int width = 950;

            if (gameState.roundManager.holes.Count > 7)
            {
                width += (gameState.roundManager.holes.Count - 7) * 90;
            }

            if (gameState.roundManager is TournamentRoundManager)
            {
                TournamentRoundManager round = (TournamentRoundManager)gameState.roundManager;

                if (round.currentTournament.type != Tournament.TournamentType.Open)
                {
                    height = 1000;
                }

                scoreCards.AddRange(round.opponentScoreCards);
                scoreCards.Sort((x, y) => x.score.CompareTo(y.score));
            }
            if (gameState.roundManager is MultiplayerRoundManager)
            {
                height = 1000;
                var rm = (MultiplayerRoundManager)gameState.roundManager;
                rm.GetScoredelayed(this);
                scoreCards = rm.scoreCards;
                endTime = rm.endTime;
                countdown = true;
            }
            if (gameState.roundManager is FriendlyRoundManager)
            {
                height = 1000;
                var rm = (FriendlyRoundManager)gameState.roundManager;
                rm.GetScoredelayed(this);
                scoreCards = rm.scoreCards;             
            }

            scoreCardRect.sizeDelta = new Vector2(width, height);
            screen.alpha = 0;
            LeanTween.alphaCanvas(screen, 1f, 1.5f).setEaseOutExpo().setDelay(0.2f);
            courseName.text = gameState.roundManager.roundName;

            PopulateScore();

            holeText.text = "Hole " + gameState.roundManager.currentHoleNumber;
            strokesText.text = strokes.ToString();

            var diff = strokes - gameState.roundManager.currentHole.par;

            parText.text = DiffText(diff);
            parText.color = DiffColor(diff);

            if (gameState.roundManager.canRestart)
            {
                restartButton.SetActive(true);
            }
            else
            {
                restartButton.SetActive(false);
            }

            if (gameState.roundManager.FinalHole)
            {
                retireButton.SetActive(false);
                restartButton.SetActive(false);
                nextHoleButton.SetActive(false);
                finishRoundButton.SetActive(true);
                MusicManager.Instance.PlayPostRoundMusic();
                gameState.roundManager.EndRound();
                RoundRating();
            }
        }

        private void RoundRating()
        {
            //  Debug.Log("Rating round: " + gameState.roundManager.roundRating);
            if (gameState.roundManager.roundRating != null)
            {
                holeRatingText.gameObject.SetActive(true);
                holeRatingText.text = gameState.roundManager.roundRating.rating.ToString("#");
            }
        }

        public void PopulateScore()
        {
            if (gameState.roundManager != null && gameState.roundManager is MultiplayerRoundManager)
            {
                var rm = (MultiplayerRoundManager)gameState.roundManager;
                scoreCards = rm.scoreCards;
            }
            int i = 0;
            if (scoreCards == null || scoreCards.Count < 1)
            {
                Debug.LogWarning("No ScoreCards");
                return;
            }
            foreach (ScoreCard scoreCard in scoreCards)
            {
                var row = Instantiate(scoreScreenRowPrefab);
                row.transform.SetParent(holeHolder, false);
                scoreLines.Add(row);
                ScoreCells(scoreCard, row.transform);

                row.nameText.text = scoreCard.ownerName;
                row.icon.sprite = iconData.GetIcon(scoreCard.icon);
                // var ratingtext = scoreCard.rating == 0 ? "" : scoreCard.rating.ToString();
                //row.ratingText.text = ratingtext;
                if (scoreCard.multiplayerStatus == "retired")
                {
                    row.nameText.color = Color.red;
                }

                if (scoreCard.playerID == gameState.playerSave.FirebaseManager.playerID)
                {
                    row.nameText.color = Color.yellow;
                }

                i++;
            }
        }

        internal void ClearScores()
        {
            foreach (ScoreScreenRow score in scoreLines)
            {
                Destroy(score.gameObject);
            }
            scoreLines = new List<ScoreScreenRow>();
        }

        private void ScoreCells(ScoreCard scoreCard, Transform scoreCardRow)
        {
            int totalDiffFromPar = 0;
            int holeNumber = 1;

            holeData = holeData == null ? gameState.roundManager.holes : holeData;
            
            foreach (int hole in scoreCard.scores)
            {
                var score = Instantiate(scoreScreenCellPrefab);
                score.transform.SetParent(scoreCardRow, false);
                var manager = score.GetComponent<ScoreScreenCell>();

                manager.holeText.text = holeNumber.ToString();
                var s = scoreCard.scores[holeNumber - 1];
                if (s == 0)
                {
                    manager.scoreText.text = "-";
                }
                else
                {
                    manager.scoreText.text = s.ToString();
                    var diff = s - holeData[holeNumber - 1].par;
                    totalDiffFromPar += diff;
                    manager.background.color = DiffColor(diff);
                }
                holeNumber++;
            }
            var totalCell = Instantiate(scoreScreenCellPrefab);
            totalCell.transform.SetParent(scoreCardRow, false);
            var m = totalCell.GetComponent<ScoreScreenCell>();
            m.scoreText.text = scoreCard.score.ToString();
            m.holeText.text = "Total";
            m.holeText.fontSize = 30f;
            m.background.color = Color.grey;

            var parCell = Instantiate(scoreScreenCellPrefab);
            parCell.transform.SetParent(scoreCardRow, false);
            var p = parCell.GetComponent<ScoreScreenCell>();

            var pText = totalDiffFromPar > 0 ? "+" + totalDiffFromPar.ToString() : totalDiffFromPar.ToString();

            p.scoreText.text = totalDiffFromPar == 0 ? "E" : pText;
            p.holeText.text = "Par";
            p.background.color = diffColor;

        }

        private Color DiffColor(int diff)
        {
            switch (diff)
            {
                case 3:
                    return bogeyColor;
                case 2:
                    return bogeyColor;
                case 1:
                    return bogeyColor;
                case 0:
                    return parColor;
                case -1:
                    return birdieColor;
                case -2:
                    return Color.blue;
                case -3:
                    return Color.yellow;
                default:
                    return bogeyColor;

            }
        }

        private string DiffText(int diff)
        {
            switch (diff)
            {
                case 3:
                    return "Triple Bogey";
                case 2:
                    return "Double Bogey";
                case 1:
                    return "Bogey";
                case 0:
                    return "Par";
                case -1:
                    return "Birdie!";
                case -2:
                    return "Eagle!";
                case -3:
                    return "Albatross!";
                default:
                    return "Bogey " + diff;
            }
        }
       
        public void NextHole()
        {
            //UnsubscribeScores();
            gameState.roundManager.NextHole();
        }

        public void EndRound()
        {
            //UnsubscribeScores();
            gameState.roundManager.BackToMenu();
        }

        public void Retire()
        {
            //UnsubscribeScores();
            if (confirmRetire)
            {
                gameState.roundManager.Retire();
            }
            else
            {
                confirmRetire = true;
                var text = "Tap again to confirm";
                if (gameState.roundManager is MultiplayerRoundManager)
                {
                    text = "Confirm: -rating";
                }
                retireButtonText.text = text;
            }
        }

        public void Restart()
        {
            if (confirmRestart)
            {
                gameState.roundManager.Restart();
            }
            else
            {
                confirmRestart = true;
                var text = "Tap again to confirm";
                restartButtonText.text = text;
            }
        }

        internal void UpdateScore()
        {
            Debug.Log("Update Score");            
            ClearScores();
            PopulateScore();
            newScoresAvailable = false;
        }

        internal void NewScoresAvailable(List<ScoreCard> scoreCards)
        {
            Debug.Log("new scores available");
            this.scoreCards = scoreCards;
            newScoresAvailable = true;    
        }

        internal void HighlightWinner()
        {            
            int bestScore = 999;
            int current = 0;
            int winner = 0;

            foreach (ScoreCard card in scoreCards)
            {
                if(card.score < bestScore && card.multiplayerStatus != "retired")
                {
                    winner = current;
                    bestScore = card.score;
                }
                current++;
            }

            Debug.Log("winner = " + scoreCards[winner].ownerName);
            scoreLines[winner].winnerCrownIcon.SetActive(true);
            scoreLines[winner].winnerCrownIcon.transform.SetAsLastSibling();
        }

        //internal void UnsubscribeScores()
        //{
        //    if (scoreHandler != null)
        //    {
        //        scoreRef.ValueChanged -= scoreHandler;
        //    }
        //}
    }
}
