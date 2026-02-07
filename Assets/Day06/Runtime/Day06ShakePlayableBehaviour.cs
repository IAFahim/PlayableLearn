using UnityEngine.Playables;

namespace AV.Day06.Runtime
{
    public class Day06ShakePlayableBehaviour : PlayableBehaviour
    {
        public float intensity;

        // We don't need LogFull here because the Mixer will do the heavy lifting today.
    }
}
