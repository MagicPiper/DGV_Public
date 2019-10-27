using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class DiscLauncher : MonoBehaviour
    {
        [SerializeField] private DiscBehavior discPrefab;
        [SerializeField] private Disc[] discTemplates;

        [SerializeField] private Transform[] targets;
        [SerializeField] private Player player;
        [SerializeField] private float power;
        [SerializeField] private Terrain terrain;


        //private void Start()
        //{
        //    player.action = new DiscTestBehavior();            
        //}

        [ContextMenu("test")]
        public void StartLaunching()
        {
            StartCoroutine(LaunchDiscs());
        }

        private IEnumerator LaunchDiscs()
        {
            var counter = 0;
            
            for (int i = 0; i < 5; i++)
            {
                if (counter >= targets.Length)
                {
                    counter = 0;
                }

                var disc = Instantiate(discPrefab);
                disc.transform.position = this.transform.position;
                disc.transform.LookAt(targets[counter]);

                var rand = 0.1f;
                disc.transform.rotation = disc.transform.rotation * Quaternion.Euler(Random.Range(rand*-1, rand), Random.Range(rand*-1, rand), Random.Range(rand*-1, rand));

                disc.PopulateMenuDisc(discTemplates[Random.Range(0, discTemplates.Length)], true);
                disc.player = player;
                yield return new WaitForSeconds(1);


                if (counter==2)
                {                   
                    disc.Throw(0.6f, 0f);
                }
                else
                {
                    disc.Throw(power, 0f);

                }
                counter++;                

                yield return new WaitForSeconds(6f);
            }
        }
    }
}