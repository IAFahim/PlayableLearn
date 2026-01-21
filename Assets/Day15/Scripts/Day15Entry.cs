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
using PlayableLearn.Day13;
using Unity.Mathematics;

namespace PlayableLearn.Day15
{
    /// <summary>
    /// MonoBehaviour entry point for Day 15: Weighted Blending.
    /// Demonstrates setting explicit weights (e.g., 0.5/0.5) on mixer inputs.
    ///
    /// Key Concepts:
    /// - Weighted blending: Set explicit weights on mixer inputs
    /// - Equal blending: 0.5/0.5 split between two animations
    /// - Smooth transitions: Gradually interpolate between weights
    /// - Weight normalization: Ensure weights sum to 1.0
    ///
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day15BlendData, Day13MixerHandle, Day12ClipHandle, etc.)
    /// - Layer B: Operations (WeightBlendOps, MixerOps, ClipPlayableOps, etc.)
    /// - Layer C: Extensions (Day15BlendExtensions, Day13MixerExtensions, Day12ClipExtensions, etc.)
    ///
    /// Day 15 Focus: Setting Input Weights 0.5/0.5
    /// </summary>
    public class Day15Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day10DisposableGraph disposableGraph;

        [SerializeField]
        private Day13MixerHandle mixerHandle;

        [SerializeField]
        private Day12ClipHandle clipHandle1;

        [SerializeField]
        private Day12ClipHandle clipHandle2;

        [SerializeField]
        private Day15BlendData blendData;

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

        // Animation clip references (for weighted blending)
        [Header("Animation Clip Settings")]
        [SerializeField]
        private AnimationClip animationClip1;

        [SerializeField]
        private AnimationClip animationClip2;

        // Animator reference (required for AnimationPlayableOutput)
        [Header("Animator Settings")]
        [SerializeField]
        private Animator targetAnimator;

        // Output type selection
        [Header("Output Type Settings")]
        [SerializeField]
        private bool useAnimationOutput = true;

        [SerializeField]
        private bool showBlendInfo = true;

        // Mixer settings
        [Header("Mixer Settings")]
        [SerializeField]
        private int mixerInputCount = 2;

        // Weighted blend settings
        [Header("Weighted Blend Settings")]
        [SerializeField]
        private bool autoSetEqualBlendOnStart = true;

        [SerializeField]
        private bool enableSmoothTransitions = true;

        [SerializeField]
        private float blendSpeed = 2.0f;

        [SerializeField]
        private bool normalizeWeights = false;

        [SerializeField]
        private bool showWeightControls = true;

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
        private string customGraphName = "WeightedBlendGraph";

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
        private Color blendActiveColor = Color.magenta;

        [SerializeField]
        private Color blendInactiveColor = Color.gray;

        [SerializeField]
        private Color blendErrorColor = Color.red;

        private void OnEnable()
        {
            ValidateAnimationClips();
            ValidateAnimator();
            ValidateMixerSettings();
            ValidateBlendSettings();
            InitializeDisposableGraph();
            InitializeGraphNaming();
            InitializeVisualizer();
            InitializeMixer();
            InitializeClipPlayables();
            InitializeOutput();
            InitializeBlendData();
            ConnectClipsToMixer();
            InitializeRotator();
            InitializeSpeedControl();
            InitializeReverseTimeControl();
            InitializePlayStateControl();
            ConnectMixerToOutput();
            PerformAutoBlend();
            LogSystemInfo();
        }

        private void OnDisable()
        {
            CleanupVisualizer();
            CleanupPlayStateControl();
            CleanupReverseTimeControl();
            CleanupSpeedControl();
            CleanupRotator();
            CleanupBlendData();
            CleanupMixer();
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
                    speedData.SetTargetSpeed(clampedSpeed);
                }
            }

            // Update clip time wrapping
            if (loopClips)
            {
                UpdateClipTime(ref clipHandle1);
                UpdateClipTime(ref clipHandle2);
            }

            // Update weighted blend (smooth transitions)
            if (blendData.IsValidBlend() && enableSmoothTransitions)
            {
                blendData.UpdateWeights(Time.deltaTime);
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

            // Blend info display
            if (showBlendInfo && blendData.IsValidBlend())
            {
                if (blendData.TryGetBlend(out float currentBlend))
                {
                    string blendText = $"Blend: {currentBlend:F2}";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), blendText);
                    currentY += buttonHeight + padding;

                    blendData.TryGetWeight(0, out float weight0);
                    blendData.TryGetWeight(1, out float weight1);
                    string weightText = $"Weights: {weight0:F2} / {weight1:F2}";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), weightText);
                    currentY += buttonHeight + padding;

                    string equalText = blendData.IsEqualBlend() ? "Equal Blend: Yes" : "Equal Blend: No";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), equalText);
                    currentY += buttonHeight + padding;
                }
            }

            // Weight control buttons
            if (showWeightControls && blendData.IsValidBlend())
            {
                // Equal blend button (0.5/0.5)
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Set Equal Blend (0.5/0.5)"))
                {
                    var blend = blendData;
                    blend.SetEqualBlend(!enableSmoothTransitions);
                    blendData = blend;
                }
                currentY += buttonHeight + padding;

                // Input 0 only
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Input 0 Only (1.0/0.0)"))
                {
                    var blend = blendData;
                    blend.SetWeights(1.0f, 0.0f, !enableSmoothTransitions);
                    blendData = blend;
                }
                currentY += buttonHeight + padding;

                // Input 1 only
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Input 1 Only (0.0/1.0)"))
                {
                    var blend = blendData;
                    blend.SetWeights(0.0f, 1.0f, !enableSmoothTransitions);
                    blendData = blend;
                }
                currentY += buttonHeight + padding;

                // Custom blend
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Custom 0.7/0.3 Blend"))
                {
                    var blend = blendData;
                    blend.SetWeights(0.7f, 0.3f, !enableSmoothTransitions);
                    blendData = blend;
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
            if (animationClip1 == null)
            {
                Debug.LogWarning("[Day 15] No AnimationClip1 assigned. Weighted blend will not work properly without clips.");
            }
            else
            {
                Debug.Log($"[Day 15] AnimationClip1 assigned: {animationClip1.name}");
            }

            if (animationClip2 == null)
            {
                Debug.LogWarning("[Day 15] No AnimationClip2 assigned. Weighted blend will not work properly without clips.");
            }
            else
            {
                Debug.Log($"[Day 15] AnimationClip2 assigned: {animationClip2.name}");
            }
        }

        private void ValidateAnimator()
        {
            if (useAnimationOutput && targetAnimator == null)
            {
                Debug.LogWarning("[Day 15] AnimationPlayableOutput enabled but no Animator assigned. Looking for Animator on this GameObject.");
                targetAnimator = GetComponent<Animator>();

                if (targetAnimator == null)
                {
                    Debug.LogError("[Day 15] No Animator component found. AnimationPlayableOutput will not work without an Animator.");
                }
                else
                {
                    Debug.Log($"[Day 15] Found Animator component: {targetAnimator.name}");
                }
            }
        }

        private void ValidateMixerSettings()
        {
            if (mixerInputCount < 2)
            {
                Debug.LogWarning($"[Day 15] Mixer input count must be at least 2. Adjusting from {mixerInputCount} to 2.");
                mixerInputCount = 2;
            }

            if (mixerInputCount > 16)
            {
                Debug.LogWarning($"[Day 15] Mixer input count capped at 16. Adjusting from {mixerInputCount} to 16.");
                mixerInputCount = 16;
            }
        }

        private void ValidateBlendSettings()
        {
            blendSpeed = Mathf.Max(0.0f, blendSpeed);

            if (blendSpeed < 0.0f)
            {
                Debug.LogWarning($"[Day 15] Blend speed should be non-negative. Adjusting to 0.0.");
            }
        }

        private void InitializeDisposableGraph()
        {
            if (!useDisposablePattern) return;

            string graphName = enableGraphNaming ? customGraphName : $"{gameObject.name}_WeightedBlendGraph_{Time.frameCount}";
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
                Debug.LogError("[Day 15] Cannot initialize graph naming: Graph is not initialized.");
                return;
            }

            if (!enableGraphNaming) return;

            string graphName = string.IsNullOrEmpty(customGraphName) ? $"{gameObject.name}_WeightedBlendGraph" : customGraphName;
            var namedGraph = namedGraphData;
            namedGraph.Initialize(in graph, graphName);
            namedGraphData = namedGraph;

            Debug.Log("[Day 15] Graph naming initialized.");
        }

        private void InitializeVisualizer()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 15] Cannot initialize visualizer: Graph is not initialized.");
                return;
            }

            var visualizer = visualizerData;
            visualizer.Initialize(in graph, VisualizationDetailLevel.Basic, VisualizerColorScheme.Default);
            visualizerData = visualizer;

            Debug.Log("[Day 15] Graph Visualizer initialized.");
        }

        private void InitializeMixer()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 15] Cannot initialize mixer: Graph is not initialized.");
                return;
            }

            var mixer = mixerHandle;
            mixer.Initialize(in graph, mixerInputCount);
            mixerHandle = mixer;

            Debug.Log("[Day 15] AnimationMixerPlayable initialized.");
        }

        private void InitializeClipPlayables()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 15] Cannot initialize clips: Graph is not initialized.");
                return;
            }

            // Initialize first clip
            if (animationClip1 != null)
            {
                var clip1 = clipHandle1;
                clip1.Initialize(in graph, animationClip1);
                clipHandle1 = clip1;
                Debug.Log("[Day 15] AnimationClip1 initialized.");
            }
            else
            {
                Debug.LogError("[Day 15] Cannot initialize clip1: AnimationClip1 is null.");
            }

            // Initialize second clip
            if (animationClip2 != null)
            {
                var clip2 = clipHandle2;
                clip2.Initialize(in graph, animationClip2);
                clipHandle2 = clip2;
                Debug.Log("[Day 15] AnimationClip2 initialized.");
            }
            else
            {
                Debug.LogError("[Day 15] Cannot initialize clip2: AnimationClip2 is null.");
            }

            // Set initial play state
            if (!autoPlayOnStart)
            {
                clipHandle1.SetPlayState(false);
                clipHandle2.SetPlayState(false);
            }
        }

        private void InitializeOutput()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 15] Cannot initialize output: Graph is not initialized.");
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

                    Debug.Log("[Day 15] AnimationPlayableOutput initialized.");
                }
                else
                {
                    Debug.LogError("[Day 15] Cannot initialize AnimationPlayableOutput: No Animator assigned.");
                }
            }
            else
            {
                string outputName = $"{gameObject.name}_ScriptOutput";
                var scriptOutput = scriptOutputHandle;
                scriptOutput.Initialize(in graph, outputName);
                scriptOutputHandle = scriptOutput;

                Debug.Log("[Day 15] ScriptPlayableOutput initialized.");
            }
        }

        private void InitializeBlendData()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 15] Cannot initialize blend data: Graph is not initialized.");
                return;
            }

            if (!mixerHandle.IsValidMixer())
            {
                Debug.LogError("[Day 15] Cannot initialize blend data: Mixer is not valid.");
                return;
            }

            if (!mixerHandle.TryGetPlayable(out Playable mixerPlayable))
            {
                Debug.LogError("[Day 15] Cannot initialize blend data: Failed to get mixer playable.");
                return;
            }

            var blend = blendData;
            blend.Initialize(in graph, in mixerPlayable, mixerInputCount);
            blend.SetBlendSpeed(blendSpeed);
            blend.SetNormalizeWeights(normalizeWeights);
            blendData = blend;

            Debug.Log("[Day 15] Weighted blend data initialized.");
        }

        private void ConnectClipsToMixer()
        {
            if (!mixerHandle.IsValidMixer())
            {
                Debug.LogError("[Day 15] Cannot connect clips: Mixer is not valid.");
                return;
            }

            // Connect clip 1 to mixer input 0
            if (clipHandle1.IsValidClip() && clipHandle1.TryGetPlayable(out Playable playable1))
            {
                mixerHandle.ConnectInput(ref playable1, 0, 1.0f);
                Debug.Log("[Day 15] Connected clip1 to mixer input 0 with weight 1.0.");
            }
            else
            {
                Debug.LogWarning("[Day 15] Cannot connect clip1: Clip1 is not valid.");
            }

            // Connect clip 2 to mixer input 1
            if (clipHandle2.IsValidClip() && clipHandle2.TryGetPlayable(out Playable playable2))
            {
                mixerHandle.ConnectInput(ref playable2, 1, 0.0f);
                Debug.Log("[Day 15] Connected clip2 to mixer input 1 with weight 0.0.");
            }
            else
            {
                Debug.LogWarning("[Day 15] Cannot connect clip2: Clip2 is not valid.");
            }
        }

        private void PerformAutoBlend()
        {
            if (!autoSetEqualBlendOnStart || !blendData.IsValidBlend())
            {
                return;
            }

            // Set equal blend (0.5/0.5) on start
            blendData.SetEqualBlend(!enableSmoothTransitions);
            Debug.Log("[Day 15] Set equal blend (0.5/0.5) on start.");
        }

        private void ConnectMixerToOutput()
        {
            if (!mixerHandle.IsValidMixer())
            {
                Debug.LogError("[Day 15] Cannot connect mixer: Mixer is not valid.");
                return;
            }

            if (useAnimationOutput && animOutputHandle.IsValidOutput())
            {
                if (animOutputHandle.TryGetPlayableOutput(out PlayableOutput output))
                {
                    mixerHandle.ConnectToOutput(in output);
                    Debug.Log("[Day 15] Successfully linked mixer to AnimationPlayableOutput!");
                }
            }
            else if (!useAnimationOutput && scriptOutputHandle.IsValidOutput())
            {
                PlayableOutput output = scriptOutputHandle.Output;
                mixerHandle.ConnectToOutput(in output);
                Debug.Log("[Day 15] Successfully linked mixer to ScriptPlayableOutput!");
            }
            else
            {
                Debug.LogWarning("[Day 15] No valid output available for linking.");
            }
        }

        private void InitializeRotator()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 15] Cannot initialize rotator: Graph is not initialized.");
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
                Debug.LogError("[Day 15] Cannot initialize speed control: Graph is not initialized.");
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
                Debug.LogError("[Day 15] Cannot initialize reverse time control: Graph is not initialized.");
                return;
            }

            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 15] Cannot initialize reverse time control: Speed control is not initialized.");
                return;
            }

            string reverseControlName = $"{gameObject.name}_ReverseTimeControl";
            var reverse = reverseData;
            reverse.Initialize(in graph, reverseControlName, enableReverseTime, enableTimeWrapping);
            reverseData = reverse;

            Debug.Log("[Day 15] Reverse time control initialized.");
        }

        private void InitializePlayStateControl()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 15] Cannot initialize PlayState control: Graph is not initialized.");
                return;
            }

            string playStateControllerName = $"{gameObject.name}_PlayStateController";
            var playState = playStateData;
            playState.Initialize(in graph, playStateControllerName, autoPlayStateControl && autoPlayOnStart);
            playStateData = playState;

            Debug.Log("[Day 15] PlayState control initialized.");
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
            Debug.Log("[Day 15] === System Information ===");

            Debug.Log($"[Day 15] Output Type: {(useAnimationOutput ? "AnimationPlayableOutput" : "ScriptPlayableOutput")}");

            if (mixerHandle.IsValidMixer())
            {
                mixerHandle.LogToConsole("Mixer");
            }

            if (blendData.IsValidBlend())
            {
                blendData.LogToConsole("Weighted Blend");
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

            Debug.Log("[Day 15] === End System Information ===");
        }

        private void UpdateVisualization()
        {
            Color targetColor;
            if (!blendData.IsValidBlend())
            {
                targetColor = blendErrorColor;
            }
            else if (!playStateData.IsPlaying())
            {
                targetColor = blendInactiveColor;
            }
            else
            {
                targetColor = blendActiveColor;
            }

            GetComponent<Renderer>().material.color = targetColor;

            // Scale effect based on equal blend
            if (blendData.IsValidBlend())
            {
                float scale = blendData.IsEqualBlend() ? 1.3f : 1.0f;
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

        private void CleanupBlendData()
        {
            var blend = blendData;
            blend.Dispose();
            blendData = blend;
        }

        private void CleanupMixer()
        {
            var mixer = mixerHandle;
            mixer.Dispose();
            mixerHandle = mixer;
        }

        private void CleanupClipPlayables()
        {
            var clip1 = clipHandle1;
            clip1.Dispose();
            clipHandle1 = clip1;

            var clip2 = clipHandle2;
            clip2.Dispose();
            clipHandle2 = clip2;
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
                Debug.Log("[Day 15] Auto-disposed graph on OnDisable");
            }
        }

        private void Reset()
        {
            // Defaults
            useDisposablePattern = true;
            enableDisposalLogging = true;
            enableGraphNaming = true;
            customGraphName = "WeightedBlendGraph";
            useAnimationOutput = true;
            showBlendInfo = true;
            mixerInputCount = 2;
            autoSetEqualBlendOnStart = true;
            enableSmoothTransitions = true;
            blendSpeed = 2.0f;
            normalizeWeights = false;
            showWeightControls = true;
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
            blendActiveColor = Color.magenta;
            blendInactiveColor = Color.gray;
            blendErrorColor = Color.red;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw blend info (Day 15)
            if (blendData.IsValidBlend())
            {
                Gizmos.color = playStateData.IsPlaying() ? blendActiveColor : blendInactiveColor;
                Vector3 blendLabelPos = transform.position + Vector3.up * 4.0f;
                UnityEditor.Handles.Label(blendLabelPos, $"Weighted Blend");

                if (blendData.TryGetWeight(0, out float w0) && blendData.TryGetWeight(1, out float w1))
                {
                    Vector3 weightLabelPos = transform.position + Vector3.up * 3.5f;
                    UnityEditor.Handles.Label(weightLabelPos, $"Weights: {w0:F2} / {w1:F2}");
                }

                Vector3 equalLabelPos = transform.position + Vector3.up * 3.0f;
                UnityEditor.Handles.Label(equalLabelPos, $"Equal Blend: {blendData.IsEqualBlend()}");
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
