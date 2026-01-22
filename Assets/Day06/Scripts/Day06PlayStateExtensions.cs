using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day06
{
    /// <summary>
    /// Extension methods for Day06PlayStateData.
    /// Layer C: Extensions - High-level adapter methods that combine operations.
    /// </summary>
    public static class Day06PlayStateExtensions
    {
        /// <summary>
        /// Initializes PlayState control for the given graph.
        /// </summary>
        public static void Initialize(ref this Day06PlayStateData data, in PlayableGraph graph, string controllerName, bool autoPlayOnStart = true)
        {
            if (data.IsActive)
            {
                Debug.LogWarning($"[PlayStateControl] Already initialized: {controllerName}");
                return;
            }

            if (!graph.IsValid())
            {
                Debug.LogError($"[PlayStateControl] Cannot initialize: Graph is invalid.");
                return;
            }

            data.Graph = graph;
            data.IsActive = true;
            data.ControllerId = controllerName.GetHashCode();
            data.IsGraphValid = true;
            data.AutoPlayOnStart = autoPlayOnStart;

            // Get current state
            data.CurrentState = graph.IsPlaying() ? PlayState.Playing : PlayState.Paused;
            data.PreviousState = data.CurrentState;

            // Auto-play if requested
            if (autoPlayOnStart && PlayStateOps.IsPaused(data.CurrentState))
            {
                data.Play();
            }

            Debug.Log($"[PlayStateControl] Initialized: {controllerName}, AutoPlay: {autoPlayOnStart}, State: {data.CurrentState}");
        }

        /// <summary>
        /// Disposes the PlayState control.
        /// </summary>
        public static void Dispose(ref this Day06PlayStateData data)
        {
            if (!data.IsActive) return;

            data.IsActive = false;
            data.ControllerId = 0;
            data.IsGraphValid = false;
            Debug.Log("[PlayStateControl] Disposed.");
        }

        /// <summary>
        /// Plays the graph.
        /// </summary>
        public static void Play(this in Day06PlayStateData data)
        {
            if (!data.IsActive)
            {
                Debug.LogWarning("[PlayStateControl] Cannot play: Control is not active.");
                return;
            }

            if (!PlayStateOps.CanControlGraph(data.IsGraphValid))
            {
                Debug.LogWarning("[PlayStateControl] Cannot play: Graph is not valid.");
                return;
            }

            if (PlayStateOps.IsPlaying(data.CurrentState))
            {
                Debug.LogWarning("[PlayStateControl] Graph is already playing.");
                return;
            }

            data.Graph.Play();
            Debug.Log("[PlayStateControl] Graph started playing.");
        }

        /// <summary>
        /// Pauses the graph.
        /// </summary>
        public static void Pause(this in Day06PlayStateData data)
        {
            if (!data.IsActive)
            {
                Debug.LogWarning("[PlayStateControl] Cannot pause: Control is not active.");
                return;
            }

            if (!PlayStateOps.CanControlGraph(data.IsGraphValid))
            {
                Debug.LogWarning("[PlayStateControl] Cannot pause: Graph is not valid.");
                return;
            }

            if (PlayStateOps.IsPaused(data.CurrentState))
            {
                Debug.LogWarning("[PlayStateControl] Graph is already paused.");
                return;
            }

            // Store previous state before pausing
            var tempData = data;
            tempData.PreviousState = tempData.CurrentState;

            data.Graph.Stop();

            Debug.Log("[PlayStateControl] Graph paused.");
        }

        /// <summary>
        /// Toggles between play and pause states.
        /// </summary>
        public static void TogglePlayPause(this in Day06PlayStateData data)
        {
            if (!data.IsActive)
            {
                Debug.LogWarning("[PlayStateControl] Cannot toggle: Control is not active.");
                return;
            }

            if (!PlayStateOps.CanControlGraph(data.IsGraphValid))
            {
                Debug.LogWarning("[PlayStateControl] Cannot toggle: Graph is not valid.");
                return;
            }

            PlayStateOps.GetToggledState(data.CurrentState, out PlayState targetState);

            if (PlayStateOps.IsPlaying(targetState))
            {
                data.Play();
            }
            else
            {
                data.Pause();
            }
        }

        /// <summary>
        /// Updates the current play state from the graph.
        /// Should be called regularly to track state changes.
        /// </summary>
        public static void UpdateState(ref this Day06PlayStateData data)
        {
            if (!data.IsActive) return;

            if (!data.Graph.IsValid())
            {
                data.IsGraphValid = false;
                return;
            }

            PlayState newState = data.Graph.IsPlaying() ? PlayState.Playing : PlayState.Paused;

            // Check for state change
            if (PlayStateOps.DidStateChange(data.CurrentState, newState))
            {
                data.PreviousState = data.CurrentState;
                data.CurrentState = newState;

                if (PlayStateOps.IsTransitionToPause(data.PreviousState, newState))
                {
                    Debug.Log("[PlayStateControl] State changed: Playing -> Paused");
                }
                else if (PlayStateOps.IsTransitionToPlay(data.PreviousState, newState))
                {
                    Debug.Log("[PlayStateControl] State changed: Paused -> Playing");
                }
            }
        }

        /// <summary>
        /// Checks if the graph is currently playing.
        /// </summary>
        public static bool IsPlaying(this in Day06PlayStateData data)
        {
            if (!data.IsActive) return false;

            PlayStateOps.StateToInt(data.CurrentState, out int stateValue);
            return PlayStateOps.IsPlaying(data.CurrentState);
        }

        /// <summary>
        /// Checks if the graph is currently paused.
        /// </summary>
        public static bool IsPaused(this in Day06PlayStateData data)
        {
            if (!data.IsActive) return false;

            return PlayStateOps.IsPaused(data.CurrentState);
        }

        /// <summary>
        /// Checks if the play state just changed to paused.
        /// </summary>
        public static bool JustPaused(this in Day06PlayStateData data)
        {
            if (!data.IsActive) return false;

            return PlayStateOps.IsTransitionToPause(data.PreviousState, data.CurrentState);
        }

        /// <summary>
        /// Checks if the play state just changed to playing.
        /// </summary>
        public static bool JustStartedPlaying(this in Day06PlayStateData data)
        {
            if (!data.IsActive) return false;

            return PlayStateOps.IsTransitionToPlay(data.PreviousState, data.CurrentState);
        }

        /// <summary>
        /// Gets the current state as a string for debugging.
        /// </summary>
        public static string GetStateString(this in Day06PlayStateData data)
        {
            if (!data.IsActive) return "Inactive";

            return data.CurrentState.ToString();
        }

        /// <summary>
        /// Checks if the PlayState control is valid and active.
        /// </summary>
        public static bool IsValidControl(this in Day06PlayStateData data)
        {
            return data.IsActive && data.IsGraphValid && data.Graph.IsValid();
        }

        /// <summary>
        /// Logs the current play state information.
        /// </summary>
        public static void LogStateInfo(this in Day06PlayStateData data, string controllerName)
        {
            if (!data.IsActive)
            {
                Debug.LogWarning($"[PlayStateControl] Cannot log: Control is not active.");
                return;
            }

            string stateStr = data.GetStateString();
            string previousStateStr = data.PreviousState.ToString();
            bool isValid = data.IsValidControl();

            Debug.Log($"[PlayStateControl] Name: {controllerName}, State: {stateStr}, Previous: {previousStateStr}, Valid: {isValid}");
        }

        /// <summary>
        /// Gets the progress value (1.0 for playing, 0.0 for paused).
        /// Useful for UI visualization.
        /// </summary>
        public static float GetProgress(this in Day06PlayStateData data)
        {
            if (!data.IsActive) return 0.0f;

            PlayStateOps.CalculateProgress(data.CurrentState, out float progress);
            return progress;
        }
    }
}
