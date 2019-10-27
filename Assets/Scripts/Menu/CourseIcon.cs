using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class CourseIcon : MonoBehaviour
    {
        public Image lockIcon;
        public List<Image> stars;
        public TMP_Text nameText;
        public PlayerSave playerSave;

        public TMP_Text lockStars;

        public Course course;

        // Use this for initialization
        void Awake()
        {
            //var rect = GetComponent<RectTransform>();
            //var origSize = rect.localScale;
            //rect.localScale = new Vector2(0f, 0f);
            //LeanTween.scale(rect, origSize, 1f);

            nameText.text = course.courseName;

            CheckUnlock();
        }

        public void CheckUnlock()
        {
            if (playerSave.IsCourseUnlocked(course))
            {
                lockIcon.gameObject.SetActive(false);
                gameObject.GetComponent<Button>().interactable = true;
                Stars(true);
            }
            else
            {
                lockIcon.gameObject.SetActive(true);
                nameText.enabled = false;
                gameObject.GetComponent<Button>().interactable = false;
                lockStars.text = "=" + course.starsToUnlock.ToString();
            }
        }

        private void Stars(bool showStars)
        {
            var s = playerSave.GetUnlockedStars(course.courseID);
            foreach (Image star in stars)
            {
                star.gameObject.SetActive(true);
                if (s > 0)
                {
                    star.color = Color.white;
                }
                else
                {
                    star.color = Color.black;
                }
                s--;
            }
        }
    }
}