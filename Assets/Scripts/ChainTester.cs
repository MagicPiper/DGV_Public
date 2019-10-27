using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChainTester : MonoBehaviour
    {
        [SerializeField] private DiscBehavior discPrefab;
        [SerializeField] private Disc[] discTemplates;

        [SerializeField] private Transform basket;
        [SerializeField] private Player player;
        [SerializeField] private float power;
        [SerializeField] private float distance;
        [SerializeField] private float delay;


        //[SerializeField] private Link[] links;

        //[SerializeField] private float mass;
        //[SerializeField] private float drag;

        //[SerializeField] private float linearLimitBounciness;

        //[SerializeField] private float linearLimit;
        //[SerializeField] private float linearLimitContactDistance;
        //[SerializeField] private float damper;
        //[SerializeField] private float spring;
        //[SerializeField] private JointProjectionMode projectionMode;
        //[SerializeField] private float projectionDistance;

        //void Awake()
        //{
        //    QualitySettings.vSyncCount = 0;  // VSync must be disabled
        //    Application.targetFrameRate = 30;
        //}

        //[ContextMenu("get links")]
        //public void GetLinks()
        //{
        //    links = FindObjectsOfType<Link>();
        //    var rb = links[0].GetComponent<Rigidbody>();
        //    mass = rb.mass;
        //    drag = rb.drag;

        //    var joint = links[0].GetComponent<ConfigurableJoint>();
        //    linearLimitBounciness = joint.linearLimit.bounciness;
        //    linearLimit = joint.linearLimit.limit;
        //    linearLimitContactDistance = joint.linearLimit.contactDistance;

        //    damper = joint.linearLimitSpring.damper;
        //    spring = spring = joint.linearLimitSpring.spring;

        //    projectionMode = joint.projectionMode;
        //    projectionDistance = joint.projectionDistance;
        //}

        //[ContextMenu("links")]
        //public void UpdateLinks()
        //{
        //    foreach (Link link in links)
        //    {
        //        var rb = link.GetComponent<Rigidbody>();
        //        //  rb.mass = mass;
        //        //rb.mass = 0.02f;
        //        // rb.drag = drag;

        //        var joint = link.GetComponent<ConfigurableJoint>();

        //        var l = new SoftJointLimit();
        //        l.bounciness = linearLimitBounciness;
        //        l.limit = linearLimit;
        //        l.contactDistance = linearLimitContactDistance;
        //        joint.linearLimit = l;

        //        var s = new SoftJointLimitSpring();
        //        s.damper = damper;
        //        s.spring = spring;
        //        joint.linearLimitSpring = s;

        //        joint.projectionMode = projectionMode;
        //        joint.projectionDistance = projectionDistance;

        //    }
        //}

        [ContextMenu("test")]
        public void Start()
        {

            StartCoroutine(LaunchDiscs());
            distance = Vector3.Distance(basket.position, this.transform.position);
        }

        private IEnumerator LaunchDiscs()
        {
            yield return new WaitForSeconds(delay);
            for (int i = 0; i < 50; i++)
            {
                var disc = Instantiate(discPrefab);
                disc.transform.position = this.transform.position;
                disc.transform.LookAt(basket);

                var rand = 0.6f;
                disc.transform.rotation = disc.transform.rotation * Quaternion.Euler(Random.Range(rand * -1, rand), Random.Range(rand * -1, rand), Random.Range(rand * -1, rand));

                disc.PopulateMenuDisc(discTemplates[Random.Range(0, discTemplates.Length)], true);
                disc.player = player;
                yield return new WaitForSeconds(1);

                //disc.Throw(power*Random.Range(0.9f, 1.1f));
                disc.Throw(power, 0f);

                yield return new WaitForSeconds(7);
                Destroy(disc.gameObject);
                yield return new WaitForSeconds(3f);

            }
        }
    }
}