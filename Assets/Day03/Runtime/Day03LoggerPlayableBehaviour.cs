using Common;
using UnityEngine.Playables;

namespace AV.Day03.Runtime
{
    public class Day03LoggerPlayableBehaviour : PlayableBehaviour
    {
        public override void OnGraphStart(Playable playable)
        {
            playable.LogFull(null, "white", null, "CLIP", depth: 2);
        }

        public override void OnGraphStop(Playable playable)
        {
            playable.LogFull(null, "white", null, "CLIP", depth: 2);
        }

        public override void OnPlayableCreate(Playable playable)
        {
            playable.LogFull(null, "white", null, "CLIP", depth: 2);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            playable.LogFull(null, "white", null, "CLIP", depth: 2);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            playable.LogFull(info, "green", null, "CLIP", depth: 2);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            playable.LogFull(info, "red", null, "CLIP", depth: 2);
        }

        public override void PrepareData(Playable playable, FrameData info)
        {
            playable.LogFull(info, "yellow", null, "CLIP", depth: 2);
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            playable.LogFull(info, "orange", null, "CLIP", depth: 2);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            playable.LogFull(info, "white", playerData, "CLIP", depth: 2);
        }
    }
}
