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
    public class FireBaseMultiplayer
    {
        public FireBaseManager fbmg;
       
        private MultiplayerPanel multiplayerPanel;
        private DatabaseReference multiplayerLookingForGameStatusRef;
       // private DatabaseReference multiplayerPlayersInQueueRef;
        private int fetchCount;     

        internal void MultiplayerQueuePlayer(MultiplayerPanel multiplayerPanel, string lastGameID)
        {
            this.multiplayerPanel = multiplayerPanel;
            Debug.Log("Trying to find a multiplayer game in firebase");
            var queue = fbmg.FBdata.GetReference(fbmg.devPrefix + "mp/pq/" + fbmg.playerID);
            multiplayerLookingForGameStatusRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "playerData/" + fbmg.playerID + "/multiplayerGame/");
            //multiplayerPlayersInQueueRef = FBdata.GetReference(devPrefix + "mp/pq/");

            MultiPlayerScoreCard sc = new MultiPlayerScoreCard
            {
                on = fbmg.playerSave.PlayerDisplayName(),
                ms = lastGameID,
                i = fbmg.playerSave.playerSettings.settingsData.icon
            };
            string JSON = JsonUtility.ToJson(sc);

            multiplayerLookingForGameStatusRef.SetValueAsync("wfg").ContinueWith(task =>
        {
            queue.SetRawJsonValueAsync(JSON);

            multiplayerLookingForGameStatusRef.ValueChanged += WaitingForGameStatusChanged;
            // multiplayerPlayersInQueueRef.ValueChanged += PlayersInQueueChanged;
        });
        }
        
        void WaitingForGameStatusChanged(object sender, ValueChangedEventArgs args)
        {
            Debug.Log("multiplayerLookingForGameStatus changed");
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            multiplayerPanel.lookForGameStatus = args.Snapshot.Value.ToString();
        }

        //for testing
        internal void QueueTestPlayer()
        {
            var queue = fbmg.FBdata.GetReference(fbmg.devPrefix + "mp/pq/" + "rzu0N7asSRVplhbKbx8Ebedepd");
            var playerRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "playerData/" + "rzu0N7asSRVplhbKbx8Ebedepd" + "/multiplayerGame/");

            MultiPlayerScoreCard sc = new MultiPlayerScoreCard
            {
                on = "testplayer",
                ms = "ig",
                i = fbmg.playerSave.playerSettings.settingsData.icon
            };
            string JSON = JsonUtility.ToJson(sc);

            playerRef.SetValueAsync("wfg").ContinueWith(task =>
            {
                queue.SetRawJsonValueAsync(JSON);
            });
        }

        internal void MultiplayerUnqueue(MultiplayerPanel multiplayerPanel)
        {
            Debug.Log("Removing player from queue");
            var queue = fbmg.FBdata.GetReference(fbmg.devPrefix + "mp/pq/" + fbmg.playerID);
            var playerRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "playerData/" + fbmg.playerID + "/multiplayerGame/");

            queue.RemoveValueAsync().ContinueWith(task =>
            {
                playerRef.SetValueAsync("nq");
            });
        }

        internal void StopWaitingForGameListener()
        {
            multiplayerLookingForGameStatusRef.ValueChanged -= WaitingForGameStatusChanged;
            // multiplayerPlayersInQueueRef.ValueChanged -= PlayersInQueueChanged;
        }

        internal void MultiplayerGetHoles(MultiplayerPanel multiplayerPanel, string gameID)
        {
            Debug.Log("Getting holes+wind");

            RandomRoundWrapper tournament = new RandomRoundWrapper();
            long startTime = 0;
            var gamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "mpg/g/" + gameID + "/t/"); //tournament
            var timeRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "mpg/g/" + gameID + "/st/");  //startTime
            var playerRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "playerData/" + fbmg.playerID + "/openGames/" + gameID + "/holes/");

            gamesRef.GetValueAsync().ContinueWith(task =>
             {
                 Debug.Log("Got tournamentID: " + task.Result.Value.ToString());
                 timeRef.GetValueAsync().ContinueWith(timeTask =>
                 {
                     var s = timeTask.Result.Value;
                     startTime = (long)s;
                     fbmg.FBdata.GetReference(fbmg.devPrefix + "tournaments/" + task.Result.Value.ToString()).GetValueAsync().ContinueWith(t =>
                      {
                          Debug.Log("Got tournament holes: " + t.Result.Value.ToString());
                          tournament = JsonUtility.FromJson<RandomRoundWrapper>(t.Result.GetRawJsonValue());
                          multiplayerPanel.tournamentWrapper = tournament;
                          multiplayerPanel.startTime = startTime;
                          multiplayerPanel.hasHoles = true;

                          Debug.Log(t.Result.Child("holes").Value);

                          playerRef.SetRawJsonValueAsync(t.Result.Child("holes").GetRawJsonValue());
                      });
                 });
             });
        }        

        internal void MultiPlayerSaveScorecard(ScoreCard card, string gameID, bool retire = false)
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
            Debug.Log(fbmg.devPrefix + "playerData/" + fbmg.playerID + "/openGames/" + gameID + "/status/");
            var gameRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "/playerData/" + fbmg.playerID + "/openGames/" + gameID + "/status/");
            // var gameRef = FBdata.GetReference(devPrefix + "playerData/" + playerID + "/openGames/" + gameID);
            var scoreRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "/mpg/g/" + gameID + "/sc/" + fbmg.playerID);

            gameRef.GetValueAsync().ContinueWith(status =>
            {
                if (status.Result == null || status.Result.Value == null || status.Result.Value.ToString() == "o")
                {
                    Debug.Log("ongoing");
                    scoreRef.SetRawJsonValueAsync(JsonUtility.ToJson(mpSC)).ContinueWith(task =>
                    {
                        Debug.Log("saved multiplayer score");
                        if (retire)
                        {
                            MultplayerClaimReward(gameID);
                        }
                    });
                }
                else
                {
                    Debug.Log("game has expired, score not submited");
                    if (retire)
                    {
                        MultplayerClaimReward(gameID);
                        // playerSave.playerStats.RatingsUpdate(-10);
                    }
                }
            });
        }                

        internal void MultiplayerGetOpenGames(MultiplayerPanel panel)
        {
            Debug.Log("Get Open Games");
            var gamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "playerData/" + fbmg.playerID + "/openGames/");
            List<MultiplayerGame> list = new List<MultiplayerGame>();
            gamesRef.GetValueAsync().ContinueWith(task =>
            {
                var hasPreviousGames = "false";

                if (task.Result != null)
                {
                    Debug.Log("childrencount " + task.Result.ChildrenCount);
                    foreach (var t in task.Result.Children)
                    {
                        var json = t.GetRawJsonValue();

                        if (json.Length > 0)
                        {
                            var game = JsonUtility.FromJson<MultiplayerGame>(t.GetRawJsonValue());
                            game.startTime = Convert.ToInt64(t.Key);
                            game.gameID = t.Key;
                            list.Add(game);
                            hasPreviousGames = "true";
                        }
                    }
                }
                panel.multiplayerGames = list;
                panel.loadGamesStatus = "hasGames";
            });
        }

        internal void MultiplayerGetGameStatus(MultiplayerUIGame UIgame, string gameID)
        {
            Debug.Log("Get Game status");
            var gamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "playerData/" + fbmg.playerID + "/openGames/" + gameID);

            EventHandler<ValueChangedEventArgs> handler = (object s, ValueChangedEventArgs e) =>
            {
                var val = e.Snapshot.Value;
                var game = JsonUtility.FromJson<MultiplayerGame>(e.Snapshot.GetRawJsonValue());
                UIgame.game = game;
                UIgame.UpdateGame();
            };

            gamesRef.ValueChanged += handler;
            UIgame.statusHandler = handler;
            UIgame.statusRef = gamesRef;
        }

        internal void MultplayerClaimReward(string gameID)
        {
            Debug.Log("Claim Multiplayer Reward");
            var gamesRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "playerData/" + fbmg.playerID + "/openGames/" + gameID);
            gamesRef.RemoveValueAsync();
        }

        internal void MultiplayerGetScore(MultiplayerRoundManager rm, string gameID)
        {
            Debug.Log("get multiplayer standings");
            var scoreRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "mpg/g/" + gameID + "/sc/");
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
                    if (card.scores == null || card.scores.Length < 1)
                    {
                        card.scores = new int[9];
                    }
                }

                rm.UpdateScore(cards);
            });
        }                

        internal void MultiplayerGetScore(MultiplayerUIGame UIgame, string gameID)
        {
            Debug.Log("get multiplayer standings");
            var scoreRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "mpg/g/" + gameID + "/sc/");

            EventHandler<ValueChangedEventArgs> handler = (object s, ValueChangedEventArgs e) =>
            {
                Debug.Log("fetching scorecards: " + fetchCount);
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
                UIgame.scoreCards = cards;
                UIgame.UpdateGame();
            };

            scoreRef.ValueChanged += handler;
            UIgame.scoreHandler = handler;
            UIgame.scoreRef = scoreRef;
        }


        //only for admin
        internal void ClearOldMultiplayerGames()
        {
            var cuttOff = DateTime.Today.AddMonths(-1);
            long unixTime = ((DateTimeOffset)cuttOff).ToUnixTimeMilliseconds();
            Debug.Log(unixTime);
            // time = unixTime.ToString();
            int count = 0;

            var gameRef = fbmg.FBdata.GetReference(fbmg.devPrefix + "multiplayerOngoing/games/");
            // gameRef.OrderByKey().EndAt(time).GetValueAsync().ContinueWith(task =>
            gameRef.GetValueAsync().ContinueWith(task =>
            // gameRef.OrderByKey().LimitToFirst(10).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Not able to get open tournament ref");
                }
                else if (task.IsCompleted)
                {
                    foreach (var t in task.Result.Children)
                    {
                        long time;
                        long.TryParse(t.Key, out time);
                        // Debug.Log(time);
                        if (time < unixTime)
                        {
                            t.Reference.RemoveValueAsync();
                            count++;
                        }
                    }
                    Debug.Log("Removed " + count + " old games");
                }
            });
        }
    }
}
