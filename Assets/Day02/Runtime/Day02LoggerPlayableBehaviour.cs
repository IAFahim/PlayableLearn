using Common;
using UnityEngine.Playables;

namespace AV.Day02.Runtime
{
    public class Day02LoggerPlayableBehaviour : PlayableBehaviour
    {
        public override void OnGraphStart(Playable playable)
        {
            playable.LogFull();
        }

        public override void OnGraphStop(Playable playable)
        {
            playable.LogFull();
        }

        public override void OnPlayableCreate(Playable playable)
        {
            playable.LogFull();
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            playable.LogFull();
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            playable.LogFull(info, "green");
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            playable.LogFull(info, "red");
        }

        public override void PrepareData(Playable playable, FrameData info)
        {
            playable.LogFull(info, "yellow");
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            playable.LogFull(info, "orange");
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            playable.LogFull(info, "white", playerData);
        }
    }
}