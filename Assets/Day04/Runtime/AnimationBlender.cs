using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day04.Runtime
{
    /// <summary>
    /// Day 04: The Art of Mixing.
    /// Demonstrates how to blend two animation clips using an AnimationMixerPlayable.
    /// This is the foundational logic for Crossfades and Blend Trees.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationBlender : MonoBehaviour
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

            // 1. Create Graph
            _graph = PlayableGraph.Create("Day04_AnimationBlender");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // 2. Create Output
            var animOutput = AnimationPlayableOutput.Create(_graph, "AnimationOutput", GetComponent<Animator>());

            // 3. Create the Mixer (The Processor)
            // This node takes multiple inputs and blends them based on weights.
            _mixer = AnimationMixerPlayable.Create(_graph, 2); // 2 inputs
            animOutput.SetSourcePlayable(_mixer);

            // 4. Create the Inputs (The Sources)
            var clipPlayableA = AnimationClipPlayable.Create(_graph, clipA);
            var clipPlayableB = AnimationClipPlayable.Create(_graph, clipB);

            // 5. Connect Inputs to Mixer
            // Connect(source, sourceOutputPort, destination, destinationInputPort)
            _graph.Connect(clipPlayableA, 0, _mixer, 0);
            _graph.Connect(clipPlayableB, 0, _mixer, 1);

            // 6. Play
            _graph.Play();
            
            Debug.Log("<color=cyan><b>[Day 04]</b> Animation Blender Created.</color>");
        }

        private void Update()
        {
            if (!_graph.IsValid()) return;

            // 7. Update Weights (The Logic)
            // The sum of weights usually should be 1.0 for normalized blending,
            // but the system allows any values (additive blending etc).
            _mixer.SetInputWeight(0, 1f - weight);
            _mixer.SetInputWeight(1, weight);
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
