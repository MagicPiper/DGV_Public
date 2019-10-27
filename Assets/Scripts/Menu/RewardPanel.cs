using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class RewardPanel : MenuPane
    {
        //DiscReward
        public CanvasGroup discRewardPane;
        public GameObject UIDiscPrefab;
        public Transform rewardHolder;
        private List<GameObject> discRewards;
        public TMP_Text chooseOneText;
        public Button acceptButton;
        //XPReward
        public CanvasGroup XPrewardPane;
        public TMP_Text rewardText;
        public TMP_Text positionText;
        public TMP_Text XPText;
       // public TMP_Text ratingChangeText;
        public Slider XPslider;
        public Image sliderBackground;
        public Color flashColor;
        public TMP_Text currentXP;
        public Button OKButton;
        public TMP_Text levelText;
        private int rewards;
        private int rewardLevel;
        //public GameObject openStatsPanel;
        public int testValue;

        public GameState gameState;

        internal void DiscReward(List<Disc> rewards)
        {
            discRewardPane.gameObject.SetActive(true);
            XPrewardPane.gameObject.SetActive(false);
            FadeInPane(GetComponent<CanvasGroup>());
            chooseOneText.gameObject.SetActive(false);

            gameState.playerSave.SaveDiscUnlocks(rewards);
            PopulateDiscs(rewards, UIDisc.Context.Reward);

            FadeInPane(discRewardPane);
            acceptButton.gameObject.SetActive(true);
        }

        private void PopulateDiscs(List<Disc> rewards, UIDisc.Context context)
        {
            if (discRewards != null)
            {
                foreach (GameObject o in discRewards)
                {
                    Destroy(o);
                }
            }
            discRewards = new List<GameObject>();

            gameObject.SetActive(true);
            foreach (Disc disc in rewards)
            {
                var reward = Instantiate(UIDiscPrefab);
                reward.transform.SetParent(rewardHolder, false);
                reward.GetComponent<UIDisc>().Populate(disc, context);
                reward.GetComponent<UIDisc>().rewardPanel = this;
                discRewards.Add(reward);
            }
        }

        internal void ChooseOneDiscReward(List<Disc> rewards)
        {
            discRewardPane.gameObject.SetActive(true);
            XPrewardPane.gameObject.SetActive(false);
            FadeInPane(GetComponent<CanvasGroup>());
            chooseOneText.gameObject.SetActive(true);

            PopulateDiscs(rewards, UIDisc.Context.SelectOne);

            FadeInPane(discRewardPane);
            acceptButton.gameObject.SetActive(false);
        }

        internal void Selected(Disc disc)
        {
            gameState.playerSave.SaveDiscUnlocks(disc);
            discRewardPane.gameObject.SetActive(false);
            XPrewardPane.gameObject.SetActive(true);

            if (rewards > 0)
            {
                rewardLevel++;
                ChooseOneDiscReward(gameState.playerSave.discData.GetLevelUpDiscs(rewardLevel));
                rewards--;
            }
        }

        [ContextMenu("Test XP Reward")]
        public void TestXpReward()
        {
            XPReward(testValue, 1, "Tournament Reward");
        }

        internal void XPReward(int XP, int position, string rewardText)
        {
            discRewardPane.gameObject.SetActive(false);
            XPrewardPane.gameObject.SetActive(true);
            FadeInPane(GetComponent<CanvasGroup>());
            this.rewardText.text = rewardText;
            positionText.text = position.ToString();
            XPText.text = "+" + XP.ToString();
            levelText.text = gameState.playerSave.playerStats.statsData.playerLevel.ToString();
            //if(ratingsChange< 0)
            //{
            //    ratingChangeText.text = ratingsChange.ToString();
            //    ratingChangeText.color = Color.red;
            //}
            //else
            //{
            //    ratingChangeText.text = "+" + ratingsChange.ToString();
            //    ratingChangeText.color = Color.green;
            //}

            var stats = gameState.playerSave.playerStats;
            var oldXP = stats.statsData.playerXP;
            var oldLevel = gameState.playerSave.playerStats.statsData.playerLevel;
            levelText.text = oldLevel.ToString();
            int levelUp = stats.XPUpdate(XP);
            var newXP = stats.statsData.playerXP;
            StartCoroutine(GainXP(oldXP, newXP, oldLevel, levelUp, XP));
           // gameState.playerSave.playerStats.RatingsUpdate(ratingsChange);
        }

        private IEnumerator GainXP(int oldXP, int newXP, int oldLevel, int levelUp, int xp)
        {
            OKButton.interactable = false;
            int levelCounter = levelUp;
            int playerLevel = oldLevel;
            XPslider.maxValue = gameState.playerSave.playerStats.LevelUpLimit(playerLevel);
            XPslider.value = oldXP;
            var speed = 10f + ((float)xp * 0.2f);
            var target = 0;
            currentXP.text = oldXP + "/" + gameState.playerSave.playerStats.LevelUpLimit(playerLevel);

            while (levelCounter > 0)
            {
                target = gameState.playerSave.playerStats.LevelUpLimit(playerLevel);
                XPslider.maxValue = target;
                playerLevel++;
                yield return StartCoroutine(AnimateXPSlider(target, speed, target));
                levelText.text = playerLevel.ToString();

                XPslider.value = 0f;
                currentXP.text = "0/" + gameState.playerSave.playerStats.LevelUpLimit(playerLevel);

                StartCoroutine(Flash());
                levelCounter--;
            }
            target = newXP;
            XPslider.maxValue = gameState.playerSave.playerStats.LevelUpLimit(playerLevel);
            yield return StartCoroutine(AnimateXPSlider(target, speed, gameState.playerSave.playerStats.LevelUpLimit(playerLevel)));
            levelText.text = gameState.playerSave.playerStats.statsData.playerLevel.ToString();

            if(levelUp > 0)
            {
                yield return new WaitForSeconds(1.5f);
                rewards = levelUp-1;
                rewardLevel = oldLevel + 1;
                ChooseOneDiscReward(gameState.playerSave.discData.GetLevelUpDiscs(rewardLevel));
            }

            OKButton.interactable = true;
        }

        private IEnumerator AnimateXPSlider(int target, float speed, int max)
        {
            while (XPslider.value < target)
            {
                XPslider.value += speed * Time.deltaTime;
                currentXP.text = (int)XPslider.value + "/" + max;

                yield return null;
            }
        }

        [ContextMenu("Test flash")]
        public void Testflash()
        {
            StartCoroutine(Flash());

        }

        private IEnumerator Flash()
        {
            var c = sliderBackground.color;
            var time = 0.2f;
            sliderBackground.CrossFadeColor(flashColor, time, false, false);
            yield return new WaitForSeconds(time);
            sliderBackground.CrossFadeColor(c, time, false, false);
            yield return new WaitForSeconds(time);
            sliderBackground.CrossFadeColor(flashColor, time, false, false);
            yield return new WaitForSeconds(time);
            sliderBackground.CrossFadeColor(c, time, false, false);
        }

        public override void Back()
        {

        }
    }
}

