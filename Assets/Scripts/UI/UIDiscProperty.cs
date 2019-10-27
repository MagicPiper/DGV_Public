using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIDiscProperty : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text description;
        [SerializeField]
        private Image pane;

        internal void Populate(DiscProperty prop)
        {
            nameText.text = prop.propertyName;
            nameText.color = Color.Lerp(prop.uiColor, Color.white, 0.4f);
            pane.color = Color.Lerp(prop.uiColor, Color.black, 0.4f);
            description.text = prop.description;
            gameObject.SetActive(true);
        }
    }
}