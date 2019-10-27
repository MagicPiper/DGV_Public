using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class TournamentPanel : MenuPane
    {
        public Tournament currentTournament;
        private int tournamentPar;
        public GameState gameState;

        public Button startOpenTournament;
        public TMP_Text playButtonText;

        [SerializeField] private ScoreLine scorePrefab;
        [SerializeField] private Transform scoreHolder;
        [SerializeField] private ScrollRect scoreScrollRect;

        public bool hasOpenTournamentData;
        public bool hasOpenTournamentScores;
        public bool countdown = false;
        public List<OpenScore> scores;
        public List<ScoreLine> scoreObjects;
        public Tournament openTournament;
        private DateTime openTournamentEndTime;
        public GameObject needToUpdateNote;

        public TMP_Text timeLeftText;
        public TMP_Text divisionText;
        internal string hasResult;
        internal OpenResult result;
        public bool hasHoles;
        internal OpenTournamentWrapper openWrapper;
        internal RandomRoundWrapper openHoles;
        public ResultPanel resultPanel;
        public RewardPanel rewardPanel;
        private OpenScore playerScore;
        private bool hasPlayerScore;

        // public int scrollTo;

        private void OnEnable()
        {
            //gameState.playerSave.GetOpenReward();
            hasResult = "pending";
            hasOpenTournamentData = false;
            hasOpenTournamentScores = false;
            divisionText.text = "Division: " + gameState.playerSave.playerStats.statsData.division.ToString();
            gameState.playerSave.FirebaseManager.GetOpenTournament(this, gameState.playerSave.playerStats.statsData.division);
            startOpenTournament.interactable = false;
            playButtonText.text = "play";

            //gameState.playerSave.FirebaseManager.GetCurrentVersion();
            StartCoroutine(LoadOpenTournament());
            // StartCoroutine(LoadOpenTournamentScores());
        }

        private void Update()
        {
            if (countdown)
            {
                var now = DateTime.UtcNow;
                var timeLeft = (openTournamentEndTime - now).ToString(@"hh\:mm\:ss");
                timeLeftText.text = timeLeft;
            }
        }

        public void StartTournament(Tournament tournament)
        {
            currentTournament = tournament;
            currentTournament.Generate();

            var rm = gameState.NewTournamentRound();
            rm.StartTournamentRound(currentTournament, 0);

            StartCoroutine("LoadNewScene");
            //playButtonText.text = "Loading";
            MusicManager.Instance.FadeOutMusic();
        }

        public void StartOpenTournament()
        {
            gameState.playerSave.FirebaseManager.OpenGetHoles(this, openWrapper.round);
            hasHoles = false;
            StartCoroutine(StartOpenCR());
        }

        public IEnumerator StartOpenCR()
        {
            while (!hasHoles)
            {
                yield return new WaitForSeconds(1f);
            }

            var rm = gameState.NewTournamentRound();
            openTournament.holes = openHoles.holes;
            openTournament.windSeed = openHoles.windSeeds;

            rm.StartTournamentRound(openTournament, openWrapper.week);

            MusicManager.Instance.FadeOutMusic();

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(gameState.roundManager.currentHole.holeScene);

        }

        private IEnumerator LoadOpenTournament()
        {
            while (!hasOpenTournamentData || !hasOpenTournamentScores || hasResult == "pending" || !hasPlayerScore)
            {
                yield return new WaitForSeconds(1);
            }

            PopulateOpenTournamentScores();
            PopulateOpenTournament();
            hasResult = "pending";
            hasOpenTournamentData = false;
            hasOpenTournamentScores = false;

            yield return null;
        }

        private void PopulateOpenTournamentScores()
        {
            if (hasResult == "true")
            {
                resultPanel.gameObject.SetActive(true);

                resultPanel.PopulateOpen(result);
            }

            if (scoreObjects != null)
            {
                foreach (ScoreLine o in scoreObjects)
                {
                    Destroy(o.gameObject);
                }
            }
            scoreObjects = new List<ScoreLine>();
            Debug.Log("populating scores");
            int pos = 0;
            int actualPos = 0;
            int lastScore = 0;
            int playerPos = 1;
            scores.Sort((x, y) => x.score.CompareTo(y.score));

            foreach (OpenScore score in scores)
            {
                actualPos++;

                if (score.score != lastScore)
                {
                    pos = actualPos;

                }
                lastScore = score.score;
                var s = Instantiate(scorePrefab, scoreHolder);
                var isMe = false;

                if (score.userid == gameState.playerSave.FirebaseManager.playerID)
                {
                    isMe = true;
                    playerPos = actualPos;
                }

                s.Populate(pos, score.userName, score.score, isMe, score.proRating, score.parDiff.ToString(), score.icon);
                scoreObjects.Add(s);
            }

            if (playerScore != null)
            {
                var p = Instantiate(scorePrefab, scoreHolder);
                var isMe = true;
                playerPos = 25;
                p.Populate(9999, playerScore.userName, playerScore.score, isMe, playerScore.proRating, playerScore.parDiff.ToString(), playerScore.icon);
                scoreObjects.Add(p);
            }

            StartCoroutine(ScrollToPlayerScore(playerPos, actualPos));
        }

        public void CloseResult()
        {
            gameObject.SetActive(false);
            resultPanel.gameObject.SetActive(false);

            rewardPanel.XPReward(CalculateXPGain(result.position), result.position, "Open Reward");

            Return();
        }

        internal void Return()
        {
            Invoke("Show", 3f);
        }

        public int CalculateXPGain(int position)
        {
            var xp = 50 + ((26 - position) * 10);
            return xp < 150 ? 150 : xp;
        }

        private IEnumerator ScrollToPlayerScore(int playerPos, int total)
        {
            yield return new WaitForSeconds(0.1f);
            float pos = (float)(playerPos - 1) / (float)total;
            pos = pos > 1.0f ? 1.0f : pos;
            pos = pos < 0.0f ? 0.0f : pos;

            Debug.Log("Scroll Position: " + total + " " + playerPos + " " + pos);

            scoreScrollRect.verticalNormalizedPosition = (1f - pos);
        }



        //[ContextMenu("test course")]
        //public void TestSetCourseScore()
        //{
        //    gameState.playerSave.FirebaseManager.SubmitCourseScore("0", 20);
        //}

        public void GotOpenData(OpenTournamentWrapper tournament)
        {
            openWrapper = tournament;
            var t = DateTime.UtcNow.Date;
            openTournamentEndTime = t.AddDays(1);
            hasOpenTournamentData = true;
            countdown = true;
        }

        private void PopulateOpenTournament()
        {
            var currentVersion = float.Parse(Application.version, CultureInfo.InvariantCulture);

            if (gameState.playerSave.LatestVersion > currentVersion + 0.002 || openWrapper.closed == true)
            {
                playButtonText.text = "closed";
            }
            else
            {
                startOpenTournament.interactable = true;
            }
        }

        IEnumerator LoadNewScene()
        {
            // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
            // AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(gameState.roundManager.currentHole.holeScene);
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(gameState.roundManager.currentHole.holeScene);

            // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
            //while (!async.isDone)
            //{
            //    var t = Mathf.PingPong(Time.time, 1f);
            //    playButtonText.alpha = t;
            yield return null;
            //}
        }

        internal void GotOpenScores(List<OpenScore> scores)
        {
            Debug.Log("gotopenscores " + scores.Count);
            this.scores = scores;
            hasOpenTournamentScores = true;
        }

        internal void GotPlayerScore(OpenScore playerScore)
        {
            Debug.Log("got player score");
            this.playerScore = playerScore;
            hasPlayerScore = true;
        }

        internal void GotPlayerScore()
        {
            Debug.Log("got player score. In top 25");
            hasPlayerScore = true;
        }
    }
}