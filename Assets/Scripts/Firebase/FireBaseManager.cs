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
    public class FireBaseManager
    {
        public FirebaseDatabase FBdata;
        private FirebaseAuth auth;
        private DatabaseReference reference;
        public string devPrefix = "";

        public int newPlayer = 0;  // 0 = no response yet, 1 = existing player, 2 = new player 
        public bool firebaseInitiated = false;
        public PlayerSave playerSave;
        public bool playerAuthenticated = false;
        public bool authenticationFailed = false;
        public bool authenticationCanceled = false;
        public string playerDisplayName;
        public string playerID;
        public string playerEmail;

        private FireBaseMultiplayer multiPlayerFunctions;
        public FireBaseMultiplayer MultiPlayerFunctions
        {
            get
            {
                if (multiPlayerFunctions == null)
                {
                    multiPlayerFunctions = new FireBaseMultiplayer();
                    multiPlayerFunctions.fbmg = this;
                }
                return multiPlayerFunctions;
            }
        }

        private FireBaseFriendly friendlyFunctions;
        public FireBaseFriendly FriendlyFunctions
        {
            get
            {
                if (friendlyFunctions == null)
                {
                    friendlyFunctions = new FireBaseFriendly();
                    friendlyFunctions.fbmg = this;
                }
                return friendlyFunctions;
            }
        }

        private int fetchCount;
        private string friendlyGameID;

        public virtual void NewPlayerCheck()
        {
            Debug.Log("Starting NewPlayerCheck");
            Task<DataSnapshot> t = FBdata.GetReference(devPrefix + "playerData/" + auth.CurrentUser.UserId + "/playerProfile/").GetValueAsync();
            t.ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Debug.Log("Not able to get user data");
                            }
                            else if (task.IsCompleted)
                            {
                                Debug.Log("NewPlayerCheck completed");

                                if (task.Result == null || task.Result.Value == null)
                                {
                                    Debug.Log("new player");
                                    playerSave.CreateNewPlayer();
                                    PlayerSave.playerGet = true;
                                    PlayerSave.offline = false;
                                }
                                else
                                {
                                    Debug.Log("existing player");
                                    newPlayer = 1;
                                    var profile = JsonUtility.FromJson<PlayerProfile>(task.Result.GetRawJsonValue());
                                    playerSave.SetCurrentProfile(profile);
                                    GetPlayerData();
                                }
                            }
                            else if (task.IsCanceled)
                            {
                                Debug.Log("canceled");
                            }
                        });
        }

        public virtual void GetPlayerData()
        {
            Debug.Log("getting player data");

            FBdata.GetReference(devPrefix + "playerData/" + auth.CurrentUser.UserId + "/playerStats/").GetValueAsync().ContinueWith(statsTask =>
             {
                 if (statsTask.IsFaulted)
                 {
                     Debug.Log("Not able to get user data");
                 }
                 else if (statsTask.IsCompleted)
                 {
                     if (statsTask.Result != null)
                     {
                         var stats = JsonUtility.FromJson<PlayerStatsData>(statsTask.Result.GetRawJsonValue());
                         playerSave.SetPlayerStats(stats);
                     }
                     else
                     {
                         Debug.Log("No player stats, creating new");
                         playerSave.playerStats.NewStats();
                     }

                     FBdata.GetReference(devPrefix + "playerData/" + auth.CurrentUser.UserId + "/playerSettings/").GetValueAsync().ContinueWith(settingsTask =>
                     {
                         if (settingsTask.Result != null)
                         {
                             var settings = JsonUtility.FromJson<PlayerSettingsData>(settingsTask.Result.GetRawJsonValue());
                             playerSave.SetPlayerSettings(settings);
                         }
                         else
                         {
                             Debug.Log("No player settings, creating new");
                             playerSave.playerSettings.NewSettings();
                         }
                         PlayerSave.playerGet = true;
                         PlayerSave.offline = false;
                     });
                 }
             });
        }

        public virtual void UpdatePlayerData(string data)
        {
            Debug.Log("saving player data");

            FBdata.GetReference(devPrefix + "playerData/" + auth.CurrentUser.UserId).Child("/playerProfile/").SetRawJsonValueAsync(data).ContinueWith(task =>
             {
                 if (task.IsFaulted)
                 {
                     Debug.Log("Not able to set user data");
                 }
                 else if (task.IsCompleted)
                 {
                     Debug.Log("saved user data");
                 }
             });
        }

        public virtual void UpdatePlayerStats(string data)
        {
            Debug.Log("saving player stats");

            FBdata.GetReference(devPrefix + "playerData/" + auth.CurrentUser.UserId).Child("/playerStats/").SetRawJsonValueAsync(data).ContinueWith(task =>
             {
                 if (task.IsFaulted)
                 {
                     Debug.Log("Not able to set user stats");
                 }
                 else if (task.IsCompleted)
                 {
                     Debug.Log("saved user stats");
                 }
             });
        }

        public virtual void UpdatePlayerSettings(string data)
        {
            Debug.Log("saving player settings");

            FBdata.GetReference(devPrefix + "playerData/" + auth.CurrentUser.UserId).Child("/playerSettings/").SetRawJsonValueAsync(data).ContinueWith(task =>
             {
                 if (task.IsFaulted)
                 {
                     Debug.Log("Not able to set user settings");
                 }
                 else if (task.IsCompleted)
                 {
                     Debug.Log("saved user settings");
                 }
             });
        }

        public void InitFirebase()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Set a flag here indiciating that Firebase is ready to use by your
                    // application.
                    firebaseInitiated = true;
                    FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://discgolfvalley.firebaseio.com/");
                    //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://testproject-dde8b.firebaseio.com/");
                    FBdata = FirebaseDatabase.DefaultInstance;
                    // FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);

                    if (Application.platform != RuntimePlatform.WindowsEditor)
                    {
                        devPrefix = "";
                    }

                    //FBdata.SetPersistenceEnabled(false);
                    auth = FirebaseAuth.DefaultInstance;
                }
                else
                {
                    Debug.LogError(String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.

                    // Get the root reference location of the database.                  
                }
            });
        }

        internal void ClaimReward(string id)
        {
            Debug.Log("claiming reward" + id);

            FBdata.GetReference(devPrefix + "playerData/" + auth.CurrentUser.UserId + "/results/" + id).SetValueAsync(null);
        }

        public virtual void PlayerAuthenticatedCheck()
        {
#if UNITY_EDITOR
            GoogleSignInEditorConfig.Secret = "8k9cP_nufZUV_EFEDz221n3W";
#endif


            if (GoogleSignIn.Configuration == null)
            {
                GoogleSignIn.Configuration = new GoogleSignInConfiguration
                {
                    RequestIdToken = true,
                    // Copy this value from the google-service.json file.
                    // oauth_client with type == 3
                    WebClientId = "480706417920-t1ge6ucbuvehabjppeeeilc6v1gk57ud.apps.googleusercontent.com"
                };
            }

            Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

            Debug.Log("Starting  google auth");
            authenticationCanceled = false;
            authenticationFailed = false;
            playerAuthenticated = false;

            TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

            signIn.ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    signInCompleted.SetCanceled();
                    authenticationCanceled = true;
                }
                else if (task.IsFaulted)
                {
                    signInCompleted.SetException(task.Exception);
                    Debug.Log("google auth faulted" + task.Exception.Message);
                    Debug.Log("google auth faulted" + task.Exception.StackTrace);
                    Debug.Log("google auth faulted" + task.Exception.HResult);
                    authenticationFailed = true;
                }
                else
                {
                    Debug.Log("google auth is good!");
                    Debug.Log("credential ");

                    Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);

                    Debug.Log("starting firebaseauth");

                    auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
                    {
                        if (authTask.IsCanceled)
                        {
                            Debug.Log("firebase auth canceled");

                            signInCompleted.SetCanceled();
                            authenticationFailed = true;
                        }
                        else if (authTask.IsFaulted)
                        {
                            Debug.Log("firebase auth faulted");

                            signInCompleted.SetException(authTask.Exception);
                            authenticationFailed = true;
                        }
                        else
                        {
                            Debug.Log("firebase auth is good!");

                            signInCompleted.SetResult(((Task<FirebaseUser>)authTask).Result);
                            playerAuthenticated = true;
                            playerDisplayName = auth.CurrentUser.DisplayName;
                            playerID = auth.CurrentUser.UserId;
                            playerEmail = auth.CurrentUser.Email;
                        }
                    });
                }
            });
        }

        public virtual void GetPracticeHighscores(PracticeMode.PracticeType type, PracticeRoundManager returnTo)
        {
            if (!playerAuthenticated) return;
            Debug.Log("get highscores");

            var reference = FBdata.GetReference(devPrefix + "highscores/" + type.ToString() + "/scores/");
            reference.OrderByChild("score").LimitToLast(20).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Not able to get highscores");
                }
                else if (task.IsCompleted)
                {
                    var s = GetHighScores(task, 5);
                    returnTo.GotHighScores(s);
                }
            });
        }

        private List<Score> GetHighScores(Task<DataSnapshot> task, int count)
        {
            List<Score> list = new List<Score>();
            DataSnapshot snapshot = task.Result;

            foreach (var s in snapshot.Children)
            {
                var score = JsonUtility.FromJson<Score>(s.GetRawJsonValue());
                score.userid = s.Key;
                list.Add(score);
            }
            return list;
        }

        internal virtual void SubmitPracticeScore(int score, PracticeMode.PracticeType type)
        {
            if (!playerAuthenticated) return;

            Debug.Log("Trying to save a score");

            var scoresRef = FBdata.GetReference(devPrefix + "highscores/" + type.ToString() + "/scores/" + auth.CurrentUser.UserId);

            SubmitScore(score, scoresRef, false);
        }

        internal virtual void SubmitOpenScore(int totalScore, int parDiff, ProTourWrapper.Division division, int week)
        {
            Debug.Log("Trying to save a open score");
            var weekRef = FBdata.GetReference(devPrefix + "openTournament/" + division.ToString() + "/properties/week/");
            var scoreRef = FBdata.GetReference(devPrefix + "openTournament/" + division.ToString() + "/scores/" + auth.CurrentUser.UserId);

            weekRef.GetValueAsync().ContinueWith(weekTask =>
            {
                if (weekTask.IsFaulted)
                {
                    Debug.Log("Not able to get week");
                }
                else if (weekTask.IsCompleted)
                {
                    if (week == Convert.ToInt32(weekTask.Result.Value))
                    {
                        scoreRef.GetValueAsync().ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Debug.Log("Not able to get open scores");
                            }
                            else if (task.IsCompleted)
                            {
                                Debug.Log("task completed succesfully");

                                if (task.Result == null || task.Result.Value == null)
                                {
                                    scoreRef.Child("userName").SetValueAsync(playerSave.PlayerDisplayName());
                                    scoreRef.Child("userid").SetValueAsync(playerID);
                                    scoreRef.Child("icon").SetValueAsync(playerSave.playerSettings.settingsData.icon);
                                    scoreRef.Child("/score").SetValueAsync(totalScore);
                                    scoreRef.Child("/parDiff").SetValueAsync(parDiff);
                                }
                                else
                                {
                                    Debug.Log("score exists");
                                    var currentScore = task.Result.Child("score").Value;
                                    scoreRef.Child("icon").SetValueAsync(playerSave.playerSettings.settingsData.icon);

                                    if (Convert.ToInt32(currentScore) > totalScore)
                                    {
                                        scoreRef.Child("/score").SetValueAsync(totalScore);
                                        scoreRef.Child("/parDiff/").SetValueAsync(parDiff);
                                    }
                                }
                            }
                        });
                    }
                }
            });
        }

        internal virtual void SubmitProTourScore(int score, int par, ProTourWrapper.Division division, int round, int week)
        {
            Debug.Log("Trying to save a pro tour score");
            var weekRef = FBdata.GetReference(devPrefix + "proTour/" + division.ToString() + "/tourProperties/week/");

            var scoreRef = FBdata.GetReference(devPrefix + "proTour/" + division.ToString() + "/scores/" + auth.CurrentUser.UserId);

            weekRef.GetValueAsync().ContinueWith(weekTask =>
            {
                if (weekTask.IsFaulted)
                {
                    Debug.Log("Not able to get week");
                }
                else if (weekTask.IsCompleted)
                {
                    if (week == Convert.ToInt32(weekTask.Result.Value))
                    {
                        scoreRef.GetValueAsync().ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Debug.Log("Not able to get highscores");
                            }
                            else if (task.IsCompleted)
                            {
                                Debug.Log("task completed succesfully");

                                if (task.Result == null || task.Result.Value == null)
                                {
                                    scoreRef.Child("playerName").SetValueAsync(playerSave.PlayerDisplayName());
                                    scoreRef.Child("playerID").SetValueAsync(playerID);
                                    scoreRef.Child("icon").SetValueAsync(playerSave.playerSettings.settingsData.icon);
                                    int total = 0;

                                    for (int i = 0; i < 4; i++)
                                    {
                                        var s = 0;
                                        var p = 0;
                                        if (i == round)
                                        {
                                            s = score;
                                            p = par;
                                            total += score;
                                        }
                                        else
                                        {
                                            total += 999;
                                        }
                                        scoreRef.Child("/scores/" + i).SetValueAsync(s);
                                        scoreRef.Child("/pars/" + i).SetValueAsync(p);
                                    }
                                    scoreRef.Child("/sortScore/").SetValueAsync(total);

                                }
                                else
                                {
                                    scoreRef.Child("/scores/" + round.ToString()).SetValueAsync(score);
                                    scoreRef.Child("/pars/" + round.ToString()).SetValueAsync(par);
                                    int total = Convert.ToInt32(task.Result.Child("/sortScore/").Value);
                                    scoreRef.Child("/sortScore/").SetValueAsync(total - 999 + score);
                                    scoreRef.Child("icon").SetValueAsync(playerSave.playerSettings.settingsData.icon);
                                }
                            }
                        });
                    }
                }
            });
        }

        private void SubmitScore(int newScore, DatabaseReference scoresRef, bool lowerIsBetter)
        {
            scoresRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Not able to get highscores");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("task completed succesfully");


                    if (task.Result == null || task.Result.ChildrenCount < 1)
                    {
                        var score = new Score()
                        {
                            score = newScore,
                            userName = playerSave.PlayerDisplayName(),
                            currentRating = playerSave.playerStats.statsData.openTournamentRating
                        };
                        Debug.Log("new user");
                        scoresRef.SetRawJsonValueAsync(JsonUtility.ToJson(score));
                    }
                    else
                    {
                        Debug.Log("existing user");

                        var currentscore = task.Result.Child("score").Value;
                        Debug.Log("current score: " + currentscore + "  new score: " + newScore);

                        if (lowerIsBetter && Convert.ToInt32(currentscore) > newScore)
                        {
                            Debug.Log("Setting new score: " + newScore);
                            scoresRef.Child("score").SetValueAsync(newScore);
                        }
                        else if (!lowerIsBetter && Convert.ToInt32(currentscore) < newScore)
                        {
                            Debug.Log("Setting new score: " + newScore);
                            scoresRef.Child("score").SetValueAsync(newScore);
                        }
                    }
                }
            });
        }

        public virtual void GetOpenTournament(TournamentPanel panel, ProTourWrapper.Division division)
        {
            Debug.Log("Trying to get open tournament data");
            var openRef = FBdata.GetReference(devPrefix + "openTournament/" + division.ToString() + "/properties/");
            var scoreRef = FBdata.GetReference(devPrefix + "openTournament/" + division.ToString() + "/scores/");
            var openResultRef = FBdata.GetReference(devPrefix + "playerData/" + playerID + "/openResult/");

            openResultRef.GetValueAsync().ContinueWith(resultTask =>
            {
                if (resultTask.IsFaulted)
                {
                    Debug.Log("Not able to get open result");
                }
                else if (resultTask.IsCompleted)
                {
                    if (resultTask.Result != null && resultTask.Result.ChildrenCount > 0)
                    {
                        var open = JsonUtility.FromJson<OpenResult>(resultTask.Result.GetRawJsonValue());
                        panel.hasResult = "true";
                        panel.result = open;
                        resultTask.Result.Reference.SetValueAsync(null);
                    }
                    else
                    {
                        panel.hasResult = "false";
                    }

                    openRef.GetValueAsync().ContinueWith(propTask =>
                    {
                        if (propTask.IsFaulted)
                        {
                            Debug.Log("Not able to get pro tour");
                        }
                        else if (propTask.IsCompleted)
                        {
                            var open = JsonUtility.FromJson<OpenTournamentWrapper>(propTask.Result.GetRawJsonValue());
                            panel.GotOpenData(open);
                        }
                    });
                    scoreRef.OrderByChild("score").LimitToFirst(25).GetValueAsync().ContinueWith(scoreTask =>
                    {
                        if (scoreTask.IsFaulted)
                        {
                            Debug.Log("Not able to get pro tour");
                        }
                        else if (scoreTask.IsCompleted)
                        {
                            List<OpenScore> scores = new List<OpenScore>();

                            if (scoreTask.Result != null)
                            {
                                foreach (var s in scoreTask.Result.Children)
                                {
                                    var score = JsonUtility.FromJson<OpenScore>(s.GetRawJsonValue());
                                    scores.Add(score);
                                }
                            }
                            panel.GotOpenScores(scores);
                            var playerScore = scores.Find(x => x.userid == auth.CurrentUser.UserId);

                            if (playerScore == null)
                            {
                                Debug.Log("player score is null");

                                scoreRef.Child(auth.CurrentUser.UserId).GetValueAsync().ContinueWith(playerScoreTask =>
                                {
                                    Debug.Log("getting player score");

                                    if (playerScoreTask.IsFaulted)
                                    {
                                        Debug.Log("not able to get player score");
                                        panel.GotPlayerScore();

                                    }
                                    else if (playerScoreTask.IsCompleted && playerScoreTask.Result != null && playerScoreTask.Result.ChildrenCount > 0)
                                    {
                                        Debug.Log("got player score");

                                        var playerScoreObject = JsonUtility.FromJson<OpenScore>(playerScoreTask.Result.GetRawJsonValue());

                                        panel.GotPlayerScore(playerScoreObject);
                                    }
                                    else
                                    {
                                        Debug.Log("else");

                                        panel.GotPlayerScore();
                                    }
                                });
                            }
                            else
                            {
                                Debug.Log("player score was not null");

                                panel.GotPlayerScore();
                            }
                        }
                    });
                }
            });
        }

        public virtual void GetProTour(ProTourPanel panel, ProTourWrapper.Division division)
        {
            Debug.Log("Trying to get pro tour data");
            var scoreRef = FBdata.GetReference(devPrefix + "proTour/" + division.ToString() + "/scores/");
            var proTourRef = FBdata.GetReference(devPrefix + "proTour/" + division.ToString() + "/tourProperties/");
            var proTourResultRef = FBdata.GetReference(devPrefix + "playerData/" + playerID + "/proTourResult/");

            proTourResultRef.GetValueAsync().ContinueWith(resultTask =>
            {
                if (resultTask.IsFaulted)
                {
                    Debug.Log("Not able to get pro tour result");
                }
                else if (resultTask.IsCompleted)
                {
                    if (resultTask.Result != null && resultTask.Result.ChildrenCount > 0)
                    {
                        var proTour = JsonUtility.FromJson<ProTourResult>(resultTask.Result.GetRawJsonValue());
                        panel.hasResult = "true";
                        panel.result = proTour;
                        resultTask.Result.Reference.SetValueAsync(null);
                    }
                    else
                    {
                        panel.hasResult = "false";
                    }

                    proTourRef.GetValueAsync().ContinueWith(propTask =>
                    {
                        if (propTask.IsFaulted)
                        {
                            Debug.Log("Not able to get pro tour");
                        }
                        else if (propTask.IsCompleted)
                        {
                            Debug.Log("task completed succesfully");

                            var t = propTask.Result;

                            var proTour = JsonUtility.FromJson<ProTourWrapper>(t.GetRawJsonValue());

                            panel.GotTourData(proTour);
                        }
                    });
                    scoreRef.OrderByChild("sortScore").StartAt(1).LimitToFirst(25).GetValueAsync().ContinueWith(scoreTask =>
                    {
                        if (scoreTask.IsFaulted)
                        {
                            Debug.Log("Not able to get pro tour");
                        }
                        else if (scoreTask.IsCompleted)
                        {
                            Debug.Log("task completed succesfully");
                            List<ProTourScore> scores = new List<ProTourScore>();

                            if (scoreTask.Result != null)
                            {
                                foreach (var s in scoreTask.Result.Children)
                                {
                                    var score = JsonUtility.FromJson<ProTourScore>(s.GetRawJsonValue());
                                    scores.Add(score);
                                }
                            }

                            panel.GotScoreData(scores);
                            var playerScore = scores.Find(x => x.playerID == auth.CurrentUser.UserId);

                            if (playerScore == null)
                            {
                                scoreRef.Child(auth.CurrentUser.UserId).GetValueAsync().ContinueWith(playerScoreTask =>
                                {
                                    if (playerScoreTask.IsFaulted)
                                    {
                                        Debug.Log("not able to get player score");
                                    }
                                    else if (playerScoreTask.IsCompleted && playerScoreTask.Result != null && playerScoreTask.Result.ChildrenCount > 0)
                                    {
                                        var playerScoreObject = JsonUtility.FromJson<ProTourScore>(playerScoreTask.Result.GetRawJsonValue());

                                        panel.GotPlayerScore(playerScoreObject);
                                    }
                                    else
                                    {
                                        panel.GotPlayerScore();
                                    }
                                });
                            }
                            else
                            {
                                panel.GotPlayerScore();
                            }
                        }
                    });
                }
            });
        }

        public virtual void GetCurrentVersion()
        {
            Debug.Log("Trying to get the current version from firebase");
            var versionRef = FBdata.GetReference(devPrefix + "currentVersion/");

            //versionRef.SetValueAsync("1033");

            versionRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Not able to get version");
                }
                else if (task.IsCompleted)
                {
                    // playerSave.LatestVersion = (float)Convert.ToDouble(task.Result.Value);
                    playerSave.LatestVersion = float.Parse(task.Result.Value.ToString());
                }
            });
        }

        //for testing
        internal void QueueTestPlayer()
        {
            var queue = FBdata.GetReference(devPrefix + "mp/pq/" + "rzu0N7asSRVplhbKbx8Ebedepd");
            var playerRef = FBdata.GetReference(devPrefix + "playerData/" + "rzu0N7asSRVplhbKbx8Ebedepd" + "/multiplayerGame/");

            MultiPlayerScoreCard sc = new MultiPlayerScoreCard
            {
                on = "testplayer",
                ms = "ig",
                i = playerSave.playerSettings.settingsData.icon
            };
            string JSON = JsonUtility.ToJson(sc);

            playerRef.SetValueAsync("wfg").ContinueWith(task =>
            {
                queue.SetRawJsonValueAsync(JSON);
            });
        }

        internal void ProTourGetHoles(ProTourPanel ptPanel, string gameID)
        {
            FBdata.GetReference(devPrefix + "tournaments/" + gameID).GetValueAsync().ContinueWith(t =>
            {
                Debug.Log("Got tournament holes: " + t.Result.Value.ToString());

                var tournament = JsonUtility.FromJson<RandomRoundWrapper>(t.Result.GetRawJsonValue());
                ptPanel.tournamentWrapper = tournament;
                ptPanel.hasHoles = true;
            });
        }

        internal void OpenGetHoles(TournamentPanel openPanel, string gameID)
        {
            FBdata.GetReference(devPrefix + "tournaments/" + gameID).GetValueAsync().ContinueWith(t =>
            {
                Debug.Log("Got tournament holes: " + t.Result.Value.ToString());

                var tournament = JsonUtility.FromJson<RandomRoundWrapper>(t.Result.GetRawJsonValue());
                openPanel.openHoles = tournament;
                openPanel.hasHoles = true;
            });
        }

        //only for testing
        public virtual void GetPastOpenTournament(TournamentPanel panel, string ID)
        {
            Debug.Log("Trying to get open tournament data");
            var tournamentsRef = FBdata.GetReference(devPrefix + "tournaments/");
            tournamentsRef.Child(ID).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Not able to get open tournament ref");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("task completed succesfully");
                    Debug.Log(task.Result.ChildrenCount);
                    List<OpenScore> scores = new List<OpenScore>();

                    foreach (var s in task.Result.Child("/scores/").Children)
                    {
                        var score = JsonUtility.FromJson<OpenScore>(s.GetRawJsonValue());
                        score.userid = s.Key;
                        scores.Add(score);
                    }
                    panel.GotOpenScores(scores);
                }
            });
        }

        //only for admin
        internal void PushTournament(RandomRoundWrapper open)
        {
            var r = FBdata.GetReference(devPrefix + "tournaments");
            var key = r.Push().Key;

            r.Child(key).SetRawJsonValueAsync(JsonUtility.ToJson(open));
            r.Child(key + "/tournamentKey/").SetValueAsync(key);
        }

        //only for admin
        internal void ClearTournaments()
        {
            var tournamentsRef = FBdata.GetReference(devPrefix + "tournaments/");
            tournamentsRef.OrderByChild("status").EqualTo("closed").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Not able to get open tournament ref");
                }
                else if (task.IsCompleted)
                {
                    foreach (var t in task.Result.Children)
                    {
                        t.Reference.RemoveValueAsync();
                    }
                }
            });
        }

        //only for admin
        //internal void SetPlayerRating()
        //{
        //    Debug.Log("getting result data");
        //    FBdata.GetReference(devPrefix + "playerData/").GetValueAsync().ContinueWith(task1 =>
        //     {
        //         if (task1.IsFaulted)
        //         {
        //             Debug.Log("Not able to get results");
        //         }
        //         else if (task1.IsCompleted)
        //         {
        //             //  var testCounter = 0;
        //             foreach (var user in task1.Result.Children)
        //             {
        //                 Debug.Log("processing user: " + user.Key);
        //                 if (user.Child("/results/").ChildrenCount > 0)
        //                 {
        //                     Debug.Log("user has results");
        //                     float rating = 100f;
        //                     OpenResult result = new OpenResult();
        //                     foreach (var t in user.Child("/results/").Children)
        //                     {
        //                         result = JsonUtility.FromJson<OpenResult>(t.GetRawJsonValue());

        //                         float percentile = (float)result.position / (float)result.participants < 0.1f ? 0.1f : (float)result.position / (float)result.participants;
        //                         float log = Mathf.Log(percentile, 0.01f);
        //                         var mod = (50f * (1f - rating / 1000f) * log) + 1f;
        //                         if (result.claimedReward == 1)
        //                         {
        //                             rating += mod;
        //                         }

        //                         //result.ratingMod = mod;
        //                         Debug.Log("Setting rating percentile: " + percentile + "score: " + rating + "mod: " + mod);

        //                         FBdata.GetReference(devPrefix + "playerData/" + user.Key + "/results/" + t.Key).SetRawJsonValueAsync(JsonUtility.ToJson(result));
        //                     }
        //                     FBdata.GetReference(devPrefix + "playerData/" + user.Key + "/playerStats/openTournamentRating").SetValueAsync((int)Math.Round(rating));
        //                     //testCounter++;
        //                     //if (testCounter > 9) return;
        //                 }
        //             }
        //         }
        //     });
        //}

        //only for admin
        public virtual void TestProTourResult()
        {
            var proTourRef = FBdata.GetReference(devPrefix + "proTour/Recreational/");

            var tour = new ProTourWrapper();
            tour.division = ProTourWrapper.Division.Recreational;

            tour.rounds = new List<ProTourRound>();
            var round = new ProTourRound
            {
                unlocked = true,
                holesID = "-LaUNT0jh4WBIXRkvZEO"
            };
            tour.rounds.Add(round);
            tour.rounds.Add(round);
            tour.rounds.Add(round);
            tour.rounds.Add(round);

            var jsonTour = JsonUtility.ToJson(tour);

            proTourRef.SetRawJsonValueAsync(jsonTour);
        }

        //only for admin
        public virtual void CreateOpenTour()
        {
            var proTourRef = FBdata.GetReference(devPrefix + "openTournament/Recreational/");

            var open = new OpenTournamentWrapper();
            open.division = ProTourWrapper.Division.Recreational;
            open.round = "-LaUNT0jh4WBIXRkvZEO";
            open.week = 0;

            var jsonTour = JsonUtility.ToJson(open);

            proTourRef.SetRawJsonValueAsync(jsonTour);
        }
    }
}
