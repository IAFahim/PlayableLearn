using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day04.Runtime
{
    public interface IPreviewSystem
    {
        void Initialize(ClipConfigState initialState);
        bool TryGatherProperties(PlayableDirector director, IPropertyCollector driver);
        bool TryProcessFrame(Playable playable, FrameData info, object playerData);
    }

    public class Day04PreviewBehaviour : PlayableBehaviour, IPropertyPreview, IPreviewSystem
    {
        private PreviewState _state;

        void IPreviewSystem.Initialize(ClipConfigState initialState)
        {
            _state = PreviewState.Create(initialState);
        }

        bool IPreviewSystem.TryGatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            return _state.TryGatherProperties(director, driver, out _);
        }

        bool IPreviewSystem.TryProcessFrame(Playable playable, FrameData info, object playerData)
        {
            return _state.TryProcessFrame(playable, info, playerData, out _);
        }

        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            ((IPreviewSystem)this).TryGatherProperties(director, driver);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            ((IPreviewSystem)this).TryProcessFrame(playable, info, playerData);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct PreviewState
    {
        public float TargetX;
        public float TargetY;
        public float TargetZ;
        public float OriginalX;
        public float OriginalY;
        public float OriginalZ;
        public bool HasOriginalPosition;

        public override string ToString()
        {
            return $"[Preview] Target:({TargetX:F1},{TargetY:F1},{TargetZ:F1}) | HasOriginal: {HasOriginalPosition}";
            // Debug view
        }

        public static PreviewState Create(ClipConfigState config)
        {
            return new PreviewState
            {
                TargetX = config.PositionX,
                TargetY = config.PositionY,
                TargetZ = config.PositionZ,
                HasOriginalPosition = false
            };
        }
    }

    public static class PreviewLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBoundGameObject(PlayableDirector director, out GameObject obj)
        {
            obj = null; // Default: No binding found

            var graph = director.playableGraph;
            if (graph.GetOutputCount() == 0) return; // Guard: No outputs

            var output = graph.GetOutput(0);
            var sourceObject = output.GetReferenceObject();

            if (sourceObject != null) obj = director.GetGenericBinding(sourceObject) as GameObject; // Atomic fetch
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateGameObject(GameObject obj, out bool isValid)
        {
            isValid = obj != null; // Atomic validation
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreOriginalPosition(Transform transform, out float x, out float y, out float z)
        {
            x = transform.localPosition.x;
            y = transform.localPosition.y;
            z = transform.localPosition.z; // Atomic extraction
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateLerp(float currentX, float currentY, float currentZ, float targetX, float targetY,
            float targetZ, float t, out Vector3 result)
        {
            var current = new Vector3(currentX, currentY, currentZ);
            var target = new Vector3(targetX, targetY, targetZ);
            result = Vector3.Lerp(current, target, t); // Atomic calculation
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateWeight(float weight, out bool isValid)
        {
            isValid = weight > 0f; // Atomic validation
        }
    }

    public static class PreviewExtensions
    {
        public static bool TryGatherProperties(ref this PreviewState state, PlayableDirector director,
            IPropertyCollector driver, out Transform boundTransform)
        {
            boundTransform = null;

            PreviewLogic.GetBoundGameObject(director, out var obj);
            PreviewLogic.ValidateGameObject(obj, out var isValid);
            if (!isValid) return false; // Guard: No object

            boundTransform = obj.transform;

            PreviewLogic.StoreOriginalPosition(boundTransform, out state.OriginalX, out state.OriginalY,
                out state.OriginalZ);
            state.HasOriginalPosition = true;

            driver.AddFromName<Transform>(obj, "m_LocalPosition");

            return true; // Success
        }

        public static bool TryProcessFrame(ref this PreviewState state, Playable playable, FrameData info,
            object playerData, out Vector3 newPosition)
        {
            newPosition = Vector3.zero;

            var boundObj = playerData as GameObject;
            PreviewLogic.ValidateGameObject(boundObj, out var isValid);
            if (!isValid) return false; // Guard: No object

            var weight = info.weight;
            PreviewLogic.ValidateWeight(weight, out var weightValid);
            if (!weightValid) return false; // Guard: Invalid weight

            var transform = boundObj.transform;
            PreviewLogic.StoreOriginalPosition(transform, out var currentX, out var currentY, out var currentZ);

            PreviewLogic.CalculateLerp(currentX, currentY, currentZ, state.TargetX, state.TargetY, state.TargetZ,
                weight, out newPosition);

            transform.localPosition = newPosition;

            return true; // Success
        }
    }
}