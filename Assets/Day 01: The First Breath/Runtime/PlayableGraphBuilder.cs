using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Layer C: The Adapter / Graph Orchestration.
    /// Extension methods to create and connect graph nodes cleanly.
    /// </summary>
    public static class PlayableGraphBuilder
    {
        /// <summary>
        /// Creates a simple clip player graph with the given clip.
        /// Graph structure: [AnimationClipPlayable] -> [AnimationPlayableOutput]
        /// </summary>
        public static PlayableGraph CreateClipPlayerGraph(
            this PlayableGraph graph,
            AnimationClipData clipData,
            Animator animator,
            out ScriptPlayable<SimpleClipPlayerBehaviour> behaviourNode)
        {
            // 1. Create the Animation Clip Playable (holds the actual animation)
            var clipPlayable = AnimationClipPlayable.Create(graph, clipData.Clip);
            clipPlayable.SetSpeed(clipData.Speed);

            // 2. Create the Logic Node (tracks playback state)
            behaviourNode = ScriptPlayable<SimpleClipPlayerBehaviour>.Create(graph);
            var behaviour = behaviourNode.GetBehaviour();
            behaviour.Play();

            // 3. Connect: Clip -> Behaviour
            // The behaviour wraps and controls the clip
            behaviourNode.AddInput(clipPlayable, 0, 1.0f);

            // 4. Create Output and connect to Behaviour
            var output = AnimationPlayableOutput.Create(graph, "AnimationOutput", animator);
            output.SetSourcePlayable(behaviourNode);

            return graph;
        }
    }
}
