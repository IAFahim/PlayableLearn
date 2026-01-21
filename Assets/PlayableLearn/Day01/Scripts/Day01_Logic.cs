using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Layer B: The Logic
    /// The "Brain" node that sits in the graph and monitors playback.
    /// For Day 01, just existing is enough - future days will add math here.
    /// </summary>
    public class Day01_Logic : PlayableBehaviour
    {
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // The logic: Just existing is enough for Day 1
            // In future days, we will do math here.
        }
    }
}
