using System.Collections.Generic;
using UnityEngine;

namespace Meteoroid
{
    [CreateAssetMenu]
    public class MeteoroidPatternSerialized : ScriptableObject
    {
        public List<MeteoroidPattern> Patterns;
    }
}