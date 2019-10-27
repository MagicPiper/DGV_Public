using Assets.Scripts;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class PlayerProfile
    {
        public List<CourseProgress> courseProgress;
        public List<Disc> discBag;
        public List<Disc> collection;
        public TutorialChecklist tutorialChecklist;
        public string version;
    }
}
