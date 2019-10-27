using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerCamera : MonoBehaviour
    {
        private bool droneCam;
        private bool discCam;
        private bool lieCam;
        private bool skipFlight;
        private float dollySpeedMod;
        private float fullDistanceToTravel;
        [SerializeField] private Transform playerCamTransform;
        [SerializeField] private Player playerScript;
        [SerializeField] private float dollySpeed;
        [SerializeField] private float camFollowSpeed;
        [SerializeField] private float camRotSpeed;

        private int flipRotationDirection;
        private Vector3 basketPosition;
        private BezierCurve cameraPath;
        [SerializeField] private GameObject debugSphere;

        public bool DroneCam
        {
            get
            {
                return droneCam;
            }
            set
            {
                droneCam = value;
            }
        }

        public bool DiscCam
        {
            get
            {
                return discCam;
            }
            set
            {
                discCam = value;
            }
        }
        public bool LieCam
        {
            get
            {
                return lieCam;
            }
            set
            {
                lieCam = value;
            }
        }

        public bool SkipFlight
        {
            get
            {
                return skipFlight;
            }
            set
            {
                skipFlight = value;
            }
        }

        public void StartDroneCam(HoleSceneManager holeMan)
        {
            // holeManager = holeMan;
            skipFlight = false;
            flipRotationDirection = holeMan.DroneCamRotDirection ? -1 : 1;
            dollySpeedMod = holeMan.DroneCamSpeedMod;
            var b = holeMan.Basket.basketPosition.transform.position;
            basketPosition = new Vector3(b.x, b.y + 3, b.z);

            cameraPath = holeMan.CameraPath;
            Invoke("StartCam", 2f);
        }

        private void StartCam()
        {
            droneCam = true;
            if (!skipFlight)
            {
                StartCoroutine(DroneFlyToCam());
            }
        }

        public void StartDiscFollowCam()
        {
            discCam = true;
            StartCoroutine(DiscFollowCam());
        }

        public void MoveToLieCam()
        {
            lieCam = true;
            StartCoroutine(MoveToLie());
        }

        private IEnumerator DroneFlyToCam()
        {
            camFollowSpeed = 1f;
            camRotSpeed = 4f;
            var startPoint = playerCamTransform.position;
            var startRot = playerCamTransform.rotation;
            
            GameObject dolly = new GameObject("flyPoint");
            // GameObject dolly = Instantiate(debugSphere);
            bool flying = true;
            bool rotating = false;
            dollySpeed = 0.1f * dollySpeedMod;
            var dollypos = 0.0f;
            Quaternion lookAt = Quaternion.LookRotation(transform.forward, transform.up);           
            var step1 = dollySpeed;
            while (droneCam)
            {
                if (flying)
                {
                    dolly.transform.position = cameraPath.GetPoint(dollypos);
                }
                else
                {
                    dolly.transform.position = new Vector3(dolly.transform.position.x, Mathf.Lerp(dolly.transform.position.y, basketPosition.y, 0.001f), dolly.transform.position.z);
                    dolly.transform.RotateAround(basketPosition, Vector3.up, 7 * flipRotationDirection * Time.deltaTime);
                    
                    if (!rotating)
                    {
                        rotating = true;
                        Invoke("DroneCamComplete", 2f);
                    }
                }

                dollypos += step1 * Time.deltaTime;
                var basketDir = Quaternion.LookRotation(basketPosition - transform.position, Vector3.up);
                lookAt = Quaternion.Slerp(startRot, basketDir, dollypos);

                flying = Vector3.Distance(dolly.transform.position, basketPosition) > 10;

                MoveCam(dolly.transform.position, lookAt);

                yield return null;
            }

            playerCamTransform.position = startPoint;
            playerCamTransform.rotation = startRot;
            playerScript.windManager.UpdateWindPosition();
        }



        private IEnumerator MoveToLie()
        {
            camFollowSpeed = 5f;
            camRotSpeed = 4f;

            while (lieCam)
            {
                MoveCam(playerScript.mainCam.position, playerScript.mainCam.rotation);

                var angle = Quaternion.Angle(playerScript.mainCam.rotation, transform.rotation);
                var distance = Vector3.Distance(playerScript.mainCam.position, transform.position);

                if (angle < 0.1f && distance < 0.01f)
                {
                    lieCam = false;
                    playerScript.playerCamera.transform.SetParent(playerScript.mainCam);
                   if(playerScript.throwUI.windIndicator != null) playerScript.throwUI.windIndicator.UpdateRotation(transform.rotation);

                    Debug.Log("lie cam ends");

                }

                yield return null;
            }
        }


        private IEnumerator DiscFollowCam()
        {
            camFollowSpeed = 2f;
            camRotSpeed = 3f;

            dollySpeed = 2f;

            GameObject dolly = new GameObject("flyPoint");
            // GameObject dolly = Instantiate(debugSphere);

            dolly.transform.position = playerScript.curentDisc.transform.position + (Vector3.up * 1.5f);
            bool follow = false;
            bool rotate = false;
            var up = 1.5f;

            while (discCam)
            {
                var moveTarget = playerScript.curentDisc.transform.position + (transform.up * up);
                var lookTarget = Quaternion.LookRotation(moveTarget - transform.position, Vector3.up);

                var camDistance = Vector3.Distance(transform.position, moveTarget);
                var dollyDistance = Vector3.Distance(dolly.transform.position, moveTarget);
                var step1 = dollySpeed * dollyDistance * Time.deltaTime;

                dolly.transform.position = Vector3.MoveTowards(dolly.transform.position, moveTarget, step1);

                follow = camDistance > 15f ? true : follow;

                if (follow && !rotate)
                {
                    MoveCam(dolly.transform.position, lookTarget);
                    rotate = camDistance < 5f;
                }
                else if (rotate)
                {
                    dollySpeed = 0;
                    dolly.transform.RotateAround(moveTarget, Vector3.up, -7 * Time.deltaTime);
                    MoveCam(dolly.transform.position, lookTarget);
                }

                yield return null;
            }
        }

        private void MoveCam(Vector3 flyToPoint, Quaternion lookAtAngle)
        {
            var d = Vector3.Distance(transform.position, flyToPoint);
            var moveStep = (d * 3 * Time.deltaTime) * camFollowSpeed;
            playerCamTransform.position = Vector3.MoveTowards(playerCamTransform.position, flyToPoint, moveStep);

            playerCamTransform.rotation = Quaternion.Slerp(playerCamTransform.rotation, lookAtAngle, camRotSpeed * Time.deltaTime);

            if (playerScript.windManager != null)
            {
                playerScript.windManager.UpdateWindPosition();
            }
        }

        private void DroneCamComplete()
        {
            if (Menu.PlayerSave.playerGet)
            {
                playerScript.UI.popUpText2.ShowText("Hole " + playerScript.gameState.roundManager.currentHoleNumber.ToString(), 10f);
                playerScript.UI.popUpText.ShowText("Par " + playerScript.gameState.roundManager.currentHole.par.ToString(), 10f, Color.green);
            }
            else
            {
                playerScript.UI.popUpText2.ShowText("Hole Test" , 10f);
                playerScript.UI.popUpText.ShowText("Par Test" , 10f, Color.green);
            }
            playerScript.UI.readyToThrowButton.HighLightButton("Let's Throw!", new Vector2(350, 100));
        }
    }
}