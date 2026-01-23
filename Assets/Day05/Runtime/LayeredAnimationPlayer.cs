using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day05.Runtime
{
    /// <summary>
    /// Day 05: Layered Animation.
    /// Demonstrates how to play animations on specific body parts using AvatarMasks.
    /// Layer 0: Base Layer (Full Body)
    /// Layer 1: Override Layer (Masked)
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class LayeredAnimationPlayer : MonoBehaviour
    {
        [Header("Layers")]
        [Tooltip("The base animation (e.g., Idle/Walk)")]
        public AnimationClip baseClip;
        
        [Tooltip("The override animation (e.g., Wave/Shoot)")]
        public AnimationClip upperBodyClip;

        [Header("Configuration")]
        public AvatarMask upperBodyMask;
        [Range(0f, 1f)]
        public float layerWeight = 1f;

        private PlayableGraph _graph;
        private AnimationLayerMixerPlayable _layerMixer;

        private void Start()
        {
            if (baseClip == null || upperBodyClip == null || upperBodyMask == null)
            {
                Debug.LogError("Please assign Base Clip, Upper Body Clip, and the Avatar Mask.");
                return;
            }

            _graph = PlayableGraph.Create("Day05_LayeredAnimation");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            var output = AnimationPlayableOutput.Create(_graph, "LayerOutput", GetComponent<Animator>());

            // 1. Create Layer Mixer
            // Takes inputs as "Layers". Higher index = drawn on top.
            _layerMixer = AnimationLayerMixerPlayable.Create(_graph, 2);
            output.SetSourcePlayable(_layerMixer);

            // 2. Setup Layer 0 (Base)
            var basePlayable = AnimationClipPlayable.Create(_graph, baseClip);
            _graph.Connect(basePlayable, 0, _layerMixer, 0);
            _layerMixer.SetInputWeight(0, 1f); // Base is always fully active

            // 3. Setup Layer 1 (Upper Body)
            var upperPlayable = AnimationClipPlayable.Create(_graph, upperBodyClip);
            _graph.Connect(upperPlayable, 0, _layerMixer, 1);
            _layerMixer.SetInputWeight(1, layerWeight);

            // 4. Apply Mask
            // This is crucial. It tells the mixer which bones Layer 1 is allowed to touch.
            _layerMixer.SetLayerMaskFromAvatarMask(1, upperBodyMask);

            _graph.Play();
            Debug.Log("<color=cyan><b>[Day 05]</b> Layered Animation Graph Created.</color>");
        }

        private void Update()
        {
            if (_graph.IsValid())
            {
                _layerMixer.SetInputWeight(1, layerWeight);
            }
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
