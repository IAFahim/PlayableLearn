using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day03.Runtime
{
    /// <summary>
    /// Day 03: The First Movement.
    /// Demonstrates playing a single AnimationClip using the Playables API.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Day03AnimationPlayer : MonoBehaviour
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

            _graph = PlayableGraph.Create("Day03_SimpleAnimation");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            _clipPlayable = AnimationClipPlayable.Create(_graph, clip);

            var animOutput = AnimationPlayableOutput.Create(_graph, "AnimationOutput", GetComponent<Animator>());
            animOutput.SetSourcePlayable(_clipPlayable);

            _graph.Play();
            Debug.Log($"<color=cyan><b>[Day 03]</b> Playing clip '{clip.name}' on {name}.</color>");
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
