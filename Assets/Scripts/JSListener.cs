//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Assets.Scripts.Menu
//{
//    public class JSListener : MonoBehaviour
//    {
//        [SerializeField] private ChallengeMap map;
//        [SerializeField] private StartMenu start;
//        [SerializeField] private string json;
//        [SerializeField] private ScoreCollection output;
//        [SerializeField] private GameState gameState;
//        [SerializeField] private TournamentPanel tournamnetPanel;
               
//        void CourseHighScoreResponse(string str)
//        {
//            Debug.Log("Got course highscore response: " + str);
//            var yup = "{\"scores\":" + str + "}";

//            var score = JsonUtility.FromJson<ScoreCollection>(yup);
//            map.GotHighScores(score.scores);
//        }

//        void PracticeHighScoreResponse(string str)
//        {
//            Debug.Log("Got practice highscore response: " + str);
//            var yup = "{\"scores\":" + str + "}";

//            var score = JsonUtility.FromJson<ScoreCollection>(yup);

//            if (gameState.roundManager != null && gameState.roundManager is PracticeRoundManager)
//            {
//                var rm = (PracticeRoundManager)gameState.roundManager;
//                rm.GotHighScores(score.scores);
//            }
//        }

//        void NewPlayerResponse(int newPlayer)
//        {
//            start.playerSave.FirebaseManager.newPlayer = newPlayer;
//        }

//        void GetPlayerDataResponse(string data)
//        {
//            var progressData = JsonUtility.FromJson<PlayerProfile>(data);
//            start.playerSave.SetCurrentProfile(progressData);
//        }

//        void GetOpenTournamentResponse(string data)
//        {
//            var tournamentData = JsonUtility.FromJson<RandomRoundWrapper>(data);
//            Debug.Log("Got open data response: " + data);
//            tournamnetPanel.GotOpenData(tournamentData);
//        }

//        void PlayerAuthenticated(string displayName)
//        {
//            Debug.Log("player authenticated with displayName: " + displayName);
//            start.playerSave.FirebaseManager.playerAuthenticated = true;
//            start.playerSave.FirebaseManager.playerDisplayName = displayName;
//            start.playerSave.FirebaseManager.firebaseInitiated = true;
//        }

//       public void GetHoleDataResponse(string holeData)
//        {
//            string[] butts = holeData.Split(',');
//            int[] hist = new int[butts.Length];
//            var best = int.MaxValue;
//            var counter = 0;
//            foreach (var s in butts)
//            {
//                var score = Convert.ToInt32(s);
//                hist[counter] = score;

//                if (score < best)
//                {
//                    best = score;
//                }
//                counter++;
//            }
//            gameState.roundManager.holeBest = best;
//            gameState.roundManager.holeHistory = hist;
//            gameState.roundManager.gotHoleStats = true;
//        }

//        //[ContextMenu("TestJson")]
//        //public void TestJSON()
//        //{
//        //    //var score = JsonConvert.DeserializeObject<List<Score>>(json);
//        //    //Debug.Log("Got course highscore response: " + score);

//        //    var yup = "{\"scores\":" + json + "}";

//        //    output = JsonUtility.FromJson<ScoreCollection>(yup);
//        //}
//    }
//}