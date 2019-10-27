using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class HoleSceneManager : DGSceneManager
    {     
        //[SerializeField] private float step = 1;
       
        [SerializeField] private bool droneCamFlipRotDirection;
        [SerializeField] private float droneCamSpeedMod;        

        public bool DroneCamRotDirection
        {
            get
            {
                return droneCamFlipRotDirection;
            }            
        }

        public float DroneCamSpeedMod
        {
            get
            {
                return droneCamSpeedMod;
            }

            set
            {
                droneCamSpeedMod = value;
            }
        }

        protected override void InitScene()
        {
            if (hasWater)
            {
                playerScript.waterDepthScript.Set();
            }
            Wind();
            Weather();
            playerScript.BirbController.StartAmbienceSounds(currentWind, birbs);
            playerScript.InitPlayer(currentWind, windDirection);
        }
    }
}
