using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day09.Runtime
{
    // The "Data" Node
    public class LightControlBehaviour : PlayableBehaviour
    {
        public Color color = Color.white;
        public float intensity = 1f;
    }
}
