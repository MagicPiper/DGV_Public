using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class PracticePanel : MenuPane
    {
        public GameState gameState;
        private string practiceScene;

        public void StartPractice(PracticeMode practice)
        {
            var rm = gameState.NewPracticeRound();
            rm.StartPracticeRound(practice.type);
            practiceScene = practice.sceneName;
            StartCoroutine("LoadNewScene");
            //playButtonText.text = "Loading";
            MusicManager.Instance.FadeOutMusic();
        }

        IEnumerator LoadNewScene()
        {
            // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
           // AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(practiceScene);
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(practiceScene);

            // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
            //while (!async.isDone)
            //{
            //    var t = Mathf.PingPong(Time.time, 1f);
            //    playButtonText.alpha = t;
            yield return null;
            //}
        }
    }
}