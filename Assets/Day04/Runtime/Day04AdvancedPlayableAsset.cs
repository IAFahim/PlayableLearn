using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day04.Runtime
{
    public interface IClipConfigSystem
    {
        bool TryGetClipCaps(out ClipCaps caps);
    }

    [Serializable]
    public class Day04AdvancedPlayableAsset : PlayableAsset, ITimelineClipAsset, IClipConfigSystem
    {
        [SerializeField] private ClipConfigState _state;

        bool IClipConfigSystem.TryGetClipCaps(out ClipCaps caps)
        {
            return _state.TryGetClipCaps(out caps);
        }

        public ClipCaps clipCaps => ((IClipConfigSystem)this).TryGetClipCaps(out var caps) ? caps : ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<Day04PreviewBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            ((IPreviewSystem)behaviour).Initialize(_state);
            return playable;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ClipConfigState
    {
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float ColorR;
        public float ColorG;
        public float ColorB;
        public float ColorA;
        public ClipCapabilitiesFlags CapsFlags;

        public override string ToString()
        {
            return $"[ClipConfig] Pos:({PositionX:F1},{PositionY:F1},{PositionZ:F1}) | Caps: {CapsFlags}";
            // Debug view
        }

        [Flags]
        public enum ClipCapabilitiesFlags
        {
            None = 0,
            Blending = 1 << 0,
            Extrapolation = 1 << 1,
            SpeedMultiplier = 1 << 2,
            All = Blending | Extrapolation | SpeedMultiplier
        }
    }

    public static class ClipConfigLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FlagsToCaps(ClipConfigState.ClipCapabilitiesFlags flags, out ClipCaps caps)
        {
            caps = ClipCaps.None;

            if ((flags & ClipConfigState.ClipCapabilitiesFlags.Blending) != 0)
                caps |= ClipCaps.Blending;

            if ((flags & ClipConfigState.ClipCapabilitiesFlags.Extrapolation) != 0)
                caps |= ClipCaps.Extrapolation;

            if ((flags & ClipConfigState.ClipCapabilitiesFlags.SpeedMultiplier) != 0)
                caps |= ClipCaps.SpeedMultiplier; // Atomic conversion
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExtractPosition(Vector3 position, out float x, out float y, out float z)
        {
            x = position.x;
            y = position.y;
            z = position.z; // Atomic extraction
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ComposePosition(float x, float y, float z, out Vector3 position)
        {
            position = new Vector3(x, y, z); // Atomic composition
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExtractColor(Color color, out float r, out float g, out float b, out float a)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a; // Atomic extraction
        }
    }

    public static class ClipConfigExtensions
    {
        public static bool TryGetClipCaps(this ClipConfigState state, out ClipCaps caps)
        {
            caps = ClipCaps.None;

            ClipConfigLogic.FlagsToCaps(state.CapsFlags, out caps);

            return true; // Success
        }

        public static bool TrySetPosition(ref this ClipConfigState state, Vector3 position, out Vector3 previous)
        {
            previous = Vector3.zero;

            ClipConfigLogic.ExtractPosition(position, out state.PositionX, out state.PositionY, out state.PositionZ);

            return true; // Success
        }

        public static bool TryGetPosition(this ClipConfigState state, out Vector3 position)
        {
            position = Vector3.zero;

            ClipConfigLogic.ComposePosition(state.PositionX, state.PositionY, state.PositionZ, out position);

            return true; // Success
        }

        public static bool TrySetColor(ref this ClipConfigState state, Color color, out Color previous)
        {
            previous = Color.white;

            ClipConfigLogic.ExtractColor(color, out state.ColorR, out state.ColorG, out state.ColorB, out state.ColorA);

            return true; // Success
        }

        public static bool TryGetColor(this ClipConfigState state, out Color color)
        {
            color = Color.white;

            color = new Color(state.ColorR, state.ColorG, state.ColorB, state.ColorA);

            return true; // Success
        }
    }
}