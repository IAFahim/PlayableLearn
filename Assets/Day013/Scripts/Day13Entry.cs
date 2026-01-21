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
using Unity.Mathematics;

namespace PlayableLearn.Day13
{
    /// <summary>
    /// MonoBehaviour entry point for Day 13: The Mixer.
    /// Demonstrates wrapping AnimationMixerPlayable for blending multiple animations.
    ///
    /// Key Concepts:
    /// - AnimationMixerPlayable blends multiple animation inputs
    /// - Each input has a weight that controls its influence
    /// - Weights can be adjusted for smooth transitions
    /// - The mixer outputs a blended animation result
    ///
    /// Follows the Playable Protocol:
    /// - Layer A: Data (Day13MixerHandle, Day12ClipHandle, etc.)
    /// - Layer B: Operations (MixerOps, ClipPlayableOps, etc.)
    /// - Layer C: Extensions (Day13MixerExtensions, Day12ClipExtensions, etc.)
    /// </summary>
    public class Day13Entry : MonoBehaviour
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

        // Animation clip references (for blending)
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
        private bool showMixerInfo = true;

        // Mixer settings
        [Header("Mixer Settings")]
        [SerializeField]
        private int mixerInputCount = 2;

        [SerializeField]
        private bool autoBlendOnStart = true;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float initialBlend = 0.5f;

        [SerializeField]
        private bool normalizeWeights = false;

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
        private string customGraphName = "MixerGraph";

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

        // Runtime controls
        [Header("Runtime Controls")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float targetBlend = 0.5f;

        [SerializeField]
        [Range(-3.0f, 3.0f)]
        private float targetSpeed = 1.0f;

        // Visualization
        [Header("Visualization")]
        [SerializeField]
        private Color mixerActiveColor = Color.cyan;

        [SerializeField]
        private Color mixerInactiveColor = Color.gray;

        [SerializeField]
        private Color mixerErrorColor = Color.red;

        private void OnEnable()
        {
            ValidateAnimationClips();
            ValidateAnimator();
            ValidateMixerSettings();
            InitializeDisposableGraph();
            InitializeGraphNaming();
            InitializeVisualizer();
            InitializeMixer();
            InitializeClipPlayables();
            InitializeOutput();
            ConnectClipsToMixer();
            InitializeRotator();
            InitializeSpeedControl();
            InitializeReverseTimeControl();
            InitializePlayStateControl();
            ConnectMixerToOutput();
            LogSystemInfo();
        }

        private void OnDisable()
        {
            CleanupVisualizer();
            CleanupPlayStateControl();
            CleanupReverseTimeControl();
            CleanupSpeedControl();
            CleanupRotator();
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
                    ? reverseData.ClampSpeed(-3.0f, 3.0f, targetSpeed)
                    : targetSpeed;

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

            // Update mixer blend
            if (mixerHandle.IsValidMixer())
            {
                mixerHandle.SetBlend(targetBlend);
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

            // Mixer info display
            if (showMixerInfo && mixerHandle.IsValidMixer())
            {
                if (mixerHandle.TryGetInputCount(out int inputCount))
                {
                    string mixerText = $"Mixer Inputs: {inputCount}";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), mixerText);
                    currentY += buttonHeight + padding;

                    string blendText = $"Blend: {targetBlend:F2}";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), blendText);
                    currentY += buttonHeight + padding;

                    // Show clip names
                    string clip1Name = animationClip1 != null ? animationClip1.name : "None";
                    string clip2Name = animationClip2 != null ? animationClip2.name : "None";
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), $"Clip 1: {clip1Name}");
                    currentY += buttonHeight + padding;
                    GUI.Label(new Rect(startX, currentY, buttonWidth, buttonHeight), $"Clip 2: {clip2Name}");
                    currentY += buttonHeight + padding;
                }
            }

            // Blend slider
            string sliderLabel = $"Blend: {targetBlend:F2}";
            float newBlend = GUI.HorizontalSlider(new Rect(startX, currentY, buttonWidth, buttonHeight), targetBlend, 0.0f, 1.0f);
            GUI.Label(new Rect(startX + buttonWidth + padding, currentY, buttonWidth, buttonHeight), sliderLabel);
            currentY += buttonHeight + padding;

            if (math.abs(newBlend - targetBlend) > 0.001f)
            {
                targetBlend = newBlend;
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

        private void ValidateAnimationClips()
        {
            if (animationClip1 == null)
            {
                Debug.LogWarning("[Day 13] No AnimationClip1 assigned. Mixer will not work properly without clips.");
            }
            else
            {
                Debug.Log($"[Day 13] AnimationClip1 assigned: {animationClip1.name}");
            }

            if (animationClip2 == null)
            {
                Debug.LogWarning("[Day 13] No AnimationClip2 assigned. Mixer will not work properly without clips.");
            }
            else
            {
                Debug.Log($"[Day 13] AnimationClip2 assigned: {animationClip2.name}");
            }
        }

        private void ValidateAnimator()
        {
            if (useAnimationOutput && targetAnimator == null)
            {
                Debug.LogWarning("[Day 13] AnimationPlayableOutput enabled but no Animator assigned. Looking for Animator on this GameObject.");
                targetAnimator = GetComponent<Animator>();

                if (targetAnimator == null)
                {
                    Debug.LogError("[Day 13] No Animator component found. AnimationPlayableOutput will not work without an Animator.");
                }
                else
                {
                    Debug.Log($"[Day 13] Found Animator component: {targetAnimator.name}");
                }
            }
        }

        private void ValidateMixerSettings()
        {
            if (mixerInputCount < 2)
            {
                Debug.LogWarning($"[Day 13] Mixer input count must be at least 2. Adjusting from {mixerInputCount} to 2.");
                mixerInputCount = 2;
            }

            if (mixerInputCount > 16)
            {
                Debug.LogWarning($"[Day 13] Mixer input count capped at 16. Adjusting from {mixerInputCount} to 16.");
                mixerInputCount = 16;
            }
        }

        private void InitializeDisposableGraph()
        {
            if (!useDisposablePattern) return;

            string graphName = enableGraphNaming ? customGraphName : $"{gameObject.name}_MixerGraph_{Time.frameCount}";
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
                Debug.LogError("[Day 13] Cannot initialize graph naming: Graph is not initialized.");
                return;
            }

            if (!enableGraphNaming) return;

            string graphName = string.IsNullOrEmpty(customGraphName) ? $"{gameObject.name}_MixerGraph" : customGraphName;
            var namedGraph = namedGraphData;
            namedGraph.Initialize(in graph, graphName);
            namedGraphData = namedGraph;

            Debug.Log("[Day 13] Graph naming initialized.");
        }

        private void InitializeVisualizer()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 13] Cannot initialize visualizer: Graph is not initialized.");
                return;
            }

            var visualizer = visualizerData;
            visualizer.Initialize(in graph, VisualizationDetailLevel.Basic, VisualizerColorScheme.Default);
            visualizerData = visualizer;

            Debug.Log("[Day 13] Graph Visualizer initialized.");
        }

        private void InitializeMixer()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 13] Cannot initialize mixer: Graph is not initialized.");
                return;
            }

            var mixer = mixerHandle;
            mixer.Initialize(in graph, mixerInputCount);
            mixerHandle = mixer;

            Debug.Log("[Day 13] AnimationMixerPlayable initialized.");
        }

        private void InitializeClipPlayables()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 13] Cannot initialize clips: Graph is not initialized.");
                return;
            }

            // Initialize first clip
            if (animationClip1 != null)
            {
                var clip1 = clipHandle1;
                clip1.Initialize(in graph, animationClip1);
                clipHandle1 = clip1;
                Debug.Log("[Day 13] AnimationClip1 initialized.");
            }
            else
            {
                Debug.LogError("[Day 13] Cannot initialize clip1: AnimationClip1 is null.");
            }

            // Initialize second clip
            if (animationClip2 != null)
            {
                var clip2 = clipHandle2;
                clip2.Initialize(in graph, animationClip2);
                clipHandle2 = clip2;
                Debug.Log("[Day 13] AnimationClip2 initialized.");
            }
            else
            {
                Debug.LogError("[Day 13] Cannot initialize clip2: AnimationClip2 is null.");
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
                Debug.LogError("[Day 13] Cannot initialize output: Graph is not initialized.");
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

                    Debug.Log("[Day 13] AnimationPlayableOutput initialized.");
                }
                else
                {
                    Debug.LogError("[Day 13] Cannot initialize AnimationPlayableOutput: No Animator assigned.");
                }
            }
            else
            {
                string outputName = $"{gameObject.name}_ScriptOutput";
                var scriptOutput = scriptOutputHandle;
                scriptOutput.Initialize(in graph, outputName);
                scriptOutputHandle = scriptOutput;

                Debug.Log("[Day 13] ScriptPlayableOutput initialized.");
            }
        }

        private void ConnectClipsToMixer()
        {
            if (!mixerHandle.IsValidMixer())
            {
                Debug.LogError("[Day 13] Cannot connect clips: Mixer is not valid.");
                return;
            }

            // Connect clip 1 to mixer input 0
            if (clipHandle1.IsValidClip() && clipHandle1.TryGetPlayable(out Playable playable1))
            {
                mixerHandle.ConnectInput(ref playable1, 0, 0.0f);
                Debug.Log("[Day 13] Connected clip1 to mixer input 0.");
            }
            else
            {
                Debug.LogWarning("[Day 13] Cannot connect clip1: Clip1 is not valid.");
            }

            // Connect clip 2 to mixer input 1
            if (clipHandle2.IsValidClip() && clipHandle2.TryGetPlayable(out Playable playable2))
            {
                mixerHandle.ConnectInput(ref playable2, 1, 0.0f);
                Debug.Log("[Day 13] Connected clip2 to mixer input 1.");
            }
            else
            {
                Debug.LogWarning("[Day 13] Cannot connect clip2: Clip2 is not valid.");
            }

            // Set initial blend
            if (autoBlendOnStart)
            {
                mixerHandle.SetBlend(initialBlend);
                Debug.Log($"[Day 13] Set initial blend to {initialBlend}.");
            }
        }

        private void ConnectMixerToOutput()
        {
            if (!mixerHandle.IsValidMixer())
            {
                Debug.LogError("[Day 13] Cannot connect mixer: Mixer is not valid.");
                return;
            }

            if (useAnimationOutput && animOutputHandle.IsValidOutput())
            {
                if (animOutputHandle.TryGetPlayableOutput(out PlayableOutput output))
                {
                    mixerHandle.ConnectToOutput(in output);
                    Debug.Log("[Day 13] Successfully linked mixer to AnimationPlayableOutput!");
                }
            }
            else if (!useAnimationOutput && scriptOutputHandle.IsValidOutput())
            {
                PlayableOutput output = scriptOutputHandle.Output;
                mixerHandle.ConnectToOutput(in output);
                Debug.Log("[Day 13] Successfully linked mixer to ScriptPlayableOutput!");
            }
            else
            {
                Debug.LogWarning("[Day 13] No valid output available for linking.");
            }
        }

        private void InitializeRotator()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 13] Cannot initialize rotator: Graph is not initialized.");
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
                Debug.LogError("[Day 13] Cannot initialize speed control: Graph is not initialized.");
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
                Debug.LogError("[Day 13] Cannot initialize reverse time control: Graph is not initialized.");
                return;
            }

            if (!speedData.IsValidSpeedControl())
            {
                Debug.LogError("[Day 13] Cannot initialize reverse time control: Speed control is not initialized.");
                return;
            }

            string reverseControlName = $"{gameObject.name}_ReverseTimeControl";
            var reverse = reverseData;
            reverse.Initialize(in graph, reverseControlName, enableReverseTime, enableTimeWrapping);
            reverseData = reverse;

            Debug.Log("[Day 13] Reverse time control initialized.");
        }

        private void InitializePlayStateControl()
        {
            PlayableGraph graph = disposableGraph.Graph;

            if (!graph.IsValid())
            {
                Debug.LogError("[Day 13] Cannot initialize PlayState control: Graph is not initialized.");
                return;
            }

            string playStateControllerName = $"{gameObject.name}_PlayStateController";
            var playState = playStateData;
            playState.Initialize(in graph, playStateControllerName, autoPlayStateControl && autoPlayOnStart);
            playStateData = playState;

            Debug.Log("[Day 13] PlayState control initialized.");
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
            Debug.Log("[Day 13] === System Information ===");

            Debug.Log($"[Day 13] Output Type: {(useAnimationOutput ? "AnimationPlayableOutput" : "ScriptPlayableOutput")}");

            if (mixerHandle.IsValidMixer())
            {
                mixerHandle.LogToConsole("Mixer");
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

            Debug.Log("[Day 13] === End System Information ===");
        }

        private void UpdateVisualization()
        {
            Color targetColor;
            if (!mixerHandle.IsValidMixer())
            {
                targetColor = mixerErrorColor;
            }
            else if (!playStateData.IsPlaying())
            {
                targetColor = mixerInactiveColor;
            }
            else
            {
                targetColor = mixerActiveColor;
            }

            GetComponent<Renderer>().material.color = targetColor;

            // Pulse effect based on blend value
            if (mixerHandle.IsValidMixer())
            {
                float pulse = 1.0f + Mathf.Sin(targetBlend * Mathf.PI) * 0.1f;
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
                Debug.Log("[Day 13] Auto-disposed graph on OnDisable");
            }
        }

        private void Reset()
        {
            // Defaults
            useDisposablePattern = true;
            enableDisposalLogging = true;
            enableGraphNaming = true;
            customGraphName = "MixerGraph";
            useAnimationOutput = true;
            showMixerInfo = true;
            mixerInputCount = 2;
            autoBlendOnStart = true;
            initialBlend = 0.5f;
            normalizeWeights = false;
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
            targetBlend = 0.5f;
            targetSpeed = 1.0f;
            mixerActiveColor = Color.cyan;
            mixerInactiveColor = Color.gray;
            mixerErrorColor = Color.red;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw mixer info (Day 13)
            if (mixerHandle.IsValidMixer())
            {
                Gizmos.color = playStateData.IsPlaying() ? mixerActiveColor : mixerInactiveColor;
                Vector3 mixerLabelPos = transform.position + Vector3.up * 4.0f;
                UnityEditor.Handles.Label(mixerLabelPos, $"Mixer: {mixerInputCount} Inputs");

                Vector3 blendLabelPos = transform.position + Vector3.up * 3.5f;
                UnityEditor.Handles.Label(blendLabelPos, $"Blend: {targetBlend:F2}");

                // Draw clip names
                if (animationClip1 != null)
                {
                    Vector3 clip1LabelPos = transform.position + Vector3.up * 3.0f;
                    UnityEditor.Handles.Label(clip1LabelPos, $"Clip 1: {animationClip1.name}");
                }

                if (animationClip2 != null)
                {
                    Vector3 clip2LabelPos = transform.position + Vector3.up * 2.5f;
                    UnityEditor.Handles.Label(clip2LabelPos, $"Clip 2: {animationClip2.name}");
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
