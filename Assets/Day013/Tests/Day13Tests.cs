using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;
using PlayableLearn.Day10;
using PlayableLearn.Day13;

namespace PlayableLearn.Day13.Tests
{
    /// <summary>
    /// Tests for Day 13: The Mixer (AnimationMixerPlayable).
    /// Tests mixer creation, weight management, input connection, and blending.
    /// </summary>
    public class Day13Tests
    {
        private PlayableGraph graph;
        private Day10DisposableGraph disposableGraph;

        [SetUp]
        public void SetUp()
        {
            disposableGraph = new Day10DisposableGraph();
            disposableGraph.Initialize("TestGraph");
            graph = disposableGraph.Graph;
        }

        [TearDown]
        public void TearDown()
        {
            disposableGraph.Dispose();
        }

        [Test]
        public void MixerHandle_Initialize_IsActive()
        {
            // Arrange & Act
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);

            // Assert
            Assert.IsTrue(handle.IsActive, "Mixer handle should be active after initialization.");
            Assert.IsTrue(handle.IsValidMixer(), "Mixer handle should be valid after initialization.");
        }

        [Test]
        public void MixerHandle_Initialize_InvalidInputCount_FailsGracefully()
        {
            // Arrange & Act
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 0);

            // Assert
            Assert.IsFalse(handle.IsActive, "Mixer handle should not be active with invalid input count.");
        }

        [Test]
        public void MixerHandle_Dispose_IsNotActive()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);

            // Act
            handle.Dispose();

            // Assert
            Assert.IsFalse(handle.IsActive, "Mixer handle should not be active after disposal.");
            Assert.IsFalse(handle.IsValidMixer(), "Mixer handle should not be valid after disposal.");
        }

        [Test]
        public void MixerHandle_GetInputCount_ReturnsCorrectCount()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            int expectedInputCount = 4;
            handle.Initialize(in graph, expectedInputCount);

            // Act
            bool success = handle.TryGetInputCount(out int actualInputCount);

            // Assert
            Assert.IsTrue(success, "Should successfully get input count.");
            Assert.AreEqual(expectedInputCount, actualInputCount, "Input count should match the initialized value.");
        }

        [Test]
        public void MixerHandle_SetInputWeight_WeightIsSet()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);
            float expectedWeight = 0.75f;

            // Act
            handle.SetInputWeight(0, expectedWeight);

            // Assert
            bool success = handle.TryGetInputWeight(0, out float actualWeight);
            Assert.IsTrue(success, "Should successfully get input weight.");
            Assert.AreEqual(expectedWeight, actualWeight, 0.001f, "Input weight should match the set value.");
        }

        [Test]
        public void MixerHandle_SetInputWeight_InvalidIndex_DoesNotCrash()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);

            // Act & Assert (should not throw)
            Assert.DoesNotThrow(() => handle.SetInputWeight(5, 0.5f));
        }

        [Test]
        public void MixerHandle_ConnectInput_InputIsConnected()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);

            // Create a simple playable to connect
            var playable = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);

            // Act
            bool connected = handle.ConnectInput(ref playable, 0, 0.5f);

            // Assert
            Assert.IsTrue(connected, "Input should be successfully connected.");
            Assert.IsTrue(handle.IsInputConnected(0), "Input should be marked as connected.");
        }

        [Test]
        public void MixerHandle_DisconnectInput_InputIsNotConnected()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);
            var playable = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            handle.ConnectInput(ref playable, 0, 0.5f);

            // Act
            handle.DisconnectInput(0);

            // Assert
            Assert.IsFalse(handle.IsInputConnected(0), "Input should not be connected after disconnection.");
        }

        [Test]
        public void MixerHandle_SetBlend_SetsWeightsCorrectly()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);
            float blendValue = 0.6f;

            // Act
            handle.SetBlend(blendValue);

            // Assert
            handle.TryGetInputWeight(0, out float weight0);
            handle.TryGetInputWeight(1, out float weight1);

            Assert.AreEqual(1.0f - blendValue, weight0, 0.001f, "Input 0 weight should be (1 - blend).");
            Assert.AreEqual(blendValue, weight1, 0.001f, "Input 1 weight should be blend.");
        }

        [Test]
        public void MixerHandle_SetBlend_ClampsToZero()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);

            // Act
            handle.SetBlend(-0.5f);

            // Assert
            handle.TryGetInputWeight(0, out float weight0);
            handle.TryGetInputWeight(1, out float weight1);

            Assert.AreEqual(1.0f, weight0, 0.001f, "Input 0 weight should be 1.0 when blend is clamped to 0.");
            Assert.AreEqual(0.0f, weight1, 0.001f, "Input 1 weight should be 0.0 when blend is clamped to 0.");
        }

        [Test]
        public void MixerHandle_SetBlend_ClampsToOne()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);

            // Act
            handle.SetBlend(1.5f);

            // Assert
            handle.TryGetInputWeight(0, out float weight0);
            handle.TryGetInputWeight(1, out float weight1);

            Assert.AreEqual(0.0f, weight0, 0.001f, "Input 0 weight should be 0.0 when blend is clamped to 1.");
            Assert.AreEqual(1.0f, weight1, 0.001f, "Input 1 weight should be 1.0 when blend is clamped to 1.");
        }

        [Test]
        public void MixerHandle_TryGetBlend_ReturnsCorrectBlend()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);
            float expectedBlend = 0.8f;
            handle.SetBlend(expectedBlend);

            // Act
            bool success = handle.TryGetBlend(out float actualBlend);

            // Assert
            Assert.IsTrue(success, "Should successfully get blend value.");
            Assert.AreEqual(expectedBlend, actualBlend, 0.001f, "Blend value should match the set value.");
        }

        [Test]
        public void MixerHandle_ClearAllWeights_AllWeightsAreZero()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 3);
            handle.SetInputWeight(0, 0.5f);
            handle.SetInputWeight(1, 0.7f);
            handle.SetInputWeight(2, 0.3f);

            // Act
            handle.ClearAllWeights();

            // Assert
            handle.TryGetInputWeight(0, out float weight0);
            handle.TryGetInputWeight(1, out float weight1);
            handle.TryGetInputWeight(2, out float weight2);

            Assert.AreEqual(0.0f, weight0, 0.001f, "Input 0 weight should be 0.0 after clearing.");
            Assert.AreEqual(0.0f, weight1, 0.001f, "Input 1 weight should be 0.0 after clearing.");
            Assert.AreEqual(0.0f, weight2, 0.001f, "Input 2 weight should be 0.0 after clearing.");
        }

        [Test]
        public void MixerHandle_NormalizeWeights_WeightsSumToOne()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 3);
            handle.SetInputWeight(0, 2.0f);
            handle.SetInputWeight(1, 1.0f);
            handle.SetInputWeight(2, 1.0f);

            // Act
            handle.NormalizeWeights();

            // Assert
            handle.TryGetInputWeight(0, out float weight0);
            handle.TryGetInputWeight(1, out float weight1);
            handle.TryGetInputWeight(2, out float weight2);

            float sum = weight0 + weight1 + weight2;
            Assert.AreEqual(1.0f, sum, 0.001f, "Normalized weights should sum to 1.0.");
        }

        [Test]
        public void MixerHandle_TryGetPlayable_ReturnsPlayable()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);

            // Act
            bool success = handle.TryGetPlayable(out Playable playable);

            // Assert
            Assert.IsTrue(success, "Should successfully get playable.");
            Assert.IsTrue(playable.IsValid(), "Returned playable should be valid.");
        }

        [Test]
        public void MixerHandle_ConnectToOutput_ConnectsSuccessfully()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);

            // Create a script output
            var outputHandle = new Day02OutputHandle();
            outputHandle.Initialize(in graph, "TestOutput");

            // Act
            if (outputHandle.TryGetOutput(out PlayableOutput output))
            {
                handle.ConnectToOutput(in output);
            }

            // Assert
            Assert.IsTrue(outputHandle.Output.IsOutputValid(), "Output should be valid.");
            Assert.IsTrue(handle.IsValidMixer(), "Mixer should still be valid after connection.");
        }

        [Test]
        public void MixerOps_Create_ValidMixer()
        {
            // Act
            MixerOps.Create(in graph, 2, out Playable playable);

            // Assert
            Assert.IsTrue(playable.IsValid(), "Mixer playable should be valid.");
        }

        [Test]
        public void MixerOps_Create_ZeroInputs_ReturnsNull()
        {
            // Act
            MixerOps.Create(in graph, 0, out Playable playable);

            // Assert
            Assert.IsFalse(playable.IsValid(), "Mixer playable should not be valid with 0 inputs.");
        }

        [Test]
        public void MixerOps_SetInputWeight_WeightIsSet()
        {
            // Arrange
            MixerOps.Create(in graph, 2, out Playable playable);
            float expectedWeight = 0.6f;

            // Act
            MixerOps.SetInputWeight(in playable, 0, expectedWeight);

            // Assert
            MixerOps.GetInputWeight(in playable, 0, out float actualWeight);
            Assert.AreEqual(expectedWeight, actualWeight, 0.001f, "Weight should match the set value.");
        }

        [Test]
        public void MixerOps_IsInputConnected_ReturnsCorrectStatus()
        {
            // Arrange
            MixerOps.Create(in graph, 2, out Playable mixer);
            var inputPlayable = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);

            // Act
            bool connected = MixerOps.ConnectInput(in mixer, in inputPlayable, 0);

            // Assert
            Assert.IsTrue(connected, "Input should be connected.");
            Assert.IsTrue(MixerOps.IsInputConnected(in mixer, 0), "Input should report as connected.");
        }

        // Helper class for creating test playables
        private class EmptyPlayableBehaviour : PlayableBehaviour
        {
            // Empty behaviour for testing
        }

        [Test]
        public void MixerHandle_MultipleInputs_AllConnected()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            int inputCount = 4;
            handle.Initialize(in graph, inputCount);

            // Act
            for (int i = 0; i < inputCount; i++)
            {
                var playable = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
                handle.ConnectInput(ref playable, i, 0.25f);
            }

            // Assert
            for (int i = 0; i < inputCount; i++)
            {
                Assert.IsTrue(handle.IsInputConnected(i), $"Input {i} should be connected.");
            }
        }

        [Test]
        public void MixerHandle_BlendTransition_SmoothTransition()
        {
            // Arrange
            var handle = new Day13MixerHandle();
            handle.Initialize(in graph, 2);
            handle.SetBlend(0.0f);

            // Act - Simulate transition
            for (int i = 0; i <= 10; i++)
            {
                float blend = i / 10.0f;
                handle.SetBlend(blend);

                handle.TryGetInputWeight(0, out float weight0);
                handle.TryGetInputWeight(1, out float weight1);

                // Verify weights sum to approximately 1.0
                float sum = weight0 + weight1;
                Assert.AreEqual(1.0f, sum, 0.001f, $"Weights should sum to 1.0 at blend {blend}.");
            }
        }

        [Test]
        public void MixerHandle_InitializationWithDifferentInputCounts_AllSucceed()
        {
            // Test various input counts
            int[] inputCounts = { 2, 3, 4, 8, 16 };

            foreach (int inputCount in inputCounts)
            {
                // Arrange & Act
                var handle = new Day13MixerHandle();
                handle.Initialize(in graph, inputCount);

                // Assert
                Assert.IsTrue(handle.IsValidMixer(), $"Mixer with {inputCount} inputs should be valid.");

                bool success = handle.TryGetInputCount(out int actualCount);
                Assert.IsTrue(success, "Should get input count.");
                Assert.AreEqual(inputCount, actualCount, $"Input count should be {inputCount}.");

                // Cleanup
                handle.Dispose();
            }
        }
    }
}
