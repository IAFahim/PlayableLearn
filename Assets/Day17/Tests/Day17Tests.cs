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
using PlayableLearn.Day17;

namespace PlayableLearn.Day17.Tests
{
    public class Day17Tests
    {
        private Day10DisposableGraph disposableGraph;
        private Day17LayerHandle layerHandle;
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
            layerHandle = new Day17LayerHandle();
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

            if (layerHandle.IsValidLayerMixer())
            {
                layerHandle.Dispose();
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

        #region LayerMixerOps Tests

        [Test]
        public void LayerMixerOps_Create_WithValidGraph_CreatesPlayable()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            // Act
            LayerMixerOps.Create(in unityGraph, 2, out Playable playable);

            // Assert
            Assert.IsTrue(playable.IsValid(), "Playable should be valid");
            Assert.AreEqual(2, playable.GetInputCount(), "Should have 2 layers");
        }

        [Test]
        public void LayerMixerOps_IsValid_WithValidPlayable_ReturnsTrue()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            LayerMixerOps.Create(in unityGraph, 2, out Playable playable);

            // Act
            bool isValid = LayerMixerOps.IsValid(in playable);

            // Assert
            Assert.IsTrue(isValid, "Should be valid");
        }

        [Test]
        public void LayerMixerOps_GetLayerCount_WithTwoLayers_ReturnsTwo()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            LayerMixerOps.Create(in unityGraph, 2, out Playable playable);

            // Act
            LayerMixerOps.GetLayerCount(in playable, out int layerCount);

            // Assert
            Assert.AreEqual(2, layerCount, "Should have 2 layers");
        }

        [Test]
        public void LayerMixerOps_SetLayerWeight_WithValidLayer_SetsWeight()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            LayerMixerOps.Create(in unityGraph, 2, out Playable playable);

            // Act
            LayerMixerOps.SetLayerWeight(in playable, 1, 0.5f);

            // Assert
            LayerMixerOps.GetLayerWeight(in playable, 1, out float weight);
            Assert.AreEqual(0.5f, weight, 0.001f, "Layer 1 weight should be 0.5");
        }

        [Test]
        public void LayerMixerOps_GetLayerWeight_WithValidLayer_ReturnsWeight()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            LayerMixerOps.Create(in unityGraph, 2, out Playable playable);
            LayerMixerOps.SetLayerWeight(in playable, 0, 1.0f);

            // Act
            LayerMixerOps.GetLayerWeight(in playable, 0, out float weight);

            // Assert
            Assert.AreEqual(1.0f, weight, 0.001f, "Layer 0 weight should be 1.0");
        }

        [Test]
        public void LayerMixerOps_IsBaseLayer_WithLayerZero_ReturnsTrue()
        {
            // Act
            bool isBase = LayerMixerOps.IsBaseLayer(0);

            // Assert
            Assert.IsTrue(isBase, "Layer 0 should be base layer");
        }

        [Test]
        public void LayerMixerOps_IsBaseLayer_WithLayerOne_ReturnsFalse()
        {
            // Act
            bool isBase = LayerMixerOps.IsBaseLayer(1);

            // Assert
            Assert.IsFalse(isBase, "Layer 1 should not be base layer");
        }

        [Test]
        public void LayerMixerOps_IsValidLayer_WithLayerOne_ReturnsTrue()
        {
            // Act
            bool isValid = LayerMixerOps.IsValidLayer(1);

            // Assert
            Assert.IsTrue(isValid, "Layer 1 should be valid layer");
        }

        [Test]
        public void LayerMixerOps_IsValidLayer_WithLayerZero_ReturnsFalse()
        {
            // Act
            bool isValid = LayerMixerOps.IsValidLayer(0);

            // Assert
            Assert.IsFalse(isValid, "Layer 0 should not be valid layer (it's base layer)");
        }

        [Test]
        public void LayerMixerOps_ResetToBaseLayer_WithValidPlayable_ResetsWeights()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            LayerMixerOps.Create(in unityGraph, 2, out Playable playable);
            LayerMixerOps.SetLayerWeight(in playable, 1, 0.5f);

            // Act
            LayerMixerOps.ResetToBaseLayer(in playable);

            // Assert
            LayerMixerOps.GetLayerWeight(in playable, 0, out float weight0);
            LayerMixerOps.GetLayerWeight(in playable, 1, out float weight1);
            Assert.AreEqual(1.0f, weight0, 0.001f, "Base layer weight should be 1.0");
            Assert.AreEqual(0.0f, weight1, 0.001f, "Layer 1 weight should be 0.0");
        }

        [Test]
        public void LayerMixerOps_IsLayerActive_WithActiveLayer_ReturnsTrue()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            LayerMixerOps.Create(in unityGraph, 2, out Playable playable);
            LayerMixerOps.SetLayerWeight(in playable, 1, 0.5f);

            // Act
            bool isActive = LayerMixerOps.IsLayerActive(in playable, 1, 0.001f);

            // Assert
            Assert.IsTrue(isActive, "Layer 1 should be active");
        }

        [Test]
        public void LayerMixerOps_ClampLayerWeight_WithNegativeValue_ReturnsZero()
        {
            // Act
            float clamped = LayerMixerOps.ClampLayerWeight(-0.5f);

            // Assert
            Assert.AreEqual(0.0f, clamped, 0.001f, "Should clamp negative to 0");
        }

        [Test]
        public void LayerMixerOps_ClampLayerWeight_WithValueAboveOne_ReturnsOne()
        {
            // Act
            float clamped = LayerMixerOps.ClampLayerWeight(1.5f);

            // Assert
            Assert.AreEqual(1.0f, clamped, 0.001f, "Should clamp above 1 to 1");
        }

        #endregion

        #region Day17LayerHandle Tests

        [Test]
        public void Day17LayerHandle_Initialize_WithValidGraph_Initializes()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            // Act
            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            // Assert
            Assert.IsTrue(layerHandle.IsValidLayerMixer(), "Layer handle should be valid");
            Assert.AreEqual(2, layerHandle.LayerCount, "Should have 2 layers");
        }

        [Test]
        public void Day17LayerHandle_Dispose_DisposesHandle()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            // Act
            layerHandle.Dispose();

            // Assert
            Assert.IsFalse(layerHandle.IsValidLayerMixer(), "Layer handle should be invalid after disposal");
        }

        #endregion

        #region Day17LayerExtensions Tests

        [Test]
        public void Day17LayerExtensions_Initialize_WithValidGraph_InitializesHandle()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            // Act
            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            // Assert
            Assert.IsTrue(layerHandle.IsValidLayerMixer(), "Layer handle should be valid");
        }

        [Test]
        public void Day17LayerExtensions_SetLayerWeight_WithValidLayer_SetsWeight()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            // Act
            layerHandle.SetLayerWeight(1, 0.5f);

            // Assert
            Assert.IsTrue(layerHandle.TryGetLayerWeight(1, out float weight), "Should get layer weight");
            Assert.AreEqual(0.5f, weight, 0.001f, "Layer 1 weight should be 0.5");
        }

        [Test]
        public void Day17LayerExtensions_TryGetLayerWeight_WithValidLayer_ReturnsWeight()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;
            layerHandle.SetLayerWeight(0, 1.0f);

            // Act
            bool success = layerHandle.TryGetLayerWeight(0, out float weight);

            // Assert
            Assert.IsTrue(success, "Should get layer weight successfully");
            Assert.AreEqual(1.0f, weight, 0.001f, "Layer 0 weight should be 1.0");
        }

        [Test]
        public void Day17LayerExtensions_EnableLayer_WithValidLayer_EnablesLayer()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            // Act
            layerHandle.EnableLayer(1);

            // Assert
            Assert.IsTrue(layerHandle.TryGetLayerWeight(1, out float weight), "Should get layer weight");
            Assert.AreEqual(1.0f, weight, 0.001f, "Layer 1 weight should be 1.0");
        }

        [Test]
        public void Day17LayerExtensions_DisableLayer_WithValidLayer_DisablesLayer()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;
            layerHandle.EnableLayer(1);

            // Act
            layerHandle.DisableLayer(1);

            // Assert
            Assert.IsTrue(layerHandle.TryGetLayerWeight(1, out float weight), "Should get layer weight");
            Assert.AreEqual(0.0f, weight, 0.001f, "Layer 1 weight should be 0.0");
        }

        [Test]
        public void Day17LayerExtensions_ResetToBaseLayer_WithValidHandle_ResetsToBase()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;
            layerHandle.EnableLayer(1);

            // Act
            layerHandle.ResetToBaseLayer();

            // Assert
            Assert.IsTrue(layerHandle.TryGetLayerWeight(0, out float weight0), "Should get layer 0 weight");
            Assert.IsTrue(layerHandle.TryGetLayerWeight(1, out float weight1), "Should get layer 1 weight");
            Assert.AreEqual(1.0f, weight0, 0.001f, "Base layer weight should be 1.0");
            Assert.AreEqual(0.0f, weight1, 0.001f, "Layer 1 weight should be 0.0");
        }

        [Test]
        public void Day17LayerExtensions_IsLayerActive_WithActiveLayer_ReturnsTrue()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;
            layerHandle.EnableLayer(1);

            // Act
            bool isActive = layerHandle.IsLayerActive(1, 0.001f);

            // Assert
            Assert.IsTrue(isActive, "Layer 1 should be active");
        }

        [Test]
        public void Day17LayerExtensions_SetTwoLayerBlend_WithValidBlend_SetsWeights()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            // Act
            layerHandle.SetTwoLayerBlend(0.5f);

            // Assert
            Assert.IsTrue(layerHandle.TryGetLayerWeight(0, out float weight0), "Should get layer 0 weight");
            Assert.IsTrue(layerHandle.TryGetLayerWeight(1, out float weight1), "Should get layer 1 weight");
            Assert.AreEqual(1.0f, weight0, 0.001f, "Base layer weight should be 1.0");
            Assert.AreEqual(0.5f, weight1, 0.001f, "Layer 1 weight should be 0.5");
        }

        [Test]
        public void Day17LayerExtensions_TryGetTwoLayerBlend_WithValidHandle_ReturnsBlend()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;
            layerHandle.SetTwoLayerBlend(0.7f);

            // Act
            bool success = layerHandle.TryGetTwoLayerBlend(out float blend);

            // Assert
            Assert.IsTrue(success, "Should get blend successfully");
            Assert.AreEqual(0.7f, blend, 0.001f, "Blend should be 0.7");
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Day17Integration_WithAllPreviousDays_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("IntegrationTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

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
            Assert.IsTrue(layerHandle.IsValidLayerMixer(), "Layer mixer should be valid");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState should be valid");
            Assert.IsTrue(reverseData.IsValidReverseControl(), "Reverse control should be valid");
            Assert.IsTrue(namedGraphData.IsValidControl(), "Named graph should be valid");
        }

        [Test]
        public void Day17Integration_LayerWeightControl_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("LayerTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            // Act - Enable additive layer
            layerHandle.EnableLayer(1);

            // Assert - Check layer 1 is active
            Assert.IsTrue(layerHandle.IsLayerActive(1, 0.001f), "Layer 1 should be active");
            Assert.IsTrue(layerHandle.IsValidLayerMixer(), "Layer mixer should remain valid");
        }

        [Test]
        public void Day17Integration_MultipleLayerOperations_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("LayerOpsTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            // Act & Assert - Test layer 0 (base layer)
            Assert.IsTrue(layerHandle.IsLayerActive(0, 0.001f), "Base layer should be active initially");

            // Act & Assert - Enable layer 1
            layerHandle.EnableLayer(1);
            Assert.IsTrue(layerHandle.IsLayerActive(1, 0.001f), "Layer 1 should be active after enable");

            // Act & Assert - Set layer 1 to 50%
            layerHandle.SetLayerWeight(1, 0.5f);
            Assert.IsTrue(layerHandle.TryGetLayerWeight(1, out float weight), "Should get layer 1 weight");
            Assert.AreEqual(0.5f, weight, 0.001f, "Layer 1 weight should be 0.5");

            // Act & Assert - Reset to base layer
            layerHandle.ResetToBaseLayer();
            Assert.IsFalse(layerHandle.IsLayerActive(1, 0.001f), "Layer 1 should be inactive after reset");
            Assert.IsTrue(layerHandle.IsLayerActive(0, 0.001f), "Base layer should still be active");
        }

        [Test]
        public void Day17Integration_TwoLayerBlend_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("BlendTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            // Act - Set blend to 0.5
            layerHandle.SetTwoLayerBlend(0.5f);

            // Assert
            Assert.IsTrue(layerHandle.TryGetTwoLayerBlend(out float blend), "Should get blend value");
            Assert.AreEqual(0.5f, blend, 0.001f, "Blend should be 0.5");

            // Act - Set blend to 1.0 (full additive)
            layerHandle.SetTwoLayerBlend(1.0f);

            // Assert
            Assert.IsTrue(layerHandle.TryGetTwoLayerBlend(out blend), "Should get blend value");
            Assert.AreEqual(1.0f, blend, 0.001f, "Blend should be 1.0");
        }

        [Test]
        public void Day17Integration_Dispose_CleansUpAllResources()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("IntegrationTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var layer = layerHandle;
            layer.Initialize(in unityGraph, 2);
            layerHandle = layer;

            var rotator = rotatorData;
            rotator.Initialize(in unityGraph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);
            rotatorData = rotator;

            // Act - Dispose in reverse order
            rotatorData.Dispose();
            layerHandle.Dispose();

            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;

            // Assert
            Assert.IsFalse(disposableGraph.IsValid(), "Disposable graph should be disposed");
            Assert.IsFalse(rotatorData.IsActive, "Rotator should be inactive");
            Assert.IsFalse(layerHandle.IsValidLayerMixer(), "Layer mixer should be inactive");
        }

        #endregion
    }
}
