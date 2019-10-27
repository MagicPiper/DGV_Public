using UnityEngine;

namespace Assets.Scripts
{
    public class OBCollider : MonoBehaviour
    {
        public MiniMap map;

        private void OnTriggerExit2D(Collider2D collision)
        {
            map.OBLineCrossed(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            map.OBLineCrossed(true);
        }
    }
}