using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class OpenTournamentWrapper
    {       
        public int week;
        public ProTourWrapper.Division division;
        public string round;
        public bool closed;
    }
}
