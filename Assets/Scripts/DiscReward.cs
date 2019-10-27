using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "DiscReward", menuName = "DiscGolf/DiscReward", order = 1)]
    public class DiscReward : ScriptableObject
    {
        public List<DiscMould> moulds;
       // public List<DiscProperty> properties;

        public bool colorsBurst;
        public bool colorsRecycled;
        public bool colorsRareStamps;

        public int minimumProperties;
        public int maximumProperties;
    }    
}
