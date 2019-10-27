using System;
using UnityEngine;
namespace Assets.Scripts
{
    public class MiniMap : MonoBehaviour
    {
        [SerializeField]
        private RectTransform playerDiscIcon;
        [SerializeField]
        private RectTransform basketIcon;
        public bool trackFlight = false;
        public GameObject playerDisc;
        public DGSceneManager manager;
        [SerializeField] private ButtonBehavior rootObject;
        [SerializeField] private Transform mandoPoint;
        private bool mandoMade;

        public bool flip;

        public float mapFactor;

        void Start()
        {
            float mapWidth = GetComponent<RectTransform>().rect.width;

            float terrainWidth;

            if (flip)
            {
                 terrainWidth = manager.Terrain.terrain.terrainData.size.z;
            }            
            else
            {
                 terrainWidth = manager.Terrain.terrain.terrainData.size.x;
            }

            mapFactor = mapWidth / terrainWidth;
            playerDiscIcon.anchoredPosition = WorldToMapPos(manager.playerScript.transform.position);
            basketIcon.anchoredPosition = WorldToMapPos(manager.playerScript.sceneManager.Basket.basketPosition.position);
        }

        // Update is called once per frame
        void Update()
        {
            if (trackFlight)
            {
                playerDiscIcon.anchoredPosition = WorldToMapPos(playerDisc.transform.position);
            }
        }

        internal void HideMiniMap()
        {
            rootObject.gameObject.SetActive(false);
        }

        internal void OBLineCrossed(bool inBounds)
        {
            if (inBounds)
            {
                manager.playerScript.outOfBounds = false;    
            }
            else
            {
                var obPos = playerDisc.transform.position;
                manager.playerScript.outOfBounds = true;
                manager.playerScript.outOfBoundsPosition = obPos;
            }
        }

        private Vector3 WorldToMapPos(Vector3 worldPos)
        {
            var mapPos = new Vector3((worldPos.x * mapFactor), worldPos.z * mapFactor, 0);

            return mapPos;
        }

        internal void UpdateDiscPosition(Vector3 pos)
        {
            playerDiscIcon.anchoredPosition = WorldToMapPos(pos);
        }

        public void ShowMiniMap()
        {
            rootObject.SlideIn(ButtonBehavior.Direction.left, 1f, 0.5f);
        }

        internal void MandoLineCrossed(bool v)
        {
            if (v)
            {
                mandoMade = true;
            }
            else if(!v && !mandoMade)
            {
                manager.playerScript.MissedMando(mandoPoint.position);
            }
            Debug.Log("mando line crossed. made mando: " + v.ToString());
        }


        //private Vector3 MapToWorldPos(Vector3 mapPos)
        //{
        //    var worldPos = new Vector3((mapPos.x / mapFactor), mapPos.z / mapFactor, 0);

        //    return worldPos;
        //}
    }
}