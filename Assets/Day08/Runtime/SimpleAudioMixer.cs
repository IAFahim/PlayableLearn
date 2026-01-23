using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace AV.Day08.Runtime
{
    /// <summary>
    /// Day 08: The Sound of Data.
    /// Demonstrates mixing Audio using the Playables API.
    /// This allows frame-perfect synchronization between Audio and other game systems.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SimpleAudioMixer : MonoBehaviour
    {
        public AudioClip clipA;
        public AudioClip clipB;
        
        [Range(0f, 1f)]
        public float mix = 0.5f;

        private PlayableGraph _graph;
        private AudioMixerPlayable _mixer;

        private void Start()
        {
            if (clipA == null || clipB == null)
            {
                Debug.LogError("Please assign two AudioClips.");
                return;
            }

            _graph = PlayableGraph.Create("Day08_AudioMixer");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // 1. Create Output
            // Wraps an AudioSource
            var audioOutput = AudioPlayableOutput.Create(_graph, "AudioOut", GetComponent<AudioSource>());

            // 2. Create Mixer
            _mixer = AudioMixerPlayable.Create(_graph, 2);
            audioOutput.SetSourcePlayable(_mixer);

            // 3. Create Inputs
            // looped = true for continuous testing
            var playableA = AudioClipPlayable.Create(_graph, clipA, true);
            var playableB = AudioClipPlayable.Create(_graph, clipB, true);

            _graph.Connect(playableA, 0, _mixer, 0);
            _graph.Connect(playableB, 0, _mixer, 1);

            _graph.Play();
            Debug.Log("<color=cyan><b>[Day 08]</b> Audio Graph Playing.</color>");
        }

        private void Update()
        {
            if (!_graph.IsValid()) return;

            // Audio blending is linear
            _mixer.SetInputWeight(0, 1f - mix);
            _mixer.SetInputWeight(1, mix);
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
