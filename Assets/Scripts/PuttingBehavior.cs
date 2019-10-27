using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class PuttingBehavior : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public DiscBehavior discScript;
        public bool isDragging = false;
        public bool puttComplete = false;
        public Vector3 moveTo;
        public float dragDamper = 5.0f;
        public float speed;
        private float dist;
        private float maxVelocity;
        public Vector3 velocity;
        public float distance;
        public Plane plane;
        public SpriteRenderer puttingIndicator;
        public AudioSource puttStartSound;
        public Collider touchCollider;

        public bool menu;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (menu || discScript.player.throwUI.shotSelect.SelectedShot == ShotSelector.ShotType.Putt)
            {
                isDragging = true;
                dist = discScript.player.DistanceToBasket;
                var logDist = Mathf.Log(dist, 10f);
                maxVelocity = logDist > 1.3f ? 1.4f : Mathf.Lerp(0.5f, 1.4f, logDist / 1.3f);
                var angle = dist > 12f ? 15f : Mathf.Lerp(0f, 15f, dist / 12f);
                angle = dist < 3.5f ? 0f : angle;
                //maxVelocity = dist > 12f ? 1.25f : Mathf.Lerp(0.6f, 1.25f, dist / 12f);
                //var angle = dist > 12f ? 16f : Mathf.Lerp(2f, 16f, dist / 12);
                //angle = dist < 6f ? 1f : angle;
                //angle = angle * 2f;

                //Debug.Log("distance: " + dist + " logDist: " + logDist);
                //Debug.Log("angle: " + angle + " maxVelocity: " + maxVelocity);


                if (!menu) discScript.player.throwUI.HideThrowUI();
                plane = new Plane(Quaternion.AngleAxis(-angle, transform.right) * transform.up, transform.position);
                discScript.ShowPuttingGuide(false);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (menu || discScript.player.throwUI.shotSelect.SelectedShot == ShotSelector.ShotType.Putt)
            {
                Ray ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
                float d;
                if (plane.Raycast(ray, out d))
                {
                    moveTo = ray.GetPoint(d);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
           // Debug.Log("End Drag. Putt Complete: " + puttComplete);

            puttingIndicator.enabled = false;
            if (!puttComplete)
            {
                Invoke("ResetDisc", 0.8f);
            }

            //if (menu || discScript.player.throwUI.shotSelect.SelectedShot == ShotSelector.ShotType.Putt)
            //{
            //    if (transform.localPosition.z > 1f)
            //    {
            //        if (!menu)
            //        {
            //            discScript.Putt();
            //        }
            //        else
            //        {
            //            discScript.MenuPutt();

            //        }
            //        isDragging = false;
            //    }
            //}
        }

        private void ResetDisc()
        {
            if (!puttComplete)
            {
                discScript.transform.position = discScript.player.discHolder.transform.position;
                discScript.rb.velocity = Vector3.zero;
                touchCollider.enabled = true;
                if (!menu) discScript.player.throwUI.ShowThrowUI();

                isDragging = false;
            }
        }

        void FixedUpdate()
        {
            if (isDragging)
            {
                velocity = moveTo - discScript.transform.position;
                velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
                discScript.rb.velocity = velocity * 10; // makes sense
                speed = discScript.rb.velocity.magnitude;

                distance = discScript.transform.localPosition.z;
                if (distance > 1f)
                {
                    if (!menu)
                    {
                        discScript.Putt();
                    }
                    else
                    {
                        discScript.MenuPutt();

                    }
                    isDragging = false;
                    puttComplete = true;

                    puttingIndicator.enabled = false;
                }
            }

            int nbTouches = Input.touchCount;

            if (nbTouches > 0)
            {
                for (int i = 0; i < nbTouches; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    if (touch.phase == TouchPhase.Began)
                    {
                        Ray screenRay = Camera.main.ScreenPointToRay(touch.position);

                        RaycastHit hit;
                        if (Physics.Raycast(screenRay, out hit))
                        {
                            if (hit.collider.gameObject == this.gameObject)
                            {
                                // Handheld.Vibrate();
                                puttStartSound.Play();
                                puttingIndicator.enabled = true;
                            }
                        }
                    }
                }
            }
            else
            {
                puttingIndicator.enabled = false;
            }
        }

        internal void EnableCollider(bool show)
        {
            touchCollider.enabled = show;
        }
    }
}
