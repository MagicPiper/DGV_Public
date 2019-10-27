using UnityEngine;
using TMPro;

namespace Assets.Scripts
{
    public class PopUpText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;
        public int direction;
        private Vector3 startPosition;
        private RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            startPosition = rect.anchoredPosition;
        }

        internal void ShowText(string text, float duration)
        {
            this.text.color = Color.white;
            Show(text, duration);
        }

        private void Show(string text, float duration)
        {
            this.text.text = text;
            gameObject.SetActive(true);
            rect.anchoredPosition = new Vector2(direction * Screen.width, startPosition.y);
            LeanTween.move(rect, startPosition, 0.5f).setEase(LeanTweenType.easeOutCubic);
            Invoke("Disable", duration);
        }

        void TextGlow(float val, TMP_Text text)
        {
            text.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, val);
        }

        internal void ShowText(string text, float duration, Color color)
        {
            this.text.color = color;            
            Show(text, duration);
        }

        internal void Disable()
        {               
            var newpos = new Vector2(direction*Screen.width*-1, startPosition.y);
            LeanTween.move(rect, newpos, 0.5f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
            {
                gameObject.SetActive(false);
                rect.anchoredPosition = startPosition;
            });
          
            
        }
    }
}