
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Course", menuName = "DiscGolf/Course", order = 1)]
    public class Course : ScriptableObject
    {
        public int courseID;
        public string courseVersion;
        public string courseName;
        [TextArea]
        public string courseDescription;
        public List<Hole> holes;
        public List<int> challengeLimits;
        public Disc[] challengeRewards;
        public int starsToUnlock;
        public Sprite courseImage;

        public int Par
        {
            get
            {
                int p=0;      
                foreach (Hole hole in holes)
                {
                    p = p + hole.par;
                }
                return p;
            }           
        }
    }
}
