using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day03;
using PlayableLearn.Day04;
using PlayableLearn.Day05;

namespace PlayableLearn.Day06
{
    /// <summary>
    /// MonoBehaviour entry point for Day 06: The Pause Button.
    /// Demonstrates playing/stopping the graph programmatically using PlayState.
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day06PlayStateData, Day05SpeedData, Day04RotatorData, Day02OutputHandle, Day01GraphHandle)
    /// - Layer B: Operations (PlayStateOps, SpeedOps, RotatorLogic, ScriptPlayableOps)
    /// - Layer C: Extensions (Day06PlayStateExtensions, Day05SpeedExtensions, Day04RotatorExtensions, Day02OutputHandleExtensions, Day01GraphHandleExtensions)
    /// </summary>
    public class Day06Entry : MonoBehaviour
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
        [Range(0.0f, 5.0f)]
        private float targetSpeed = 1.0f;

        // PlayState visualization
        [Header("PlayState Visualization")]
        [SerializeField]
        private Color playingColor = Color.green;

        [SerializeField]
        private Color pausedColor = Color.red;

        // LAYER C: Adapter Calls
        private void OnEnable()
        {
            InitializeGraph();
            InitializeOutput();
            InitializeRotator();
            InitializeSpeedControl();
            InitializePlayStateControl();
            LinkNodesTogether();
            LogSystemInfo();
        }

        private void OnDisable()
        {
            CleanupPlayStateControl();
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
                playStateData.UpdateState();
            }

            // Update speed control
            if (speedData.IsValidSpeedControl() && math.abs(targetSpeed - speedData.TargetSpeed) > 0.01f)
            {
                speedData.SetTargetSpeed(targetSpeed);
            }

            // Visual feedback: Show play state with color
            UpdateVisualization();
        }

        /// <summary>
        /// GUI controls for play state manipulation.
        /// </summary>
        private void OnGUI()
        {
            if (!showPlayStateInUI || !playStateData.IsValidControl()) return;

            float buttonWidth = 120;
            float buttonHeight = 30;
            float padding = 10;
            float startX = 10;
            float startY = 10;

            // Play/Pause button
            string buttonText = playStateData.IsPlaying() ? "Pause" : "Play";
            if (GUI.Button(new Rect(startX, startY, buttonWidth, buttonHeight), buttonText))
            {
                playStateData.TogglePlayPause();
            }

            // Status display
            float statusY = startY + buttonHeight + padding;
            string statusText = $"State: {playStateData.GetStateString()}";
            GUI.Label(new Rect(startX, statusY, buttonWidth * 2, buttonHeight), statusText);

            // Speed indicator
            float speedY = statusY + buttonHeight + padding;
            if (speedData.IsValidSpeedControl())
            {
                float currentSpeed = speedData.GetCurrentSpeed();
                string speedText = $"Speed: {currentSpeed:F2}x";
                GUI.Label(new Rect(startX, speedY, buttonWidth * 2, buttonHeight), speedText);
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
                Debug.LogError("[Day 06] Cannot initialize output: Graph is not initialized.");
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
                Debug.LogError("[Day 06] Cannot initialize rotator: Graph is not initialized.");
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
                Debug.LogError("[Day 06] Cannot initialize speed control: Graph is not initialized.");
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

        private void InitializePlayStateControl()
        {
            if (!graphHandle.IsActive)
            {
                Debug.LogError("[Day 06] Cannot initialize PlayState control: Graph is not initialized.");
                return;
            }

            string playStateControllerName = $"{gameObject.name}_PlayStateController";
            PlayableGraph graph = graphHandle.Graph;
            var playState = playStateData;
            playState.Initialize(in graph, playStateControllerName, autoPlayOnStart);
            playStateData = playState;

            Debug.Log("[Day 06] PlayState control initialized.");
        }

        private void LinkNodesTogether()
        {
            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 06] Cannot link: Speed control is not valid.");
                return;
            }

            if (!rotatorData.IsValidRotator())
            {
                Debug.LogError("[Day 06] Cannot link: Rotator is not valid.");
                return;
            }

            // Connect rotator to speed control
            // This creates the chain: Output -> SpeedControl -> Rotator
            Playable speedPlayable = speedData.Node;
            rotatorData.Node.AddInput(speedPlayable, 0, 1.0f);

            // Connect speed control to output
            PlayableOutput output = outputHandle.Output;
            speedData.ConnectToOutput(in output);

            Debug.Log("[Day 06] Successfully linked nodes: Output -> SpeedControl -> Rotator!");
        }

        private void LogSystemInfo()
        {
            if (!playStateData.IsValidControl())
            {
                Debug.LogWarning("[Day 06] PlayState control is not valid, cannot log info.");
                return;
            }

            string playStateControllerName = $"{gameObject.name}_PlayStateController";
            playStateData.LogStateInfo(playStateControllerName);

            // Log speed control info
            if (speedData.IsValidSpeedControl())
            {
                string speedControlName = $"{gameObject.name}_SpeedControl";
                speedData.LogSpeedInfo(speedControlName);
            }
        }

        private void UpdateVisualization()
        {
            // Color-based play state visualization
            if (playStateData.IsValidControl())
            {
                // Change cube color based on play state
                GetComponent<Renderer>().material.color = playStateData.IsPlaying() ? playingColor : pausedColor;

                // Pulse effect based on speed
                if (speedData.IsValidSpeedControl())
                {
                    float currentSpeed = speedData.GetCurrentSpeed();
                    float pulse = 1.0f + Mathf.Sin(Time.time * currentSpeed * 3.0f) * 0.05f;
                    transform.localScale = Vector3.one * pulse;
                }
            }
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
            autoPlayOnStart = true;
            showPlayStateInUI = true;
            initialSpeedMultiplier = 1.0f;
            enableTimeDilation = true;
            interpolationSpeed = 2.0f;
            targetSpeed = 1.0f;
            rotationSpeed = 90.0f;
            rotationAxis = Vector3.up;
            playingColor = Color.green;
            pausedColor = Color.red;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only: Draw gizmos to visualize the play state and rotation axis.
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
                Gizmos.color = currentSpeed > 1.0f ? Color.red :
                              currentSpeed < 1.0f && currentSpeed > 0 ? Color.blue :
                              currentSpeed <= 0 ? Color.gray : Color.green;

                Vector3 speedLabelPos = transform.position + Vector3.up * 2.5f;
                UnityEditor.Handles.Label(speedLabelPos, $"Speed: {currentSpeed:F2}x");
            }

            // Draw play state indicator (Day 06)
            if (playStateData.IsValidControl())
            {
                Gizmos.color = playStateData.IsPlaying() ? playingColor : pausedColor;
                Vector3 stateLabelPos = transform.position + Vector3.up * 3.0f;
                UnityEditor.Handles.Label(stateLabelPos, $"State: {playStateData.GetStateString()}");
            }
        }
#endif
    }
}
