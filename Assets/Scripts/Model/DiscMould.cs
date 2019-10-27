using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "DiscMould", menuName = "DiscGolf/Disc Mould", order = 1)]
    public class DiscMould : ScriptableObject
    {
        public enum MouldName
        {
            Undefined = 0,
            Dagger = 1,
            Compass = 2,
            Explorer = 3,
            Fuse = 4,
            Pure = 5,
            River = 6,
            Musket = 7,
            Keystone = 8,
            Anchor = 9,
            Pioneer = 10,
            Recoil = 11,
            Ballista = 12,
            Crab = 98,
            TestDisc = 99,
            MenuDisc = 100
        }
        public MouldName mouldName;
        [TextArea]
        public string mouldDescription;
        public enum DiscType
        {
            Undefined = 0,
            Putter = 1,
            Midrange = 2,
            Fairway = 3,
            Distance = 4
        }

        public DiscType discType;

        public enum Stability
        {
            NULL = 0,
            OS = 1,
            N = 2,
            US = 3
        }

        public Stability stability;

        //Disc mold properties: 1-10
        public float drag;
        public float lift;
        public float turn;
        public float fade;

        public int baseWeight;
        public int speed;
        public Sprite mouldStamp;
        public Sprite mouldStampAlternate1;

        public DiscData.MouldRarity rarity;
    }
}
