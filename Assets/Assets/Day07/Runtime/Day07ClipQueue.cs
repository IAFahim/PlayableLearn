using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AV.Day07.Runtime
{
    /// <summary>
    /// Day 07: The Sequencer.
    /// Demonstrates how to chain animations together dynamically.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Day07ClipQueue : MonoBehaviour
    {
        public List<AnimationClip> clips;
        public float fadeDuration = 1.0f;
        public bool loopQueue = true;

        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixer;
        
        private int _currentClipIndex = 0;
        private bool _isFading;
        private int _inputPortCurrent = 0; 
        private int _inputPortNext = 1;

        private void Start()
        {
            if (clips == null || clips.Count == 0) return;

            _graph = PlayableGraph.Create("Day07_Queue");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            var output = AnimationPlayableOutput.Create(_graph, "Output", GetComponent<Animator>());
            _mixer = AnimationMixerPlayable.Create(_graph, 2);
            output.SetSourcePlayable(_mixer);

            PlayClipImmediately(clips[_currentClipIndex]);

            _graph.Play();
            Debug.Log("<color=cyan><b>[Day 07]</b> Queue Started.</color>");
        }

        private void Update()
        {
            if (!_graph.IsValid() || clips.Count == 0) return;

            var currentPlayable = _mixer.GetInput(_inputPortCurrent);
            if (!currentPlayable.IsValid()) return;

            double duration = currentPlayable.GetDuration();
            double time = currentPlayable.GetTime();
            double remaining = duration - time;

            if (!_isFading && remaining <= fadeDuration)
            {
                int nextIndex = _currentClipIndex + 1;
                if (nextIndex >= clips.Count)
                {
                    if (loopQueue) nextIndex = 0;
                    else return;
                }
                
                StartFade(clips[nextIndex]);
            }

            if (_isFading)
            {
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
            if (_mixer.GetInput(_inputPortCurrent).IsValid())
                _graph.DestroySubgraph(_mixer.GetInput(_inputPortCurrent));

            var clipPlayable = AnimationClipPlayable.Create(_graph, clip);
            _graph.Connect(clipPlayable, 0, _mixer, _inputPortCurrent);
            
            _mixer.SetInputWeight(_inputPortCurrent, 1f);
            _mixer.SetInputWeight(_inputPortNext, 0f);
            
            _isFading = false;
        }

        private void StartFade(AnimationClip nextClip)
        {
            _isFading = true;
            _currentClipIndex = (clips.IndexOf(nextClip)); 

            if (_mixer.GetInput(_inputPortNext).IsValid())
                _graph.DestroySubgraph(_mixer.GetInput(_inputPortNext));

            var nextPlayable = AnimationClipPlayable.Create(_graph, nextClip);
            _graph.Connect(nextPlayable, 0, _mixer, _inputPortNext);
            
            nextPlayable.SetTime(0);
        }

        private void FinishFade()
        {
            _isFading = false;

            var oldCurrent = _mixer.GetInput(_inputPortCurrent);
            if (oldCurrent.IsValid()) _graph.DestroySubgraph(oldCurrent);

            int temp = _inputPortCurrent;
            _inputPortCurrent = _inputPortNext;
            _inputPortNext = temp;

            _mixer.SetInputWeight(_inputPortCurrent, 1f);
            _mixer.SetInputWeight(_inputPortNext, 0f);
        }

        private void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}
