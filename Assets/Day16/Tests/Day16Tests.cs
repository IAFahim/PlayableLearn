using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Mathematics;
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
using PlayableLearn.Day16;

namespace PlayableLearn.Day16.Tests
{
    public class Day16Tests
    {
        private Day10DisposableGraph disposableGraph;
        private Day13MixerHandle mixerHandle;
        private Day16CrossfadeData crossfadeData;
        private Day11AnimOutputHandle animOutputHandle;
        private Day02OutputHandle scriptOutputHandle;
        private Day04RotatorData rotatorData;
        private Day05SpeedData speedData;
        private Day06PlayStateData playStateData;
        private Day07ReverseData reverseData;
        private Day08NamedGraphData namedGraphData;
        private Day09VisualizerData visualizerData;
        private GameObject testGameObject;
        private GameObject testAnimatorObject;
        private Animator testAnimator;

        [SetUp]
        public void SetUp()
        {
            // Reset controller ID counter for consistent testing
            Day08NamedGraphExtensions.ResetControllerIdCounter();
            Day10DisposableGraphExtensions.ResetIdCounter();

            disposableGraph = new Day10DisposableGraph();
            mixerHandle = new Day13MixerHandle();
            crossfadeData = new Day16CrossfadeData();
            animOutputHandle = new Day11AnimOutputHandle();
            scriptOutputHandle = new Day02OutputHandle();
            rotatorData = new Day04RotatorData();
            speedData = new Day05SpeedData();
            playStateData = new Day06PlayStateData();
            reverseData = new Day07ReverseData();
            namedGraphData = new Day08NamedGraphData();
            visualizerData = new Day09VisualizerData();
            testGameObject = new GameObject("TestCube");
            testAnimatorObject = new GameObject("TestAnimator");
            testAnimator = testAnimatorObject.AddComponent<Animator>();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up in reverse order
            if (visualizerData.IsActive)
            {
                visualizerData.Dispose();
            }

            if (namedGraphData.IsActive)
            {
                namedGraphData.Dispose();
            }

            if (reverseData.IsActive)
            {
                reverseData.Dispose();
            }

            if (playStateData.IsActive)
            {
                playStateData.Dispose();
            }

            if (speedData.IsActive)
            {
                speedData.Dispose();
            }

            if (rotatorData.IsActive)
            {
                rotatorData.Dispose();
            }

            if (crossfadeData.IsActive)
            {
                crossfadeData.Dispose();
            }

            if (mixerHandle.IsValidMixer())
            {
                mixerHandle.Dispose();
            }

            if (animOutputHandle.IsValidOutput())
            {
                animOutputHandle.Dispose();
            }

            if (scriptOutputHandle.IsValidOutput())
            {
                scriptOutputHandle.Dispose();
            }

            if (disposableGraph.IsActive && !disposableGraph.IsDisposed)
            {
                var temp = disposableGraph;
                temp.Dispose();
                disposableGraph = temp;
            }

            if (testGameObject != null)
            {
                Object.DestroyImmediate(testGameObject);
            }

            if (testAnimatorObject != null)
            {
                Object.DestroyImmediate(testAnimatorObject);
            }
        }

        #region CrossfadeOps Tests

        [Test]
        public void CrossfadeOps_ApplyCurve_WithLinearCurve_ReturnsSameValue()
        {
            // Arrange
            float t = 0.5f;

            // Act
            float result = CrossfadeOps.ApplyCurve(t, CrossfadeOps.CrossfadeCurve.Linear);

            // Assert
            Assert.AreEqual(0.5f, result, 0.001f, "Linear curve should return the same value");
        }

        [Test]
        public void CrossfadeOps_ApplyCurve_WithEaseInCurve_ReturnsQuadratic()
        {
            // Arrange
            float t = 0.5f;

            // Act
            float result = CrossfadeOps.ApplyCurve(t, CrossfadeOps.CrossfadeCurve.EaseIn);

            // Assert
            Assert.AreEqual(0.25f, result, 0.001f, "Ease-in curve should return t^2");
        }

        [Test]
        public void CrossfadeOps_ApplyCurve_WithEaseOutCurve_ReturnsDecelerated()
        {
            // Arrange
            float t = 0.5f;

            // Act
            float result = CrossfadeOps.ApplyCurve(t, CrossfadeOps.CrossfadeCurve.EaseOut);

            // Assert
            Assert.AreEqual(0.75f, result, 0.001f, "Ease-out curve should return t * (2 - t)");
        }

        [Test]
        public void CrossfadeOps_ApplyCurve_WithEaseInOutCurve_ReturnsSmoothStep()
        {
            // Arrange
            float t = 0.5f;

            // Act
            float result = CrossfadeOps.ApplyCurve(t, CrossfadeOps.CrossfadeCurve.EaseInOut);

            // Assert
            Assert.AreEqual(0.5f, result, 0.001f, "Ease-in-out curve should return smooth step at 0.5");
        }

        [Test]
        public void CrossfadeOps_Lerp_WithValidInputs_ReturnsInterpolatedValue()
        {
            // Arrange
            float a = 0.0f;
            float b = 1.0f;
            float t = 0.5f;

            // Act
            float result = CrossfadeOps.Lerp(a, b, t);

            // Assert
            Assert.AreEqual(0.5f, result, 0.001f, "Lerp should return 0.5 at t=0.5");
        }

        [Test]
        public void CrossfadeOps_CalculateCrossfadeWeights_WithHalfProgress_ReturnsMidWeights()
        {
            // Arrange
            float startWeight0 = 1.0f;
            float startWeight1 = 0.0f;
            float targetWeight0 = 0.0f;
            float targetWeight1 = 1.0f;
            float progress = 0.5f;

            // Act
            CrossfadeOps.CalculateCrossfadeWeights(
                startWeight0, startWeight1,
                targetWeight0, targetWeight1,
                progress, false,
                out float weight0, out float weight1
            );

            // Assert
            Assert.AreEqual(0.5f, weight0, 0.001f, "Weight 0 should be 0.5 at half progress");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Weight 1 should be 0.5 at half progress");
        }

        [Test]
        public void CrossfadeOps_UpdateProgress_WithHalfDuration_ReturnsHalfProgress()
        {
            // Arrange
            float elapsedTime = 0.5f;
            float duration = 1.0f;

            // Act
            float progress = CrossfadeOps.UpdateProgress(elapsedTime, duration);

            // Assert
            Assert.AreEqual(0.5f, progress, 0.001f, "Progress should be 0.5 at half duration");
        }

        [Test]
        public void CrossfadeOps_IsCrossfadeComplete_WithFullProgress_ReturnsTrue()
        {
            // Arrange
            float progress = 1.0f;

            // Act
            bool isComplete = CrossfadeOps.IsCrossfadeComplete(progress);

            // Assert
            Assert.IsTrue(isComplete, "Should be complete at progress 1.0");
        }

        [Test]
        public void CrossfadeOps_IsCrossfadeComplete_WithPartialProgress_ReturnsFalse()
        {
            // Arrange
            float progress = 0.5f;

            // Act
            bool isComplete = CrossfadeOps.IsCrossfadeComplete(progress);

            // Assert
            Assert.IsFalse(isComplete, "Should not be complete at progress 0.5");
        }

        [Test]
        public void CrossfadeOps_ValidateCrossfadeParams_WithValidParams_ReturnsTrue()
        {
            // Arrange
            float duration = 1.0f;
            float startWeight0 = 1.0f;
            float startWeight1 = 0.0f;
            float targetWeight0 = 0.0f;
            float targetWeight1 = 1.0f;

            // Act
            bool isValid = CrossfadeOps.ValidateCrossfadeParams(
                duration, startWeight0, startWeight1,
                targetWeight0, targetWeight1
            );

            // Assert
            Assert.IsTrue(isValid, "Should validate correct parameters");
        }

        [Test]
        public void CrossfadeOps_ValidateCrossfadeParams_WithNegativeDuration_ReturnsFalse()
        {
            // Arrange
            float duration = -1.0f;

            // Act
            bool isValid = CrossfadeOps.ValidateCrossfadeParams(
                duration, 1.0f, 0.0f, 0.0f, 1.0f
            );

            // Assert
            Assert.IsFalse(isValid, "Should reject negative duration");
        }

        [Test]
        public void CrossfadeOps_CalculateTimeRemaining_WithPartialProgress_ReturnsPositiveTime()
        {
            // Arrange
            float elapsedTime = 0.5f;
            float duration = 1.0f;

            // Act
            float timeRemaining = CrossfadeOps.CalculateTimeRemaining(elapsedTime, duration);

            // Assert
            Assert.AreEqual(0.5f, timeRemaining, 0.001f, "Should have 0.5s remaining");
        }

        [Test]
        public void CrossfadeOps_NormalizeWeights_WithUnnormalizedWeights_Normalizes()
        {
            // Arrange
            float weight0 = 2.0f;
            float weight1 = 2.0f;

            // Act
            CrossfadeOps.NormalizeWeights(ref weight0, ref weight1);

            // Assert
            Assert.AreEqual(0.5f, weight0, 0.001f, "Weight 0 should normalize to 0.5");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Weight 1 should normalize to 0.5");
        }

        [Test]
        public void CrossfadeOps_AreWeightsEqual_WithEqualWeights_ReturnsTrue()
        {
            // Arrange
            float weight0 = 0.5f;
            float weight1 = 0.5f;

            // Act
            bool areEqual = CrossfadeOps.AreWeightsEqual(weight0, weight1);

            // Assert
            Assert.IsTrue(areEqual, "Should detect equal weights");
        }

        [Test]
        public void CrossfadeOps_AreWeightsAtTarget_WithTargetWeights_ReturnsTrue()
        {
            // Arrange
            float currentWeight0 = 0.5f;
            float currentWeight1 = 0.5f;
            float targetWeight0 = 0.5f;
            float targetWeight1 = 0.5f;

            // Act
            bool atTarget = CrossfadeOps.AreWeightsAtTarget(
                currentWeight0, currentWeight1,
                targetWeight0, targetWeight1
            );

            // Assert
            Assert.IsTrue(atTarget, "Should detect weights at target");
        }

        [Test]
        public void CrossfadeOps_Clamp01_WithNegativeValue_ReturnsZero()
        {
            // Arrange
            float value = -0.5f;

            // Act
            float result = CrossfadeOps.Clamp01(value);

            // Assert
            Assert.AreEqual(0.0f, result, 0.001f, "Should clamp negative to 0");
        }

        [Test]
        public void CrossfadeOps_Clamp01_WithValueAboveOne_ReturnsOne()
        {
            // Arrange
            float value = 1.5f;

            // Act
            float result = CrossfadeOps.Clamp01(value);

            // Assert
            Assert.AreEqual(1.0f, result, 0.001f, "Should clamp above 1 to 1");
        }

        #endregion

        #region Day16CrossfadeData Tests

        [Test]
        public void Day16CrossfadeData_Initialize_WithValidMixer_Initializes()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            // Act
            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            // Assert
            Assert.IsTrue(crossfadeData.IsActive, "Crossfade data should be active");
            Assert.AreEqual(1.0f, crossfadeData.CurrentWeight0, 0.001f, "Initial weight 0 should be 1.0");
            Assert.AreEqual(0.0f, crossfadeData.CurrentWeight1, 0.001f, "Initial weight 1 should be 0.0");
            Assert.IsFalse(crossfadeData.IsCrossfading, "Should not be crossfading initially");
        }

        [Test]
        public void Day16CrossfadeData_Dispose_DisposesCrossfadeData()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            // Act
            crossfadeData.Dispose();

            // Assert
            Assert.IsFalse(crossfadeData.IsActive, "Crossfade data should be inactive after disposal");
        }

        #endregion

        #region Day16CrossfadeExtensions Tests

        [Test]
        public void Day16CrossfadeExtensions_StartCrossfade_WithValidParameters_StartsCrossfade()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            // Act
            crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);

            // Assert
            Assert.IsTrue(crossfadeData.IsCrossfading, "Should be crossfading");
            Assert.AreEqual(0.0f, crossfadeData.TargetWeight0, 0.001f, "Target weight 0 should be 0.0");
            Assert.AreEqual(1.0f, crossfadeData.TargetWeight1, 0.001f, "Target weight 1 should be 1.0");
            Assert.AreEqual(1.0f, crossfadeData.CrossfadeDuration, 0.001f, "Duration should be 1.0");
        }

        [Test]
        public void Day16CrossfadeExtensions_UpdateCrossfade_WithPartialDelta_UpdatesWeights()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;
            crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);

            // Act
            crossfadeData.UpdateCrossfade(0.5f);

            // Assert
            Assert.IsTrue(crossfadeData.IsCrossfading, "Should still be crossfading");
            Assert.AreEqual(0.5f, crossfadeData.Progress, 0.001f, "Progress should be 0.5");
        }

        [Test]
        public void Day16CrossfadeExtensions_UpdateCrossfade_WithFullDuration_CompletesCrossfade()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;
            crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);

            // Act
            crossfadeData.UpdateCrossfade(1.0f);

            // Assert
            Assert.IsFalse(crossfadeData.IsCrossfading, "Should not be crossfading after completion");
            Assert.IsTrue(crossfadeData.IsCrossfadeComplete(), "Should be complete");
        }

        [Test]
        public void Day16CrossfadeExtensions_StopCrossfade_WithActiveCrossfade_StopsCrossfade()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;
            crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);

            // Act
            crossfadeData.StopCrossfade();

            // Assert
            Assert.IsFalse(crossfadeData.IsCrossfading, "Should not be crossfading after stop");
        }

        [Test]
        public void Day16CrossfadeExtensions_CompleteCrossfade_WithActiveCrossfade_JumpsToTarget()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;
            crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);

            // Act
            crossfadeData.CompleteCrossfade();

            // Assert
            Assert.IsFalse(crossfadeData.IsCrossfading, "Should not be crossfading after completion");
            Assert.AreEqual(0.0f, crossfadeData.CurrentWeight0, 0.001f, "Current weight 0 should be 0.0");
            Assert.AreEqual(1.0f, crossfadeData.CurrentWeight1, 0.001f, "Current weight 1 should be 1.0");
        }

        [Test]
        public void Day16CrossfadeExtensions_SetWeightsImmediate_WithValidWeights_SetsWeights()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            // Act
            crossfadeData.SetWeightsImmediate(0.5f, 0.5f);

            // Assert
            Assert.AreEqual(0.5f, crossfadeData.CurrentWeight0, 0.001f, "Current weight 0 should be 0.5");
            Assert.AreEqual(0.5f, crossfadeData.CurrentWeight1, 0.001f, "Current weight 1 should be 0.5");
            Assert.IsFalse(crossfadeData.IsCrossfading, "Should not be crossfading");
        }

        [Test]
        public void Day16CrossfadeExtensions_TryGetProgress_WithActiveCrossfade_ReturnsProgress()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;
            crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);
            crossfadeData.UpdateCrossfade(0.5f);

            // Act
            bool success = crossfadeData.TryGetProgress(out float progress);

            // Assert
            Assert.IsTrue(success, "Should get progress successfully");
            Assert.AreEqual(0.5f, progress, 0.001f, "Progress should be 0.5");
        }

        [Test]
        public void Day16CrossfadeExtensions_TryGetTimeRemaining_WithActiveCrossfade_ReturnsTimeRemaining()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;
            crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);
            crossfadeData.UpdateCrossfade(0.3f);

            // Act
            bool success = crossfadeData.TryGetTimeRemaining(out float timeRemaining);

            // Assert
            Assert.IsTrue(success, "Should get time remaining successfully");
            Assert.AreEqual(0.7f, timeRemaining, 0.001f, "Time remaining should be 0.7s");
        }

        [Test]
        public void Day16CrossfadeExtensions_IsValidCrossfade_WithValidData_ReturnsTrue()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            // Act
            bool isValid = crossfadeData.IsValidCrossfade();

            // Assert
            Assert.IsTrue(isValid, "Crossfade data should be valid");
        }

        [Test]
        public void Day16CrossfadeExtensions_SetCurveType_WithValidCurve_SetsCurve()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            // Act
            crossfadeData.SetCurveType(CrossfadeOps.CrossfadeCurve.EaseInOut);

            // Assert
            Assert.AreEqual((int)CrossfadeOps.CrossfadeCurve.EaseInOut, crossfadeData.CurveType, "Curve type should be EaseInOut");
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Day16Integration_WithAllPreviousDays_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("IntegrationTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            var rotator = rotatorData;
            rotator.Initialize(in unityGraph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);
            rotatorData = rotator;

            var speed = speedData;
            speed.Initialize(in unityGraph, "TestSpeed", 1.0f, true, 2.0f);
            speedData = speed;

            var playState = playStateData;
            playState.Initialize(in unityGraph, "TestPlayState", true);
            playStateData = playState;

            var reverse = reverseData;
            reverse.Initialize(in unityGraph, "TestReverse", true, true);
            reverseData = reverse;

            var namedGraph = namedGraphData;
            namedGraph.Initialize(in unityGraph, "NamedGraph");
            namedGraphData = namedGraph;

            // Act & Assert
            Assert.IsTrue(disposableGraph.IsValid(), "Disposable graph should be valid");
            Assert.IsTrue(mixerHandle.IsValidMixer(), "Mixer should be valid");
            Assert.IsTrue(crossfadeData.IsValidCrossfade(), "Crossfade data should be valid");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState should be valid");
            Assert.IsTrue(reverseData.IsValidReverseControl(), "Reverse control should be valid");
            Assert.IsTrue(namedGraphData.IsValidControl(), "Named graph should be valid");
        }

        [Test]
        public void Day16Integration_CrossfadeWithMixer_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("CrossfadeTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            // Act
            crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);
            crossfadeData.UpdateCrossfade(0.5f);

            // Assert
            Assert.IsTrue(crossfadeData.IsCrossfading(), "Should be crossfading");
            Assert.IsTrue(crossfadeData.IsValidCrossfade(), "Crossfade data should be valid");
            Assert.IsTrue(mixerHandle.IsValidMixer(), "Mixer should be valid");
        }

        [Test]
        public void Day16Integration_CrossfadeWithDifferentCurves_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("CurveTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            // Act & Assert - Test each curve type
            CrossfadeOps.CrossfadeCurve[] curves = {
                CrossfadeOps.CrossfadeCurve.Linear,
                CrossfadeOps.CrossfadeCurve.EaseIn,
                CrossfadeOps.CrossfadeCurve.EaseOut,
                CrossfadeOps.CrossfadeCurve.EaseInOut
            };

            foreach (var curve in curves)
            {
                crossfadeData.StartCrossfade(0.0f, 1.0f, 1.0f, curve);
                Assert.IsTrue(crossfadeData.IsCrossfading(), $"Should be crossfading with {curve} curve");

                crossfadeData.UpdateCrossfade(0.5f);
                Assert.IsTrue(crossfadeData.IsValidCrossfade(), $"Crossfade should remain valid with {curve} curve");
            }
        }

        [Test]
        public void Day16Integration_MultipleCrossfadesInSequence_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("SequentialTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            // Act - First crossfade
            crossfadeData.StartCrossfade(0.0f, 1.0f, 0.5f, CrossfadeOps.CrossfadeCurve.Linear);
            crossfadeData.UpdateCrossfade(0.5f);

            // Assert - First crossfade
            Assert.IsTrue(crossfadeData.IsCrossfading(), "Should be crossfading");

            // Act - Complete first crossfade
            crossfadeData.UpdateCrossfade(0.5f);
            Assert.IsFalse(crossfadeData.IsCrossfading(), "First crossfade should be complete");

            // Act - Start second crossfade
            crossfadeData.StartCrossfade(1.0f, 0.0f, 0.5f, CrossfadeOps.CrossfadeCurve.Linear);
            crossfadeData.UpdateCrossfade(0.25f);

            // Assert - Second crossfade
            Assert.IsTrue(crossfadeData.IsCrossfading(), "Should be crossfading again");
        }

        [Test]
        public void Day16Integration_WeightNormalization_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("NormalizationTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;
            crossfadeData.SetNormalizeWeights(true);

            // Act
            crossfadeData.StartCrossfade(2.0f, 2.0f, 1.0f, CrossfadeOps.CrossfadeCurve.Linear);
            crossfadeData.UpdateCrossfade(1.0f);

            // Assert
            // Even though we requested 2.0/2.0, with normalization it should be 0.5/0.5
            crossfadeData.TryGetWeight(0, out float weight0);
            crossfadeData.TryGetWeight(1, out float weight1);
            Assert.IsTrue(weight0 > 0.4f && weight0 < 0.6f, "Weight 0 should be around 0.5 with normalization");
            Assert.IsTrue(weight1 > 0.4f && weight1 < 0.6f, "Weight 1 should be around 0.5 with normalization");
        }

        [Test]
        public void Day16Integration_Dispose_CleansUpAllResources()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("IntegrationTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var crossfade = crossfadeData;
            crossfade.Initialize(in unityGraph, in mixerPlayable, 2);
            crossfadeData = crossfade;

            var rotator = rotatorData;
            rotator.Initialize(in unityGraph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);
            rotatorData = rotator;

            // Act - Dispose in reverse order
            rotatorData.Dispose();
            crossfadeData.Dispose();
            mixerHandle.Dispose();

            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;

            // Assert
            Assert.IsFalse(disposableGraph.IsValid(), "Disposable graph should be disposed");
            Assert.IsFalse(rotatorData.IsActive, "Rotator should be inactive");
            Assert.IsFalse(crossfadeData.IsActive, "Crossfade data should be inactive");
            Assert.IsFalse(mixerHandle.IsValidMixer(), "Mixer should be inactive");
        }

        #endregion
    }
}
