using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class DGSceneManager : MonoBehaviour
    {
        [SerializeField] protected GameObject playerPrefab;
        public Player playerScript;

        [SerializeField] protected BasketBehavior basket;
        [SerializeField] protected MiniMap miniMap;
        [SerializeField] protected TerrainManager terrain;
        [SerializeField] private BezierCurve cameraPath;
        [SerializeField] private MeshRenderer[] flags;

        [SerializeField] private int minWind;
        [SerializeField] private int maxWind;

        [SerializeField] protected bool hasWater;
        [SerializeField] private float snowChance;
        [SerializeField] private float rainChance;
        [SerializeField] private Material overastskybox;

        [SerializeField] private Transform waterLevel;

        [SerializeField] public bool OBDropZone;
        [SerializeField] public Transform OBDropZoneposition;
        [SerializeField] public Transform AltDropZoneposition;

        protected float currentWind;
        protected float windDirection;

        [SerializeField] protected List<BirbController.birb> birbs;

        public Vector3 WaterLevel
        {
            get
            {
                if (waterLevel != null)
                {
                    return waterLevel.position;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

        public BasketBehavior Basket
        {
            get
            {
                return basket;
            }
        }

        public MiniMap MiniMap
        {
            get
            {
                return miniMap;
            }
        }

        public TerrainManager Terrain
        {
            get
            {
                return terrain;
            }
        }

        public BezierCurve CameraPath
        {
            get
            {
                return cameraPath;
            }
        }

        void Awake()
        {
            var player = Instantiate(playerPrefab);

            player.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            playerScript = player.GetComponent<Player>();
            SetPlayerBehavior(player);
            playerScript.action.player = playerScript;
            playerScript.sceneManager = this;

            InitScene();
        }

        protected void Weather()
        {
            bool rain = false;
            bool snow = false;

            if (UnityEngine.Random.Range(0f, 1f) < rainChance)
            {
                rain = true;
                RenderSettings.skybox = overastskybox;
                RenderSettings.fogDensity = 0.01f;
                playerScript.BirbController.StartRainSounds();

            }
            else if (UnityEngine.Random.Range(0f, 1f) < snowChance)
            {
                snow = true;
                //  RenderSettings.skybox = overastskybox;
                RenderSettings.fogDensity = 0.01f;
            }
            playerScript.UpdateWeather(rain, snow);
        }

        protected virtual void InitScene()
        {

        }

        protected virtual void SetPlayerBehavior(GameObject player)
        {
            playerScript.action = player.AddComponent<PlayerBehavior>();
        }

        protected void Wind()
        {
            if (playerScript.gameState.roundManager is TournamentRoundManager)
            {
                var tm = (TournamentRoundManager)playerScript.gameState.roundManager;
                if (tm.currentTournament.type == Tournament.TournamentType.Open)
                {
                    Random.InitState(tm.currentTournament.windSeed[tm.currentHoleNumber - 1]);
                }
            }
            if (playerScript.gameState.roundManager is MultiplayerRoundManager)
            {
                var tm = (MultiplayerRoundManager)playerScript.gameState.roundManager;
                Random.InitState(tm.currentTournament.windSeed[tm.currentHoleNumber - 1]);
            }

            if (playerScript.gameState.roundManager is ProTourRoundManager)
            {
                var tm = (ProTourRoundManager)playerScript.gameState.roundManager;
                Random.InitState(tm.currentTournament.windSeed[tm.currentHoleNumber - 1]);
            }

            currentWind = Random.Range(minWind, maxWind + 1) * 0.1f;
            terrain.terrain.terrainData.wavingGrassStrength = currentWind;
            windDirection = Random.Range(50f, 130f);
            if (Random.value > 0.5f)
            {
                windDirection += 180;
            }

            if (flags !=null)
            {
                foreach (MeshRenderer flag in flags)
                {
                    flag.material.SetFloat("_WaveSpeed", (200 * currentWind) - 20 + Random.Range(4f, 8));
                }
            }
        }

        //[ContextMenu("Set Curve Points")]
        //private void SetCurvePoints()
        //{
        //    SetStartAndEndPoints();
        //    cameraPath.points[1] = Vector3.Lerp(cameraPath.points[0], cameraPath.points[3], 0.25f); 
        //    cameraPath.points[2] = Vector3.Lerp(cameraPath.points[0], cameraPath.points[3], 0.75f);
        //    EditorUtility.SetDirty(cameraPath);
        //}

        //[ContextMenu("Set StartEnd Points")]
        //private void SetStartAndEndPoints()
        //{
        //    cameraPath.points[0] = transform.position+(Vector3.up*3) + (transform.forward*-2.5f);
        //    cameraPath.points[3] = basket.transform.position + (Vector3.up * 3);
        //    EditorUtility.SetDirty(cameraPath);
        //}
    }
}
