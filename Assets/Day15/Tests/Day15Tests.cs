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
using PlayableLearn.Day15;

namespace PlayableLearn.Day15.Tests
{
    public class Day15Tests
    {
        private Day10DisposableGraph disposableGraph;
        private Day13MixerHandle mixerHandle;
        private Day15BlendData blendData;
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
            blendData = new Day15BlendData();
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

            if (blendData.IsActive)
            {
                blendData.Dispose();
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

        #region WeightBlendOps Tests

        [Test]
        public void WeightBlendOps_SetWeights_WithValidMixer_SetsWeights()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            MixerOps.Create(in graph, 2, out Playable mixer);

            // Act
            WeightBlendOps.SetWeights(in mixer, 0.5f, 0.5f);

            // Assert
            WeightBlendOps.GetWeight(in mixer, 0, out float weight0);
            WeightBlendOps.GetWeight(in mixer, 1, out float weight1);
            Assert.AreEqual(0.5f, weight0, 0.001f, "Weight 0 should be 0.5");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Weight 1 should be 0.5");

            // Cleanup
            MixerOps.Destroy(in graph, in mixer);
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void WeightBlendOps_SetWeight_WithValidMixer_SetsSingleWeight()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            MixerOps.Create(in graph, 2, out Playable mixer);

            // Act
            WeightBlendOps.SetWeight(in mixer, 0, 0.7f);

            // Assert
            WeightBlendOps.GetWeight(in mixer, 0, out float weight0);
            Assert.AreEqual(0.7f, weight0, 0.001f, "Weight 0 should be 0.7");

            // Cleanup
            MixerOps.Destroy(in graph, in mixer);
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void WeightBlendOps_SetBlend_WithValidMixer_SetsWeightsFromBlend()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            MixerOps.Create(in graph, 2, out Playable mixer);

            // Act
            WeightBlendOps.SetBlend(in mixer, 0.5f);

            // Assert
            WeightBlendOps.GetWeight(in mixer, 0, out float weight0);
            WeightBlendOps.GetWeight(in mixer, 1, out float weight1);
            Assert.AreEqual(0.5f, weight0, 0.001f, "Weight 0 should be 0.5");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Weight 1 should be 0.5");

            // Cleanup
            MixerOps.Destroy(in graph, in mixer);
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void WeightBlendOps_SetEqualBlend_WithValidMixer_SetsEqualWeights()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            MixerOps.Create(in graph, 2, out Playable mixer);

            // Act
            WeightBlendOps.SetEqualBlend(in mixer);

            // Assert
            WeightBlendOps.GetWeight(in mixer, 0, out float weight0);
            WeightBlendOps.GetWeight(in mixer, 1, out float weight1);
            Assert.AreEqual(0.5f, weight0, 0.001f, "Weight 0 should be 0.5");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Weight 1 should be 0.5");

            // Cleanup
            MixerOps.Destroy(in graph, in mixer);
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void WeightBlendOps_NormalizeWeights_WithUnnormalizedWeights_Normalizes()
        {
            // Arrange
            float weight0 = 2.0f;
            float weight1 = 2.0f;

            // Act
            WeightBlendOps.NormalizeWeights(ref weight0, ref weight1);

            // Assert
            Assert.AreEqual(0.5f, weight0, 0.001f, "Weight 0 should normalize to 0.5");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Weight 1 should normalize to 0.5");
        }

        [Test]
        public void WeightBlendOps_IsEqualBlend_WithEqualWeights_ReturnsTrue()
        {
            // Arrange
            float weight0 = 0.5f;
            float weight1 = 0.5f;

            // Act
            bool isEqual = WeightBlendOps.IsEqualBlend(weight0, weight1);

            // Assert
            Assert.IsTrue(isEqual, "Should detect equal blend");
        }

        [Test]
        public void WeightBlendOps_IsEqualBlend_WithUnequalWeights_ReturnsFalse()
        {
            // Arrange
            float weight0 = 0.7f;
            float weight1 = 0.3f;

            // Act
            bool isEqual = WeightBlendOps.IsEqualBlend(weight0, weight1);

            // Assert
            Assert.IsFalse(isEqual, "Should not detect equal blend");
        }

        [Test]
        public void WeightBlendOps_UpdateWeights_WithValidInputs_UpdatesWeights()
        {
            // Arrange
            float currentWeight0 = 1.0f;
            float currentWeight1 = 0.0f;
            float targetWeight0 = 0.5f;
            float targetWeight1 = 0.5f;
            float blendSpeed = 1.0f;
            float deltaTime = 1.0f;

            // Act
            WeightBlendOps.UpdateWeights(ref currentWeight0, ref currentWeight1, targetWeight0, targetWeight1, blendSpeed, deltaTime);

            // Assert
            Assert.AreEqual(0.5f, currentWeight0, 0.001f, "Weight 0 should reach target");
            Assert.AreEqual(0.5f, currentWeight1, 0.001f, "Weight 1 should reach target");
        }

        [Test]
        public void WeightBlendOps_IsBlendComplete_WithMatchingWeights_ReturnsTrue()
        {
            // Arrange
            float currentWeight0 = 0.5f;
            float currentWeight1 = 0.5f;
            float targetWeight0 = 0.5f;
            float targetWeight1 = 0.5f;

            // Act
            bool isComplete = WeightBlendOps.IsBlendComplete(currentWeight0, currentWeight1, targetWeight0, targetWeight1);

            // Assert
            Assert.IsTrue(isComplete, "Should detect blend complete");
        }

        #endregion

        #region Day15BlendData Tests

        [Test]
        public void Day15BlendData_Initialize_WithValidMixer_Initializes()
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
            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            // Assert
            Assert.IsTrue(blendData.IsActive, "Blend data should be active");
            Assert.AreEqual(1.0f, blendData.Weight0, 0.001f, "Initial weight 0 should be 1.0");
            Assert.AreEqual(0.0f, blendData.Weight1, 0.001f, "Initial weight 1 should be 0.0");
        }

        [Test]
        public void Day15BlendData_Dispose_DisposesBlendData()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            // Act
            blendData.Dispose();

            // Assert
            Assert.IsFalse(blendData.IsActive, "Blend data should be inactive after disposal");
        }

        #endregion

        #region Day15BlendExtensions Tests

        [Test]
        public void Day15BlendExtensions_SetWeights_WithValidWeights_SetsWeights()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            // Act
            blendData.SetWeights(0.5f, 0.5f, true);

            // Assert
            Assert.IsTrue(blendData.TryGetWeight(0, out float weight0), "Should get weight 0");
            Assert.IsTrue(blendData.TryGetWeight(1, out float weight1), "Should get weight 1");
            Assert.AreEqual(0.5f, weight0, 0.001f, "Weight 0 should be 0.5");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Weight 1 should be 0.5");
        }

        [Test]
        public void Day15BlendExtensions_SetEqualBlend_WithValidBlendData_SetsEqualWeights()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            // Act
            blendData.SetEqualBlend(true);

            // Assert
            Assert.IsTrue(blendData.IsEqualBlend(), "Should be at equal blend");
            Assert.IsTrue(blendData.TryGetWeight(0, out float weight0), "Should get weight 0");
            Assert.IsTrue(blendData.TryGetWeight(1, out float weight1), "Should get weight 1");
            Assert.AreEqual(0.5f, weight0, 0.001f, "Weight 0 should be 0.5");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Weight 1 should be 0.5");
        }

        [Test]
        public void Day15BlendExtensions_SetBlend_WithValidBlendValue_SetsWeights()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            // Act
            blendData.SetBlend(0.5f, true);

            // Assert
            Assert.IsTrue(blendData.TryGetBlend(out float blendValue), "Should get blend value");
            Assert.AreEqual(0.5f, blendValue, 0.001f, "Blend value should be 0.5");
        }

        [Test]
        public void Day15BlendExtensions_SetBlendSpeed_WithValidSpeed_SetsSpeed()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            // Act
            blendData.SetBlendSpeed(3.0f);

            // Assert
            Assert.AreEqual(3.0f, blendData.BlendSpeed, 0.001f, "Blend speed should be 3.0");
        }

        [Test]
        public void Day15BlendExtensions_SetNormalizeWeights_WithValidFlag_SetsFlag()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            // Act
            blendData.SetNormalizeWeights(true);

            // Assert
            Assert.IsTrue(blendData.NormalizeWeights, "Normalize weights should be true");
        }

        [Test]
        public void Day15BlendExtensions_UpdateWeights_WithSmoothTransition_UpdatesWeights()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;
            blendData.SetBlendSpeed(1.0f);
            blendData.SetWeights(0.0f, 1.0f, false);

            // Act
            blendData.UpdateWeights(1.0f);

            // Assert
            Assert.IsFalse(blendData.IsBlendComplete(), "Blend should not be complete after partial update");
        }

        [Test]
        public void Day15BlendExtensions_IsValidBlend_WithValidBlendData_ReturnsTrue()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            // Act
            bool isValid = blendData.IsValidBlend();

            // Assert
            Assert.IsTrue(isValid, "Blend data should be valid");
        }

        [Test]
        public void Day15BlendExtensions_IsEqualBlend_WithEqualWeights_ReturnsTrue()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;
            blendData.SetEqualBlend(true);

            // Act
            bool isEqual = blendData.IsEqualBlend();

            // Assert
            Assert.IsTrue(isEqual, "Should detect equal blend");
        }

        [Test]
        public void Day15BlendExtensions_TryGetWeight_WithValidInput_ReturnsWeight()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;
            blendData.SetWeight(0, 0.7f, true);

            // Act
            bool success = blendData.TryGetWeight(0, out float weight);

            // Assert
            Assert.IsTrue(success, "Should get weight successfully");
            Assert.AreEqual(0.7f, weight, 0.001f, "Weight should be 0.7");
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Day15Integration_WithAllPreviousDays_WorksCorrectly()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

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
            Assert.IsTrue(blendData.IsValidBlend(), "Blend data should be valid");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState should be valid");
            Assert.IsTrue(reverseData.IsValidReverseControl(), "Reverse control should be valid");
            Assert.IsTrue(namedGraphData.IsValidControl(), "Named graph should be valid");
        }

        [Test]
        public void Day15Integration_WeightedBlendingWithMixer_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("WeightedBlendTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            // Act
            blendData.SetEqualBlend(true);

            // Assert
            Assert.IsTrue(blendData.IsEqualBlend(), "Should be at equal blend");
            Assert.IsTrue(blendData.IsValidBlend(), "Blend data should be valid");
            Assert.IsTrue(mixerHandle.IsValidMixer(), "Mixer should be valid");
        }

        [Test]
        public void Day15Integration_SmoothTransition_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("SmoothTransitionTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var mixer = mixerHandle;
            mixer.Initialize(in unityGraph, 2);
            mixerHandle = mixer;

            mixerHandle.TryGetPlayable(out Playable mixerPlayable);

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;
            blendData.SetBlendSpeed(2.0f);
            blendData.SetWeights(0.0f, 1.0f, false);

            // Act
            blendData.UpdateWeights(0.5f);

            // Assert
            Assert.IsFalse(blendData.IsBlendComplete(), "Blend should not be complete after partial update");
            Assert.IsTrue(blendData.IsValidBlend(), "Blend data should remain valid");
        }

        [Test]
        public void Day15Integration_WeightNormalization_WorksCorrectly()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;
            blendData.SetNormalizeWeights(true);
            blendData.SetWeights(2.0f, 2.0f, true);

            // Act
            blendData.TryGetWeight(0, out float weight0);
            blendData.TryGetWeight(1, out float weight1);

            // Assert
            Assert.AreEqual(0.5f, weight0, 0.001f, "Weight 0 should be normalized to 0.5");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Weight 1 should be normalized to 0.5");
        }

        [Test]
        public void Day15Integration_Dispose_CleansUpAllResources()
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

            var blend = blendData;
            blend.Initialize(in unityGraph, in mixerPlayable, 2);
            blendData = blend;

            var rotator = rotatorData;
            rotator.Initialize(in unityGraph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);
            rotatorData = rotator;

            // Act - Dispose in reverse order
            rotatorData.Dispose();
            blendData.Dispose();
            mixerHandle.Dispose();

            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;

            // Assert
            Assert.IsFalse(disposableGraph.IsValid(), "Disposable graph should be disposed");
            Assert.IsFalse(rotatorData.IsActive, "Rotator should be inactive");
            Assert.IsFalse(blendData.IsActive, "Blend data should be inactive");
            Assert.IsFalse(mixerHandle.IsValidMixer(), "Mixer should be inactive");
        }

        #endregion
    }
}
