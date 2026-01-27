using System;
using UnityEngine;
using UnityEngine.Playables;
using AV.Traject.Runtime.Core;
using AV.Traject.Runtime.Integration;
using Unity.Mathematics;

namespace AV.Traject.Runtime.Timeline
{
    [Serializable]
    public class TrajectTimelineBehaviour : PlayableBehaviour
    {
        [SerializeField] private TrajectAsset asset;
        [SerializeField] private float range = 10f;
        [SerializeField] private bool useCachedOrigin = true;

        private TrajectBasis _cachedBasis;
        private bool _isInitialized;

        public TrajectAsset Asset => asset;
        public float Range => range;
        public bool UseCachedOrigin => useCachedOrigin;

        public void Initialize(Transform transform)
        {
            if (transform == null) return;
            if (_isInitialized) return;

            _cachedBasis = new TrajectBasis(
                transform.position,
                transform.forward,
                transform.right,
                transform.up
            );
            _isInitialized = true;
        }

        public override void OnGraphStop(Playable playable)
        {
            _isInitialized = false;
        }

        public void Evaluate(Transform transform, float progress, out float3 position)
        {
            position = float3.zero;

            if (asset == null || transform == null) return;

            var basis = useCachedOrigin ? _cachedBasis : GetLiveBasis(transform);
            asset.Evaluate(in basis, range, progress, out position);
        }

        private TrajectBasis GetLiveBasis(Transform transform)
        {
            return new TrajectBasis(
                transform.position,
                transform.forward,
                transform.right,
                transform.up
            );
        }
    }
}
