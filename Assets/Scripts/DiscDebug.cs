using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DiscDebug : MonoBehaviour
    {

        public DiscBehavior disc;

        public Text lift;    
        
        public Text drag;
            
        public Text turn;
               
        public Text fade;
       
        public Text wind;
                
        internal void Drag(Vector3 drag)
        {
            var dragM = Math.Round(drag.magnitude, 2);
            this.drag.text = dragM.ToString() + " " + drag.ToString();
        }

        internal void Lift(Vector3 lift, float windAdjustedSpeed, Vector3 noWind)
        {
            var liftM = Math.Round(lift.magnitude, 2);
            var windA = Math.Round(lift.magnitude / noWind.magnitude, 2);
           var nWind = Math.Round(noWind.magnitude, 2);


            this.lift.text = liftM.ToString() + "/ wind: " + windA.ToString() + " nWoind " + nWind.ToString();
        }

        internal void Fade(Vector3 fade)
        {
            var fadeM = Math.Round(fade.magnitude, 2);
            this.fade.text = fadeM.ToString() + " " + fade.ToString();
        }

        internal void Turn(Vector3 turn)
        {
            var turnM = Math.Round(turn.magnitude, 2);
            this.turn.text = turnM.ToString() + " " + turn.ToString();
        }

        internal void Wind(Vector3 wind, float windAngleFactor, float windAngle)
        {
            var windM = Math.Round(wind.magnitude, 2);
            var windA = Math.Round(windAngle, 2);
            var windAF = Math.Round(windAngleFactor, 2);

            this.wind.text = windM.ToString() + " angle%: " + windAF.ToString() + " angleModifier: " + windA;
        }
    }
}