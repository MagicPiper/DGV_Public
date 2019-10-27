using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public float rotMod = 0.01f;

       // public static bool discTesting = true;
        public List<Disc> testDiscs;
        public List<Disc> offlineDiscs;
        public GameObject discPrefab;
        public GameObject curentDisc;
        public DiscBehavior curentDiscScript;
        public GameObject discHolder;
        public Transform rotateAroundPoint;
        public int strokes = 0;
        public float throwDistance;
        public Vector3 lie;
        public string[] goodShot;
        public string[] goodLongShot;
        public string[] missedPutt;
        public float DistanceToBasket { get; set; }
        public string DistanceToBasketString
        {
            get
            {
                return gameState.playerSave.playerSettings.DistanceConverter(DistanceToBasket);
            }
        }

        public Camera playerCamera;
        public PlayerCamera playerCameraScript;
        public Transform mainCam;

        public Transform puttCam;
        public PlayerBehavior action;
        public bool outOfBounds;
        public bool outOfBoundsWater;
        public Vector3 outOfBoundsPosition;
        public bool missedMando;
        public Vector3 mandoPosition;

        public WindManager windManager;

        public EnableCameraDepthInForward waterDepthScript;
        [SerializeField] private GameObject snow;
        [SerializeField] private GameObject rain;

        [SerializeField] private GameObject UICanvas;
        public UI UI;

        [SerializeField] private GameObject throwUICanvas;
        public ThrowUI throwUI;

        [SerializeField] private ScoreScreen scoreScreenCanvas;
        [SerializeField] private PracticeScoreScreen practiceScoreScreenCanvas;

        [SerializeField] private GameObject discSelectCanvas;
        public DiscSelect discSelect;

        internal DGSceneManager sceneManager;
        public GameState gameState;
        public LineRenderer reachBackLineRenderer;
        [SerializeField] private BirbController birbController;

        public ActionReplayThrow replayThrow;
        internal bool discInBasket;
        
        public BirbController BirbController
        {
            get
            {
                return birbController;
            }
        }

        public ScoreScreen scoreScreen { get; private set; }

        public void InitPlayer(float windSpeed, float windDirection)
        {
            action.player = this;
           
            throwUI = Instantiate(throwUICanvas).GetComponent<ThrowUI>();
            throwUI.playerScript = this;
            discSelect = Instantiate(discSelectCanvas).GetComponent<DiscSelect>();
            discSelect.playerScript = this;
            UI = Instantiate(UICanvas).GetComponent<UI>();
            UI.playerScript = this;
            windManager.SetWind(windSpeed, windDirection);

            action.PlayerStart();
        }

        internal void MissedMando(Vector3 position)
        {
            mandoPosition = position;
            missedMando = true;            
        }

        public void MoveToLie()
        {
            action.MoveToLie();
        }

        public void PlayReplay()
        {
            action.PlayReplay();
        }

        public void ThrowAgain()
        {
            action.ThrowAgain();
        }

        public void TapIn()
        {
            action.TapIn();
        }

        internal void ShowScoreScreen()
        {
            scoreScreen = Instantiate(scoreScreenCanvas);
            scoreScreen.ShowScoreScreen(strokes);
        }

        internal void ShowPracticeScoreScreen()
        {
            var scoreScreen = Instantiate(practiceScoreScreenCanvas);
            scoreScreen.ShowPracticeScoreScreen();
        }

        internal void UpdateWeather(bool rain, bool snow)
        {
            this.rain.SetActive(rain);
            this.snow.SetActive(snow);
        }       
    }
}
