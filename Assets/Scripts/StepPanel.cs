using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    [AddComponentMenu("Camera-Control/Touch Look")]
    public class StepPanel : MonoBehaviour, IDragHandler
    {
        public float sensitivityX = 50.0f;        
        [SerializeField]
        private ThrowUI throwUI;
        [SerializeField]
        private Transform centerPoint;

        public float posX;
       
        public void ResetPos()
        {
            centerPoint.localPosition = Vector3.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.delta;
           
            float posX = delta.x * sensitivityX * Time.deltaTime;

            centerPoint.position = centerPoint.position + new Vector3(posX, 0);

            if(centerPoint.localPosition.x> 100f)
            {
                centerPoint.localPosition = new Vector3(100f, centerPoint.localPosition.y);
            }

            if (centerPoint.localPosition.x < -100f)
            {
                centerPoint.localPosition = new Vector3(-100f, centerPoint.localPosition.y);
            }

            throwUI.playerScript.action.SideStep(centerPoint.localPosition.x);
        }
    }
}
