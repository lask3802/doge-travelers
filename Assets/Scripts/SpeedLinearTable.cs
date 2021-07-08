using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu]
    public class SpeedLinearTable : ScriptableObject
    {
        public List<SpeedLinear> Elements;
        
        [Serializable]
        public class SpeedLinear
        {
            public float Time;
            public float SpeedAtTime;
        }
    }
}