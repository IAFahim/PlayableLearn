using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day03
{
    /// <summary>
    /// An empty PlayableBehaviour for Day 03.
    /// This is the simplest possible behaviour - it does nothing but exists.
    /// Used to demonstrate the creation and linking of ScriptPlayable nodes.
    /// </summary>
    public class Day03EmptyBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// Called when the playable is created.
        /// Good for initialization.
        /// </summary>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // Day 03: Empty behaviour - no logic yet
            // This is just to prove we can create and link nodes
        }

        /// <summary>
        /// Called when the playable is paused or stopped.
        /// </summary>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // Day 03: Empty behaviour - no cleanup needed yet
        }

        /// <summary>
        /// Called every frame while the playable is running.
        /// This is where the main logic would go in future days.
        /// </summary>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // Day 03: Empty behaviour - no frame preparation yet
            // We're just proving the node exists and is connected
        }

        /// <summary>
        /// Called when the playable is being destroyed.
        /// Good for cleanup.
        /// </summary>
        public override void OnPlayableDestroy(Playable playable)
        {
            // Day 03: Empty behaviour - no destruction logic needed yet
        }
    }
}
