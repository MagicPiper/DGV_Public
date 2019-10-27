using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ScoreLine : MonoBehaviour
    {
        [SerializeField] private TMP_Text position;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text parText;
        [SerializeField] private Image playerIcon;
        [SerializeField] private PlayerIconData playerIconData;


        public void Populate(int pos, string name, int score, bool isMe, int rating, string parDiff, int playerIcon)
        {
            var nameString = name.Split(' ')[0];

            nameString = nameString.IndexOf('@') == -1 ? nameString : nameString.Split('@')[0];
             
            if (nameString.Length < 1)
            {
                nameString = "anonymous";
            }
            string ratingText="";
            if (rating > 0)
            {
                ratingText = "<color=yellow> (" + rating.ToString() + ")";
            }
            
            if(pos == 9999)
            {
                position.text = "25+";
            }
            else
            {
                position.text = pos.ToString() + ".";
            }

            playerName.text = nameString + ratingText;
            scoreText.text = score.ToString();
            parText.text = parDiff;
            this.playerIcon.sprite = playerIconData.GetIcon(playerIcon);
            if (isMe)
            {
                playerName.color = Color.yellow;
            }
        }
    }
}