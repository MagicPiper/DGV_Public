using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Tournament", menuName = "DiscGolf/Tournament", order = 1)]
    public class Tournament : ScriptableObject
    {
        public enum TournamentType { Recreational, Intermediate, Advanced, Open }
        public TournamentType type;
        [SerializeField] private Course[] coursePool;
        public int tournamentLenght;

        public List<Hole> holes;
        public List<int> windSeed;        
        public List<AIOpponent> opponents;
        public DiscData discData;
        public int XPReward;
        public string openTournamentID;

        public void Generate()
        {
            Holes();
            Opponents();
            Wind();
        }

        private void Opponents()
        {
            opponents = new List<AIOpponent>();
            for (int i = 0; i < 7; i++)
            {
                opponents.Add(new AIOpponent());
            }
        }

        private void Wind()
        {
            windSeed = new List<int>();

            for (int i =0; i < tournamentLenght; i++)
            {
                windSeed.Add(UnityEngine.Random.Range(0,255));
            }
        }

        private void Holes()
        {
            holes = new List<Hole>();
            
            List<int> courseOrder = new List<int>();

            for (int i = 0; i < coursePool.Length; i++)
            {
                courseOrder.Add(i);
            }

            System.Random rnd = new System.Random();

            int[] tournamentCourseOrder = courseOrder.OrderBy(x => rnd.Next()).ToArray();

            foreach (var course in tournamentCourseOrder)
            {
                var left = tournamentLenght - holes.Count;
                if (left >= 3)
                {
                    left = 3;
                }

                holes.AddRange(this.GetRandomHoles(coursePool[course], left, rnd));
            }
        }

        internal int AIScore(int par)
        {
            float birdieChance = 0;
            float bogeyChance = 0;
            float doubleBogeyChance = 0;

            switch (type) 
            {
                case TournamentType.Recreational:
                    birdieChance = 0.1f;
                    bogeyChance = 0.4f;
                    doubleBogeyChance = 0.2f;
                    break;
                case TournamentType.Intermediate:
                    birdieChance = 0.2f;
                    bogeyChance = 0.2f;
                    doubleBogeyChance = 0.1f;
                    break;
                case TournamentType.Advanced:
                    birdieChance = 0.4f;
                    bogeyChance = 0.1f;
                    doubleBogeyChance = 0.1f;
                    break;
                default:
                    birdieChance = 0.2f;
                    bogeyChance = 0.2f;
                    doubleBogeyChance = 0.1f;
                    break;
            }

            int score = par;
            var rand = UnityEngine.Random.value;

           // Debug.Log("Scoring an AI player");

            if (rand < birdieChance)
            {
                score -= 1;
              //  Debug.Log("birdie");
            }
            else if (rand < (birdieChance + bogeyChance))
            {
                score += 1;
               // Debug.Log("bogey");
            }
            else if(rand< birdieChance + bogeyChance + doubleBogeyChance)
            {
                score += 2;
             //   Debug.Log("doubleBogey");
            }
            return score;
        }

        public IEnumerable<Hole> GetRandomHoles(Course course, int count, System.Random rnd)
        {
            return course.holes.OrderBy(x => rnd.Next()).ToList().Take(count);
        }
    }
}
