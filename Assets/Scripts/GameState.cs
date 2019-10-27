using Assets.Scripts.Menu;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "GameState", menuName = "DiscGolf/GameState", order = 7)]
    public class GameState : ScriptableObject
    {
        public RoundManager roundManager;
        public PlayerSave playerSave;


        public void NewRound()
        {
            roundManager = new RoundManager();
            roundManager.playerSave = playerSave;
        }

        public TournamentRoundManager NewTournamentRound()
        {
            roundManager = new TournamentRoundManager();
            roundManager.playerSave = playerSave;
            return (TournamentRoundManager)roundManager;
        }

        public PracticeRoundManager NewPracticeRound()
        {
            roundManager = new PracticeRoundManager();
            roundManager.playerSave = playerSave;
            return (PracticeRoundManager)roundManager;
        }

        public MultiplayerRoundManager NewMultiplayerRound()
        {
            roundManager = new MultiplayerRoundManager();
            roundManager.playerSave = playerSave;
            return (MultiplayerRoundManager)roundManager;
        }

        public FriendlyRoundManager NewFriendlyRound()
        {
            roundManager = new FriendlyRoundManager();
            roundManager.playerSave = playerSave;
            return (FriendlyRoundManager)roundManager;
        }

        public void ResetRound()
        {
            roundManager = null;
        }

        internal ProTourRoundManager NewProTourRound()
        {
            roundManager = new ProTourRoundManager();
            roundManager.playerSave = playerSave;
            return (ProTourRoundManager)roundManager;
        }
    }
}