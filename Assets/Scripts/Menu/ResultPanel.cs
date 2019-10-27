using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.Menu
{
    public class ResultPanel : MonoBehaviour
    {        
        public TMP_Text divisionText;
        public TMP_Text playerScore;
        public TMP_Text playerPosition;
        public TMP_Text participantsText;
        public TMP_Text[] top3name;
        public TMP_Text[] top3score;
        public TMP_Text newRating;
        public TMP_Text ratingDiff;
        public GameObject ratingsPanel;

        [ContextMenu("test result")]
        internal void TestResult()
        {
            var result = new ProTourResult
            {
                score = 90,
                parDiff = -20,
                position = 230,
                division = ProTourWrapper.Division.Advanced,
                completeRound = true,
                participants = 1000,
                top3 = new ProTourTop3[3]
            };

            for(int i =0; i<3; i++)
            {
                result.top3[i] = new ProTourTop3();
                result.top3[i].playerName = "player" + (i + 1);
                result.top3[i].score = 99;
            }

            PopulateProTour(result, 1000, 10);
        }

        [ContextMenu("test open")]
        internal void TestOpen()
        {
            var result = new OpenResult
            {
                score = 90,
                parDiff = -20,
                position = 230,
                participants = 1000,
                division = ProTourWrapper.Division.Advanced,                
                top3 = new ProTourTop3[3]
            };

            for (int i = 0; i < 3; i++)
            {
                result.top3[i] = new ProTourTop3();
                result.top3[i].playerName = "player" + (i + 1);
                result.top3[i].score = 90;
            }

            PopulateOpen(result);
        }

        internal void PopulateProTour(ProTourResult result, int newRating, int diff)
        {            
            playerScore.text = result.score.ToString() + "<color=#e6e6ef> |</color> <color=#75aae2>" + result.parDiff.ToString();
            playerPosition.text = result.position.ToString();
            divisionText.text = result.division.ToString() + " Division";
            this.newRating.text = newRating.ToString();
            ratingDiff.text = diff.ToString();
            participantsText.text = result.participants.ToString();
            //ratingsPanel.SetActive(true);

            var totalStrokes = result.score - result.parDiff;

            int i = 0;
            foreach(ProTourTop3 s in result.top3)
            {
                var parDiff = s.score - totalStrokes;
                string parDiffString = parDiff > 0 ? "+" + parDiff : parDiff.ToString();
                top3name[i].text = ShortName(s.playerName);          
                top3score[i].text = s.score.ToString() + "<color=#e6e6ef> |</color> <color=#75aae2>" + parDiffString;
                i++;
            }
        }

        internal void PopulateOpen(OpenResult result)
        {
            playerScore.text = result.score.ToString() + "<color=#e6e6ef> |</color> <color=#75aae2>" + result.parDiff.ToString();            
            playerPosition.text = result.position.ToString();
            divisionText.text = result.division.ToString() + " Division";
            // ratingsPanel.SetActive(false);
            var totalStrokes = result.score - result.parDiff;
            participantsText.text = result.participants.ToString();

            int i = 0;
            foreach (ProTourTop3 s in result.top3)
            {
                var parDiff = s.score - totalStrokes;
                string parDiffString = parDiff > 0 ? "+" + parDiff : parDiff.ToString();
                top3name[i].text = ShortName(s.playerName);
                top3score[i].text = s.score.ToString() + "<color=#e6e6ef> |</color> <color=#75aae2>" + parDiffString;
                i++;
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