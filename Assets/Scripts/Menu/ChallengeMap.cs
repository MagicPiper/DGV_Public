using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class ChallengeMap : MenuPane
    {
        public List<CourseIcon> courseIcons;
        public ButtonBehavior totalStarsPane;
       // public ButtonBehavior discBagButton;
        public ButtonBehavior backButton;

        public TMP_Text totalStars;
        public TMP_Text playerTitle;

        public PlayerSave playerSave;

        public Course selectedCourse = null;
        [SerializeField] private Course startCourse;
        private Color buttonColor;
        private Color buttonHighlightColor = new Color(0.537f, 0.788f, 0.537f);

        // public GameObject blockPane;
        public ButtonBehavior coursePane;

        public List<Image> coursePaneStars;
        public List<TMP_Text> starText;
        public TMP_Text courseName;
        public TMP_Text courseDescription;
        public TMP_Text coursePar;
        public TMP_Text yourBest;
        public Image courseImage;

        public Button playButton;
        public TMP_Text playButtonText;
        public UnityAction seenRewards;

        public RewardPanel rewardPanel;
        public GameState gameState;
        public bool returning = false;
        //public bool hasHighScores = false;
        //public List<Score> currentScores;

        [SerializeField] private CanvasGroup highScores;
        [SerializeField] private TMP_Text[] highScoreNames;
        [SerializeField] private TMP_Text[] highScoreScores;

        private void Awake()
        {
            buttonColor = courseIcons[0].GetComponent<Image>().color;
        }

        //private void Update()
        //{
        //    if (hasHighScores)
        //    {
        //        ShowHighScores();
        //        hasHighScores = false;
        //    }
        //}

        public void Show(bool returning)
        {
            currentPane = this;
            FadeInPane(GetComponent<CanvasGroup>());

            coursePane.gameObject.SetActive(false);
            this.returning = returning;
            float iconDelay = 1;

            foreach (CourseIcon icon in courseIcons)
            {
                icon.gameObject.SetActive(false);
                StartCoroutine(ShowIcon(iconDelay, icon.gameObject));
                iconDelay += 0.1f;
            }

            var stars = playerSave.TotalStars();
            totalStars.text = stars.ToString();
            playerTitle.text = PlayerTitle(stars);

         //   discBagButton.SlideIn(ButtonBehavior.Direction.left, 1f, 0.5f);
            totalStarsPane.SlideIn(ButtonBehavior.Direction.left, 1f, 0.5f);
            backButton.SlideIn(ButtonBehavior.Direction.left, 1f, 0.5f);

            if (!returning)
            {
                selectedCourse = startCourse;
            }
            else
            {
                selectedCourse = gameState.roundManager.currentCourse;
            }

            ShowCourse(selectedCourse);
        }

        //private void GetHighScoreCache(string courseID)
        //{
        //    playerSave.GetHighScores(courseID, this);
        //}

        private string PlayerTitle(int stars)
        {
            string title;

            if (stars <= 5)
            {
                title = "Discgolf Chump";
            }
            else if (stars < 10)
            {
                title = "Discgolf Rookie";
            }
            else if (stars < 15)
            {
                title = "Discgolf Challenger";
            }
            else if (stars < 21)
            {
                title = "Discgolf Veteran";
            }
            else
            {
                title = "Discgolf Champion";
            }
            return title;
        }

        IEnumerator ShowIcon(float delay, GameObject icon)
        {
            yield return new WaitForSeconds(delay);
            icon.GetComponent<ButtonBehavior>().EnablePopIn();
        }

        public void ChangeCourse(Course course)
        {
            selectedCourse = course;
            CourseButtonsEnabled(false);
            coursePane.SlideOut(ButtonBehavior.Direction.down, 0.5f, 0f);

            ShowCourse(course);
        }

        private void CourseButtonsEnabled(bool enabled)
        {
            foreach (CourseIcon icon in courseIcons)
            {
                if (icon.lockIcon.gameObject.activeSelf == false)
                {
                    if (icon.course == selectedCourse)
                    {
                        icon.GetComponent<Button>().interactable = false;
                        icon.GetComponent<Image>().color = buttonHighlightColor;
                    }
                    else
                    {
                        icon.GetComponent<Button>().interactable = enabled;
                        icon.GetComponent<Image>().color = buttonColor;
                    }
                }
            }
        }

        public void ShowCourse(Course course)
        {
            StartCoroutine(ShowCourseAnimation(course));
        }

        IEnumerator ShowCourseAnimation(Course course)
        {
            CourseButtonsEnabled(false);

            yield return new WaitForSeconds(0.5f);
            selectedCourse = course;

            courseName.text = course.courseName;
            courseDescription.text = course.courseDescription;
            courseImage.sprite = course.courseImage;
            var par = selectedCourse.Par;

            coursePar.text = "Par: " + par.ToString();
           // GetHighScoreCache((selectedCourse.courseID-1) + selectedCourse.courseVersion);
            coursePane.SlideIn(ButtonBehavior.Direction.up, 0.5f, 0f);


            if (returning && gameState.roundManager.newBest)
            {
                var best = gameState.roundManager.oldBest == 999 ? "No Score" : gameState.roundManager.oldBest.ToString();
                yourBest.text = "Your best: " + best;

                Stars(gameState.roundManager.oldStars);

                NewBest();
            }
            else
            {
                string best = (playerSave.BestCourseScore(course.courseID, course.courseVersion) == 999) ? "No Score" : playerSave.BestCourseScore(course.courseID, course.courseVersion).ToString();
                yourBest.text = "Your best: " + best;
                yourBest.color = Color.white;

                Stars(playerSave.GetUnlockedStars(selectedCourse.courseID));
                CourseButtonsEnabled(true);
                if (returning)
                {
                    playButtonText.text = "Play Again";
                    returning = false;
                    gameState.ResetRound();
                   // playerSave.GetOpenReward();
                }
                else
                {
                    playButtonText.text = "Play Course";
                }
            }
        }

        //public void ShowHighScores()
        //{
        //    highScores.gameObject.SetActive(true);

        //    for (int i = 0; i < 5; i++)
        //    {
        //        highScoreScores[i].text = "";
        //        highScoreNames[i].text = "";
        //    }
        //    int counter = 0;

        //    foreach (Score s in currentScores)
        //    {
        //        if (s != null)
        //        {
        //            // Debug.Log("score value is:  " + s.score.ToString());
        //            highScoreScores[counter].text = s.score.ToString();
        //            highScoreNames[counter].text = s.userName.Split(' ')[0];
        //        }
        //        else
        //        {
        //            // Debug.Log(counter.ToString() + "position was empty");
        //        }
        //        counter++;
        //    }
        //}

        //internal void GotHighScores(List<Score> highScore)
        //{
        //    currentScores = highScore;
        //    hasHighScores = true;
        //}

        private void SeenRewards()
        {
            Debug.Log("seen rewards");
            playButton.interactable = true;
            CourseButtonsEnabled(true);
            returning = false;
            gameState.ResetRound();
           // playerSave.GetOpenReward();
        }

        private void NewBest()
        {
            playButton.interactable = false;
            var newBest = gameState.roundManager.scoreCard.score.ToString();
            LeanTween.scale(yourBest.rectTransform, new Vector2(0f, 0f), 0.5f).setEase(LeanTweenType.easeInCubic).setDelay(1f).setOnComplete(() =>
             {
                 yourBest.text = "New best: " + newBest;
                 yourBest.color = Color.yellow;
                 LeanTween.scale(yourBest.rectTransform, new Vector2(1, 1f), 1f).setEase(LeanTweenType.easeOutBounce);
             });

            if (gameState.roundManager.newStars > 0)
            {
                var newStarDelay = 2f;
                var oldS = gameState.roundManager.oldStars;
                var newS = gameState.roundManager.newStars;
                foreach (Image star in coursePaneStars)
                {
                    if (oldS > 0)
                    {
                        oldS--;
                    }
                    else if (newS > 0)
                    {
                        newS--;
                        LeanTween.scale(star.rectTransform, new Vector2(0f, 0f), 0.5f).setEase(LeanTweenType.easeInCubic).setDelay(newStarDelay).setOnComplete(() =>
                        {
                            star.color = Color.white;
                            LeanTween.scale(star.rectTransform, new Vector2(1, 1f), 1f).setEase(LeanTweenType.easeOutBounce);
                        });
                        newStarDelay+= 0.6f;
                    }
                }

                if (gameState.roundManager.rewardUnlocks.Count > 0)
                {
                    seenRewards += SeenRewards;
                    Invoke("ShowRewardPane", newStarDelay + 1);                    
                }
                else
                {
                    SeenRewards();
                }
            }
            else
            {
                SeenRewards();
            }
        }

        private void ShowRewardPane()
        {
            rewardPanel.DiscReward(gameState.roundManager.rewardUnlocks);
            SeenRewards();
        }

        public void ShowCoursePane(CourseIcon course)
        {
            ChangeCourse(course.course);
        }

        //Stars for the Course Pane
        private void Stars(int stars)
        {
            foreach (Image star in coursePaneStars)
            {
                if (stars > 0)
                {
                    star.color = Color.white;
                }
                else
                {
                    star.color = Color.grey;
                }
                stars--;
            }

            var index = 0;
            foreach (TMP_Text star in starText)
            {
                star.text = (selectedCourse.challengeLimits[index] + selectedCourse.Par).ToString();
                index++;
            }
        }

        public void StartCourse()
        {
            gameState.NewRound();
            gameState.roundManager.StartRound(selectedCourse);
            StartCoroutine("LoadNewScene");
            playButtonText.text = "Loading";
            MusicManager.Instance.FadeOutMusic();
        }

        IEnumerator LoadNewScene()
        {
            // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(gameState.roundManager.currentHole.holeScene);

            // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
            while (!async.isDone)
            {
                var t = Mathf.PingPong(Time.time, 1f);
                playButtonText.alpha = t;
                yield return null;
            }
        }
    }
}

