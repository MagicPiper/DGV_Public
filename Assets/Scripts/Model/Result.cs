using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class OpenResult
    {
        public int position;
        public int participants;
        public int score;
        public ProTourTop3[] top3;
        public int parDiff;
        public ProTourWrapper.Division division;
    }
}