using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    [AddComponentMenu("Camera-Control/Touch Look")]
    public class TouchLook : MonoBehaviour, IDragHandler
    {
        public float sensitivityX = 1.0f;
        public float sensitivityY = 1.0f;

        public bool invertX = true;
        public bool invertY = true;
        [SerializeField]
        private ThrowUI throwUI;
        [SerializeField]
        private Transform centerPoint;
        public StepPanel stepPanel;

        public float rotX;
        public float rotZ;

        public void ResetPos()
        {
            centerPoint.localPosition = Vector3.zero;
            stepPanel.ResetPos();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.delta;
            float rotationZ = delta.x * sensitivityX * Time.deltaTime;
            rotationZ = invertX ? rotationZ : rotationZ * -1;
            float rotationX = delta.y * sensitivityY * Time.deltaTime;
            rotationX = invertY ? rotationX : rotationX * -1;

            throwUI.playerScript.transform.localEulerAngles += new Vector3(rotationX, rotationZ, 0);
            if (throwUI.windIndicator != null)
            {
                throwUI.windIndicator.Rotate(rotationZ);
                throwUI.playerScript.windManager.UpdateWindPosition();
            }


            rotX = rotationX;
            rotZ = rotationZ * -1;

            centerPoint.position = centerPoint.position + new Vector3(rotZ, rotX);
        }
    }
}
