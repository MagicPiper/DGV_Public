using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class ActionReplayThrow
    {
        public Disc disc;
               
        public List<Vector3> position;
        public List<Vector3> velocity;
        public List<Quaternion> rotation;
    }
}