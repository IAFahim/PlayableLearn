using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day09.Runtime
{
    /// <summary>
    /// Day 09: Custom Logic (Light Mixer).
    /// Demonstrates using Playables to drive non-animation properties.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class Day09LightBlender : MonoBehaviour
    {
        [Header("State A")]
        public Color colorA = Color.red;
        public float intensityA = 1f;

        [Header("State B")]
        public Color colorB = Color.blue;
        public float intensityB = 2f;

        [Header("Mix")]
        [Range(0f, 1f)]
        public float mix = 0.0f;

        private PlayableGraph _graph;
        private ScriptPlayable<Day09LightMixerBehaviour> _mixer;

        private void Start()
        {
            _graph = PlayableGraph.Create("Day09_LightMixer");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            var lightOutput = ScriptPlayableOutput.Create(_graph, "LightOutput");
            lightOutput.SetUserData(GetComponent<Light>());

            _mixer = ScriptPlayable<Day09LightMixerBehaviour>.Create(_graph, 2);
            lightOutput.SetSourcePlayable(_mixer);

            var playableA = ScriptPlayable<Day09LightControlBehaviour>.Create(_graph);
            playableA.GetBehaviour().color = colorA;
            playableA.GetBehaviour().intensity = intensityA;

            var playableB = ScriptPlayable<Day09LightControlBehaviour>.Create(_graph);
            playableB.GetBehaviour().color = colorB;
            playableB.GetBehaviour().intensity = intensityB;

            _graph.Connect(playableA, 0, _mixer, 0);
            _graph.Connect(playableB, 0, _mixer, 1);

            _graph.Play();
            Debug.Log("<color=cyan><b>[Day 09]</b> Light Mixer Active.</color>");
        }

        private void Update()
        {
            if (!_graph.IsValid()) return;

            var inputA = (ScriptPlayable<Day09LightControlBehaviour>)_mixer.GetInput(0);
            inputA.GetBehaviour().color = colorA;
            inputA.GetBehaviour().intensity = intensityA;

            var inputB = (ScriptPlayable<Day09LightControlBehaviour>)_mixer.GetInput(1);
            inputB.GetBehaviour().color = colorB;
            inputB.GetBehaviour().intensity = intensityB;

            _mixer.SetInputWeight(0, 1f - mix);
            _mixer.SetInputWeight(1, mix);
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
