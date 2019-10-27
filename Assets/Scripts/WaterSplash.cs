using UnityEngine;
namespace Assets.Scripts
{
    public class WaterSplash : MonoBehaviour
    {
        [SerializeField] private ParticleSystem splash;
        [SerializeField] private AudioSource splashSound;


        private void DestroySplash()
        {
            gameObject.SetActive(false);
        }

        internal void Splash(Vector3 pos)
        {
            transform.position = pos;
            gameObject.SetActive(true);
            splash.Play();
            splashSound.pitch = UnityEngine.Random.Range(0.5f, 0.8f);
            splashSound.Play();
            Invoke("DestroySplash", 2f);
        }
    }
}
