using Assets.Scripts.Menu;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{    
    public class RoundManager
    {
        public PlayerSave playerSave;
        public string roundID;
        //  public bool isPractice;
        public ScoreCard scoreCard;
        public Course currentCourse;
        // public PracticeMode currentPractice;
        public Hole currentHole;
        public int currentHoleNumber;
        public List<Hole> holes;
        //public List<string> playerNames;
        public string roundName;
        //  public int currentPracticeShot;
        public bool FinalHole;
        public bool roundComplete = false;
        public bool retiredRun = false;
        public bool newBest;
        public int oldBest;
        public int newStars;
        public int oldStars;
        public List<Disc> rewardUnlocks;

        public bool gotHoleStats;
        public int[] holeHistory;
        public int holeBest;
        public bool canRestart=false;
        public RoundRating roundRating;

        internal virtual void StartRound(Course selectedCourse)
        {
            roundID = System.Guid.NewGuid().ToString();
            currentCourse = selectedCourse;
            FinalHole = false;
            newBest = false;
            rewardUnlocks = new List<Disc>();
            newStars = 0;
            oldBest = playerSave.BestCourseScore(currentCourse.courseID, currentCourse.courseVersion);
            oldStars = playerSave.GetUnlockedStars(currentCourse.courseID);
            currentHole = currentCourse.holes[0];
            currentHoleNumber = 1;
            gotHoleStats = false;
            holeBest = 0;
            holes = currentCourse.holes;
            canRestart = true;
            
            roundName = currentCourse.courseName;

            scoreCard = new ScoreCard()
            {
                ownerName = playerSave.PlayerDisplayName(),
                //isPlayer = true,
                scores = new int[currentCourse.holes.Count],
                diffFromPar = new int[currentCourse.holes.Count],
                icon = playerSave.playerSettings.settingsData.icon
                // courseID = currentCourse.courseID
            };
        }

        internal virtual void EndRound()
        {
            var unlockedStars = 0;
            if (scoreCard.score < oldBest)    //If we have a new best we need to check if we have unlocked any new stars
            {
                newBest = true;

                if (oldStars < 3)                                      //if we already have 3 stars on this course we can stop here.
                {
                    rewardUnlocks = new List<Disc>();
                    foreach (int limit in currentCourse.challengeLimits)
                    {
                        if (scoreCard.score <= (limit + currentCourse.Par))
                        {
                            unlockedStars++;
                            if (unlockedStars > oldStars)
                            {
                                newStars++;
                                rewardUnlocks.Add(currentCourse.challengeRewards[currentCourse.challengeLimits.IndexOf(limit)]);
                            }
                        }
                    }
                }
            }
            roundComplete = true;
            playerSave.SaveScore(currentCourse.courseID, currentCourse.courseVersion, scoreCard.score, newBest, unlockedStars);
        }

        internal virtual void HoleComplete(int strokes, string holeID)
        {
            if (scoreCard.scores[currentHoleNumber-1] == 0)
            {
                scoreCard.scores[currentHoleNumber - 1] = strokes;
                var diff = strokes - currentHole.par;
                scoreCard.diffFromPar[currentHoleNumber - 1] = diff;
                scoreCard.score = scoreCard.score + strokes;
                scoreCard.totalDiffFromPar += diff;  
            }
        }

        internal virtual void NextHole()
        {
            currentHole = currentCourse.holes[currentHoleNumber];
            currentHoleNumber++;

            if (currentHoleNumber >= currentCourse.holes.Count)
            {
                FinalHole = true;
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentHole.holeScene);
            gotHoleStats = false;            
        }

        internal virtual void Retire()
        {
            retiredRun = true;
            SceneManager.LoadScene("MenuScene");
        }

        internal virtual void Restart()
        {             
            SceneManager.LoadScene(currentCourse.holes[0].holeScene);
            StartRound(currentCourse);
        }

        internal void BackToMenu()
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}