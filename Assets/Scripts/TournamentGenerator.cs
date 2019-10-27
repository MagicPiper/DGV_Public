using Assets.Scripts.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TournamentGenerator : MonoBehaviour
    {
        public Tournament open;
        public  PlayerSave save;
        public int count;
        public Course testCourse;
        public int testSCore;

        [ContextMenu("push tournament")]
        public void PushNewTournament()
        {
            StartCoroutine(PushTournaments());
        }
               
        [ContextMenu("clear old games")]
        public void ClearMultiGames()
        {
            save.FirebaseManager.MultiPlayerFunctions.ClearOldMultiplayerGames();
        }

        [ContextMenu("set up new open")]
        public void SetUpNewOpen()
        {
            save.FirebaseManager.CreateOpenTour();
        }

        [ContextMenu("test rating")]
        public void TestRating()
        {
            RatingFunctions.CalculateRating(testCourse.holes, testSCore, 1f);
        }

        public IEnumerator PushTournaments()
        {
            //save.FirebaseManager.ClearTournaments();

           // yield return new WaitForSeconds(5f);

            for (int butt = 0; butt < count; butt++)
            {
                yield return 0.2f;
                open.Generate();
                var buttbutt = new RandomRoundWrapper()
                {
                    holes = open.holes,
                    status = "closed",
                    windSeeds = open.windSeed
                    //scores = new List<Score>(),
                };
                // t.scores.Add(new Score() { userName = "dummy", score = 99 });

                save.FirebaseManager.PushTournament(buttbutt);
            }
        }
    }
}