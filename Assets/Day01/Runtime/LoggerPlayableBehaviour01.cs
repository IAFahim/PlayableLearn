using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day01.Runtime
{
    public class LoggerPlayableBehaviour01 : PlayableBehaviour
    {

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Debug.Log($"<color=green>Entered:</color> {info}");
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Debug.Log($"<color=red>Left:</color> {info}");
        }
    }
}