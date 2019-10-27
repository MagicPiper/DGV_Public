using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class UIRound : MonoBehaviour
    {
        [SerializeField] private TMP_Text rountTypeText;
        [SerializeField] private TMP_Text ratingText;
        [SerializeField] private TMP_Text numText;
        [SerializeField] private Color backgroundColor1;
        [SerializeField] private Color backgroundColor2;
        [SerializeField] private Color abandonedColor;
        [SerializeField] private Image background;

        public void Populate(string roundType, float rating, bool abandoned, int i)
        {
            rountTypeText.text = roundType;
            ratingText.text = rating.ToString("####");
            if (abandoned)
            {
                rountTypeText.color = abandonedColor;
            }
            numText.text = i.ToString() + ".";

            if (i % 2 == 0)
            {
                background.color = backgroundColor1;
            }
            else
            {
                background.color = backgroundColor2;
            }
        }
    }
}
