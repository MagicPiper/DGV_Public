using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Mando : MonoBehaviour
    {
        public MiniMap map;

        private void OnTriggerEnter2D(Collider2D collision)
        {           
            Debug.Log(transform.InverseTransformPoint(collision.transform.position));
            var pos = transform.InverseTransformPoint(collision.transform.position);

            map.MandoLineCrossed(pos.y < 0f);
        }
    }
}