using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ProScoreLine : MonoBehaviour
    {
        public TMP_Text[] roundScoreTexts;
        public TMP_Text total;
        public TMP_Text playerName;        
        public Color parColor;
        public Image background;
        public Color backgroundAltColor;
        [SerializeField] private Image playerIcon;
        [SerializeField] private PlayerIconData playerIconData;

        public void Populate(ProTourScoreSortable score, bool isPlayer, int counter)
        {
            int t = 0;
            int tp = 0;
            int i = 0;
            foreach (TMP_Text roundText in roundScoreTexts)
            {
                if (score.scores != null && score.scores[i] > 0 && score.scores[i] < 999)
                {
                    roundText.text = score.scores[i].ToString() + "<color=#e6e6ef> |</color> <color=#" + ColorUtility.ToHtmlStringRGB(parColor) + ">" + score.pars[i].ToString();
                    t += score.scores[i];
                    tp += score.pars[i];
                }
                else
                {
                    roundText.text = "-";
                    if(score.scores[i] == 999)
                    {
                        roundText.color = Color.red;
                    }
                }
                i++;
            }
            playerName.text = ShortName(score.playerName);
            this.playerIcon.sprite = playerIconData.GetIcon(score.icon);
            total.text = t.ToString() + "<color=#e6e6ef> |</color> <color=#" + ColorUtility.ToHtmlStringRGB(parColor) + ">" + tp.ToString();
            total.fontStyle = FontStyles.Bold;
            if (isPlayer)
            {
                playerName.color = Color.yellow;
            }
            if(counter % 2 == 0)
            {
                background.color = backgroundAltColor;
            }
        }

        public string ShortName(string fullName)
        {
            var nameString = fullName.Split(' ')[0];

            nameString = nameString.IndexOf('@') == -1 ? nameString : nameString.Split('@')[0];

            if (nameString.Length < 1)
            {
                nameString = "anonymous";
            }
            return nameString;
        }
    }
}
