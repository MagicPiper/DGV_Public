using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class PlayerStatsData
    {
        public int longestDrive;
        public int longestPutt;
             
        public int eagles;
        public int albatrosses;
        public int aces;

        public int openTournamentRating; //LEGACY
        public int playerXP;
        public int playerLevel;

        public bool gotLegacyCrab;

        public List<RoundRating> roundRatings;
        public float playerRating;
        public ProTourWrapper.Division division;
        public bool devSupport;
        public string email;
    }
}
