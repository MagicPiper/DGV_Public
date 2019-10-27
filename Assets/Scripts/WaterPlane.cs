using UnityEngine;
namespace Assets.Scripts
{
    public class WaterPlane : MonoBehaviour
    {
        [SerializeField]
        private WaterSplash splashPrefab;

        private WaterSplash splash;

        private void Start()
        {
            splash = Instantiate(splashPrefab);
            splash.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (!other.GetComponent<DiscBehavior>().hitGround)
            if (other.GetComponent<Rigidbody>().velocity.magnitude > 3f) 
            {
                var pos = new Vector3(other.transform.position.x, other.transform.position.y + 0.15f, other.transform.position.z);
                splash.Splash(pos);
            }

            other.GetComponent<DiscBehavior>().HitWater();
        }
    }
}
