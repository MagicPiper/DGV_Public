using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class UITheme : MonoBehaviour
{
    [SerializeField] private Color buttonColor;
    [SerializeField] private Color panelColor;
    [SerializeField] private Color titleTextColor;

    [SerializeField] private Image[] buttons;
    [SerializeField] private Image[] panels;
    [SerializeField] private TMP_Text[] titleTexts;

    //private void Update()
    //{
    //    SetTheme();
    //}


    [ContextMenu( "Set Theme")]
    public void SetTheme()
    {
        SetImages(buttons, buttonColor);
        SetImages(panels, panelColor);
        SetTexts(titleTexts, titleTextColor);
    }

    private void SetImages(Image[] images, Color color)
    {
        foreach (Image image in images)
        {
            image.color = color;
        }
    }

    private void SetTexts(TMP_Text[] texts, Color color)
    {
        foreach (TMP_Text text in texts)
        {
            text.color = color;
        }
    }
}
