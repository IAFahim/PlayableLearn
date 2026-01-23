using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace AV.Day10.Runtime
{
    /// <summary>
    /// Day 10: The Master Director.
    /// Demonstrates a "Multi-Output" Graph.
    /// One Graph driving two completely different systems (Animation & Audio) in perfect sync.
    /// </summary>
    public class Day10CinematicDirector : MonoBehaviour
    {
        [Header("Components")]
        public Animator targetAnimator;
        public AudioSource targetAudioSource;

        [Header("Assets")]
        public AnimationClip animationClip;
        public AudioClip audioClip;

        private PlayableGraph _graph;

        private void Start()
        {
            if (targetAnimator == null || targetAudioSource == null)
            {
                Debug.LogError("Assign Animator and AudioSource.");
                return;
            }

            // 1. Create the Master Graph
            _graph = PlayableGraph.Create("Day10_MasterDirector");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // --- SYSTEM A: ANIMATION ---
            var animOutput = AnimationPlayableOutput.Create(_graph, "AnimOut", targetAnimator);
            var animPlayable = AnimationClipPlayable.Create(_graph, animationClip);
            animOutput.SetSourcePlayable(animPlayable);

            // --- SYSTEM B: AUDIO ---
            var audioOutput = AudioPlayableOutput.Create(_graph, "AudioOut", targetAudioSource);
            var audioPlayable = AudioClipPlayable.Create(_graph, audioClip, true);
            audioOutput.SetSourcePlayable(audioPlayable);

            // 3. Play The Whole System
            _graph.Play();
            
            Debug.Log("<color=cyan><b>[Day 10]</b> Cinematic Graph Playing (Anim + Audio).</color>");
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
