using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class BasketBehavior : MonoBehaviour
    {        
        public Transform basketPosition;
        public Collider insideBasketCollider;
        public GameObject physicsChains;
        public GameObject staticChains;
        [SerializeField] private RectTransform basketIcon;
        [SerializeField] private TMP_Text iconText;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Disc") && other.GetComponent<DiscBehavior>().isThrown)
            {
                other.GetComponent<DiscBehavior>().PuttingCollider();
                staticChains.SetActive(false);
                physicsChains.SetActive(true);
            }
        }

        public void ShowBasketIcon(string distanceTobasket,  float distance, Transform playerPosition)
        {
            basketIcon.gameObject.SetActive(true);
            basketIcon.transform.LookAt(playerPosition);
            iconText.text = distanceTobasket;

            var scale = Mathf.Lerp(0.3f, 9f, distance / 300f);

            basketIcon.localScale = new Vector3(scale, scale, scale);
        }

        public void HideBasketIcon()
        {
            basketIcon.gameObject.SetActive(false);
        }
    }
}
