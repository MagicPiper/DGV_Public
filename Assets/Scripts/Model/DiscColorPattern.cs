using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "DiscColorPattern", menuName = "DiscGolf/Disc Color Pattern", order = 1)]
    public class DiscColorPattern : DiscColor
    {      
        public Color patternColor;
        public Sprite pattern;
    }
}
