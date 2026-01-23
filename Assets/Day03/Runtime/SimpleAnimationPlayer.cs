using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day03.Runtime
{
    /// <summary>
    /// Day 03: The First Movement.
    /// Demonstrates playing a single AnimationClip using the Playables API.
    /// This bypasses the Animator Controller state machine entirely.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class SimpleAnimationPlayer : MonoBehaviour
    {
        [Tooltip("The clip to play.")]
        public AnimationClip clip;

        private PlayableGraph _graph;
        private AnimationClipPlayable _clipPlayable;

        private void Start()
        {
            if (clip == null)
            {
                Debug.LogError("Please assign an AnimationClip.");
                return;
            }

            // 1. Create the Graph
            _graph = PlayableGraph.Create("Day03_SimpleAnimation");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // 2. Create the Node (The Data Source)
            // AnimationClipPlayable wraps an AnimationClip so the graph can use it.
            _clipPlayable = AnimationClipPlayable.Create(_graph, clip);

            // 3. Create the Output (The Driver)
            // AnimationPlayableOutput links the graph to an Animator component on a GameObject.
            // This is what actually applies the pose to the transform hierarchy.
            var animOutput = AnimationPlayableOutput.Create(_graph, "AnimationOutput", GetComponent<Animator>());

            // 4. Connect Node -> Output
            // Connect port 0 of the playable to the source of the output.
            animOutput.SetSourcePlayable(_clipPlayable);

            // 5. Play
            _graph.Play();
            
            Debug.Log($"<color=cyan><b>[Day 03]</b> Playing clip '{clip.name}' on {name}.</color>");
        }

        private void OnDestroy()
        {
            if (_graph.IsValid())
            {
                _graph.Destroy();
            }
        }
    }
}
