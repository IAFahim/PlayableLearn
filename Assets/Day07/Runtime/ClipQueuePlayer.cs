using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day07.Runtime
{
    /// <summary>
    /// Day 07: The Sequencer.
    /// Demonstrates how to chain animations together dynamically.
    /// Uses an AnimationMixerPlayable to crossfade between the "Current" and "Next" clip.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class ClipQueuePlayer : MonoBehaviour
    {
        public List<AnimationClip> clips;
        public float fadeDuration = 1.0f;
        public bool loopQueue = true;

        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixer;
        
        // State
        private int _currentClipIndex = 0;
        private double _timeToNextFade;
        private bool _isFading;
        private int _inputPortCurrent = 0; // We toggle between port 0 and 1
        private int _inputPortNext = 1;

        private void Start()
        {
            if (clips == null || clips.Count == 0) return;

            _graph = PlayableGraph.Create("Day07_Queue");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            var output = AnimationPlayableOutput.Create(_graph, "Output", GetComponent<Animator>());
            _mixer = AnimationMixerPlayable.Create(_graph, 2);
            output.SetSourcePlayable(_mixer);

            // Start the first clip
            PlayClipImmediately(clips[_currentClipIndex]);

            _graph.Play();
            Debug.Log("<color=cyan><b>[Day 07]</b> Queue Started.</color>");
        }

        private void Update()
        {
            if (!_graph.IsValid() || clips.Count == 0) return;

            // Get the playable currently active
            var currentPlayable = _mixer.GetInput(_inputPortCurrent);
            if (!currentPlayable.IsValid()) return;

            // Check if we need to start fading
            double duration = currentPlayable.GetDuration();
            double time = currentPlayable.GetTime();
            double remaining = duration - time;

            if (!_isFading && remaining <= fadeDuration)
            {
                // Trigger Fade
                int nextIndex = _currentClipIndex + 1;
                if (nextIndex >= clips.Count)
                {
                    if (loopQueue) nextIndex = 0;
                    else return; // Stop at end
                }
                
                StartFade(clips[nextIndex]);
            }

            // Handle Fade Logic
            if (_isFading)
            {
                // The fade logic could be calculated based on remaining time
                // But for simplicity, we'll let the mixer weights be driven by the "remaining" time ratio
                // Normalized fade progress: 1.0 (start of fade) -> 0.0 (end of clip)
                
                // However, Playables don't automatically stop/disconnect when done.
                // We need to manage the weights manually here.
                
                float weightNext = 1f - Mathf.Clamp01((float)(remaining / fadeDuration));
                float weightCurrent = 1f - weightNext;

                _mixer.SetInputWeight(_inputPortCurrent, weightCurrent);
                _mixer.SetInputWeight(_inputPortNext, weightNext);

                if (remaining <= 0)
                {
                    FinishFade();
                }
            }
        }

        private void PlayClipImmediately(AnimationClip clip)
        {
            // Clean up old inputs
            if (_mixer.GetInput(_inputPortCurrent).IsValid())
                _graph.DestroySubgraph(_mixer.GetInput(_inputPortCurrent));

            // Create new
            var clipPlayable = AnimationClipPlayable.Create(_graph, clip);
            _graph.Connect(clipPlayable, 0, _mixer, _inputPortCurrent);
            
            _mixer.SetInputWeight(_inputPortCurrent, 1f);
            _mixer.SetInputWeight(_inputPortNext, 0f);
            
            _isFading = false;
        }

        private void StartFade(AnimationClip nextClip)
        {
            _isFading = true;
            _currentClipIndex = (clips.IndexOf(nextClip)); // Update index logic

            // Setup Next Port
            if (_mixer.GetInput(_inputPortNext).IsValid())
                _graph.DestroySubgraph(_mixer.GetInput(_inputPortNext));

            var nextPlayable = AnimationClipPlayable.Create(_graph, nextClip);
            _graph.Connect(nextPlayable, 0, _mixer, _inputPortNext);
            
            // Ensure time is 0
            nextPlayable.SetTime(0);
        }

        private void FinishFade()
        {
            _isFading = false;

            // Swap Ports logic: Next becomes Current
            // We disconnect the old current, and swap our index tracking
            var oldCurrent = _mixer.GetInput(_inputPortCurrent);
            if (oldCurrent.IsValid()) _graph.DestroySubgraph(oldCurrent);

            // Swap indices
            int temp = _inputPortCurrent;
            _inputPortCurrent = _inputPortNext;
            _inputPortNext = temp;

            // Finalize weights
            _mixer.SetInputWeight(_inputPortCurrent, 1f);
            _mixer.SetInputWeight(_inputPortNext, 0f);
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
