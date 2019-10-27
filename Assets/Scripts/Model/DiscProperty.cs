using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "DiscProperty", menuName = "DiscGolf/Disc Property", order = 2)]
    public class DiscProperty : ScriptableObject
    {        
        public enum PropertyType
        {
            Undefined = 0,
            BeatUp = 1,
            ExtraGlide = 2,
            BigSkip = 3,
            Sticky = 4,
            Accurate = 5,
            Heavy = 6,
            Light = 7,
            WindBreak = 8,
            Fade = 9,
            Turn = 10
        }

        public PropertyType type;

        public string description;
        public string propertyName;

        public float value;

        public Color uiColor;
        
        internal void ApplyProp(DiscBehavior disc)
        {
            switch (type)
            {
                case PropertyType.BeatUp:
                    disc.dragMod = value;
                    break;
                case PropertyType.ExtraGlide:
                    disc.dragMod = value;
                    break;
                case PropertyType.BigSkip:
                    disc.Cl.material = disc.bouncyMaterial;                    
                    disc.boxCl.material = disc.bouncyMaterial;
                    break;
                case PropertyType.Sticky:
                    disc.Cl.material = disc.stickyMaterial;
                    disc.boxCl.material = disc.stickyMaterial;
                    break;
                case PropertyType.Accurate:
                    disc.accuracyMod = value;
                    break;
                case PropertyType.Heavy:
                    disc.rb.mass *= 1.05f;
                    break;
                case PropertyType.Light:
                    disc.rb.mass *= 0.95f;
                    break;
                case PropertyType.Fade:
                    disc.fadeMod = value;
                    break;
                case PropertyType.Turn:
                    disc.fadeMod = value;
                    break;
                case PropertyType.WindBreak:
                    disc.windBreak = value;
                    break;
                default:
                    break;
            }
        }
    }
}
