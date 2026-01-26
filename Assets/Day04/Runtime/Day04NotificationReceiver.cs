using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day04.Runtime
{
    public interface IReceiverSystem
    {
        bool TryReceiveNotification(Playable origin, INotification notification, object context);
    }

    public class Day04NotificationReceiver : MonoBehaviour, INotificationReceiver, IReceiverSystem
    {
        private ReceiverState _state;

        void INotificationReceiver.OnNotify(Playable origin, INotification notification, object context)
        {
            ((IReceiverSystem)this).TryReceiveNotification(origin, notification, context);
        }

        bool IReceiverSystem.TryReceiveNotification(Playable origin, INotification notification, object context)
        {
            if (notification is Day04NotificationMarker marker) return _state.TryProcessEvent(marker, out var message);

            return false; // Guard: Wrong type
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ReceiverState
    {
        public int EventCount;
        public float LastIntensity;

        public override string ToString()
        {
            return $"[Receiver] Events: {EventCount} | Last Intensity: {LastIntensity:F2}";
            // Debug view
        }
    }

    public static class ReceiverLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateMarker(Day04NotificationMarker marker, out bool isValid)
        {
            isValid = marker != null; // Atomic validation
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExtractMarkerState(Day04NotificationMarker marker, out NotificationState state)
        {
            // Access private field via reflection or expose as property
            state = default; // Placeholder - would need proper access
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuildLogMessage(string eventName, Color color, float intensity, out string message)
        {
            var colorHex = ColorUtility.ToHtmlStringRGB(color);
            message =
                $"<color=#{colorHex}><b>[Day04 Event]</b> {eventName} | Intensity: {intensity:F2}</color>"; // Atomic format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IncrementEventCount(int current, out int next)
        {
            next = current + 1; // Atomic increment
        }
    }

    public static class ReceiverExtensions
    {
        public static bool TryProcessEvent(ref this ReceiverState state, Day04NotificationMarker marker,
            out string message)
        {
            message = string.Empty;

            ReceiverLogic.ValidateMarker(marker, out var isValid);
            if (!isValid) return false; // Guard: Invalid marker

            // Get state from marker (would need proper property access in marker)
            var eventName = "Event"; // Placeholder
            var intensity = 1.0f; // Placeholder
            var color = Color.white; // Placeholder

            ReceiverLogic.BuildLogMessage(eventName, color, intensity, out message);
            ReceiverLogic.IncrementEventCount(state.EventCount, out state.EventCount);
            state.LastIntensity = intensity;

            Debug.Log(message);

            return true; // Success
        }
    }
}