using System;

namespace Assets.Scripts
{
    [Serializable]
    public class CourseProgress
    {
        public int courseID;
        public string courseVersion;
        public int bestScore;
        //Challenge Mode attributes 
        public int unlockedStars;
    }
}
