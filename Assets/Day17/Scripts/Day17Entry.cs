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
using PlayableLearn.Day12;
using PlayableLearn.Day16;

namespace PlayableLearn.Day17
{
    /// <summary>
    /// MonoBehaviour entry point for Day 17: Layering.
    /// Demonstrates hierarchical animation blending with AnimationLayerMixerPlayable.
    ///
    /// Key Concepts:
    /// - Layering: Hierarchical animation layers (base, additive, override)
    /// - Layer Weights: Controlling layer influence independently
    /// - Base Layer: Layer 0 is always the base layer (thumbody)
    /// - Additive/Override Layers: Layers 1+ are additive or override layers
    /// - Layer Independence: Each layer has its own weight (unlike mixer normalization)
    ///
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day17LayerHandle, Day12ClipHandle, Day16CrossfadeData, etc.)
    /// - Layer B: Operations (LayerMixerOps, ClipPlayableOps, CrossfadeOps, etc.)
    /// - Layer C: Extensions (Day17LayerExtensions, Day12ClipExtensions, Day16CrossfadeExtensions, etc.)
    ///
    /// Day 17 Focus: Hierarchical Animation Blending with Layers
    /// </summary>
    public class Day17Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day10DisposableGraph disposableGraph;

        [SerializeField]
        private Day17LayerHandle layerHandle;

        [SerializeField]
        private Day12ClipHandle baseClipHandle;

        [SerializeField]
        private Day12ClipHandle additiveClipHandle;

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

        // Animation clip references (for layering)
        [Header("Animation Clip Settings")]
        [SerializeField]
        private AnimationClip baseAnimationClip;

        [SerializeField]
        private AnimationClip additiveAnimationClip;

        // Animator reference (required for AnimationPlayableOutput)
        [Header("Animator Settings")]
        [SerializeField]
        private Animator targetAnimator;

        // Output type selection
        [Header("Output Type Settings")]
        [SerializeField]
        private bool useAnimationOutput = true;

        [SerializeField]
        private bool showLayerInfo = true;

        // Layer mixer settings
        [Header("Layer Mixer Settings")]
        [SerializeField]
        private int layerCount = 2;

        [SerializeField]
        private bool showLayerControls = true;

        // Layer weight settings
        [Header("Layer Weight Settings")]
        [SerializeField]
        private float baseLayerWeight = 1.0f;

        [SerializeField]
        private float additiveLayerWeight = 0.0f;

        // Clip playback settings
        [Header("Clip Playback Settings")]
        [SerializeField]
        private bool loopClips = true;

        [SerializeField]
        private bool autoPlayOnStart = true;

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
        private string customGraphName = "LayeringGraph";

        // Reverse time settings (from Day 07)
        [Header("Reverse Time Settings")]
        [SerializeField]
        private bool enableReverseTime = false;

        [SerializeField]
        private bool enableTimeWrapping = true;

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

        // Visualization
        [Header("Visualization")]
        [SerializeField]
        private Color layerActiveColor = Color.magenta;

        [SerializeField]
        private Color layerInactiveColor = Color.gray;

        [SerializeField]
        private Color layerErrorColor = Color.red;

        private void OnEnable()
        {
            ValidateAnimationClips();
            ValidateAnimator();
            ValidateLayerSettings();
            InitializeDisposableGraph();
            InitializeGraphNaming();
            InitializeVisualizer();
            InitializeLayerMixer();
            InitializeClipPlayables();
            InitializeOutput();
            ConnectClipsToLayers();
            InitializeRotator();
            InitializeSpeedControl();
            InitializeReverseTimeControl();
            InitializePlayStateControl();
            ConnectLayerMixerToOutput();
            PerformAutoLayerSetup();
            LogSystemInfo();
        }

        private void OnDisable()
        {
            CleanupVisualizer();
            CleanupPlayStateControl();
            CleanupReverseTimeControl();
            CleanupSpeedControl();
            CleanupRotator();
            CleanupLayerMixer();
            CleanupClipPlayables();
            CleanupOutput();
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

            // Update speed control
            if (speedData.IsValidSpeedControl())
            {
                float clampedSpeed = reverseData.IsValidReverseControl() && enableReverseTime
                    ? reverseData.ClampSpeed(-3.0f, 3.0f, 1.0f)
                    : 1.0f;

                if (math.abs(clampedSpeed - speedData.TargetSpeed) > 0.01f)
                {
                    var speed = speedData;
                    speed.SetTargetSpeed(clampedSpeed);
                    speedData = speed;
                }
            }

            // Update clip time wrapping
            if (loopClips)
            {
                UpdateClipTime(ref baseClipHandle);
                UpdateClipTime(ref additiveClipHandle);
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

            // Layer info display
            if (showLayerInfo && layerHandle.IsValidLayerMixer())
            {
                layerHandle.TryGetLayerWeight(0, out float baseWeight);
                layerHandle.TryGetLayerWeight(1, out float additiveWeight);

                string baseText = $"Base Layer: {baseWeight:F2}";
                GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), baseText);
                currentY += buttonHeight + padding;

                string additiveText = $"Additive Layer: {additiveWeight:F2}";
                GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), additiveText);
                currentY += buttonHeight + padding;

                if (layerHandle.IsLayerActive(1, 0.001f))
                {
                    string statusText = "Status: Additive Layer Active";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), statusText);
                }
                else
                {
                    string statusText = "Status: Base Layer Only";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), statusText);
                }
                currentY += buttonHeight + padding;
            }

            // Layer control buttons
            if (showLayerControls && layerHandle.IsValidLayerMixer())
            {
                // Enable additive layer
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Enable Additive Layer"))
                {
                    var layer = layerHandle;
                    layer.EnableLayer(1);
                    layerHandle = layer;
                }
                currentY += buttonHeight + padding;

                // Disable additive layer
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Disable Additive Layer"))
                {
                    var layer = layerHandle;
                    layer.DisableLayer(1);
                    layerHandle = layer;
                }
                currentY += buttonHeight + padding;

                // Set additive layer to 50%
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Additive Layer 50%"))
                {
                    var layer = layerHandle;
                    layer.SetLayerWeight(1, 0.5f);
                    layerHandle = layer;
                }
                currentY += buttonHeight + padding;

                // Reset to base layer
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Reset to Base Layer"))
                {
                    var layer = layerHandle;
                    layer.ResetToBaseLayer();
                    layerHandle = layer;
                }
                currentY += buttonHeight + padding;
            }

            // Play/Pause button
            if (playStateData.IsValidControl())
            {
                string playButtonText = playStateData.IsPlaying() ? "Pause" : "Play";
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), playButtonText))
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

        private void ValidateAnimationClips()
        {
            if (baseAnimationClip == null)
            {
                Debug.LogWarning("[Day 17] No BaseAnimationClip assigned. Layering will not work properly without clips.");
            }
            else
            {
                Debug.Log($"[Day 17] BaseAnimationClip assigned: {baseAnimationClip.name}");
            }

            if (additiveAnimationClip == null)
            {
                Debug.LogWarning("[Day 17] No AdditiveAnimationClip assigned. Layering will not work properly without clips.");
            }
            else
            {
                Debug.Log($"[Day 17] AdditiveAnimationClip assigned: {additiveAnimationClip.name}");
            }
        }

        private void ValidateAnimator()
        {
            if (useAnimationOutput && targetAnimator == null)
            {
                Debug.LogWarning("[Day 17] AnimationPlayableOutput enabled but no Animator assigned. Looking for Animator on this GameObject.");
                targetAnimator = GetComponent<Animator>();

                if (targetAnimator == null)
                {
                    Debug.LogError("[Day 17] No Animator component found. AnimationPlayableOutput will not work without an Animator.");
                }
                else
                {
                    Debug.Log($"[Day 17] Found Animator component: {targetAnimator.name}");
                }
            }
        }

        private void ValidateLayerSettings()
        {
            if (layerCount < 2)
            {
                Debug.LogWarning($"[Day 17] Layer count must be at least 2. Adjusting from {layerCount} to 2.");
                layerCount = 2;
            }

            if (layerCount > 16)
            {
                Debug.LogWarning($"[Day 17] Layer count capped at 16. Adjusting from {layerCount} to 16.");
                layerCount = 16;
            }

            baseLayerWeight = Mathf.Clamp01(baseLayerWeight);
            additiveLayerWeight = Mathf.Clamp01(additiveLayerWeight);
        }

        private void InitializeDisposableGraph()
        {
            if (!useDisposablePattern) return;

            string graphName = enableGraphNaming ? customGraphName : $"{gameObject.name}_LayeringGraph_{Time.frameCount}";
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
                Debug.LogError("[Day 17] Cannot initialize graph naming: Graph is not initialized.");
                return;
            }

            if (!enableGraphNaming) return;

            string graphName = string.IsNullOrEmpty(customGraphName) ? $"{gameObject.name}_LayeringGraph" : customGraphName;
            var namedGraph = namedGraphData;
            namedGraph.Initialize(in graph, graphName);
            namedGraphData = namedGraph;

            Debug.Log("[Day 17] Graph naming initialized.");
        }

        private void InitializeVisualizer()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 17] Cannot initialize visualizer: Graph is not initialized.");
                return;
            }

            var visualizer = visualizerData;
            visualizer.Initialize(in graph, VisualizationDetailLevel.Basic, VisualizerColorScheme.Default);
            visualizerData = visualizer;

            Debug.Log("[Day 17] Graph Visualizer initialized.");
        }

        private void InitializeLayerMixer()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 17] Cannot initialize layer mixer: Graph is not initialized.");
                return;
            }

            var layer = layerHandle;
            layer.Initialize(in graph, layerCount);
            layerHandle = layer;

            Debug.Log("[Day 17] AnimationLayerMixerPlayable initialized.");
        }

        private void InitializeClipPlayables()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 17] Cannot initialize clips: Graph is not initialized.");
                return;
            }

            // Initialize base clip
            if (baseAnimationClip != null)
            {
                var baseClip = baseClipHandle;
                baseClip.Initialize(in graph, baseAnimationClip);
                baseClipHandle = baseClip;
                Debug.Log("[Day 17] Base animation clip initialized.");
            }
            else
            {
                Debug.LogError("[Day 17] Cannot initialize base clip: BaseAnimationClip is null.");
            }

            // Initialize additive clip
            if (additiveAnimationClip != null)
            {
                var additiveClip = additiveClipHandle;
                additiveClip.Initialize(in graph, additiveAnimationClip);
                additiveClipHandle = additiveClip;
                Debug.Log("[Day 17] Additive animation clip initialized.");
            }
            else
            {
                Debug.LogError("[Day 17] Cannot initialize additive clip: AdditiveAnimationClip is null.");
            }

            // Set initial play state
            if (!autoPlayOnStart)
            {
                baseClipHandle.SetPlayState(false);
                additiveClipHandle.SetPlayState(false);
            }
        }

        private void InitializeOutput()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 17] Cannot initialize output: Graph is not initialized.");
                return;
            }

            if (useAnimationOutput)
            {
                if (targetAnimator != null)
                {
                    string outputName = $"{gameObject.name}_AnimOutput";
                    var animOutput = animOutputHandle;
                    animOutput.Initialize(in graph, outputName, targetAnimator);
                    animOutputHandle = animOutput;

                    Debug.Log("[Day 17] AnimationPlayableOutput initialized.");
                }
                else
                {
                    Debug.LogError("[Day 17] Cannot initialize AnimationPlayableOutput: No Animator assigned.");
                }
            }
            else
            {
                string outputName = $"{gameObject.name}_ScriptOutput";
                var scriptOutput = scriptOutputHandle;
                scriptOutput.Initialize(in graph, outputName);
                scriptOutputHandle = scriptOutput;

                Debug.Log("[Day 17] ScriptPlayableOutput initialized.");
            }
        }

        private void ConnectClipsToLayers()
        {
            if (!layerHandle.IsValidLayerMixer())
            {
                Debug.LogError("[Day 17] Cannot connect clips: Layer mixer is not valid.");
                return;
            }

            // Connect base clip to layer 0 (base layer)
            if (baseClipHandle.IsValidClip() && baseClipHandle.TryGetPlayable(out Playable basePlayable))
            {
                layerHandle.ConnectLayer(ref basePlayable, 0, baseLayerWeight);
                Debug.Log("[Day 17] Connected base clip to layer 0 (base layer) with weight 1.0.");
            }
            else
            {
                Debug.LogWarning("[Day 17] Cannot connect base clip: Base clip is not valid.");
            }

            // Connect additive clip to layer 1 (additive/override layer)
            if (additiveClipHandle.IsValidClip() && additiveClipHandle.TryGetPlayable(out Playable additivePlayable))
            {
                layerHandle.ConnectLayer(ref additivePlayable, 1, additiveLayerWeight);
                Debug.Log("[Day 17] Connected additive clip to layer 1 (additive layer) with weight 0.0.");
            }
            else
            {
                Debug.LogWarning("[Day 17] Cannot connect additive clip: Additive clip is not valid.");
            }
        }

        private void PerformAutoLayerSetup()
        {
            // No auto layer setup on start - let user control it via GUI
            Debug.Log("[Day 17] Layer system ready. Use GUI buttons to control layer weights.");
        }

        private void ConnectLayerMixerToOutput()
        {
            if (!layerHandle.IsValidLayerMixer())
            {
                Debug.LogError("[Day 17] Cannot connect layer mixer: Layer mixer is not valid.");
                return;
            }

            if (useAnimationOutput && animOutputHandle.IsValidOutput())
            {
                if (animOutputHandle.TryGetPlayableOutput(out PlayableOutput output))
                {
                    layerHandle.ConnectToOutput(in output);
                    Debug.Log("[Day 17] Successfully linked layer mixer to AnimationPlayableOutput!");
                }
            }
            else if (!useAnimationOutput && scriptOutputHandle.IsValidOutput())
            {
                PlayableOutput output = scriptOutputHandle.Output;
                layerHandle.ConnectToOutput(in output);
                Debug.Log("[Day 17] Successfully linked layer mixer to ScriptPlayableOutput!");
            }
            else
            {
                Debug.LogWarning("[Day 17] No valid output available for linking.");
            }
        }

        private void InitializeRotator()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 17] Cannot initialize rotator: Graph is not initialized.");
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
                Debug.LogError("[Day 17] Cannot initialize speed control: Graph is not initialized.");
                return;
            }

            string speedControlName = $"{gameObject.name}_SpeedControl";
            var speed = speedData;
            speed.Initialize(in graph, speedControlName, initialSpeedMultiplier, enableTimeDilation, 2.0f);
            speedData = speed;
        }

        private void InitializeReverseTimeControl()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 17] Cannot initialize reverse time control: Graph is not initialized.");
                return;
            }

            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 17] Cannot initialize reverse time control: Speed control is not initialized.");
                return;
            }

            string reverseControlName = $"{gameObject.name}_ReverseTimeControl";
            var reverse = reverseData;
            reverse.Initialize(in graph, reverseControlName, enableReverseTime, enableTimeWrapping);
            reverseData = reverse;

            Debug.Log("[Day 17] Reverse time control initialized.");
        }

        private void InitializePlayStateControl()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 17] Cannot initialize PlayState control: Graph is not initialized.");
                return;
            }

            string playStateControllerName = $"{gameObject.name}_PlayStateController";
            var playState = playStateData;
            playState.Initialize(in graph, playStateControllerName, autoPlayStateControl && autoPlayOnStart);
            playStateData = playState;

            Debug.Log("[Day 17] PlayState control initialized.");
        }

        private void UpdateClipTime(ref Day12ClipHandle clipHandle)
        {
            if (!clipHandle.IsValidClip()) return;

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
            Debug.Log("[Day 17] === System Information ===");

            Debug.Log($"[Day 17] Output Type: {(useAnimationOutput ? "AnimationPlayableOutput" : "ScriptPlayableOutput")}");

            if (layerHandle.IsValidLayerMixer())
            {
                layerHandle.LogToConsole("LayerMixer");
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

            Debug.Log("[Day 17] === End System Information ===");
        }

        private void UpdateVisualization()
        {
            Color targetColor;
            if (!layerHandle.IsValidLayerMixer())
            {
                targetColor = layerErrorColor;
            }
            else if (!playStateData.IsPlaying())
            {
                targetColor = layerInactiveColor;
            }
            else if (layerHandle.IsLayerActive(1, 0.001f))
            {
                targetColor = layerActiveColor;
            }
            else
            {
                targetColor = layerInactiveColor;
            }

            GetComponent<Renderer>().material.color = targetColor;

            // Scale effect based on additive layer weight
            if (layerHandle.IsValidLayerMixer() && layerHandle.TryGetLayerWeight(1, out float additiveWeight))
            {
                float scale = 1.0f + (additiveWeight * 0.3f);
                transform.localScale = Vector3.one * scale;
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

        private void CleanupLayerMixer()
        {
            var layer = layerHandle;
            layer.Dispose();
            layerHandle = layer;
        }

        private void CleanupClipPlayables()
        {
            var baseClip = baseClipHandle;
            baseClip.Dispose();
            baseClipHandle = baseClip;

            var additiveClip = additiveClipHandle;
            additiveClip.Dispose();
            additiveClipHandle = additiveClip;
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

        private void CleanupDisposableGraph()
        {
            if (!useDisposablePattern) return;

            var disposable = disposableGraph;
            disposable.Dispose();
            disposableGraph = disposable;

            if (enableDisposalLogging)
            {
                Debug.Log("[Day 17] Auto-disposed graph on OnDisable");
            }
        }

        private void Reset()
        {
            // Defaults
            useDisposablePattern = true;
            enableDisposalLogging = true;
            enableGraphNaming = true;
            customGraphName = "LayeringGraph";
            useAnimationOutput = true;
            showLayerInfo = true;
            layerCount = 2;
            baseLayerWeight = 1.0f;
            additiveLayerWeight = 0.0f;
            showLayerControls = true;
            loopClips = true;
            autoPlayOnStart = true;

            // Try to find Animator on this GameObject
            targetAnimator = GetComponent<Animator>();

            // Other defaults
            enableReverseTime = false;
            enableTimeWrapping = true;
            autoPlayStateControl = true;
            initialSpeedMultiplier = 1.0f;
            enableTimeDilation = true;
            rotationSpeed = 90.0f;
            rotationAxis = Vector3.up;
            layerActiveColor = Color.magenta;
            layerInactiveColor = Color.gray;
            layerErrorColor = Color.red;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw layer info (Day 17)
            if (layerHandle.IsValidLayerMixer())
            {
                Gizmos.color = layerHandle.IsLayerActive(1, 0.001f) ? layerActiveColor : layerInactiveColor;
                Vector3 layerLabelPos = transform.position + Vector3.up * 4.0f;
                UnityEditor.Handles.Label(layerLabelPos, $"LayerMixer");

                if (layerHandle.TryGetLayerWeight(0, out float baseWeight) && layerHandle.TryGetLayerWeight(1, out float additiveWeight))
                {
                    Vector3 weightLabelPos = transform.position + Vector3.up * 3.5f;
                    UnityEditor.Handles.Label(weightLabelPos, $"Layers: Base={baseWeight:F2}, Additive={additiveWeight:F2}");
                }
            }

            // Draw output type indicator (Day 11)
            if (useAnimationOutput)
            {
                Gizmos.color = Color.blue;
                Vector3 outputLabelPos = transform.position + Vector3.up * 2.0f;
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
                Vector3 speedLabelPos = transform.position + Vector3.up * 1.5f;
                UnityEditor.Handles.Label(speedLabelPos, $"Speed: {currentSpeed:F2}x");
            }

            // Draw play state indicator (Day 06)
            if (playStateData.IsValidControl())
            {
                Gizmos.color = playStateData.IsPlaying() ? Color.green : Color.red;
                Vector3 stateLabelPos = transform.position + Vector3.up * 1.0f;
                UnityEditor.Handles.Label(stateLabelPos, $"State: {playStateData.GetStateString()}");
            }
        }
#endif
    }
}
