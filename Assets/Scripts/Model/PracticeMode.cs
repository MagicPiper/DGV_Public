
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "PracticeMode", menuName = "DiscGolf/PracticeMode", order = 1)]
    public class PracticeMode : ScriptableObject
    {
        public enum PracticeType { Putting, TargetPractice};
        public PracticeType type;
        public string sceneName;                
    }
}
