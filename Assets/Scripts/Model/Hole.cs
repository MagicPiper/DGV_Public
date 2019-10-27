using System;

namespace Assets.Scripts
{
    [Serializable]
    public class Hole
    {
     //   public int holeNumber;
        public int par;
        public float difficulty;
        public string holeScene;

        public ShotSelector.ShotType preferedShot;
    }
}
