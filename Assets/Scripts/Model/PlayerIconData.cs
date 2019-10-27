using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "PlayerIcons", menuName = "DiscGolf/PlayerIconData", order = 5)]
    public class PlayerIconData : ScriptableObject
    {
        [SerializeField] private List<Sprite> icons;

        public Sprite GetIcon(int icon)
        {
            if(icon >= icons.Count)
            {
                return icons[0];

            }
            else
            {
                return icons[icon];
            }
        }

        public List<Sprite> AllIcons()
        {
            return icons;
        }
    }
}
