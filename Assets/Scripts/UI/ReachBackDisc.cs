using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Assets.Scripts
{
    public class ReachBackDisc : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private float distance;
        private bool dragging;
        private float power;
        private float angle;
        private Vector3 currentPos;
        private Vector3 anchoredStart;
        [SerializeField] private Image discImage;
        private float maxDistance;
        private float adjustedPower;
        private float accuracyMod = 10f;
        [SerializeField] public LineRenderer lr;
        [SerializeField] private ReachBack reachBackPanel;
        [SerializeField] private Outline outline;
        [SerializeField] private Image[] arrows;
        [SerializeField] private TMP_Text cancelText;
        private bool cancelDistance;

        void OnEnable()
        {
            anchoredStart = GetComponent<RectTransform>().position;
            outline.enabled = true;
            cancelText.gameObject.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                arrows[i].gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (dragging)
            {
                var allowedPos = currentPos - anchoredStart;
                maxDistance = Screen.height * 0.6f;
                allowedPos = Vector3.ClampMagnitude(allowedPos, maxDistance);
                transform.position = anchoredStart + allowedPos;

                distance = (anchoredStart - transform.position).magnitude / Screen.height;
                var distanceMod = Mathf.Pow(distance * 3, 3);
                var x = UnityEngine.Random.Range(-distanceMod, distanceMod);
                var y = UnityEngine.Random.Range(-distanceMod, distanceMod);
                var direction = new Vector3(x, y, 0f);

                transform.position = transform.position + direction;

                DrawTriangleInGUISpace(anchoredStart, transform.position, distance * 2);

                Angle();
                Power();
                cancelDistance = distance < 0.1f ? true : false;
                cancelText.gameObject.SetActive(cancelDistance);
                reachBackPanel.AccuracyChange();
            }
            else
            {
                var t = Mathf.PingPong(Time.time, 1f);
                var tempC = outline.effectColor;
                outline.effectColor = new Color(tempC.r, tempC.g, tempC.b, t);

                for (int i = 0; i < 3; i++)
                {
                    var a = arrows[i].color;
                    arrows[i].color = new Color(a.r, a.g, a.b, t * 0.5f);
                }
            }
        }

        void DrawTriangleInGUISpace(Vector2 p0, Vector2 p1, float distance)
        {
            Vector3 worldPoint0 = Camera.main.ScreenToWorldPoint(new Vector3(p0.x, p0.y, 5f));
            Vector3 worldPoint1 = Camera.main.ScreenToWorldPoint(new Vector3(p1.x, p1.y, 5f));

            lr.SetPosition(0, worldPoint0);
            lr.SetPosition(1, worldPoint1);
            lr.widthMultiplier = distance;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            reachBackPanel.startRot = reachBackPanel.throwUI.playerScript.transform.rotation;
            reachBackPanel.dampingRot = reachBackPanel.throwUI.playerScript.transform.rotation;
            reachBackPanel.targetRot = reachBackPanel.throwUI.playerScript.transform.rotation;
            reachBackPanel.changeRot = 2f;
            reachBackPanel.changeAccuracy = true;
            dragging = true;
            outline.enabled = false;
            for (int i = 0; i < 3; i++)
            {
                arrows[i].gameObject.SetActive(false);
            }
            lr.enabled = true;
            reachBackPanel.BeginReachBack();
            accuracyMod = 15f * reachBackPanel.throwUI.playerScript.curentDiscScript.accuracyMod;
          
        }

        public void OnDrag(PointerEventData eventData)
        {
            currentPos = new Vector3(eventData.position.x, eventData.position.y);
            //  discImage.color = Color.Lerp(Color.yellow, Color.red, power-0.5f);
        }

        private void Power()
        {
            power = distance / 0.6f;
        }

        private void Angle()
        {
            angle = ((Mathf.Atan2(anchoredStart.y - transform.position.y, anchoredStart.x - transform.position.x) * Mathf.Rad2Deg) - 90) / 2;
            angle = angle > 30f ? angle = 30f : angle;
            angle = angle < -30f ? angle = -30f : angle;

            var angleMod = Mathf.Lerp(0f, accuracyMod, distance);
            angleMod = UnityEngine.Random.Range(-angleMod, angleMod);
            angle = angle + angleMod;

            reachBackPanel.HyzerAngleChange(angle * -1);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            lr.enabled = false;
            dragging = false;
            this.transform.position = anchoredStart;
            reachBackPanel.changeAccuracy = false;

            if (cancelDistance)
            {
                outline.enabled = true;
                for (int i = 0; i < 3; i++)
                {
                    arrows[i].gameObject.SetActive(true);
                }
                lr.enabled = false;
                cancelText.gameObject.SetActive(false);
                reachBackPanel.CancelReachback();
            }
            else
            {
                reachBackPanel.throwUI.playerScript.action.Throw(power);
                reachBackPanel.changeRot = 2f;
                Debug.Log("power: " + power);
            }
        }

        internal void SetColor(Color discColor)
        {
            discImage.color = discColor;
        }
    }
}