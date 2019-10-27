using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "DiscColor", menuName = "DiscGolf/Disc Color", order = 1)]
    public class DiscColor : ScriptableObject
    {      
        public Color baseColor; 
        public Color stampColor; 
    }
}
