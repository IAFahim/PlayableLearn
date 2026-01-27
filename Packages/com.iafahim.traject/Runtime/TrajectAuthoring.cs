using AV.Traject.Runtime.Core;
using AV.Traject.Runtime.Extensions;
using AV.Traject.Runtime.Integration;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace AV.Traject.Integration
{
    /// <summary>
    /// A thin "Driver" component that bridges the gap between Unity GameObjects
    /// and the stateless Trajectory system.
    /// </summary>
    [AddComponentMenu("AV/Traject/Traject Authoring")]
    public class TrajectAuthoring : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The trajectory shape asset.")]
        [SerializeField] protected TrajectAsset asset;

        [Tooltip("The range/distance of the trajectory in world units.")]
        [SerializeField] [Range(0f, 100f)] protected float range = 10f;

        [Tooltip("The core runtime state. Edits here affect behavior directly.")]
        [SerializeField] protected TrajectState state = TrajectState.Default;

        [Header("Events")]
        public UnityEvent OnComplete;
        public UnityEvent OnLoop;

        private TrajectBasis _startBasis;
        private bool _isInitialized = false;

        public TrajectAsset Asset => asset;
        public float NormalizedTime => state.GetNormalizedProgress();
        public bool IsPlaying => state.IsPlaying();
        public float TimeScale { get => state.PlaybackSpeedMultiplier; set => state.PlaybackSpeedMultiplier = value; }

        private void OnEnable()
        {
            ResetOrigin();
        }

        private void OnValidate()
        {
            if (state.PlaybackTimer.Duration < 0f) state.PlaybackTimer.Duration = 0f;
        }

        private void Update()
        {
            if (asset == null) return;

            bool boundaryHit = state.Tick(UnityEngine.Time.deltaTime, out float t);

            if (boundaryHit)
            {
                if (state.IsLooping()) OnLoop?.Invoke();
                else OnComplete?.Invoke();
            }

            if (state.IsPlaying())
            {
                ApplyPosition(t);
            }
        }

        public void ResetOrigin()
        {
            _startBasis = new TrajectBasis(transform.position, transform.forward, transform.right, transform.up);
            _isInitialized = true;
        }

        private void ApplyPosition(float t)
        {
            if (asset == null) return;

            TrajectBasis basis = GetBasis();

            asset.Evaluate(in basis, range, t, out float3 pos);
            transform.position = pos;
        }

        private TrajectBasis GetBasis()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return new TrajectBasis(transform.position, transform.forward, transform.right, transform.up);
            }
#endif
            if (!_isInitialized) ResetOrigin();

            return _startBasis;
        }

        public void Play()
        {
            state.Play();
        }

        public void Pause() => state.Pause();

        public void Stop()
        {
            state.Stop();
            ApplyPosition(0f);
        }

        public void Rewind() => state.Rewind();

        public float3 GetCurrentPosition()
        {
            if (asset == null) return transform.position;

            float t = state.GetNormalizedProgress();
            TrajectBasis basis = GetBasis();

            asset.Evaluate(in basis, range, t, out float3 pos);
            return pos;
        }

#if UNITY_EDITOR
        public TrajectState GetState() => state;
        public float GetRange() => range;
        public TrajectBasis GetEditorBasis() => GetBasis();
#endif
    }
}