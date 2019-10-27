using UnityEngine;
namespace Assets.Scripts
{
    public class MenuWaterPlane : MonoBehaviour
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
            if (!other.GetComponent<DiscBehavior>().hitGround)
            {
                var pos = new Vector3(other.transform.position.x, other.transform.position.y + 0.02f, other.transform.position.z);
                splash.Splash(pos);
            }

            other.GetComponent<DiscBehavior>().HitWater();
        }
    }
}
