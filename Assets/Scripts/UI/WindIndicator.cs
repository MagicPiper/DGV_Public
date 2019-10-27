using UnityEngine;
using TMPro;
using System;

namespace Assets.Scripts
{
    public class WindIndicator : MonoBehaviour
    {
        public RectTransform arrow;
        public RectTransform background;

        public TMP_Text windSpeedText;
        private Vector3 arrowDirection;
        private Vector3 arrowLocalDirection;

        private float windSpeed;
        private bool populated = false;

        private Vector3 randomizedDirection;

        void Update()
        {
            if (populated)
            {
                var random = UnityEngine.Random.Range(0f, 30f);
                if (random < windSpeed)
                {
                    var mag = 5 + (windSpeed * 20);
                    randomizedDirection = new Vector3(arrowLocalDirection.x, arrowLocalDirection.y, arrowLocalDirection.z + UnityEngine.Random.Range(-mag, +mag));
                }

                arrow.localEulerAngles = AngleLerp(arrow.localEulerAngles, randomizedDirection, Time.deltaTime);
            }
        }

        public void Populate(float windspeed, float windDirection, Quaternion playerRotation)
        {
            this.windSpeed = windspeed;
            windspeed = windspeed * 10;
            windSpeedText.text = windspeed.ToString();
            arrowDirection = new Vector3(0, 0, 360 - windDirection);

            background.transform.eulerAngles = new Vector3(background.eulerAngles.x, background.eulerAngles.y, playerRotation.eulerAngles.y);

            arrow.eulerAngles = arrowDirection;

            arrowLocalDirection = arrow.localEulerAngles;
            randomizedDirection = arrowLocalDirection;
            populated = true;
        }

        //Used when the player pans.
        internal void Rotate(float rotationY)
        {
            background.transform.eulerAngles = new Vector3(background.eulerAngles.x, background.eulerAngles.y, background.eulerAngles.z + rotationY);
        }

        //Used when the player changes position(move to lie)        
        public void UpdateRotation(Quaternion playerRotation)
        {
            background.transform.eulerAngles = new Vector3(background.eulerAngles.x, background.eulerAngles.y, playerRotation.eulerAngles.y);
        }

        //Properly Lerp between two angles
        Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
        {
            float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
            float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
            float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
            Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
            return Lerped;
        }
    }
}