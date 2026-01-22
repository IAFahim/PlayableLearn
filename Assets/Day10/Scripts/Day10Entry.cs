using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day04;
using PlayableLearn.Day05;
using PlayableLearn.Day06;
using PlayableLearn.Day07;
using PlayableLearn.Day08;
using PlayableLearn.Day09;
using Unity.Mathematics;

namespace PlayableLearn.Day10
{
    /// <summary>
    /// MonoBehaviour entry point for Day 10: The Cleanup.
    /// Demonstrates proper IDisposable patterns for PlayableGraph and related resources,
    /// ensuring deterministic cleanup and preventing memory leaks.
    ///
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day10DisposableGraph, Day09VisualizerData, Day08NamedGraphData, Day07ReverseData, Day06PlayStateData, Day05SpeedData, Day04RotatorData, Day02OutputHandle, Day01GraphHandle)
    /// - Layer B: Operations (DisposalOps, VisualizerOps, GraphNamingOps, ReverseTimeOps, PlayStateOps, SpeedOps, RotatorLogic, ScriptPlayableOps)
    /// - Layer C: Extensions (Day10DisposableGraphExtensions, Day09VisualizerExtensions, Day08NamedGraphExtensions, Day07ReverseExtensions, Day06PlayStateExtensions, Day05SpeedExtensions, Day04RotatorExtensions, Day02OutputHandleExtensions, Day01GraphHandleExtensions)
    /// </summary>
    public class Day10Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day10DisposableGraph disposableGraph;

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

        [SerializeField]
        private Day08NamedGraphData namedGraphData;

        [SerializeField]
        private Day09VisualizerData visualizerData;

        // IDisposable settings
        [Header("IDisposable Pattern Settings")]
        [SerializeField]
        private bool useDisposablePattern = true;

        [SerializeField]
        private bool enableDisposalLogging = true;

        [SerializeField]
        private bool validateDisposalOnDisable = true;

        [SerializeField]
        private bool autoDisposeOnDisable = true;

        // Graph naming settings (from Day 08)
        [Header("Graph Naming Settings")]
        [SerializeField]
        private bool enableGraphNaming = true;

        [SerializeField]
        private string customGraphName = "DisposableGraph";

        [SerializeField]
        private bool showGraphNameInUI = true;

        // Reverse time settings (from Day 07)
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

        // PlayState control settings (from Day 06)
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

        [SerializeField]
        private Color disposableColor = Color.yellow;

        // LAYER C: Adapter Calls
        private void OnEnable()
        {
            if (useDisposablePattern)
            {
                InitializeDisposableGraph();
            }
            else
            {
                InitializeGraph();
            }

            InitializeGraphNaming();
            InitializeVisualizer();
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
            CleanupVisualizer();
            CleanupPlayStateControl();
            CleanupReverseTimeControl();
            CleanupSpeedControl();
            CleanupRotator();
            CleanupOutput();
            CleanupGraphNaming();

            if (useDisposablePattern)
            {
                CleanupDisposableGraph();
            }
            else
            {
                CleanupGraph();
            }
        }

        private void Update()
        {
            // Update play state tracking
            if (playStateData.IsValidControl())
            {
                playStateData.UpdateState();
            }

            // Update reverse time control
            if (reverseData.IsValidReverseControl())
            {
                reverseData.UpdateTimeTracking(Time.deltaTime);
            }

            // Update speed control with reverse time support
            if (speedData.IsValidSpeedControl())
            {
                float clampedSpeed = reverseData.IsValidReverseControl() && enableReverseTime
                    ? reverseData.ClampSpeed(minSpeed, maxSpeed, targetSpeed)
                    : targetSpeed;

                if (math.abs(clampedSpeed - speedData.TargetSpeed) > 0.01f)
                {
                    speedData.SetTargetSpeed(clampedSpeed);
                }
            }

            // Visual feedback
            UpdateVisualization();
        }

        /// <summary>
        /// GUI controls for disposable pattern and graph manipulation.
        /// </summary>
        private void OnGUI()
        {
            float buttonWidth = 150;
            float buttonHeight = 30;
            float padding = 10;
            float startX = 10;
            float startY = 10;
            float currentY = startY;

            // IDisposable pattern display
            if (useDisposablePattern)
            {
                string patternText = $"Pattern: {(useDisposablePattern ? "IDisposable" : "Legacy")}";
                GUI.Label(new Rect(startX, currentY, buttonWidth * 2, buttonHeight), patternText);
                currentY += buttonHeight + padding;

                string disposedText = $"Disposed: {disposableGraph.IsDisposedGraph()}";
                GUI.Label(new Rect(startX, currentY, buttonWidth * 2, buttonHeight), disposedText);
                currentY += buttonHeight + padding;

                string validText = $"Valid: {disposableGraph.IsValid()}";
                GUI.Label(new Rect(startX, currentY, buttonWidth * 2, buttonHeight), validText);
                currentY += buttonHeight + padding;

                // Test dispose button
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Test Dispose"))
                {
                    if (disposableGraph.IsValid())
                    {
                        disposableGraph.Dispose();
                        if (enableDisposalLogging)
                        {
                            Debug.Log("[Day 10] Manual dispose triggered");
                        }
                    }
                }
                currentY += buttonHeight + padding;
            }

            // Graph naming display
            if (showGraphNameInUI && namedGraphData.IsValidControl())
            {
                string graphNameText = $"Graph: {namedGraphData.GetName()}";
                GUI.Label(new Rect(startX, currentY, buttonWidth * 3, buttonHeight), graphNameText);
                currentY += buttonHeight + padding;
            }

            // Play/Pause button
            if (showPlayStateInUI && playStateData.IsValidControl())
            {
                string buttonText = playStateData.IsPlaying() ? "Pause" : "Play";
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), buttonText))
                {
                    playStateData.TogglePlayPause();
                }

                // Status display
                float statusY = currentY + buttonHeight + padding;
                string statusText = $"State: {playStateData.GetStateString()}";
                GUI.Label(new Rect(startX, statusY, buttonWidth * 2, buttonHeight), statusText);
                currentY = statusY + buttonHeight + padding;
            }

            // Reverse time toggle
            if (reverseData.IsValidReverseControl())
            {
                bool isReversing = reverseData.IsReversing();
                string reverseText = $"Reverse: {(isReversing ? "ON" : "OFF")}";

                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), reverseText))
                {
                    reverseData.ToggleReverse();
                }

                // Time display
                float timeY = currentY + buttonHeight + padding;
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

        private void InitializeDisposableGraph()
        {
            if (useDisposablePattern)
            {
                string graphName = enableGraphNaming ? customGraphName : $"{gameObject.name}_DisposableGraph_{Time.frameCount}";
                var disposable = disposableGraph;
                disposable.Initialize(graphName);
                disposableGraph = disposable;

                if (enableDisposalLogging)
                {
                    disposableGraph.LogState("Initialize");
                }
            }
        }

        private void InitializeGraph()
        {
            string graphName = enableGraphNaming ? customGraphName : $"{gameObject.name}_Graph_{Time.frameCount}";
            var graph = graphHandle;
            graph.Initialize(graphName);
            graphHandle = graph;
        }

        private void InitializeGraphNaming()
        {
            PlayableGraph graph = useDisposablePattern ? disposableGraph.Graph : graphHandle.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 10] Cannot initialize graph naming: Graph is not initialized.");
                return;
            }

            if (!enableGraphNaming)
            {
                Debug.Log("[Day 10] Graph naming is disabled.");
                return;
            }

            string graphName = string.IsNullOrEmpty(customGraphName) ? $"{gameObject.name}_PlayableGraph" : customGraphName;
            var namedGraph = namedGraphData;
            namedGraph.Initialize(in graph, graphName);
            namedGraphData = namedGraph;

            Debug.Log("[Day 10] Graph naming initialized.");
        }

        private void InitializeVisualizer()
        {
            PlayableGraph graph = useDisposablePattern ? disposableGraph.Graph : graphHandle.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 10] Cannot initialize visualizer: Graph is not initialized.");
                return;
            }

            var visualizer = visualizerData;
            visualizer.Initialize(in graph, VisualizationDetailLevel.Basic, VisualizerColorScheme.Default);
            visualizerData = visualizer;

            Debug.Log("[Day 10] Graph Visualizer initialized.");
        }

        private void InitializeOutput()
        {
            PlayableGraph graph = useDisposablePattern ? disposableGraph.Graph : graphHandle.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 10] Cannot initialize output: Graph is not initialized.");
                return;
            }

            string outputName = $"{gameObject.name}_ConsoleOutput";
            var output = outputHandle;
            output.Initialize(in graph, outputName);
            outputHandle = output;
        }

        private void InitializeRotator()
        {
            PlayableGraph graph = useDisposablePattern ? disposableGraph.Graph : graphHandle.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 10] Cannot initialize rotator: Graph is not initialized.");
                return;
            }

            string rotatorName = $"{gameObject.name}_Rotator";
            var rotator = rotatorData;
            rotator.Initialize(in graph, rotatorName, transform, rotationSpeed, rotationAxis);
            rotatorData = rotator;
        }

        private void InitializeSpeedControl()
        {
            PlayableGraph graph = useDisposablePattern ? disposableGraph.Graph : graphHandle.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 10] Cannot initialize speed control: Graph is not initialized.");
                return;
            }

            string speedControlName = $"{gameObject.name}_SpeedControl";
            var speed = speedData;
            speed.Initialize(in graph, speedControlName, initialSpeedMultiplier, enableTimeDilation, interpolationSpeed);
            speedData = speed;

            targetSpeed = initialSpeedMultiplier;
        }

        private void InitializeReverseTimeControl()
        {
            PlayableGraph graph = useDisposablePattern ? disposableGraph.Graph : graphHandle.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 10] Cannot initialize reverse time control: Graph is not initialized.");
                return;
            }

            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 10] Cannot initialize reverse time control: Speed control is not initialized.");
                return;
            }

            string reverseControlName = $"{gameObject.name}_ReverseTimeControl";
            var reverse = reverseData;
            reverse.Initialize(in graph, reverseControlName, enableReverseTime, enableTimeWrapping);
            reverseData = reverse;

            Debug.Log("[Day 10] Reverse time control initialized.");
        }

        private void InitializePlayStateControl()
        {
            PlayableGraph graph = useDisposablePattern ? disposableGraph.Graph : graphHandle.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 10] Cannot initialize PlayState control: Graph is not initialized.");
                return;
            }

            string playStateControllerName = $"{gameObject.name}_PlayStateController";
            var playState = playStateData;
            playState.Initialize(in graph, playStateControllerName, autoPlayOnStart);
            playStateData = playState;

            Debug.Log("[Day 10] PlayState control initialized.");
        }

        private void LinkNodesTogether()
        {
            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 10] Cannot link: Speed control is not valid.");
                return;
            }

            if (!rotatorData.IsValidRotator())
            {
                Debug.LogError("[Day 10] Cannot link: Rotator is not valid.");
                return;
            }

            // Connect rotator to speed control
            // This creates the chain: Output -> SpeedControl -> Rotator
            Playable speedPlayable = speedData.Node;
            rotatorData.Node.AddInput(speedPlayable, 0, 1.0f);

            // Connect speed control to output
            PlayableOutput output = outputHandle.Output;
            speedData.ConnectToOutput(in output);

            Debug.Log("[Day 10] Successfully linked nodes: Output -> SpeedControl -> Rotator!");
        }

        private void LogSystemInfo()
        {
            Debug.Log("[Day 10] === System Information ===");

            if (useDisposablePattern)
            {
                disposableGraph.LogState("Disposable Graph");
            }

            if (namedGraphData.IsValidControl())
            {
                namedGraphData.LogNamedGraphInfo("Named Graph");
            }

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

            Debug.Log("[Day 10] === End System Information ===");
        }

        private void UpdateVisualization()
        {
            // Color-based visualization
            if (!playStateData.IsValidControl()) return;

            Color targetColor;
            if (useDisposablePattern && disposableGraph.IsDisposedGraph())
            {
                targetColor = disposableColor;
            }
            else if (!playStateData.IsPlaying())
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
                float pulse = 1.0f + Mathf.Sin(Time.time * math.abs(currentSpeed) * 3.0f) * 0.05f;
                transform.localScale = Vector3.one * pulse;
            }
        }

        private void CleanupVisualizer()
        {
            var visualizer = visualizerData;
            visualizer.Dispose();
            visualizerData = visualizer;
        }

        private void CleanupGraphNaming()
        {
            var namedGraph = namedGraphData;
            namedGraph.Dispose();
            namedGraphData = namedGraph;
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

        private void CleanupDisposableGraph()
        {
            if (!useDisposablePattern) return;

            if (validateDisposalOnDisable)
            {
                if (disposableGraph.IsValid())
                {
                    if (enableDisposalLogging)
                    {
                        Debug.Log("[Day 10] Validating disposal before cleanup");
                        disposableGraph.LogState("Before Cleanup");
                    }
                }
            }

            if (autoDisposeOnDisable)
            {
                var disposable = disposableGraph;
                disposable.Dispose();
                disposableGraph = disposable;

                if (enableDisposalLogging)
                {
                    Debug.Log("[Day 10] Auto-disposed graph on OnDisable");
                }
            }
        }

        /// <summary>
        /// Called by Unity when the component is reset in the Inspector.
        /// Good for setting default values.
        /// </summary>
        private void Reset()
        {
            // IDisposable defaults
            useDisposablePattern = true;
            enableDisposalLogging = true;
            validateDisposalOnDisable = true;
            autoDisposeOnDisable = true;

            // Graph naming defaults
            enableGraphNaming = true;
            customGraphName = "DisposableGraph";
            showGraphNameInUI = true;

            // Reverse time defaults
            enableReverseTime = true;
            enableTimeWrapping = true;
            minSpeed = -1.0f;
            maxSpeed = 1.0f;

            // PlayState defaults
            autoPlayOnStart = true;
            showPlayStateInUI = true;

            // Speed defaults
            initialSpeedMultiplier = 1.0f;
            enableTimeDilation = true;
            interpolationSpeed = 2.0f;

            // Rotation defaults
            rotationSpeed = 90.0f;
            rotationAxis = Vector3.up;

            // Visualization defaults
            targetSpeed = 1.0f;
            forwardColor = Color.green;
            reverseColor = Color.magenta;
            pausedColor = Color.red;
            disposableColor = Color.yellow;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only: Draw gizmos to visualize the graph state and disposable pattern info.
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

            // Draw graph name indicator (Day 08)
            if (namedGraphData.IsValidControl())
            {
                Gizmos.color = Color.yellow;
                Vector3 nameLabelPos = transform.position + Vector3.up * 4.0f;
                string graphName = namedGraphData.GetName();
                int controllerId = namedGraphData.GetControllerId();
                UnityEditor.Handles.Label(nameLabelPos, $"Graph: '{graphName}' (ID: {controllerId})");
            }

            // Draw disposable pattern indicator (Day 10)
            if (useDisposablePattern)
            {
                Gizmos.color = disposableColor;
                Vector3 disposableLabelPos = transform.position + Vector3.up * 4.5f;
                string status = disposableGraph.IsDisposedGraph() ? "DISPOSED" : "ACTIVE";
                UnityEditor.Handles.Label(disposableLabelPos, $"[IDisposable] {status} (ID: {disposableGraph.GraphId})");

                // Draw disposal status indicator
                Gizmos.color = disposableGraph.IsValid() ? disposableColor : Color.gray;
                Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.3f);
            }
        }
#endif
    }
}
