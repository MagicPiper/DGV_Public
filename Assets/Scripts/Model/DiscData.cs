using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "DiscData", menuName = "DiscGolf/DiscData", order = 5)]
    public class DiscData : ScriptableObject
    {
        [SerializeField] private DiscMould[] discMoulds;
        [SerializeField] private DiscMould menuDisc;
        [SerializeField] private DiscProperty[] discPropertiesPutters;
        [SerializeField] private DiscProperty[] discPropertiesMidrange;
        [SerializeField] private DiscProperty[] discPropertiesFairway;
        [SerializeField] private DiscProperty[] discPropertiesDistance;
        [SerializeField] private DiscProperty[] discPropertiesAll;

        [SerializeField] private Color[] discColors; //Legacy
        [SerializeField] private Color[] stampColors; //Legacy

        [SerializeField] public DiscColor[] basicColors;
        [SerializeField] public DiscColorPattern[] burstColors;
        [SerializeField] public DiscColorPattern[] recycledColors;
        [SerializeField] public DiscColorPattern[] rareStamps;

        [SerializeField] public DiscReward[] openRewards;

        public enum MouldRarity { common, rare, test };

        public DiscMould GetMould(DiscMould.MouldName mouldName)
        {
            for (int i = 0; i < discMoulds.Length; i++)
            {
                if (discMoulds[i].mouldName == mouldName)
                {
                    return discMoulds[i];
                }
            }
            return discMoulds[0];
        }

        public DiscColor GetColor(Disc disc)
        {
            for (int i = 0; i < basicColors.Length; i++)
            {
                if (basicColors[i].name == disc.colorsName)
                {
                    return basicColors[i];
                }
            }
            for (int i = 0; i < burstColors.Length; i++)
            {
                if (burstColors[i].name == disc.colorsName)
                {
                    return burstColors[i];
                }
            }
            for (int i = 0; i < recycledColors.Length; i++)
            {
                if (recycledColors[i].name == disc.colorsName)
                {
                    return recycledColors[i];
                }
            }
            for (int i = 0; i < rareStamps.Length; i++)
            {
                if (rareStamps[i].name == disc.colorsName)
                {
                    return rareStamps[i];
                }
            }

            var randomBasic = RandomBasicColor();
            disc.colorsName = randomBasic.name;
            return randomBasic;
        }

        public DiscMould GetRandomMould(List<DiscMould> mouldsIn)
        {
            List<DiscMould> moulds = mouldsIn;
            moulds.Shuffle();
            return moulds[0];
        }

        public IEnumerable<DiscProperty> GetRandomProperties(int count, DiscMould.DiscType type)
        {
            var propsList = new List<DiscProperty>();
            var returnList = new List<DiscProperty>();
            
            switch (type)
            {
                case DiscMould.DiscType.Putter:
                    propsList.AddRange(discPropertiesPutters);
                    break;
                case DiscMould.DiscType.Midrange:
                    propsList.AddRange(discPropertiesMidrange);
                    break;
                case DiscMould.DiscType.Fairway:
                    propsList.AddRange(discPropertiesFairway);
                    break;
                case DiscMould.DiscType.Distance:
                    propsList.AddRange(discPropertiesDistance);
                    break;
            }            

            for (int i = 0; i < count; i++)
            {
                var rand = UnityEngine.Random.Range(0, propsList.Count);
                DiscProperty prop1 = propsList[rand];
                propsList.Remove(prop1);
                returnList.Add(prop1);
                if (prop1.type == DiscProperty.PropertyType.BigSkip || prop1.type == DiscProperty.PropertyType.Sticky)
                {
                    propsList.RemoveAll(s => s.type == DiscProperty.PropertyType.Sticky);
                    propsList.RemoveAll(s => s.type == DiscProperty.PropertyType.BigSkip);
                }
                if (prop1.type == DiscProperty.PropertyType.Heavy || prop1.type == DiscProperty.PropertyType.Light)
                {
                    propsList.RemoveAll(s => s.type == DiscProperty.PropertyType.Light);
                    propsList.RemoveAll(s => s.type == DiscProperty.PropertyType.Heavy);
                }
                if (prop1.type == DiscProperty.PropertyType.Fade || prop1.type == DiscProperty.PropertyType.Turn)
                {
                    propsList.RemoveAll(s => s.type == DiscProperty.PropertyType.Fade);
                    propsList.RemoveAll(s => s.type == DiscProperty.PropertyType.Turn);
                }
            }
            return returnList;            
        }

        public DiscProperty GetProperty(DiscProperty.PropertyType type)
        {
            for (int i = 0; i < discPropertiesAll.Length; i++)
            {
                if (discPropertiesAll[i].type == type)
                {
                    return discPropertiesAll[i];
                }
            }
          
            return discPropertiesAll[0];
        }

        internal DiscMould MenuDisc()
        {
            return menuDisc;
        }

        internal DiscColor RandomBasicColor()
        {
            var discColor = basicColors[UnityEngine.Random.Range(0, basicColors.Length)];
            return discColor;
        }

        public Disc GetRandomDiscReward(DiscReward reward)
        {
            var mould = GetRandomMould(reward.moulds);

            var propCount = UnityEngine.Random.Range(reward.minimumProperties, reward.maximumProperties + 1);
            var props = GetRandomProperties(propCount, mould.discType);
            List<DiscProperty.PropertyType> p = new List<DiscProperty.PropertyType>();

            foreach (DiscProperty prop in props)
            {
                p.Add(prop.type);
            }

            var discColor = RandomColor(reward);
                        

            var disc = new Disc(mould, p, discColor);

            return disc;
        }

        private DiscColor RandomColor(DiscReward reward)
        {
            List<DiscColor> optionsList = new List<DiscColor>();

            optionsList.AddRange(basicColors);
            if (reward.colorsBurst)
            {
                optionsList.AddRange(burstColors);
            }
            if (reward.colorsRecycled)
            {
                optionsList.AddRange(recycledColors);
            }
            if (reward.colorsRareStamps)
            {
                optionsList.AddRange(rareStamps);
            }

            var discColor = optionsList[UnityEngine.Random.Range(0, optionsList.Count)];
            return discColor;
        }

        internal List<Disc> GetLevelUpDiscs(int playerLevel)
        {
            var list = new List<Disc>();
            var reward = GetLevelUpDiscReward(playerLevel);

            for (int i =0; i<3; i++)
            {
                list.Add(GetRandomDiscReward(reward));
            }

            return list;
        }

        internal DiscReward GetLevelUpDiscReward(int playerLevel)
        {   
            if (playerLevel > 20)
            {
                return openRewards[3];
            }
            else if (playerLevel > 10)
            {
                return openRewards[2];
            }
            else if (playerLevel > 5)
            {
                return openRewards[1];
            }
            else
            {
                return openRewards[0];
            }
        }
    }
}
