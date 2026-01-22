using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day03;
using PlayableLearn.Day04;
using PlayableLearn.Day05;
using PlayableLearn.Day06;

namespace PlayableLearn.Day07
{
    /// <summary>
    /// MonoBehaviour entry point for Day 07: The Reverse Time.
    /// Demonstrates reverse time functionality with negative speed and time wrapping.
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day07ReverseData, Day06PlayStateData, Day05SpeedData, Day04RotatorData, Day02OutputHandle, Day01GraphHandle)
    /// - Layer B: Operations (ReverseTimeOps, PlayStateOps, SpeedOps, RotatorLogic, ScriptPlayableOps)
    /// - Layer C: Extensions (Day07ReverseExtensions, Day06PlayStateExtensions, Day05SpeedExtensions, Day04RotatorExtensions, Day02OutputHandleExtensions, Day01GraphHandleExtensions)
    /// </summary>
    public class Day07Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day01GraphHandle graphHandle;

        [SerializeField]
        private Day02OutputHandle outputHandle;

        [SerializeField]
        private Day04RotatorData rotatorData;

        [SerializeField]
        private Day05SpeedData speedData;

        [SerializeField]
        private Day06PlayStateData playStateData;

        [SerializeField]
        private Day07ReverseData reverseData;

        // Reverse time settings
        [Header("Reverse Time Settings")]
        [SerializeField]
        private bool enableReverseTime = true;

        [SerializeField]
        private bool enableTimeWrapping = true;

        [SerializeField]
        [Range(-3.0f, 3.0f)]
        private float minSpeed = -1.0f;

        [SerializeField]
        [Range(-3.0f, 3.0f)]
        private float maxSpeed = 1.0f;

        // PlayState control settings
        [Header("PlayState Control Settings")]
        [SerializeField]
        private bool autoPlayOnStart = true;

        [SerializeField]
        private bool showPlayStateInUI = true;

        // Speed control settings (from Day 05)
        [Header("Time Dilation Settings")]
        [SerializeField]
        private float initialSpeedMultiplier = 1.0f;

        [SerializeField]
        private bool enableTimeDilation = true;

        [SerializeField]
        private float interpolationSpeed = 2.0f;

        // Rotation settings (from Day 04)
        [Header("Rotation Settings")]
        [SerializeField]
        private float rotationSpeed = 90.0f;

        [SerializeField]
        private Vector3 rotationAxis = Vector3.up;

        // Runtime controls
        [Header("Runtime Controls")]
        [SerializeField]
        [Range(-3.0f, 3.0f)]
        private float targetSpeed = 1.0f;

        // Visualization
        [Header("Visualization")]
        [SerializeField]
        private Color forwardColor = Color.green;

        [SerializeField]
        private Color reverseColor = Color.magenta;

        [SerializeField]
        private Color pausedColor = Color.red;

        // LAYER C: Adapter Calls
        private void OnEnable()
        {
            InitializeGraph();
            InitializeOutput();
            InitializeRotator();
            InitializeSpeedControl();
            InitializeReverseTimeControl();
            InitializePlayStateControl();
            LinkNodesTogether();
            LogSystemInfo();
        }

        private void OnDisable()
        {
            CleanupPlayStateControl();
            CleanupReverseTimeControl();
            CleanupSpeedControl();
            CleanupRotator();
            CleanupOutput();
            CleanupGraph();
        }

        private void Update()
        {
            // Update play state tracking
            if (playStateData.IsValidControl())
            {
                var playState = playStateData;
                playState.UpdateState();
                playStateData = playState;
            }

            // Update reverse time control
            if (reverseData.IsValidReverseControl())
            {
                var reverse = reverseData;
                reverse.UpdateTimeTracking(Time.deltaTime);
                reverseData = reverse;
            }

            // Update speed control with reverse time support
            if (speedData.IsValidSpeedControl())
            {
                float clampedSpeed = reverseData.IsValidReverseControl() && enableReverseTime
                    ? reverseData.ClampSpeed(minSpeed, maxSpeed, targetSpeed)
                    : targetSpeed;

                if (Mathf.Abs(clampedSpeed - speedData.GetTargetSpeed()) > 0.01f)
                {
                    var speed = speedData;
                    speed.SetTargetSpeed(clampedSpeed);
                    speedData = speed;
                }
            }

            // Visual feedback
            UpdateVisualization();
        }

        /// <summary>
        /// GUI controls for reverse time manipulation.
        /// </summary>
        private void OnGUI()
        {
            if (!showPlayStateInUI) return;

            float buttonWidth = 120;
            float buttonHeight = 30;
            float padding = 10;
            float startX = 10;
            float startY = 10;

            // Play/Pause button
            if (playStateData.IsValidControl())
            {
                string buttonText = playStateData.IsPlaying() ? "Pause" : "Play";
                if (GUI.Button(new Rect(startX, startY, buttonWidth, buttonHeight), buttonText))
                {
                    playStateData.TogglePlayPause();
                }

                // Status display
                float statusY = startY + buttonHeight + padding;
                string statusText = $"State: {playStateData.GetStateString()}";
                GUI.Label(new Rect(startX, statusY, buttonWidth * 2, buttonHeight), statusText);
            }

            // Reverse time toggle
            if (reverseData.IsValidReverseControl())
            {
                float reverseY = startY + (buttonHeight + padding) * 2;
                bool isReversing = reverseData.IsReversing();
                string reverseText = $"Reverse: {(isReversing ? "ON" : "OFF")}";

                if (GUI.Button(new Rect(startX, reverseY, buttonWidth, buttonHeight), reverseText))
                {
                    reverseData.ToggleReverse();
                }

                // Time display
                float timeY = reverseY + buttonHeight + padding;
                double accumulatedTime = reverseData.GetAccumulatedTime();
                string timeText = $"Time: {accumulatedTime:F2}s";
                GUI.Label(new Rect(startX, timeY, buttonWidth * 2, buttonHeight), timeText);

                // Speed indicator
                float speedY = timeY + buttonHeight + padding;
                if (speedData.IsValidSpeedControl())
                {
                    float currentSpeed = speedData.GetCurrentSpeed();
                    string direction = currentSpeed < 0 ? "<<" : (currentSpeed > 0 ? ">>" : "||");
                    string speedText = $"Speed: {direction} {Mathf.Abs(currentSpeed):F2}x";
                    GUI.Label(new Rect(startX, speedY, buttonWidth * 2, buttonHeight), speedText);
                }
            }
        }

        private void InitializeGraph()
        {
            string graphName = $"{gameObject.name}_Graph_{Time.frameCount}";
            var graph = graphHandle;
            graph.Initialize(graphName);
            graphHandle = graph;
        }

        private void InitializeOutput()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 07] Cannot initialize output: Graph is not initialized.");
                return;
            }

            string outputName = $"{gameObject.name}_ConsoleOutput";
            PlayableGraph graph = graphHandle.Graph;
            var output = outputHandle;
            output.Initialize(in graph, outputName);
            outputHandle = output;
        }

        private void InitializeRotator()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 07] Cannot initialize rotator: Graph is not initialized.");
                return;
            }

            string rotatorName = $"{gameObject.name}_Rotator";
            PlayableGraph graph = graphHandle.Graph;
            var rotator = rotatorData;
            rotator.Initialize(in graph, rotatorName, transform, rotationSpeed, rotationAxis);
            rotatorData = rotator;
        }

        private void InitializeSpeedControl()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 07] Cannot initialize speed control: Graph is not initialized.");
                return;
            }

            string speedControlName = $"{gameObject.name}_SpeedControl";
            PlayableGraph graph = graphHandle.Graph;
            var speed = speedData;
            speed.Initialize(in graph, speedControlName, initialSpeedMultiplier, enableTimeDilation, interpolationSpeed);
            speedData = speed;

            // Set initial target speed
            targetSpeed = initialSpeedMultiplier;
        }

        private void InitializeReverseTimeControl()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 07] Cannot initialize reverse time control: Graph is not initialized.");
                return;
            }

            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 07] Cannot initialize reverse time control: Speed control is not initialized.");
                return;
            }

            string reverseControlName = $"{gameObject.name}_ReverseTimeControl";
            PlayableGraph graph = graphHandle.Graph;
            var reverse = reverseData;
            reverse.Initialize(in graph, reverseControlName, enableReverseTime, enableTimeWrapping);
            reverseData = reverse;

            Debug.Log("[Day 07] Reverse time control initialized.");
        }

        private void InitializePlayStateControl()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 07] Cannot initialize PlayState control: Graph is not initialized.");
                return;
            }

            string playStateControllerName = $"{gameObject.name}_PlayStateController";
            PlayableGraph graph = graphHandle.Graph;
            var playState = playStateData;
            playState.Initialize(in graph, playStateControllerName, autoPlayOnStart);
            playStateData = playState;

            Debug.Log("[Day 07] PlayState control initialized.");
        }

        private void LinkNodesTogether()
        {
            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 07] Cannot link: Speed control is not valid.");
                return;
            }

            if (!rotatorData.IsValidRotator())
            {
                Debug.LogError("[Day 07] Cannot link: Rotator is not valid.");
                return;
            }

            // Connect rotator to speed control
            // This creates the chain: Output -> SpeedControl -> Rotator
            Playable speedPlayable = speedData.Node;
            rotatorData.Node.AddInput(speedPlayable, 0, 1.0f);

            // Connect speed control to output
            PlayableOutput output = outputHandle.Output;
            speedData.ConnectToOutput(in output);

            Debug.Log("[Day 07] Successfully linked nodes: Output -> SpeedControl -> Rotator!");
        }

        private void LogSystemInfo()
        {
            if (playStateData.IsValidControl())
            {
                string playStateControllerName = $"{gameObject.name}_PlayStateController";
                playStateData.LogStateInfo(playStateControllerName);
            }

            if (speedData.IsValidSpeedControl())
            {
                string speedControlName = $"{gameObject.name}_SpeedControl";
                speedData.LogSpeedInfo(speedControlName);
            }

            if (reverseData.IsValidReverseControl())
            {
                string reverseControlName = $"{gameObject.name}_ReverseTimeControl";
                reverseData.LogReverseInfo(reverseControlName);
            }
        }

        private void UpdateVisualization()
        {
            // Color-based visualization
            if (!playStateData.IsValidControl()) return;

            Color targetColor;
            if (!playStateData.IsPlaying())
            {
                targetColor = pausedColor;
            }
            else if (reverseData.IsValidReverseControl() && reverseData.IsReversing())
            {
                targetColor = reverseColor;
            }
            else
            {
                targetColor = forwardColor;
            }

            GetComponent<Renderer>().material.color = targetColor;

            // Pulse effect based on speed
            if (speedData.IsValidSpeedControl())
            {
                float currentSpeed = speedData.GetCurrentSpeed();
                float pulse = 1.0f + Mathf.Sin(Time.time * Mathf.Abs(currentSpeed) * 3.0f) * 0.05f;
                transform.localScale = Vector3.one * pulse;
            }
        }

        private void CleanupReverseTimeControl()
        {
            var reverse = reverseData;
            reverse.Dispose();
            reverseData = reverse;
        }

        private void CleanupPlayStateControl()
        {
            var playState = playStateData;
            playState.Dispose();
            playStateData = playState;
        }

        private void CleanupSpeedControl()
        {
            var speed = speedData;
            speed.Dispose();
            speedData = speed;
        }

        private void CleanupRotator()
        {
            var rotator = rotatorData;
            rotator.Dispose();
            rotatorData = rotator;
        }

        private void CleanupOutput()
        {
            var output = outputHandle;
            output.Dispose();
            outputHandle = output;
        }

        private void CleanupGraph()
        {
            var graph = graphHandle;
            graph.Dispose();
            graphHandle = graph;
        }

        /// <summary>
        /// Called by Unity when the component is reset in the Inspector.
        /// Good for setting default values.
        /// </summary>
        private void Reset()
        {
            enableReverseTime = true;
            enableTimeWrapping = true;
            minSpeed = -1.0f;
            maxSpeed = 1.0f;
            autoPlayOnStart = true;
            showPlayStateInUI = true;
            initialSpeedMultiplier = 1.0f;
            enableTimeDilation = true;
            interpolationSpeed = 2.0f;
            targetSpeed = 1.0f;
            rotationSpeed = 90.0f;
            rotationAxis = Vector3.up;
            forwardColor = Color.green;
            reverseColor = Color.magenta;
            pausedColor = Color.red;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only: Draw gizmos to visualize the reverse time state.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!rotatorData.IsValidRotator()) return;

            // Draw rotation axis (from Day 04)
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, rotationAxis.normalized * 2.0f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + rotationAxis.normalized * 2.0f, 0.1f);

            // Draw speed indicator (from Day 05)
            if (speedData.IsValidSpeedControl())
            {
                float currentSpeed = speedData.GetCurrentSpeed();
                bool isReversing = reverseData.IsValidReverseControl() && reverseData.IsReversing();

                Gizmos.color = isReversing ? reverseColor :
                              currentSpeed < 0 ? Color.magenta :
                              currentSpeed > 1.0f ? Color.red :
                              currentSpeed < 1.0f && currentSpeed > 0 ? Color.blue :
                              Color.green;

                Vector3 speedLabelPos = transform.position + Vector3.up * 2.5f;
                UnityEditor.Handles.Label(speedLabelPos, $"Speed: {currentSpeed:F2}x");
            }

            // Draw play state indicator (Day 06)
            if (playStateData.IsValidControl())
            {
                Gizmos.color = playStateData.IsPlaying() ? forwardColor : pausedColor;
                Vector3 stateLabelPos = transform.position + Vector3.up * 3.0f;
                UnityEditor.Handles.Label(stateLabelPos, $"State: {playStateData.GetStateString()}");
            }

            // Draw reverse time indicator (Day 07)
            if (reverseData.IsValidReverseControl())
            {
                bool isReversing = reverseData.IsReversing();
                Gizmos.color = isReversing ? reverseColor : forwardColor;
                Vector3 reverseLabelPos = transform.position + Vector3.up * 3.5f;
                double accumulatedTime = reverseData.GetAccumulatedTime();
                UnityEditor.Handles.Label(reverseLabelPos, $"Reverse: {(isReversing ? "ON" : "OFF")}, Time: {accumulatedTime:F2}s");
            }
        }
#endif
    }
}
