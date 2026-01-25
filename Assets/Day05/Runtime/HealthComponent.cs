using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// LAYER A: DATA
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct HealthState
{
    public float Value;
    public float MaxValue;

    public override string ToString() => $"[Health] {Value:F1}/{MaxValue:F0}"; // Debug view
}

// LAYER B: LOGIC
public static class HealthLogic
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Lerp(float a, float b, float t, out float result)
    {
        result = a + (b - a) * t; // Atomic interpolation
    }
}

// LAYER C: EXTENSIONS
public static class HealthExtensions
{
    public static bool TrySet(ref this HealthState state, float value, out float result)
    {
        result = value; // Default

        if (value < 0) result = 0; // Clamp min
        else if (value > state.MaxValue) result = state.MaxValue; // Clamp max

        state.Value = result; // Apply
        return true; // Success
    }
}

// LAYER D: INTERFACE & BRIDGE
public interface IHealthSystem
{
    bool TrySetHealth(float value);
}

public class HealthComponent : MonoBehaviour, IHealthSystem
{
    [SerializeField] private HealthState _state = new HealthState { Value = 100, MaxValue = 100 };

    bool IHealthSystem.TrySetHealth(float value)
    {
        return _state.TrySet(value, out _); // Proxy
    }

    // Explicit binding for Timeline verification
    public void DebugState() => Debug.Log(_state); // Side check
}

// LAYER T: TIMELINE (The Master Implementation)

// 1. The Data (Clip)

// 2. The Logic (Behaviour)
public class HealthBehaviour : PlayableBehaviour
{
    public float StartHealth;
    public float EndHealth;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var system = playerData as IHealthSystem; // Safe cast
        if (system == null) return; // Fail fast

        var time = (float)(playable.GetTime() / playable.GetDuration()); // Normalized time
        
        HealthLogic.Lerp(StartHealth, EndHealth, time, out var result); // Atomic calc
        system.TrySetHealth(result); // Apply to bridge
    }
}

// 3. The Binding (Track)
[TrackColor(0.8f, 0.2f, 0.2f)]
[TrackClipType(typeof(HealthClip))]
[TrackBindingType(typeof(HealthComponent))]
public class HealthTrack : TrackAsset { } // Binds Component to Timeline