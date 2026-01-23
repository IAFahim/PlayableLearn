using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day09.Runtime
{
    /// <summary>
    /// Day 09: Custom Logic (Light Mixer).
    /// Demonstrates using Playables to drive non-animation properties.
    /// We blend two "Light States" (Color/Intensity) using a custom Mixer Behaviour.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class LightBlender : MonoBehaviour
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
        private ScriptPlayable<LightMixerBehaviour> _mixer;

        private void Start()
        {
            _graph = PlayableGraph.Create("Day09_LightMixer");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // 1. Create Output
            // ScriptPlayableOutput is the generic output for custom scripts.
            var lightOutput = ScriptPlayableOutput.Create(_graph, "LightOutput");
            
            // Set the "User Data" (The Component we want to drive)
            lightOutput.SetUserData(GetComponent<Light>());

            // 2. Create Mixer
            _mixer = ScriptPlayable<LightMixerBehaviour>.Create(_graph, 2);
            lightOutput.SetSourcePlayable(_mixer);

            // 3. Create Inputs (The Data)
            var playableA = ScriptPlayable<LightControlBehaviour>.Create(_graph);
            playableA.GetBehaviour().color = colorA;
            playableA.GetBehaviour().intensity = intensityA;

            var playableB = ScriptPlayable<LightControlBehaviour>.Create(_graph);
            playableB.GetBehaviour().color = colorB;
            playableB.GetBehaviour().intensity = intensityB;

            // 4. Connect
            _graph.Connect(playableA, 0, _mixer, 0);
            _graph.Connect(playableB, 0, _mixer, 1);

            _graph.Play();
            Debug.Log("<color=cyan><b>[Day 09]</b> Light Mixer Active.</color>");
        }

        private void Update()
        {
            if (!_graph.IsValid()) return;

            // Sync inspector values to behaviour (for realtime editing)
            var inputA = (ScriptPlayable<LightControlBehaviour>)_mixer.GetInput(0);
            inputA.GetBehaviour().color = colorA;
            inputA.GetBehaviour().intensity = intensityA;

            var inputB = (ScriptPlayable<LightControlBehaviour>)_mixer.GetInput(1);
            inputB.GetBehaviour().color = colorB;
            inputB.GetBehaviour().intensity = intensityB;

            // Blend
            _mixer.SetInputWeight(0, 1f - mix);
            _mixer.SetInputWeight(1, mix);
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
