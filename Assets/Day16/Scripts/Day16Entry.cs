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

namespace PlayableLearn.Day16
{
    /// <summary>
    /// MonoBehaviour entry point for Day 16: Crossfade Logic.
    /// Demonstrates time-based weight transitions using lerping over time.
    ///
    /// Key Concepts:
    /// - Crossfade: Time-based weight transitions between animations
    /// - Lerping: Linear interpolation with easing curves
    /// - Duration-based transitions: Configurable crossfade duration
    /// - Progress tracking: Monitor crossfade completion
    /// - Multiple easing curves: Linear, Ease-In, Ease-Out, Ease-In-Out
    ///
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day16CrossfadeData, Day13MixerHandle, Day12ClipHandle, etc.)
    /// - Layer B: Operations (CrossfadeOps, MixerOps, ClipPlayableOps, etc.)
    /// - Layer C: Extensions (Day16CrossfadeExtensions, Day13MixerExtensions, Day12ClipExtensions, etc.)
    ///
    /// Day 16 Focus: Time-Based Crossfade Transitions with Math for Lerping Weights
    /// </summary>
    public class Day16Entry : MonoBehaviour
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
        private Day16CrossfadeData crossfadeData;

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

        // Animation clip references (for crossfading)
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
        private bool showCrossfadeInfo = true;

        // Mixer settings
        [Header("Mixer Settings")]
        [SerializeField]
        private int mixerInputCount = 2;

        // Crossfade settings
        [Header("Crossfade Settings")]
        [SerializeField]
        private float defaultCrossfadeDuration = 1.0f;

        [SerializeField]
        private CrossfadeOps.CrossfadeCurve defaultCurveType = CrossfadeOps.CrossfadeCurve.EaseInOut;

        [SerializeField]
        private bool normalizeWeights = false;

        [SerializeField]
        private bool showCrossfadeControls = true;

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
        private string customGraphName = "CrossfadeGraph";

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
        private Color crossfadeActiveColor = Color.cyan;

        [SerializeField]
        private Color crossfadeInactiveColor = Color.gray;

        [SerializeField]
        private Color crossfadeErrorColor = Color.red;

        private void OnEnable()
        {
            ValidateAnimationClips();
            ValidateAnimator();
            ValidateMixerSettings();
            ValidateCrossfadeSettings();
            InitializeDisposableGraph();
            InitializeGraphNaming();
            InitializeVisualizer();
            InitializeMixer();
            InitializeClipPlayables();
            InitializeOutput();
            InitializeCrossfadeData();
            ConnectClipsToMixer();
            InitializeRotator();
            InitializeSpeedControl();
            InitializeReverseTimeControl();
            InitializePlayStateControl();
            ConnectMixerToOutput();
            PerformAutoCrossfade();
            LogSystemInfo();
        }

        private void OnDisable()
        {
            CleanupVisualizer();
            CleanupPlayStateControl();
            CleanupReverseTimeControl();
            CleanupSpeedControl();
            CleanupRotator();
            CleanupCrossfadeData();
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

            // Update crossfade (time-based weight transitions)
            if (crossfadeData.IsValidCrossfade())
            {
                crossfadeData.UpdateCrossfade(Time.deltaTime);
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

            // Crossfade info display
            if (showCrossfadeInfo && crossfadeData.IsValidCrossfade())
            {
                if (crossfadeData.TryGetProgress(out float progress))
                {
                    string progressText = $"Progress: {progress:P2}";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), progressText);
                    currentY += buttonHeight + padding;
                }

                crossfadeData.TryGetWeight(0, out float weight0);
                crossfadeData.TryGetWeight(1, out float weight1);
                string weightText = $"Weights: {weight0:F2} / {weight1:F2}";
                GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), weightText);
                currentY += buttonHeight + padding;

                if (crossfadeData.IsCrossfading())
                {
                    crossfadeData.TryGetTimeRemaining(out float timeRemaining);
                    string timeText = $"Time Remaining: {timeRemaining:F2}s";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), timeText);
                    currentY += buttonHeight + padding;
                }

                string statusText = crossfadeData.IsCrossfading() ? "Status: Crossfading" : "Status: Idle";
                GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), statusText);
                currentY += buttonHeight + padding;
            }

            // Crossfade control buttons
            if (showCrossfadeControls && crossfadeData.IsValidCrossfade())
            {
                // Crossfade to input 0 only
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Crossfade to Input 0"))
                {
                    var crossfade = crossfadeData;
                    crossfade.StartCrossfade(1.0f, 0.0f, defaultCrossfadeDuration, defaultCurveType);
                    crossfadeData = crossfade;
                }
                currentY += buttonHeight + padding;

                // Crossfade to input 1 only
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Crossfade to Input 1"))
                {
                    var crossfade = crossfadeData;
                    crossfade.StartCrossfade(0.0f, 1.0f, defaultCrossfadeDuration, defaultCurveType);
                    crossfadeData = crossfade;
                }
                currentY += buttonHeight + padding;

                // Crossfade to equal blend
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Crossfade to Equal (0.5/0.5)"))
                {
                    var crossfade = crossfadeData;
                    crossfade.StartCrossfade(0.5f, 0.5f, defaultCrossfadeDuration, defaultCurveType);
                    crossfadeData = crossfade;
                }
                currentY += buttonHeight + padding;

                // Quick crossfade (0.25s)
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Quick Crossfade (0.25s)"))
                {
                    var crossfade = crossfadeData;
                    crossfade.StartCrossfade(0.0f, 1.0f, 0.25f, CrossfadeOps.CrossfadeCurve.EaseOut);
                    crossfadeData = crossfade;
                }
                currentY += buttonHeight + padding;

                // Long crossfade (2.0s)
                if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Slow Crossfade (2.0s)"))
                {
                    var crossfade = crossfadeData;
                    crossfade.StartCrossfade(0.0f, 1.0f, 2.0f, CrossfadeOps.CrossfadeCurve.EaseInOut);
                    crossfadeData = crossfade;
                }
                currentY += buttonHeight + padding;

                // Stop crossfade
                if (crossfadeData.IsCrossfading())
                {
                    if (GUI.Button(new Rect(startX, currentY, buttonWidth, buttonHeight), "Stop Crossfade"))
                    {
                        var crossfade = crossfadeData;
                        crossfade.StopCrossfade();
                        crossfadeData = crossfade;
                    }
                    currentY += buttonHeight + padding;
                }
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
                Debug.LogWarning("[Day 16] No AnimationClip1 assigned. Crossfade will not work properly without clips.");
            }
            else
            {
                Debug.Log($"[Day 16] AnimationClip1 assigned: {animationClip1.name}");
            }

            if (animationClip2 == null)
            {
                Debug.LogWarning("[Day 16] No AnimationClip2 assigned. Crossfade will not work properly without clips.");
            }
            else
            {
                Debug.Log($"[Day 16] AnimationClip2 assigned: {animationClip2.name}");
            }
        }

        private void ValidateAnimator()
        {
            if (useAnimationOutput && targetAnimator == null)
            {
                Debug.LogWarning("[Day 16] AnimationPlayableOutput enabled but no Animator assigned. Looking for Animator on this GameObject.");
                targetAnimator = GetComponent<Animator>();

                if (targetAnimator == null)
                {
                    Debug.LogError("[Day 16] No Animator component found. AnimationPlayableOutput will not work without an Animator.");
                }
                else
                {
                    Debug.Log($"[Day 16] Found Animator component: {targetAnimator.name}");
                }
            }
        }

        private void ValidateMixerSettings()
        {
            if (mixerInputCount < 2)
            {
                Debug.LogWarning($"[Day 16] Mixer input count must be at least 2. Adjusting from {mixerInputCount} to 2.");
                mixerInputCount = 2;
            }

            if (mixerInputCount > 16)
            {
                Debug.LogWarning($"[Day 16] Mixer input count capped at 16. Adjusting from {mixerInputCount} to 16.");
                mixerInputCount = 16;
            }
        }

        private void ValidateCrossfadeSettings()
        {
            defaultCrossfadeDuration = Mathf.Max(0.001f, defaultCrossfadeDuration);

            if (defaultCrossfadeDuration < 0.0f)
            {
                Debug.LogWarning($"[Day 16] Crossfade duration should be positive. Adjusting to 1.0.");
                defaultCrossfadeDuration = 1.0f;
            }
        }

        private void InitializeDisposableGraph()
        {
            if (!useDisposablePattern) return;

            string graphName = enableGraphNaming ? customGraphName : $"{gameObject.name}_CrossfadeGraph_{Time.frameCount}";
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
                Debug.LogError("[Day 16] Cannot initialize graph naming: Graph is not initialized.");
                return;
            }

            if (!enableGraphNaming) return;

            string graphName = string.IsNullOrEmpty(customGraphName) ? $"{gameObject.name}_CrossfadeGraph" : customGraphName;
            var namedGraph = namedGraphData;
            namedGraph.Initialize(in graph, graphName);
            namedGraphData = namedGraph;

            Debug.Log("[Day 16] Graph naming initialized.");
        }

        private void InitializeVisualizer()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 16] Cannot initialize visualizer: Graph is not initialized.");
                return;
            }

            var visualizer = visualizerData;
            visualizer.Initialize(in graph, VisualizationDetailLevel.Basic, VisualizerColorScheme.Default);
            visualizerData = visualizer;

            Debug.Log("[Day 16] Graph Visualizer initialized.");
        }

        private void InitializeMixer()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 16] Cannot initialize mixer: Graph is not initialized.");
                return;
            }

            var mixer = mixerHandle;
            mixer.Initialize(in graph, mixerInputCount);
            mixerHandle = mixer;

            Debug.Log("[Day 16] AnimationMixerPlayable initialized.");
        }

        private void InitializeClipPlayables()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 16] Cannot initialize clips: Graph is not initialized.");
                return;
            }

            // Initialize first clip
            if (animationClip1 != null)
            {
                var clip1 = clipHandle1;
                clip1.Initialize(in graph, animationClip1);
                clipHandle1 = clip1;
                Debug.Log("[Day 16] AnimationClip1 initialized.");
            }
            else
            {
                Debug.LogError("[Day 16] Cannot initialize clip1: AnimationClip1 is null.");
            }

            // Initialize second clip
            if (animationClip2 != null)
            {
                var clip2 = clipHandle2;
                clip2.Initialize(in graph, animationClip2);
                clipHandle2 = clip2;
                Debug.Log("[Day 16] AnimationClip2 initialized.");
            }
            else
            {
                Debug.LogError("[Day 16] Cannot initialize clip2: AnimationClip2 is null.");
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
                Debug.LogError("[Day 16] Cannot initialize output: Graph is not initialized.");
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

                    Debug.Log("[Day 16] AnimationPlayableOutput initialized.");
                }
                else
                {
                    Debug.LogError("[Day 16] Cannot initialize AnimationPlayableOutput: No Animator assigned.");
                }
            }
            else
            {
                string outputName = $"{gameObject.name}_ScriptOutput";
                var scriptOutput = scriptOutputHandle;
                scriptOutput.Initialize(in graph, outputName);
                scriptOutputHandle = scriptOutput;

                Debug.Log("[Day 16] ScriptPlayableOutput initialized.");
            }
        }

        private void InitializeCrossfadeData()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 16] Cannot initialize crossfade data: Graph is not initialized.");
                return;
            }

            if (!mixerHandle.IsValidMixer())
            {
                Debug.LogError("[Day 16] Cannot initialize crossfade data: Mixer is not valid.");
                return;
            }

            if (!mixerHandle.TryGetPlayable(out Playable mixerPlayable))
            {
                Debug.LogError("[Day 16] Cannot initialize crossfade data: Failed to get mixer playable.");
                return;
            }

            var crossfade = crossfadeData;
            crossfade.Initialize(in graph, in mixerPlayable, mixerInputCount);
            crossfade.SetNormalizeWeights(normalizeWeights);
            crossfadeData = crossfade;

            Debug.Log("[Day 16] Crossfade data initialized.");
        }

        private void ConnectClipsToMixer()
        {
            if (!mixerHandle.IsValidMixer())
            {
                Debug.LogError("[Day 16] Cannot connect clips: Mixer is not valid.");
                return;
            }

            // Connect clip 1 to mixer input 0
            if (clipHandle1.IsValidClip() && clipHandle1.TryGetPlayable(out Playable playable1))
            {
                mixerHandle.ConnectInput(ref playable1, 0, 1.0f);
                Debug.Log("[Day 16] Connected clip1 to mixer input 0 with weight 1.0.");
            }
            else
            {
                Debug.LogWarning("[Day 16] Cannot connect clip1: Clip1 is not valid.");
            }

            // Connect clip 2 to mixer input 1
            if (clipHandle2.IsValidClip() && clipHandle2.TryGetPlayable(out Playable playable2))
            {
                mixerHandle.ConnectInput(ref playable2, 1, 0.0f);
                Debug.Log("[Day 16] Connected clip2 to mixer input 1 with weight 0.0.");
            }
            else
            {
                Debug.LogWarning("[Day 16] Cannot connect clip2: Clip2 is not valid.");
            }
        }

        private void PerformAutoCrossfade()
        {
            // No auto crossfade on start - let user control it
            Debug.Log("[Day 16] Crossfade system ready. Use GUI buttons to trigger crossfades.");
        }

        private void ConnectMixerToOutput()
        {
            if (!mixerHandle.IsValidMixer())
            {
                Debug.LogError("[Day 16] Cannot connect mixer: Mixer is not valid.");
                return;
            }

            if (useAnimationOutput && animOutputHandle.IsValidOutput())
            {
                if (animOutputHandle.TryGetPlayableOutput(out PlayableOutput output))
                {
                    mixerHandle.ConnectToOutput(in output);
                    Debug.Log("[Day 16] Successfully linked mixer to AnimationPlayableOutput!");
                }
            }
            else if (!useAnimationOutput && scriptOutputHandle.IsValidOutput())
            {
                PlayableOutput output = scriptOutputHandle.Output;
                mixerHandle.ConnectToOutput(in output);
                Debug.Log("[Day 16] Successfully linked mixer to ScriptPlayableOutput!");
            }
            else
            {
                Debug.LogWarning("[Day 16] No valid output available for linking.");
            }
        }

        private void InitializeRotator()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 16] Cannot initialize rotator: Graph is not initialized.");
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
                Debug.LogError("[Day 16] Cannot initialize speed control: Graph is not initialized.");
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
                Debug.LogError("[Day 16] Cannot initialize reverse time control: Graph is not initialized.");
                return;
            }

            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 16] Cannot initialize reverse time control: Speed control is not initialized.");
                return;
            }

            string reverseControlName = $"{gameObject.name}_ReverseTimeControl";
            var reverse = reverseData;
            reverse.Initialize(in graph, reverseControlName, enableReverseTime, enableTimeWrapping);
            reverseData = reverse;

            Debug.Log("[Day 16] Reverse time control initialized.");
        }

        private void InitializePlayStateControl()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 16] Cannot initialize PlayState control: Graph is not initialized.");
                return;
            }

            string playStateControllerName = $"{gameObject.name}_PlayStateController";
            var playState = playStateData;
            playState.Initialize(in graph, playStateControllerName, autoPlayStateControl && autoPlayOnStart);
            playStateData = playState;

            Debug.Log("[Day 16] PlayState control initialized.");
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
            Debug.Log("[Day 16] === System Information ===");

            Debug.Log($"[Day 16] Output Type: {(useAnimationOutput ? "AnimationPlayableOutput" : "ScriptPlayableOutput")}");

            if (mixerHandle.IsValidMixer())
            {
                mixerHandle.LogToConsole("Mixer");
            }

            if (crossfadeData.IsValidCrossfade())
            {
                crossfadeData.LogToConsole("Crossfade");
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

            Debug.Log("[Day 16] === End System Information ===");
        }

        private void UpdateVisualization()
        {
            Color targetColor;
            if (!crossfadeData.IsValidCrossfade())
            {
                targetColor = crossfadeErrorColor;
            }
            else if (!playStateData.IsPlaying())
            {
                targetColor = crossfadeInactiveColor;
            }
            else if (crossfadeData.IsCrossfading())
            {
                targetColor = crossfadeActiveColor;
            }
            else
            {
                targetColor = crossfadeInactiveColor;
            }

            GetComponent<Renderer>().material.color = targetColor;

            // Scale effect based on crossfade progress
            if (crossfadeData.IsValidCrossfade() && crossfadeData.TryGetProgress(out float progress))
            {
                float scale = 1.0f + (progress * 0.3f);
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

        private void CleanupCrossfadeData()
        {
            var crossfade = crossfadeData;
            crossfade.Dispose();
            crossfadeData = crossfade;
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
                Debug.Log("[Day 16] Auto-disposed graph on OnDisable");
            }
        }

        private void Reset()
        {
            // Defaults
            useDisposablePattern = true;
            enableDisposalLogging = true;
            enableGraphNaming = true;
            customGraphName = "CrossfadeGraph";
            useAnimationOutput = true;
            showCrossfadeInfo = true;
            mixerInputCount = 2;
            defaultCrossfadeDuration = 1.0f;
            defaultCurveType = CrossfadeOps.CrossfadeCurve.EaseInOut;
            normalizeWeights = false;
            showCrossfadeControls = true;
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
            crossfadeActiveColor = Color.cyan;
            crossfadeInactiveColor = Color.gray;
            crossfadeErrorColor = Color.red;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw crossfade info (Day 16)
            if (crossfadeData.IsValidCrossfade())
            {
                Gizmos.color = crossfadeData.IsCrossfading() ? crossfadeActiveColor : crossfadeInactiveColor;
                Vector3 crossfadeLabelPos = transform.position + Vector3.up * 4.0f;
                UnityEditor.Handles.Label(crossfadeLabelPos, $"Crossfade");

                if (crossfadeData.TryGetWeight(0, out float w0) && crossfadeData.TryGetWeight(1, out float w1))
                {
                    Vector3 weightLabelPos = transform.position + Vector3.up * 3.5f;
                    UnityEditor.Handles.Label(weightLabelPos, $"Weights: {w0:F2} / {w1:F2}");
                }

                if (crossfadeData.TryGetProgress(out float progress))
                {
                    Vector3 progressLabelPos = transform.position + Vector3.up * 3.0f;
                    UnityEditor.Handles.Label(progressLabelPos, $"Progress: {progress:P2}");
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
