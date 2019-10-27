using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class DiscTesterSceneManager : DGSceneManager
    {
        [SerializeField] private DiscBehavior discPrefab;
        [SerializeField] private Disc discTemplate;        
        [SerializeField] private float power;
        [SerializeField] private GameObject discTrack;
        [SerializeField] private TMP_Text discText;
        [SerializeField] public TMP_Text distanceText;    
        [SerializeField] public float angle;
        [SerializeField] public float upDownAngle;

        [ContextMenu("Throw")]
        public void Throw(float angle=0)
        {
            playerScript.action.ChangeDisc(discTemplate);
            discText.text = discTemplate.mouldName.ToString();

            var tracker = Instantiate(discTrack, playerScript.curentDisc.transform);

            var rot = playerScript.curentDisc.transform.localRotation;
            playerScript.curentDisc.transform.localRotation = Quaternion.Euler(rot.eulerAngles.x, 0, angle);
            playerScript.curentDisc.transform.Rotate(Vector3.left,upDownAngle);

            playerScript.action.Throw(power);
        }

        [ContextMenu("hyzer")]
        public void Hyzer()
        {
            Throw(angle);
        }
        [ContextMenu("anhyzer")]
        public void Anhyzer()
        {
            Throw(angle*-1);
        }

        protected override void SetPlayerBehavior(GameObject player)
        {    
            playerScript.action = player.AddComponent<DiscTestBehavior>();
        }  

        protected override void InitScene()
        {
            //playerScript.action.player = playerScript;
        }
    }
}
