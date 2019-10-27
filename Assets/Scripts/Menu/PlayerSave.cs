using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Assets.Scripts.Menu
{
    [CreateAssetMenu(fileName = "PlayerSave", menuName = "DiscGolf/PlayerSave", order = 6)]
    public class PlayerSave : ScriptableObject
    {
        public Disc startingDisc;
        public Disc legacyCrab;
        private PlayerProfile currentProfile;
       // public PlayerProfile offlineProfile;
        public PlayerStats playerStats = new PlayerStats();
        public PlayerSettings playerSettings = new PlayerSettings();
        public static bool playerGet = false;
        public static bool offline = false;
        public StartMenu start;
        public DiscData discData;

        public float LatestVersion { get; internal set; } = -1;

        private FireBaseManager firebaseManager;
        public FireBaseManager FirebaseManager
        {
            get
            {
                if (firebaseManager == null)
                {
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        // firebaseManager = new WebGLFireBaseManager();
                        //firebaseManager.firebaseInitiated = true;
                    }
                    else if (Application.platform == RuntimePlatform.Android)
                    {
                        firebaseManager = new FireBaseManager();
                    }
                    else
                    {
                        firebaseManager = new FireBaseManager();
                    }
                    firebaseManager.playerSave = this;
                }
                return firebaseManager;
            }
        }

        internal void SavePlayerSettings(PlayerSettingsData settingsData)
        {
            Debug.Log("Saving Player Data");
            var json = JsonUtility.ToJson(settingsData);

            firebaseManager.UpdatePlayerSettings(json);
        }

        internal void SavePlayerStats(PlayerStatsData statsData)
        {
            Debug.Log("Saving Player Data");
            var json = JsonUtility.ToJson(statsData);

            firebaseManager.UpdatePlayerStats(json);
        }

        public IEnumerator GetPlayer()
        {
            playerStats.playerSave = this;
            playerSettings.playerSave = this;
            float timeout = 0f;
            while (!firebaseManager.firebaseInitiated)
            {
                yield return new WaitForSeconds(1f);
                timeout += 1f;
                if (timeout > 30f)
                {
                    Debug.Log("Firebase not initiated");
                    yield break;
                }
            }
            yield return new WaitForSeconds(1f);

            firebaseManager.playerAuthenticated = false;
            firebaseManager.authenticationFailed = false;
            firebaseManager.PlayerAuthenticatedCheck();

            while (!firebaseManager.playerAuthenticated)
            {
                if (firebaseManager.authenticationFailed)
                {
                    start.AuthFailed();
                    yield break;
                }
                if (firebaseManager.authenticationCanceled)
                {
                    start.AuthCanceled();
                    yield break;
                }
                yield return new WaitForSeconds(1f);
            }

            Debug.Log("player authenticated: " + firebaseManager.playerAuthenticated);
            
            playerGet = false;
            firebaseManager.NewPlayerCheck();

            while (!playerGet)
            {
                yield return new WaitForSeconds(0.2f);

            }
            firebaseManager.GetCurrentVersion();

            LegacyCrabCheck();

            start.IntroAnimation();

            yield return null;
        }

        private void LegacyCrabCheck()
        {
            if (!playerStats.statsData.gotLegacyCrab)
            {
                Debug.Log("Legacy Crab added");

                playerStats.GotLegacyCrab();

                SaveDiscUnlocks(legacyCrab);

            }
        }

        internal void SetPlayerSettings(PlayerSettingsData settings)
        {
            playerSettings.settingsData = settings;
        }

        internal void SetPlayerStats(PlayerStatsData stats)
        {
            playerStats.statsData = stats;
        }

        internal void PlayOffline()
        {
            currentProfile = null;
            //  firebaseManager = new OfflineFireBaseManager();
            playerStats.statsData = null;
            playerSettings.settingsData = null;
            offline = true;
           // {
           //     leftHandedMode = false,
           //     volume = 1f
           // };
           // Debug.Log("creating a test player profile");
           // currentProfile = new PlayerProfile();
           // currentProfile.tutorialChecklist = new TutorialChecklist();
           // //currentProfile.courseProgress = new List<CourseProgress>();
           // currentProfile.discBag = new List<Disc>();
           //// currentProfile.collection = new List<Disc>();
           // currentProfile.discBag.Add(startingDisc);           
        }

        internal string PlayerDisplayName()
        {
            string casualDisplayName = "";
            if (string.IsNullOrEmpty(playerSettings.settingsData.displayName))
            {
                var s = FirebaseManager.playerDisplayName;
                string[] cleaned = s.Split(new char[] { ' ' });
                casualDisplayName = cleaned[0];
            }
            else
            {
                casualDisplayName = playerSettings.settingsData.displayName;
            }
            
            return casualDisplayName;
        }

        internal List<Disc> GetDiscBagContent()
        {
            return currentProfile.discBag;
        }

        internal List<Disc> GetDiscCollection()
        {
            return currentProfile.collection;
        }

        internal TutorialChecklist GetTutorialCheck()
        {
            return currentProfile.tutorialChecklist;
        }

        internal int GetDiscCount()
        {
            return currentProfile.discBag.Count + currentProfile.collection.Count;
        }

        internal int TotalStars()
        {
            int totalStars = 0;

            foreach (CourseProgress course in currentProfile.courseProgress)
            {
                totalStars = totalStars + course.unlockedStars;
            }
            return totalStars;
        }

        internal void CreateNewPlayer()
        {
            Debug.Log("creating a new player profile");
            currentProfile = new PlayerProfile();

            currentProfile.courseProgress = new List<CourseProgress>();
            currentProfile.discBag = new List<Disc>();
            currentProfile.collection = new List<Disc>();
            currentProfile.discBag.Add(startingDisc);
            currentProfile.tutorialChecklist = new TutorialChecklist();
            playerStats.NewStats();
            playerSettings.NewSettings();

            SaveProfile();
            playerGet = true;
        }

        internal void SaveScore(int courseID, string courseVersion, int totalScore, bool newBest, int unlockedStars)
        {
           // FirebaseManager.SubmitCourseScore((courseID - 1) + courseVersion, totalScore);
            var score = currentProfile.courseProgress.FirstOrDefault(i => i.courseID == courseID);

            if (newBest)
            {
                score.bestScore = totalScore;
                score.courseVersion = courseVersion;
            }
            if (unlockedStars > score.unlockedStars)
            {
                score.unlockedStars = unlockedStars;
            }
            SaveProfile();
        }

        //internal void GetOpenReward()
        //{
        //    firebaseManager.GetFirstUnclaimedReward();
        //}

        public void SaveDiscUnlocks(List<Disc> discUnlocks)
        {
            foreach (Disc disc in discUnlocks)
            {
                if (currentProfile.discBag.Count < 6)
                {
                    currentProfile.discBag.Add(disc);
                }
                else
                {
                    currentProfile.collection.Add(disc);
                }
            }
            SaveProfile();
        }

        public void SaveDiscUnlocks(Disc discUnlock)
        {
            var list = new List<Disc>();
            list.Add(discUnlock);
            SaveDiscUnlocks(list);
        }

        internal int BestCourseScore(int courseID, string courseVersion)
        {
            var courseProgress = currentProfile.courseProgress.FirstOrDefault(i => i.courseID == courseID);

            //if both strings are empty we don't need to do anything
            if (String.IsNullOrEmpty(courseProgress.courseVersion) && String.IsNullOrEmpty(courseVersion))
            {
                return courseProgress.bestScore;
            }
            else
            {
                //if the versions diff we reset the best score.
                if (courseProgress.courseVersion != courseVersion)
                {
                    Debug.Log("newversion. courseProgress version: " + courseProgress.courseVersion + " actual course version " + courseVersion);

                    courseProgress.bestScore = 999;
                    //courseProgress.courseVersion = courseVersion;
                }
                return courseProgress.bestScore;
            }
        }

        internal string PlayerEmail()
        {
            if(string.IsNullOrEmpty(playerStats.statsData.email))
            {
                if (string.IsNullOrEmpty(firebaseManager.playerEmail))
                {
                    playerStats.statsData.email = "";
                }
                else
                {
                    playerStats.statsData.email = firebaseManager.playerEmail;
                }
            }

            return playerStats.statsData.email;
        }

        internal void SetCurrentProfile(PlayerProfile playerProfile)
        {
            currentProfile = playerProfile;
        }

        internal void SaveProfile()
        {
            Debug.Log("Saving Player Profile");
            var jsonProgress = JsonUtility.ToJson(currentProfile);

            firebaseManager.UpdatePlayerData(jsonProgress);
        }

        internal bool IsCourseUnlocked(Course course)
        {
            var check = currentProfile.courseProgress.FirstOrDefault(i => i.courseID == course.courseID);
            if (check != null)
            {
                return true;
            }
            else
            {
                if (TotalStars() >= course.starsToUnlock)
                {
                    var newProgress = new CourseProgress { courseID = course.courseID, bestScore = 999, unlockedStars = 0 };
                    currentProfile.courseProgress.Add(newProgress);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal int GetUnlockedStars(int courseID)
        {
            var course = currentProfile.courseProgress.FirstOrDefault(i => i.courseID == courseID).unlockedStars;
            return course;
        }

        //internal void GetHighScores(string courseID, ChallengeMap returnTo)
        //{
        //    FirebaseManager.GetCourseHighScores(courseID, returnTo);
        //}
    }
}
