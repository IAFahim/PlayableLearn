using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Layer C: The Adapter
    /// Connects the raw graph nodes together.
    /// </summary>
    public static class Day01_Binder
    {
        public static ScriptPlayable<PlaybackMonitorLogic> CreateSimpleGraph(
            this PlayableGraph graph,
            AnimationConfig data,
            Animator outputTarget)
        {
            // 1. Create the Animation Node (The Muscle)
            var clipNode = AnimationClipPlayable.Create(graph, data.Clip);
            clipNode.SetSpeed(data.Speed);

            // 2. Create the Logic Node (The Brain)
            var logicNode = ScriptPlayable<PlaybackMonitorLogic>.Create(graph);

            // 3. Connect Muscle -> Brain
            // The Logic Node wraps the Clip Node.
            // Input 0 of LogicNode receives the animation stream.
            logicNode.AddInput(clipNode, 0, 1.0f);

            // 4. Create the Output (The Socket)
            var output = AnimationPlayableOutput.Create(graph, "MeshOutput", outputTarget);

            // 5. Connect Brain -> Socket
            output.SetSourcePlayable(logicNode);

            return logicNode;
        }
    }
}
