//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.UI;

//namespace Assets.Scripts.Menu
//{
//    public class RewardNotification : MonoBehaviour
//    {
//        public Button button;
//        public PlayerSave playerSave;
//        public RewardPanel rewardPanel;
//        private int ratingsMod;
//        private int XPgain;
//        private OpenResult result;
//        private string rewardText;
//        public ButtonBehavior buttonBehavior;
//        private bool newReward = false;
//        private bool populated = false;

//        private void Update()
//        {
//            if (newReward && !populated)
//            {
//                PopulateReward();
//            }
//        }

//        private void PopulateReward()
//        {
//            Debug.Log("populate");
           
//            buttonBehavior.SlideIn(ButtonBehavior.Direction.up, 0.5f, 0.5f);
//            populated = true;
//        }

//        internal void NewReward(OpenResult result)
//        {           
//            this.result = result;
//            newReward = true;
//            //ratingsMod = CalculateRatingsMod(result);
//            XPgain = CalculateXPGain(result);
//            rewardText = "Open Tournament Reward";
//        }

//        //public int CalculateRatingsMod(Result result)
//        //{
//        //    float percentile = (float)result.position / (float)result.participants < 0.1f ? 0.1f : (float)result.position / (float)result.participants;
//        //    float log = Mathf.Log(percentile, 0.01f);
//        //    var mod = (50f * (1f - (float)playerSave.playerStats.GetOpenTournamentRating() / 1000f) * log) + 1f;
//        //    mod = mod <= 1 ? 1:mod;  

//        //    return (int)Mathf.Ceil(mod);
//        //}

//        public int CalculateXPGain(OpenResult result)
//        {
//            var xp = 3000 /(result.position + 5);
//            return (int)xp > 100 ? (int)xp : 100; 
//        }

//        public void OnClick()
//        {
//            rewardPanel.XPReward(XPgain, result.position, rewardText);
//            playerSave.FirebaseManager.ClaimReward(result.id);
//            buttonBehavior.SlideOut(ButtonBehavior.Direction.up, 0.5f, 0.5f);
//        }
//    }
//}

