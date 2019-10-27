using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScoreBar : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] panels;
        [SerializeField]
        private TMP_Text[] labels;
        [SerializeField]
        private TMP_Text[] values;

        private List<string> activePanels = new List<string>();
        
        public void AddPanel(string panelID, string lable, string startValue)
        {
            AddPanel(panelID, lable, startValue, false);
        }

        public void AddPanel(string panelID, string lable, string startValue, bool longLable)
        {
            if (activePanels.Count <= panels.Length)
            {
                activePanels.Add(panelID);
                UpdatePanel(panelID, lable, startValue);
                if (longLable)
                {
                    labels[activePanels.IndexOf(panelID)].fontSize = 20;
                }
                panels[activePanels.IndexOf(panelID)].SetActive(true);
                StartCoroutine(UpdateCanvas());
            }
            else
            {
                Debug.Log("Not enough score bar panels bro");                
            }
        }

        private IEnumerator UpdateCanvas()
        {
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            yield return null;
        }

        public void UpdatePanel(string panelID, string lable, string value)
        {
            labels[activePanels.IndexOf(panelID)].text = lable;
            values[activePanels.IndexOf(panelID)].text = value;
        }

        public void UpdatePanel(string panelID, string value)
        {            
            values[activePanels.IndexOf(panelID)].text = value;
        }       
    }
}
