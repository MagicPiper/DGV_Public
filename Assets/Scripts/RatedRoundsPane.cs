using Assets.Scripts.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class RatedRoundsPane : MenuPane
    {
        public PlayerSave playerSave;
        public UIRound roundPrefab;
        public Transform roundHolder;
        private bool populated;

        public override void Show()
        {
            base.Show();

            PopulateRounds();
        }

        private void PopulateRounds()
        {
            if (!populated)
            {
                var rounds = playerSave.playerStats.statsData.roundRatings;
                int iter = 1;
                foreach (RoundRating roundRating in rounds)
                {
                    var roundObject = Instantiate(roundPrefab, roundHolder);
                    roundObject.gameObject.SetActive(true);

                    string roundType = roundRating.weight < 1.5f ? "Abandoned Multiplayer" : "Multiplayer Round";
                    bool abandoned = roundRating.weight < 1.5f;
                    roundType = roundRating.weight > 3f ? "Pro Tour Round" : roundType;

                    roundObject.Populate(roundType, roundRating.rating, abandoned, iter);
                    iter++;
                }
                populated = true;
            }
        }
    }
}
