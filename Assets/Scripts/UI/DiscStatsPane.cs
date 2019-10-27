using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class DiscStatsPane : MonoBehaviour
    {
        [SerializeField] private TMP_Text discName;
        [SerializeField] private TMP_Text discDescription;
        [SerializeField] private TMP_Text discType;
        [SerializeField] private TMP_Text discSpeed;
        [SerializeField] private UIDiscProperty[] attributes;        
        [SerializeField] private Transform attributeHolder;
        [SerializeField] private DiscData discData;

        internal void Populate(Disc disc)
        {
            gameObject.SetActive(true);
            var mould = discData.GetMould(disc.mouldName);
            discName.text = mould.mouldName.ToString();
            discDescription.text = mould.mouldDescription.ToString();
            discSpeed.text = mould.speed.ToString();
            discType.text = mould.discType.ToString();            
            int propCount = 0;
            foreach (DiscProperty.PropertyType prop in disc.discProperties)
            {
                var p = discData.GetProperty(prop);
                var propPanel = attributes[propCount];
                propPanel.Populate(p);
                propCount++;
            }
        }

        internal void Clear()
        {
            gameObject.SetActive(false);
            foreach (UIDiscProperty p in attributes)
            {
                p.gameObject.SetActive(false);
            }
            discName.text = "";
            discDescription.text = "";


        }
    }
}
