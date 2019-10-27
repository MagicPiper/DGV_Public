using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class FireBaseFriendly
    {
        private int fetchCount;
        private string friendlyGameID;

        public FireBaseManager fbmg;
        public FriendlyGamePanel panelRef;

        public static string RandomString(int length)
        {
            var random = new System.Random();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        internal void NewFriendlyGame(FriendlyGamePanel panel, RandomRoundWrapper round)
        {
            Debug.Log("starting a new friendly game");

            var gameID = RandomString(4);
            //var gameID = friendlyGameID.Length > 0 ? friendlyGameID : RandomString(4);

            var friendlyIDsRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/ids/");
            var friendlyGamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/");

            friendlyGamesRef.Child(gameID).GetValueAsync().ContinueWith(task =>
            {
                if (task.Result == null || task.Result.Value == null)
                {
                    //friendlyGameID = gameID;
                    MultiPlayerScoreCard sc = new MultiPlayerScoreCard
                    {
                        on = fbmg.playerSave.PlayerDisplayName(),
                        i = fbmg.playerSave.playerSettings.settingsData.icon,
                        ms = "ig"
                    };
                    string JSON = JsonUtility.ToJson(sc);

                    friendlyGamesRef.Child(gameID + "/sc/" + this.fbmg.playerID).SetRawJsonValueAsync(JSON);

                    friendlyGamesRef.Child(gameID + "/holes/").SetRawJsonValueAsync(JsonUtility.ToJson(round));

                    DateTime foo = DateTime.UtcNow;
                    long startTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();

                    friendlyIDsRef.Child(gameID).SetValueAsync(startTime.ToString());

                    panel.friendlyGameID = gameID;
                }
                else
                {
                    Debug.Log("id exists, trying again");
                    NewFriendlyGame(panel, round);
                    return;
                }
            });
        }

        internal void ResetFriendlyGame(FriendlyGamePanel panel, RandomRoundWrapper round, string gameID)
        {
            Debug.Log("resetting a friendly game");

            var friendlyIDsRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/ids/");
            var friendlyGamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/");

            friendlyGamesRef.Child(gameID + "/sc/").SetValueAsync("").ContinueWith(task =>
            {
                friendlyGamesRef.Child(gameID + "/holes/").SetRawJsonValueAsync(JsonUtility.ToJson(round));
                DateTime foo = DateTime.UtcNow;
                long startTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();

                friendlyIDsRef.Child(gameID).SetValueAsync(startTime.ToString());

                panel.friendlyGameID = gameID;
            });
        }

        internal void JoinFriendlyGame(FriendlyGamePanel friendlyGamePanel, string gameID)
        {
            var friendlyGamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/");
            // friendlyGamesRef.KeepSynced(true);

            Debug.Log("joining friendly game with id: " + gameID);

            friendlyGamesRef.Child(gameID).GetValueAsync().ContinueWith(task =>
            {
                if (task.Result == null || task.Result.Value == null)
                {
                    friendlyGamePanel.friendlyGameID = "missing";
                    friendlyGamePanel.roundWrapper = new RandomRoundWrapper();
                }
                else
                {
                    Debug.Log("game exists");
                    //friendlyGameID = gameID;
                    MultiPlayerScoreCard sc = new MultiPlayerScoreCard
                    {
                        on = fbmg.playerSave.PlayerDisplayName(),
                        i = fbmg.playerSave.playerSettings.settingsData.icon,
                        ms = "ig"
                    };

                    string JSON = JsonUtility.ToJson(sc);
                    friendlyGamesRef.Child(gameID + "/sc/" + fbmg.playerID).SetRawJsonValueAsync(JSON).ContinueWith(t =>
                    {
                        Debug.Log("set scorecard " + gameID);
                        // Debug.Log(task.Result.Child("holes").GetRawJsonValue());
                        // friendlyGamePanel.roundWrapper = JsonUtility.FromJson<RandomRoundWrapper>(task.Result.Child("holes").GetRawJsonValue());
                        friendlyGamePanel.friendlyGameID = gameID;
                    });
                }
            });
        }

        internal void SubscribeToHoles(FriendlyGamePanel panel, string gameID)
        {
            var friendlyGamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/" + gameID + "/holes/");
            Debug.Log("subscribe to holes game");

            EventHandler<ValueChangedEventArgs> handler = (object s, ValueChangedEventArgs e) =>
            {
                Debug.Log("woop " + e.Snapshot.GetRawJsonValue());
                panelRef.roundWrapper = JsonUtility.FromJson<RandomRoundWrapper>(e.Snapshot.GetRawJsonValue());
                Debug.Log("set roundwraper");
            };
            panelRef = panel;
            friendlyGamesRef.ValueChanged += handler;
            panel.holesHandler = handler;
            panel.holesRef = friendlyGamesRef;
        }

        //internal void ReJoinFriendlyGame(FriendlyGamePanel friendlyGamePanel, string gameID)
        //{
        //    var friendlyGamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/" + gameID + "/sc/" + fbmg.playerID);
        //    Debug.Log("joining friendly game");
        //    MultiPlayerScoreCard sc = new MultiPlayerScoreCard
        //    {
        //        on = fbmg.playerSave.PlayerDisplayName(),
        //        i = fbmg.playerSave.playerSettings.settingsData.icon,
        //        ms = "ig"
        //    };
        //    string JSON = JsonUtility.ToJson(sc);
        //    friendlyGamesRef.SetRawJsonValueAsync(JSON).ContinueWith(task =>
        //    {

        //        friendlyGamePanel.friendlyGameID = gameID;

        //    });
        //}

        internal void LeaveFriendlyGame(string gameID)
        {
            var friendlyGamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/" + gameID + "/sc/" + fbmg.playerID);

            friendlyGamesRef.SetValueAsync(null);
        }

        internal void FriendlySaveScorecard(ScoreCard card, string gameID)
        {
            var mpSC = new MultiPlayerScoreCard
            {
                t = card.score,
                s = card.scores,
                i = card.icon,
                ms = card.multiplayerStatus,
                on = card.ownerName
            };

            Debug.Log("SavePlayerScore");
            var scoreRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/" + gameID + "/sc/" + fbmg.playerID);
            scoreRef.SetRawJsonValueAsync(JsonUtility.ToJson(mpSC));

            DateTime foo = DateTime.UtcNow;
            long startTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();

            var friendlyIDsRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/ids/");


            friendlyIDsRef.Child(gameID).SetValueAsync(startTime.ToString());
        }

        internal void FriendlyGetScore(FriendlyRoundManager rm, string gameID)
        {
            Debug.Log("get friendly score for round manager");
            var scoreRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/" + gameID + "/sc/");
            scoreRef.GetValueAsync().ContinueWith(task =>
            {
                Debug.Log("fetching scorecards: " + fetchCount);
                fetchCount++;

                List<ScoreCard> cards = new List<ScoreCard>();
                foreach (var t in task.Result.Children)
                {
                    var data = JsonUtility.FromJson<MultiPlayerScoreCard>(t.GetRawJsonValue());

                    cards.Add(new ScoreCard
                    {
                        score = data.t,
                        scores = data.s,
                        icon = data.i,
                        multiplayerStatus = data.ms,
                        ownerName = data.on,
                        playerID = t.Key,
                        isPlayer = t.Key == fbmg.playerID
                    });
                }
                Debug.Log("got multiplayer scorecard. Players:  " + cards.Count);
                foreach (ScoreCard card in cards)
                {
                    //Debug.Log("player: " + card.ownerName);
                    if (card.scores == null || card.scores.Length < 1)
                    {
                        card.scores = new int[9];
                    }
                }

                rm.UpdateScore(cards);
            });
        }

        internal void FriendlySubscribeToScore(FriendlyGamePanel panel, string gameID)
        {
            Debug.Log("subscribe to scores");
            var scoreRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/" + gameID + "/sc/");
            //scoreRef.KeepSynced(true);

            EventHandler<ValueChangedEventArgs> handler = (object s, ValueChangedEventArgs e) =>
            {
                Debug.Log("fetching scorecars fetchcount: " + fetchCount);
                fetchCount++;

                List<ScoreCard> cards = new List<ScoreCard>();

                foreach (var t in e.Snapshot.Children)
                {
                    var data = JsonUtility.FromJson<MultiPlayerScoreCard>(t.GetRawJsonValue());

                    cards.Add(new ScoreCard
                    {
                        score = data.t,
                        scores = data.s,
                        icon = data.i,
                        multiplayerStatus = data.ms,
                        ownerName = data.on,
                        playerID = t.Key,
                        isPlayer = t.Key == fbmg.playerID
                    });
                }

                // Debug.Log("got multiplayer scorecard. Players:  " + cards.Count);
                foreach (ScoreCard card in cards)
                {
                    // Debug.Log("player: " + card.ownerName);
                    if (card.scores == null || card.scores.Length < 1)
                    {
                        card.scores = new int[9];
                    }
                }

                List<ScoreCard> SortedList = new List<ScoreCard>();

                Debug.Log("scorecards count: " + cards.Count);
                if (cards.Count > 0)
                {

                    SortedList = cards.OrderBy(o => o.playerID).ToList();
                    if (SortedList[0].playerID == fbmg.playerID)
                    {
                        panel.isLobbyOwner = true;
                    }
                    else
                    {
                        panel.isLobbyOwner = false;
                    }

                    panel.scoreCards = SortedList;
                    panel.UpdateScoreCards();
                }
                //else
                //{
                //    panel.NewRoundStarting();
                //} 
            };

            scoreRef.ValueChanged += handler;
            panel.scoreHandler = handler;
            panel.scoreRef = scoreRef;
        }

        //internal void FriendlySubscribeToHoles(FriendlyGamePanel panel, string gameID)
        //{
        //    Debug.Log("subscribe to holes");
        //    var holesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "fg/g/" + gameID + "/holes/");
        //    //scoreRef.KeepSynced(true);

        //    EventHandler<ValueChangedEventArgs> handler = (object s, ValueChangedEventArgs e) =>
        //    {
        //        Debug.Log("new holes");

        //        var roundWrapper = JsonUtility.FromJson<RandomRoundWrapper>(e.Snapshot.GetRawJsonValue());

        //        panel.NewRoundStarting(roundWrapper);
        //    };

        //    holesRef.ValueChanged += handler;
        //    panel.holeshandler = handler;
        //    panel.holesRef = holesRef;
        //}
    }
}
