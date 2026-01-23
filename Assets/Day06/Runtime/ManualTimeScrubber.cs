using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day06.Runtime
{
    /// <summary>
    /// Day 06: Masters of Time.
    /// Demonstrates "Manual" update mode. We take full responsibility for advancing time.
    /// This allows for scrubbing, pausing, and reverse playback.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class ManualTimeScrubber : MonoBehaviour
    {
        public AnimationClip clip;

        [Header("Time Control")]
        [Range(0f, 1f)]
        public float normalizedTime = 0f;
        
        public bool autoPlay = true;
        public float playSpeed = 1f;

        private PlayableGraph _graph;
        private AnimationClipPlayable _clipPlayable;

        private void Start()
        {
            if (clip == null) return;

            // 1. Create Graph in Manual Mode
            _graph = PlayableGraph.Create("Day06_TimeScrubber");
            
            // MANUAL MODE: The graph will NOT update time automatically.
            // calling _graph.Evaluate() advances the logic, but we can set time arbitrarily.
            _graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            var output = AnimationPlayableOutput.Create(_graph, "AnimOutput", GetComponent<Animator>());
            _clipPlayable = AnimationClipPlayable.Create(_graph, clip);
            output.SetSourcePlayable(_clipPlayable);

            _graph.Play(); // Start "Running" state, but time won't move until we say so.
            Debug.Log("<color=cyan><b>[Day 06]</b> Manual Time Control Active.</color>");
        }

        private void Update()
        {
            if (!_graph.IsValid()) return;

            double duration = _clipPlayable.GetAnimationClip().length;

            if (autoPlay)
            {
                // Logic: Advance our internal time counter
                // We use normalizedTime as our "Storage" for the current time cursor.
                float increment = (Time.deltaTime * playSpeed) / (float)duration;
                normalizedTime = Mathf.Repeat(normalizedTime + increment, 1f);
            }

            // 2. Apply Time
            // Convert 0-1 range back to seconds
            double targetTime = normalizedTime * duration;
            
            // Set the time on the playable (The Clip)
            _clipPlayable.SetTime(targetTime);

            // 3. Evaluate
            // Push the changes through the graph.
            // In Manual mode, Evaluate() updates the graph state based on the Playable settings.
            _graph.Evaluate();
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
