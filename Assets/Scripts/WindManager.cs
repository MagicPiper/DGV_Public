using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class WindManager : MonoBehaviour
    {
        public GameObject windZonePrefab;
        public WindZone windZone;
        public bool windParticlesOn;
        public ParticleSystem windParticles;
        public Transform windTarget;
        public float baseWindSpeed = 0f;
        public float currentWindSpeed;
        public Vector3 windDirection;
        public Player player;

        internal void SetWind(float currentWind, float windDirection)
        {
            baseWindSpeed = currentWind;
            currentWindSpeed = baseWindSpeed;

            windZone.transform.Rotate(0.0f, windDirection, 0.0f);

            this.windDirection = windZone.transform.forward;

            windZone.windMain = currentWind;
            windZone.windTurbulence = currentWind;
            windZone.transform.SetParent(player.sceneManager.transform);  //windzone stays put. Doesn't rotate with player.

            player.throwUI.windIndicator.Populate(currentWind, windDirection, player.transform.rotation);

            if (currentWind > 0.24)
            {
                windParticlesOn = true;
                windParticles.gameObject.SetActive(true);
                var burst = windParticles.emission.GetBurst(0);
                burst.repeatInterval = 2f - currentWind;
                var min = (currentWind * 4f);
                var max = (currentWind * 4f) + 2;

                burst.minCount = (short)min;
                burst.maxCount = (short)max;
                
                windParticles.emission.SetBurst(0, burst);
                var main = windParticles.main;
                main.startSpeed = (currentWind * 10) * (currentWind * 10);

                UpdateWindPosition();
            }
        }

        internal void UpdateWindPosition()
        {
            if (windParticlesOn)
            {
                windParticles.transform.position = windTarget.position;
                windParticles.transform.localPosition += Vector3.back * 30;
            }
        }
        //private void Update()
        //{
        //    if (baseWindSpeed > 0.2f)
        //    {
        //        currentWindSpeed = Mathf.Lerp(baseWindSpeed, pulse, 0.1f);

        //        pulseCounter += Random.Range(0f, baseWindSpeed);
        //        if (pulseCounter > 10)
        //        {
        //            pulse = Mathf.PingPong(pulseCounter += Random.Range(0, 0.1f), baseWindSpeed * 5);
        //        }
        //        currentWindSpeed = baseWindSpeed + pulse;
        //    }
        //}
    }
}
