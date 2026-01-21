using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Layer C: The Binder (Adapter)
    /// Extension method that connects the raw graph nodes together.
    /// </summary>
    public static class Day01_Binder
    {
        public static ScriptPlayable<Day01_Logic> CreateLessonGraph(
            this PlayableGraph graph,
            AnimationClip clip,
            Animator target)
        {
            var output = AnimationPlayableOutput.Create(graph, "AnimOut", target);

            var clipPlayable = AnimationClipPlayable.Create(graph, clip);
            var logicPlayable = ScriptPlayable<Day01_Logic>.Create(graph);

            // Wrap the clip in logic
            logicPlayable.AddInput(clipPlayable, 0, 1.0f);

            output.SetSourcePlayable(logicPlayable);

            return logicPlayable;
        }
    }
}
