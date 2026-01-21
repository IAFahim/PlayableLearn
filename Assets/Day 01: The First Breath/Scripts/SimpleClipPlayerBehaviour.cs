using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Layer B: Core Logic.
    /// A minimal PlayableBehaviour that tracks clip playback state.
    /// In this first lesson, we demonstrate the behaviour exists within the graph traversal.
    /// </summary>
    public sealed class SimpleClipPlayerBehaviour : PlayableBehaviour
    {
        private float _elapsedTime;
        private bool _isPlaying;

        public void Play()
        {
            _isPlaying = true;
            _elapsedTime = 0f;
        }

        public void Pause()
        {
            _isPlaying = false;
        }

        public void Stop()
        {
            _isPlaying = false;
            _elapsedTime = 0f;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // This runs INSIDE the graph evaluation loop
            // We track elapsed time for debug/monitoring purposes
            if (_isPlaying)
            {
                _elapsedTime += info.deltaTime;
            }
        }

        public float GetElapsedTime() => _elapsedTime;
        public bool IsPlaying() => _isPlaying;
    }
}
