using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day04.Runtime
{
    public interface INotificationSystem
    {
        bool TryGetNotificationId(out PropertyName id);
        bool TryValidateNotification(out bool isValid);
    }

    [CustomStyle("BookmarkRed")]
    [Serializable]
    public class Day04NotificationMarker : Marker, INotification, INotificationSystem
    {
        [SerializeField] public string eventName = "Event";
        [SerializeField] public Color flashColor = Color.white;
        [SerializeField] public float intensity = 1.0f;

        private NotificationState State => new()
        {
            EventName = eventName,
            R = flashColor.r,
            G = flashColor.g,
            B = flashColor.b,
            A = flashColor.a,
            Intensity = intensity
        };

        public PropertyName id => ((INotificationSystem)this).TryGetNotificationId(out var notificationId)
            ? notificationId
            : default;

        bool INotificationSystem.TryGetNotificationId(out PropertyName notificationId)
        {
            notificationId = default;

            var state = State;
            if (!state.TryValidateNotification(out var isValid)) return false; // Guard: Invalid

            state.TryGetNotificationId(out notificationId);
            return true; // Success
        }

        bool INotificationSystem.TryValidateNotification(out bool isValid)
        {
            return State.TryValidateNotification(out isValid);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct NotificationState
    {
        public string EventName;
        public float R;
        public float G;
        public float B;
        public float A;
        public float Intensity;

        public override string ToString()
        {
            return $"[Notification] {EventName} ({Intensity:F1})";
            // Debug view
        }
    }

    public static class NotificationLogic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateEventName(string name, out bool isValid)
        {
            isValid = !string.IsNullOrEmpty(name); // Atomic validation
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateNotificationId(string name, out PropertyName id)
        {
            id = new PropertyName(name); // Atomic creation
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetColorValues(Color color, out float r, out float g, out float b, out float a)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a; // Atomic extraction
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetColorValues(float r, float g, float b, float a, out Color color)
        {
            color = new Color(r, g, b, a); // Atomic composition
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatIntensity(float intensity, out string formatted)
        {
            formatted = $"{intensity:F2}"; // Atomic format
        }
    }

    public static class NotificationExtensions
    {
        public static bool TryValidateNotification(this NotificationState state, out bool isValid)
        {
            isValid = false;

            NotificationLogic.ValidateEventName(state.EventName, out isValid);

            if (state.Intensity < 0) isValid = false; // Guard: Negative intensity

            return isValid; // Success
        }

        public static bool TryGetNotificationId(this NotificationState state, out PropertyName id)
        {
            id = default;

            if (!state.TryValidateNotification(out var isValid)) return false; // Guard: Invalid

            NotificationLogic.CreateNotificationId(state.EventName, out id);

            return true; // Success
        }

        public static bool TryGetColor(this NotificationState state, out Color color)
        {
            color = Color.white;

            NotificationLogic.SetColorValues(state.R, state.G, state.B, state.A, out color);

            return true; // Success
        }

        public static bool TrySetColor(ref this NotificationState state, Color color, out Color previousColor)
        {
            previousColor = Color.white;

            state.TryGetColor(out previousColor);

            NotificationLogic.GetColorValues(color, out state.R, out state.G, out state.B, out state.A);

            return true; // Success
        }
    }
}