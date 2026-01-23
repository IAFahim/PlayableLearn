using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day06.Runtime
{
    /// <summary>
    /// Day 06: Masters of Time.
    /// Demonstrates "Manual" update mode. We take full responsibility for advancing time.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Day06TimeScrubber : MonoBehaviour
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

            _graph = PlayableGraph.Create("Day06_TimeScrubber");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            var output = AnimationPlayableOutput.Create(_graph, "AnimOutput", GetComponent<Animator>());
            _clipPlayable = AnimationClipPlayable.Create(_graph, clip);
            output.SetSourcePlayable(_clipPlayable);

            _graph.Play(); 
            Debug.Log("<color=cyan><b>[Day 06]</b> Manual Time Control Active.</color>");
        }

        private void Update()
        {
            if (!_graph.IsValid()) return;

            double duration = _clipPlayable.GetAnimationClip().length;

            if (autoPlay)
            {
                float increment = (Time.deltaTime * playSpeed) / (float)duration;
                normalizedTime = Mathf.Repeat(normalizedTime + increment, 1f);
            }

            double targetTime = normalizedTime * duration;
            _clipPlayable.SetTime(targetTime);

            _graph.Evaluate();
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
