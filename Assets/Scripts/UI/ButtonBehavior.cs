using UnityEngine;
using TMPro;

public class ButtonBehavior : MonoBehaviour {

    private Vector2 startPos;
    private Vector2 startSize;

    private RectTransform rect;
    private bool isActive;

    public TMP_Text buttonText;
    public enum Direction { left, right, up, down };
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        startSize = rect.localScale;
    }

    internal void SlideOut(Direction dir, float duration, float delay)
    {
        isActive = false;
        Vector2 direction = new Vector2();

        switch (dir)
        {
            case Direction.up:
                direction = new Vector2(startPos.x, Screen.height + 1000);
                break;
            case Direction.down:
                direction = new Vector2(startPos.x, -1000);
                break;
            case Direction.left:
                direction = new Vector2(-1000, startPos.y);
                break;
            case Direction.right:
                direction = new Vector2(Screen.width + 1000, startPos.y);
                break;
        }

        LeanTween.move(rect, direction, duration).setEase(LeanTweenType.easeInCubic).setDelay(delay).setOnComplete(() =>
        {
            gameObject.SetActive(isActive);            
        });
    }

    internal void SlideIn(Direction dir, float duration, float delay)
    {
        isActive = true;
        Vector2 direction = new Vector2();

        switch (dir)
        {
            case Direction.up:
                direction = new Vector2(startPos.x, Screen.height+1000);
                break;
            case Direction.down:
                direction = new Vector2(startPos.x, -1000);
                break;
            case Direction.left:
                direction = new Vector2(-1000, startPos.y);
                break;
            case Direction.right:
                direction = new Vector2(Screen.width+1000, startPos.y);
                break;
        }

        gameObject.SetActive(isActive);
        rect.anchoredPosition = direction;
        LeanTween.move(rect, startPos, duration).setEase(LeanTweenType.easeOutExpo).setDelay(delay);
    }

    public void EnablePopIn()
    {
        gameObject.SetActive(true);
        rect.localScale = new Vector2(0f, 0f);
        LeanTween.scale(rect, startSize, 1f).setEaseOutBounce();
    }

    public void DisablePopOut()
    {
        if (gameObject.activeSelf)
        {
            LeanTween.scale(rect, new Vector2(0, 0), 0.3f).setEaseOutExpo().setOnComplete(() =>
             {
                 gameObject.SetActive(false);

             });
        }
    }

    public void HighLightButton(string newText, Vector2 newSize)
    {
        buttonText.text = newText;
        buttonText.alpha = 255;

        LeanTween.alpha(rect, 1, 0.5f);
        LeanTween.size(rect, newSize, 0.5f).setEaseInOutBack();
    }

}
