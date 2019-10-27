using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Menu;

namespace Assets.Scripts
{
    public class UIDisc : MonoBehaviour
    {
        public Disc disc;
        public DiscMould mould;
        private DiscColor colors;
        [SerializeField] private TMP_Text discNameText;
        [SerializeField] private DiscData discData;
        //public TMP_Text discTypeText;

        [SerializeField] private Outline outline;

        [SerializeField] private Image discImage;
        [SerializeField] private Image stamp;
        [SerializeField] private Image pattern;
        [SerializeField] private Image background;
        [SerializeField] private List<TMP_Text> propertyTexts;
        [SerializeField] private List<GameObject> properties;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text stabilityText;
        [SerializeField] private GameObject fullStatsPanelPrefab;
        [SerializeField] private Sprite[] discImages;
        [SerializeField] private TMP_Text discTypeText;
        [SerializeField] private Color[] typeTextColor;
        [SerializeField] private Color[] typeBackgroundColor;

        public DiscSelect DiscSelect;
        public DiscCollection discCollection;
        public RewardPanel rewardPanel;

        public enum Context
        {
            InGame, Collection, Reward, SelectOne
        }

        private Context context;

        public void Clicked()
        {
            if (context == Context.InGame)
            {
                DiscSelect.SelectionChange(disc, this);
            }
            else if (context == Context.Collection)
            {
                discCollection.SelectedDisc(disc);
            }
            else if (context == Context.SelectOne)
            {
                rewardPanel.Selected(disc);
            }
        }

        private void ShowFullStatsPanel()
        {
            var stats = Instantiate(fullStatsPanelPrefab);
            stats.transform.SetParent(this.transform.root, false);
            stats.GetComponent<DiscStatsPane>().Populate(disc);
        }

        internal void Populate(Disc disc, Context context)
        {
            this.context = context;
            this.disc = disc;
            this.mould = discData.GetMould(disc.mouldName);
            this.colors = discData.GetColor(disc);

            discNameText.text = mould.mouldName.ToString();
            //    weightText.text = disc.weight.ToString() + "g";


            stamp.sprite = disc.stampVariant == 0 ? mould.mouldStamp : mould.mouldStampAlternate1;
            stamp.color = colors.stampColor;

            discImage.sprite = discImages[(int)mould.discType];
            discImage.color = colors.baseColor;

            if (colors is DiscColorPattern)
            {
                var patternColor = colors as DiscColorPattern;
                pattern.sprite = patternColor.pattern;
                pattern.color = patternColor.patternColor;
                pattern.gameObject.SetActive(true);
            }
            else
            {
                pattern.gameObject.SetActive(false);
            }


            stabilityText.text = mould.stability.ToString();
            speedText.text = mould.speed.ToString();
            discTypeText.text = mould.discType.ToString();
            discTypeText.color = typeTextColor[(int)mould.discType];
            background.color = typeBackgroundColor[(int)mould.discType];

            int i = 0;
            foreach (DiscProperty.PropertyType prop in disc.discProperties)
            {
                var obj = properties[i];
                var text = propertyTexts[i];
                obj.SetActive(true);
                var p = discData.GetProperty(prop);
                text.text = p.name;
                obj.GetComponent<Image>().color = p.uiColor;

                i++;
            }
        }

        internal void Deselect()
        {
            outline.enabled = false;
        }
    }
}
