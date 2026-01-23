using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day05.Runtime
{
    /// <summary>
    /// Day 05: Layered Animation.
    /// Demonstrates how to play animations on specific body parts using AvatarMasks.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Day05LayeredAnimation : MonoBehaviour
    {
        [Header("Layers")]
        public AnimationClip baseClip;
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

            _layerMixer = AnimationLayerMixerPlayable.Create(_graph, 2);
            output.SetSourcePlayable(_layerMixer);

            var basePlayable = AnimationClipPlayable.Create(_graph, baseClip);
            _graph.Connect(basePlayable, 0, _layerMixer, 0);
            _layerMixer.SetInputWeight(0, 1f); 

            var upperPlayable = AnimationClipPlayable.Create(_graph, upperBodyClip);
            _graph.Connect(upperPlayable, 0, _layerMixer, 1);
            _layerMixer.SetInputWeight(1, layerWeight);

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
