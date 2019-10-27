//using UnityEngine;

//namespace Assets.Scripts
//{
//    public class DiscFlightCamera : MonoBehaviour
//    {        
//        private Camera playerCamera;
//        public Transform cameraPosition;
//        public static DiscFlightCamera isTracking;
//        private Transform disc;
//        public float fovModifier;
//        public float distance;
   
//        private void OnTriggerEnter(Collider other)
//        {
//            if (other.tag.Equals("Disc") && other.GetComponent<DiscBehavior>().isThrown)
//            {
//                playerCamera = Camera.main;
//                disc = other.transform;
//                playerCamera.transform.position = cameraPosition.position;            
//                isTracking = this;
//            }
//        }

//        private void Update()
//        {
//            if(isTracking == this)
//            {
//                distance = Vector3.Distance(cameraPosition.transform.position, disc.position);
//                fovModifier = distance > 60 ? 1 : distance / 60;
//                playerCamera.transform.LookAt(disc.transform);
//                playerCamera.fieldOfView = Mathf.Lerp(60, 10, fovModifier);
//            }
//        }
//    }
//}