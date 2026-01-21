using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Layer B: Logic
    /// This is the "Brain" node. It sits in the graph and monitors the playback.
    /// It does not touch GameObjects. It only touches Data.
    /// </summary>
    public sealed class PlaybackMonitorLogic : PlayableBehaviour
    {
        private float _timeActive;

        // Called every frame the graph evaluates
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // Simple logic: Track how long we've been running
            _timeActive += info.deltaTime;
        }

        // Public query for the Adapter to read
        public float GetTotalRuntime() => _timeActive;
    }
}
