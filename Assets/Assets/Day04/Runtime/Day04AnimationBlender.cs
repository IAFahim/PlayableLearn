using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day04.Runtime
{
    /// <summary>
    /// Day 04: The Art of Mixing.
    /// Demonstrates how to blend two animation clips using an AnimationMixerPlayable.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Day04AnimationBlender : MonoBehaviour
    {
        [Header("Clips")]
        public AnimationClip clipA;
        public AnimationClip clipB;

        [Header("Controls")]
        [Range(0f, 1f)] 
        public float weight = 0.5f;

        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixer;

        private void Start()
        {
            if (clipA == null || clipB == null)
            {
                Debug.LogError("Please assign both Clip A and Clip B.");
                return;
            }

            _graph = PlayableGraph.Create("Day04_AnimationBlender");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            var animOutput = AnimationPlayableOutput.Create(_graph, "AnimationOutput", GetComponent<Animator>());

            _mixer = AnimationMixerPlayable.Create(_graph, 2); 
            animOutput.SetSourcePlayable(_mixer);

            var clipPlayableA = AnimationClipPlayable.Create(_graph, clipA);
            var clipPlayableB = AnimationClipPlayable.Create(_graph, clipB);

            _graph.Connect(clipPlayableA, 0, _mixer, 0);
            _graph.Connect(clipPlayableB, 0, _mixer, 1);

            _graph.Play();
            Debug.Log("<color=cyan><b>[Day 04]</b> Animation Blender Created.</color>");
        }

        private void Update()
        {
            if (!_graph.IsValid()) return;
            _mixer.SetInputWeight(0, 1f - weight);
            _mixer.SetInputWeight(1, weight);
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
