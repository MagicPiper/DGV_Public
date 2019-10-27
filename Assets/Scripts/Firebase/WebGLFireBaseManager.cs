//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UnityEngine;
//using System.Runtime.InteropServices;
//using System.Threading.Tasks;

//namespace Assets.Scripts.Menu
//{

//    public class WebGLFireBaseManager: FireBaseManager
//    {
//        private JSListener jsListener;

//        [DllImport("__Internal")]
//        private static extern string GetScoreData(string path, string responseFunction);

//        [DllImport("__Internal")]
//        private static extern void UpdateHighscore(string path, int score);

//        [DllImport("__Internal")]
//        private static extern void IsNewPlayer();

//        [DllImport("__Internal")]
//        private static extern void WebLoadPlayerData();

//        [DllImport("__Internal")]
//        private static extern void WebUpdatePlayerData(string data);

//        [DllImport("__Internal")]
//        private static extern void WebPlayerAuthenticatedCheck();

//        [DllImport("__Internal")]
//        private static extern void WebSaveHoleScore(string guid, int score, string holeID);

//        [DllImport("__Internal")]
//        private static extern void WebGetHoleScore(string holeID);

//        [DllImport("__Internal")]
//        private static extern void WebGetOpenTournament();

//        public override void NewPlayerCheck()
//        {
//            IsNewPlayer();
//        }

//        public override void GetPlayerData()
//        {
//            WebLoadPlayerData();
//        }

//        public override void PlayerAuthenticatedCheck()
//        {
//            WebPlayerAuthenticatedCheck();
//        }

//        public override void UpdatePlayerData(string data)
//        {
//            WebUpdatePlayerData(data);
//        }

//        public override void GetCourseHighScores(string id, ChallengeMap returnTo)
//        {
//            GetScoreData("highscores/" + id.ToString() + "/scores/", "CourseHighScoreResponse");
//        }

//        public override void GetPracticeHighscores(PracticeMode.PracticeType type, PracticeRoundManager returnTo)
//        {
//            GetScoreData("highscores/" + type.ToString() + "/scores/", "PracticeHighScoreResponse");
//        }

//        public override void SubmitCourseScore(string id, int score)
//        {
//            UpdateHighscore("highscores/" + id.ToString() + "/scores/", score);
//        }

//        public override void SaveHoleScore(int score, string guid, string holeID)
//        {
//            Debug.Log("saving hole score");

//            WebSaveHoleScore(guid, score, holeID);
//        }

//        public override void GetHoleStats(string holeID, RoundManager round)
//        {
//            Debug.Log("get hole stats");

//            WebGetHoleScore(holeID);
//        }

//        internal override void SubmitOpenScore(int totalScore, string tournamentRef)
//        {
//            UpdateHighscore("tournaments/" + tournamentRef + "/scores/", totalScore);
//        }

//        internal override void SubmitPracticeScore(int score, PracticeMode.PracticeType type)
//        {
//            UpdateHighscore("highscores/" + type.ToString() + "/scores/", score);
//        }

//        public override void GetOpenTournament(TournamentPanel panel)
//        {
//            Debug.Log("Trying to get open tournament data");
//            WebGetOpenTournament();
//        }
//    }
//}
