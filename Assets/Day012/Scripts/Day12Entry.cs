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
using PlayableLearn.Day10;
using PlayableLearn.Day11;
using Unity.Mathematics;

namespace PlayableLearn.Day12
{
    /// <summary>
    /// MonoBehaviour entry point for Day 12: The Clip Player.
    /// Demonstrates wrapping AnimationClipPlayable for animation playback.
    ///
    /// Key Concepts:
    /// - AnimationClipPlayable wraps an AnimationClip for playback
    /// - Clips are the actual animation data
    /// - The Clip Player plays clips through the PlayableGraph
    /// - Time management, duration, and playback control
    ///
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day12ClipHandle, Day10DisposableGraph, etc.)
    /// - Layer B: Operations (ClipPlayableOps, DisposalOps, etc.)
    /// - Layer C: Extensions (Day12ClipExtensions, Day10DisposableGraphExtensions, etc.)
    /// </summary>
    public class Day12Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day10DisposableGraph disposableGraph;

        [SerializeField]
        private Day12ClipHandle clipHandle;

        [SerializeField]
        private Day11AnimOutputHandle animOutputHandle;

        [SerializeField]
        private Day02OutputHandle scriptOutputHandle;

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

        // Animation clip reference (required for AnimationClipPlayable)
        [Header("Animation Clip Settings")]
        [SerializeField]
        private AnimationClip animationClip;

        // Animator reference (required for AnimationPlayableOutput)
        [Header("Animator Settings")]
        [SerializeField]
        private Animator targetAnimator;

        // Output type selection
        [Header("Output Type Settings")]
        [SerializeField]
        private bool useAnimationOutput = true;

        [SerializeField]
        private bool showClipInfo = true;

        // Clip playback settings
        [Header("Clip Playback Settings")]
        [SerializeField]
        private bool loopClip = true;

        [SerializeField]
        private bool autoPlayOnStart = true;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float initialTime = 0.0f;

        // IDisposable settings (from Day 10)
        [Header("IDisposable Pattern Settings")]
        [SerializeField]
        private bool useDisposablePattern = true;

        [SerializeField]
        private bool enableDisposalLogging = true;

        // Graph naming settings (from Day 08)
        [Header("Graph Naming Settings")]
        [SerializeField]
        private bool enableGraphNaming = true;

        [SerializeField]
        private string customGraphName = "ClipPlayerGraph";

        // Reverse time settings (from Day 07)
        [Header("Reverse Time Settings")]
        [SerializeField]
        private bool enableReverseTime = false;

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
        private bool autoPlayStateControl = true;

        // Speed control settings (from Day 05)
        [Header("Time Dilation Settings")]
        [SerializeField]
        private float initialSpeedMultiplier = 1.0f;

        [SerializeField]
        private bool enableTimeDilation = true;

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

        [SerializeField]
        private float targetTime = 0.0f;

        // Visualization
        [Header("Visualization")]
        [SerializeField]
        private Color clipPlayingColor = Color.green;

        [SerializeField]
        private Color clipPausedColor = Color.orange;

        [SerializeField]
        private Color clipErrorColor = Color.red;

        private void OnEnable()
        {
            ValidateAnimationClip();
            ValidateAnimator();
            InitializeDisposableGraph();
            InitializeGraphNaming();
            InitializeVisualizer();
            InitializeClipPlayable();
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
            CleanupClipPlayable();
            CleanupGraphNaming();
            CleanupDisposableGraph();
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

            // Update clip time wrapping
            if (clipHandle.IsValidClip())
            {
                UpdateClipTime();
            }

            // Visual feedback
            UpdateVisualization();
        }

        private void OnGUI()
        {
            float buttonWidth = 200;
            float buttonHeight = 30;
            float padding = 10;
            float startX = 10;
            float startY = 10;
            float currentY = startY;

            // Clip info display
            if (showClipInfo && clipHandle.IsValidClip())
            {
                if (clipHandle.TryGetTime(out double currentTime))
                {
                    string clipText = $"Clip: {animationClip?.Name ?? "None"}";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), clipText);
                    currentY += buttonHeight + padding;

                    if (clipHandle.TryGetDuration(out double duration))
                    {
                        string timeText = $"Time: {currentTime:F2} / {duration:F2}";
                        GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), timeText);
                        currentY += buttonHeight + padding;

                        string progressText = $"Progress: {(currentTime / duration * 100):F1}%";
                        GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), progressText);
                        currentY += buttonHeight + padding;
                    }
                }
            }

            // Play/Pause button
            if (playStateData.IsValidControl())
            {
                string buttonText = playStateData.IsPlaying() ? "Pause" : "Play";
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), buttonText))
                {
                    playStateData.TogglePlayPause();
                }
                currentY += buttonHeight + padding;

                float statusY = currentY;
                string statusText = $"State: {playStateData.GetStateString()}";
                GUI.Label(new Rect(startX, statusY, buttonWidth, buttonHeight), statusText);
                currentY = statusY + buttonHeight + padding;
            }

            // Speed indicator
            if (speedData.IsValidSpeedControl())
            {
                float currentSpeed = speedData.GetCurrentSpeed();
                string direction = currentSpeed < 0 ? "<<" : (currentSpeed > 0 ? ">>" : "||");
                string speedText = $"Speed: {direction} {math.abs(currentSpeed):F2}x";
                GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), speedText);
            }
        }

        private void ValidateAnimationClip()
        {
            if (animationClip == null)
            {
                Debug.LogWarning("[Day 12] No AnimationClip assigned. AnimationClipPlayable will not work without a clip.");
            }
            else
            {
                Debug.Log($"[Day 12] AnimationClip assigned: {animationClip.name}");
            }
        }

        private void ValidateAnimator()
        {
            if (useAnimationOutput && targetAnimator == null)
            {
                Debug.LogWarning("[Day 12] AnimationPlayableOutput enabled but no Animator assigned. Looking for Animator on this GameObject.");
                targetAnimator = GetComponent<Animator>();

                if (targetAnimator == null)
                {
                    Debug.LogError("[Day 12] No Animator component found. AnimationPlayableOutput will not work without an Animator.");
                }
                else
                {
                    Debug.Log($"[Day 12] Found Animator component: {targetAnimator.name}");
                }
            }
        }

        private void InitializeDisposableGraph()
        {
            if (!useDisposablePattern) return;

            string graphName = enableGraphNaming ? customGraphName : $"{gameObject.name}_ClipPlayerGraph_{Time.frameCount}";
            var disposable = disposableGraph;
            disposable.Initialize(graphName);
            disposableGraph = disposable;

            if (enableDisposalLogging)
            {
                disposableGraph.LogState("Initialize");
            }
        }

        private void InitializeGraphNaming()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 12] Cannot initialize graph naming: Graph is not initialized.");
                return;
            }

            if (!enableGraphNaming) return;

            string graphName = string.IsNullOrEmpty(customGraphName) ? $"{gameObject.name}_ClipPlayerGraph" : customGraphName;
            var namedGraph = namedGraphData;
            namedGraph.Initialize(in graph, graphName);
            namedGraphData = namedGraph;

            Debug.Log("[Day 12] Graph naming initialized.");
        }

        private void InitializeVisualizer()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 12] Cannot initialize visualizer: Graph is not initialized.");
                return;
            }

            var visualizer = visualizerData;
            visualizer.Initialize(in graph, VisualizationDetailLevel.Basic, VisualizerColorScheme.Default);
            visualizerData = visualizer;

            Debug.Log("[Day 12] Graph Visualizer initialized.");
        }

        private void InitializeClipPlayable()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 12] Cannot initialize clip playable: Graph is not initialized.");
                return;
            }

            if (animationClip == null)
            {
                Debug.LogError("[Day 12] Cannot initialize clip playable: AnimationClip is null.");
                return;
            }

            var clip = clipHandle;
            clip.Initialize(in graph, animationClip);

            // Set initial time
            if (clip.IsValidClip() && initialTime > 0.0f)
            {
                clip.SetTime(initialTime);
            }

            // Set initial play state
            if (clip.IsValidClip() && !autoPlayOnStart)
            {
                clip.SetPlayState(false);
            }

            clipHandle = clip;

            Debug.Log("[Day 12] AnimationClipPlayable initialized.");
        }

        private void InitializeOutput()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 12] Cannot initialize output: Graph is not initialized.");
                return;
            }

            if (useAnimationOutput)
            {
                // Initialize AnimationPlayableOutput (requires Animator)
                if (targetAnimator != null)
                {
                    string outputName = $"{gameObject.name}_AnimOutput";
                    var animOutput = animOutputHandle;
                    animOutput.Initialize(in graph, outputName, targetAnimator);
                    animOutputHandle = animOutput;

                    Debug.Log("[Day 12] AnimationPlayableOutput initialized.");
                }
                else
                {
                    Debug.LogError("[Day 12] Cannot initialize AnimationPlayableOutput: No Animator assigned.");
                }
            }
            else
            {
                // Initialize ScriptPlayableOutput (for comparison)
                string outputName = $"{gameObject.name}_ScriptOutput";
                var scriptOutput = scriptOutputHandle;
                scriptOutput.Initialize(in graph, outputName);
                scriptOutputHandle = scriptOutput;

                Debug.Log("[Day 12] ScriptPlayableOutput initialized.");
            }
        }

        private void InitializeRotator()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 12] Cannot initialize rotator: Graph is not initialized.");
                return;
            }

            string rotatorName = $"{gameObject.name}_Rotator";
            var rotator = rotatorData;
            rotator.Initialize(in graph, rotatorName, transform, rotationSpeed, rotationAxis);
            rotatorData = rotator;
        }

        private void InitializeSpeedControl()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 12] Cannot initialize speed control: Graph is not initialized.");
                return;
            }

            string speedControlName = $"{gameObject.name}_SpeedControl";
            var speed = speedData;
            speed.Initialize(in graph, speedControlName, initialSpeedMultiplier, enableTimeDilation, 2.0f);
            speedData = speed;

            targetSpeed = initialSpeedMultiplier;
        }

        private void InitializeReverseTimeControl()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 12] Cannot initialize reverse time control: Graph is not initialized.");
                return;
            }

            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 12] Cannot initialize reverse time control: Speed control is not initialized.");
                return;
            }

            string reverseControlName = $"{gameObject.name}_ReverseTimeControl";
            var reverse = reverseData;
            reverse.Initialize(in graph, reverseControlName, enableReverseTime, enableTimeWrapping);
            reverseData = reverse;

            Debug.Log("[Day 12] Reverse time control initialized.");
        }

        private void InitializePlayStateControl()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 12] Cannot initialize PlayState control: Graph is not initialized.");
                return;
            }

            string playStateControllerName = $"{gameObject.name}_PlayStateController";
            var playState = playStateData;
            playState.Initialize(in graph, playStateControllerName, autoPlayStateControl && autoPlayOnStart);
            playStateData = playState;

            Debug.Log("[Day 12] PlayState control initialized.");
        }

        private void LinkNodesTogether()
        {
            if (!clipHandle.IsValidClip())
            {
                Debug.LogError("[Day 12] Cannot link: Clip playable is not valid.");
                return;
            }

            // Connect clip playable to output
            if (useAnimationOutput && animOutputHandle.IsValidOutput())
            {
                if (animOutputHandle.TryGetPlayableOutput(out PlayableOutput output))
                {
                    clipHandle.ConnectToOutput(in output);
                    Debug.Log("[Day 12] Successfully linked clip to AnimationPlayableOutput!");
                }
            }
            else if (!useAnimationOutput && scriptOutputHandle.IsValidOutput())
            {
                PlayableOutput output = scriptOutputHandle.Output;
                clipHandle.ConnectToOutput(in output);
                Debug.Log("[Day 12] Successfully linked clip to ScriptPlayableOutput!");
            }
            else
            {
                Debug.LogWarning("[Day 12] No valid output available for linking.");
            }
        }

        private void UpdateClipTime()
        {
            if (!loopClip) return;

            if (clipHandle.TryGetTime(out double currentTime) && clipHandle.TryGetDuration(out double duration))
            {
                if (currentTime >= duration)
                {
                    clipHandle.SetTime(0.0);
                }
                else if (currentTime < 0.0f)
                {
                    clipHandle.SetTime(duration);
                }
            }
        }

        private void LogSystemInfo()
        {
            Debug.Log("[Day 12] === System Information ===");

            Debug.Log($"[Day 12] Output Type: {(useAnimationOutput ? "AnimationPlayableOutput" : "ScriptPlayableOutput")}");

            if (clipHandle.IsValidClip())
            {
                clipHandle.LogToConsole("Clip");
            }

            if (animOutputHandle.IsValidOutput())
            {
                animOutputHandle.LogToConsole("AnimOutput");
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

            Debug.Log("[Day 12] === End System Information ===");
        }

        private void UpdateVisualization()
        {
            // Color-based visualization showing clip playback state
            Color targetColor;
            if (!clipHandle.IsValidClip())
            {
                targetColor = clipErrorColor;
            }
            else if (!clipHandle.IsPlaying())
            {
                targetColor = clipPausedColor;
            }
            else
            {
                targetColor = clipPlayingColor;
            }

            GetComponent<Renderer>().material.color = targetColor;

            // Pulse effect based on clip progress
            if (clipHandle.IsValidClip() && clipHandle.TryGetDuration(out double duration))
            {
                if (clipHandle.TryGetTime(out double currentTime))
                {
                    float progress = (float)(currentTime / duration);
                    float pulse = 1.0f + Mathf.Sin(progress * Mathf.PI * 2.0f) * 0.1f;
                    transform.localScale = Vector3.one * pulse;
                }
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
            if (useAnimationOutput)
            {
                var animOutput = animOutputHandle;
                animOutput.Dispose();
                animOutputHandle = animOutput;
            }
            else
            {
                var scriptOutput = scriptOutputHandle;
                scriptOutput.Dispose();
                scriptOutputHandle = scriptOutput;
            }
        }

        private void CleanupClipPlayable()
        {
            var clip = clipHandle;
            clip.Dispose();
            clipHandle = clip;
        }

        private void CleanupDisposableGraph()
        {
            if (!useDisposablePattern) return;

            var disposable = disposableGraph;
            disposable.Dispose();
            disposableGraph = disposable;

            if (enableDisposalLogging)
            {
                Debug.Log("[Day 12] Auto-disposed graph on OnDisable");
            }
        }

        private void Reset()
        {
            // Defaults
            useDisposablePattern = true;
            enableDisposalLogging = true;
            enableGraphNaming = true;
            customGraphName = "ClipPlayerGraph";
            useAnimationOutput = true;
            showClipInfo = true;
            loopClip = true;
            autoPlayOnStart = true;
            initialTime = 0.0f;

            // Try to find Animator on this GameObject
            targetAnimator = GetComponent<Animator>();

            // Other defaults
            enableReverseTime = false;
            enableTimeWrapping = true;
            minSpeed = -1.0f;
            maxSpeed = 1.0f;
            autoPlayStateControl = true;
            initialSpeedMultiplier = 1.0f;
            enableTimeDilation = true;
            rotationSpeed = 90.0f;
            rotationAxis = Vector3.up;
            targetSpeed = 1.0f;
            targetTime = 0.0f;
            clipPlayingColor = Color.green;
            clipPausedColor = Color.orange;
            clipErrorColor = Color.red;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw clip info (Day 12)
            if (clipHandle.IsValidClip())
            {
                Gizmos.color = clipHandle.IsPlaying() ? clipPlayingColor : clipPausedColor;
                Vector3 clipLabelPos = transform.position + Vector3.up * 3.5f;
                string clipText = $"Clip: {animationClip?.Name ?? "None"}";
                UnityEditor.Handles.Label(clipLabelPos, clipText);

                if (clipHandle.TryGetTime(out double currentTime) && clipHandle.TryGetDuration(out double duration))
                {
                    Vector3 timeLabelPos = transform.position + Vector3.up * 3.0f;
                    string timeText = $"Time: {currentTime:F2}/{duration:F2}";
                    UnityEditor.Handles.Label(timeLabelPos, timeText);
                }
            }

            // Draw rotation axis (from Day 04)
            if (rotatorData.IsValidRotator())
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(transform.position, rotationAxis.normalized * 2.0f);
            }

            // Draw output type indicator (Day 11)
            if (useAnimationOutput)
            {
                Gizmos.color = Color.blue;
                Vector3 outputLabelPos = transform.position + Vector3.up * 2.5f;
                UnityEditor.Handles.Label(outputLabelPos, "AnimationOutput");
            }

            // Draw Animator connection if using AnimationPlayableOutput
            if (useAnimationOutput && targetAnimator != null)
            {
                Gizmos.color = Color.green;
                Vector3 animatorPos = targetAnimator.transform.position;
                Gizmos.DrawLine(transform.position, animatorPos);
                Vector3 connectionLabelPos = (transform.position + animatorPos) * 0.5f + Vector3.up * 0.5f;
                UnityEditor.Handles.Label(connectionLabelPos, $"→ {targetAnimator.name}");
            }

            // Draw speed indicator (from Day 05)
            if (speedData.IsValidSpeedControl())
            {
                float currentSpeed = speedData.GetCurrentSpeed();
                Vector3 speedLabelPos = transform.position + Vector3.up * 2.0f;
                UnityEditor.Handles.Label(speedLabelPos, $"Speed: {currentSpeed:F2}x");
            }

            // Draw play state indicator (Day 06)
            if (playStateData.IsValidControl())
            {
                Gizmos.color = playStateData.IsPlaying() ? Color.green : Color.red;
                Vector3 stateLabelPos = transform.position + Vector3.up * 1.5f;
                UnityEditor.Handles.Label(stateLabelPos, $"State: {playStateData.GetStateString()}");
            }
        }
#endif
    }
}
