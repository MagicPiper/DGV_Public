using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class RatingFunctions
    {
        internal static RoundRating CalculateRating(List<Hole> holes, int score, float w)
        {
            float roundDiff = 0f;
            float roundPar = 0f;

            foreach (Hole hole in holes)
            {
                roundDiff += hole.difficulty;
                roundPar += hole.par;
            }
            float roundStrokeDiff = (float)roundPar + roundDiff;
            Debug.Log("Round difficulty: " + roundDiff);
            Debug.Log("Round stroke average: " + roundStrokeDiff);

            float scoreDiff = (float)score - roundStrokeDiff;

             Debug.Log("Par Diff: " + scoreDiff);

            var r = 750f + ((250f / (float)holes.Count) * (scoreDiff*-1));

            var final = r < 100f ? 100f : r; 

            return new RoundRating
            {
                rating = Mathf.Round(final),
                weight = w
            };
        }

        internal static RoundRating AbandonedRound()
        {
            return new RoundRating
            {
                rating = 100f,
                weight = 1f
            };
        }
    }
}
