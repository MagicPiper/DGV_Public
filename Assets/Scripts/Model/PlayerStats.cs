using System;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class PlayerStats
    {
        public PlayerStatsData statsData;
        public PlayerSave playerSave;

        public void Ace()
        {
            statsData.aces++;
            SaveStats();
        }

        public void Eagle()
        {
            statsData.eagles++;
            SaveStats();
        }

        internal void DevSupportPurchased()
        {
            statsData.devSupport = true;
            SaveStats();
        }

        public void Albatross()
        {
            statsData.albatrosses++;
            SaveStats();
        }

        public bool LongestDrive(int distance)
        {
            if (distance > statsData.longestDrive)
            {
                statsData.longestDrive = distance;
                SaveStats();
                return true;
            }
            return false;
        }

        private void SaveStats()
        {
            Debug.Log("SaveStats");

            playerSave.SavePlayerStats(statsData);
        }

        public bool LongestPutt(int distance)
        {
            if (distance > statsData.longestPutt)
            {
                statsData.longestPutt = distance;
                SaveStats();
                return true;
            }
            return false;
        }

        internal void NewStats()
        {
            Debug.Log("newstats");
            statsData = new PlayerStatsData()
            {
                longestDrive = 0,
                longestPutt = 0,
                eagles = 0,
                albatrosses = 0,
                aces = 0,
                openTournamentRating = 100,
                gotLegacyCrab=true
            };
            SaveStats();
        }

        internal void RatingsUpdate(int ratingsMod)
        {
            if (statsData.openTournamentRating + ratingsMod <= 100)
            {
                statsData.openTournamentRating = 100;
            }
            else
            {
                statsData.openTournamentRating += ratingsMod;
            }
            SaveStats();
        }

        internal void NewRoundRating(RoundRating rating)
        {
            statsData.roundRatings.Insert(0, rating);
            if(statsData.roundRatings.Count > 25)
            {
                statsData.roundRatings.RemoveAt(25);
            }
            SaveStats();
        }

        public int UpdateRating()
        {
            float sum = 0;
            float count = 0;
            var oldRating = statsData.playerRating;
            if (statsData.roundRatings.Count < 4)
            {
                statsData.playerRating = 0f;
            }
            else
            {
                foreach (RoundRating rating in statsData.roundRatings)
                {
                    sum += rating.rating * rating.weight;
                    count += rating.weight;
                }
                statsData.playerRating = Mathf.Round(sum / count);
            }

            if (statsData.playerRating > 850)
            {
                statsData.division = ProTourWrapper.Division.Pro;
            }
            else if (statsData.playerRating > 550)
            { 
                statsData.division = ProTourWrapper.Division.Advanced;
            }
            //else if (statsData.playerRating > 650)
            //{
            //    statsData.division = ProTourWrapper.Division.Intermediate;
            //}
            else
            {
                statsData.division = ProTourWrapper.Division.Recreational;
            }       

            SaveStats();
            return (int)statsData.playerRating -(int)oldRating;
        }

        internal void MultiplayerOverwrite(RoundRating roundRating)
        {
            statsData.roundRatings[0] = roundRating;           
            SaveStats();
        }

        internal int XPUpdate(int xpMod)
        {
            int LevelUp = 0;

            statsData.playerXP += xpMod;
            while (statsData.playerXP >= LevelUpLimit(statsData.playerLevel))
            {
                statsData.playerXP -= LevelUpLimit(statsData.playerLevel);
                statsData.playerLevel++;
                LevelUp++;
            }
            SaveStats();

            return LevelUp;
        }

        public int LevelUpLimit(int level)
        {
            var l = 200 + (level * 100);
            return l > 1000 ? 1000 : l;
        }

        internal int GetOpenTournamentRating()
        {
            if (statsData.openTournamentRating == 0)
            {
                statsData.openTournamentRating = 100;
            }
            return statsData.openTournamentRating;
        }

        internal void GotLegacyCrab()
        {
            statsData.gotLegacyCrab = true;
            SaveStats();
        }

        internal void SaveEmail(string text)
        {
            statsData.email = text;
            SaveStats();
        }
    }
}
