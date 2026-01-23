using UnityEngine;
using UnityEngine.Playables;

namespace Day01.Scripts
{
    public class LoggerBehaviour : PlayableBehaviour
    {
        public string messageContent;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (Application.isPlaying)
            {
                Debug.Log($"<color=green>[PLAY MODE] Entered:</color> {messageContent}");
            }
            else
            {
                Debug.Log($"<color=cyan>[EDITOR SCRUB] Entered:</color> {messageContent}");
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (playable.GetPlayState() == PlayState.Paused) return;

            if (Application.isPlaying)
            {
                Debug.Log($"<color=red>[PLAY MODE] Left:</color> {messageContent}");
            }
            else
            {
                Debug.Log($"<color=magenta>[EDITOR SCRUB] Left:</color> {messageContent}");
            }
        }
    }
}