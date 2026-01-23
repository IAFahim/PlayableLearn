using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace AV.Day08.Runtime
{
    /// <summary>
    /// Day 08: The Sound of Data.
    /// Demonstrates mixing Audio using the Playables API.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Day08AudioMixer : MonoBehaviour
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

            var audioOutput = AudioPlayableOutput.Create(_graph, "AudioOut", GetComponent<AudioSource>());

            _mixer = AudioMixerPlayable.Create(_graph, 2);
            audioOutput.SetSourcePlayable(_mixer);

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

            _mixer.SetInputWeight(0, 1f - mix);
            _mixer.SetInputWeight(1, mix);
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
