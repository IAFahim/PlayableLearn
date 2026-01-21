using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;
using PlayableLearn.Day10;
using PlayableLearn.Day13;
using PlayableLearn.Day14;

namespace PlayableLearn.Day14.Tests
{
    /// <summary>
    /// Tests for Day 14: Hard Swapping.
    /// Tests swap data initialization, hard swap operations, connection management, and weight control.
    /// </summary>
    public class Day14Tests
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
        public void SwapData_Initialize_IsActive()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();

            // Act
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Assert
            Assert.IsTrue(swapData.IsActive, "Swap data should be active after initialization.");
            Assert.IsTrue(swapData.IsValidSwap(), "Swap data should be valid after initialization.");
        }

        [Test]
        public void SwapData_Initialize_InvalidMixer_FailsGracefully()
        {
            // Arrange
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            Playable invalidMixer = Playable.Null;

            // Act
            swapData.Initialize(in graph, in invalidMixer, in input0, in input1);

            // Assert
            Assert.IsFalse(swapData.IsActive, "Swap data should not be active with invalid mixer.");
        }

        [Test]
        public void SwapData_Dispose_IsNotActive()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Act
            swapData.Dispose();

            // Assert
            Assert.IsFalse(swapData.IsActive, "Swap data should not be active after disposal.");
            Assert.IsFalse(swapData.IsValidSwap(), "Swap data should not be valid after disposal.");
        }

        [Test]
        public void SwapData_SwapToInput1_DisconnectsInput0_ConnectsInput1()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);

            // Connect input 0 first
            mixer.AddInput(input0, 0, 1.0f);

            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Act
            bool success = swapData.SwapToInput1(1.0f);

            // Assert
            Assert.IsTrue(success, "Swap to input 1 should succeed.");
            Assert.IsFalse(mixer.IsInputConnected(0), "Input 0 should be disconnected after swap.");
            Assert.IsTrue(mixer.IsInputConnected(1), "Input 1 should be connected after swap.");
        }

        [Test]
        public void SwapData_SwapToInput0_DisconnectsInput1_ConnectsInput0()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);

            // Connect input 1 first
            mixer.AddInput(input1, 1, 1.0f);

            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Act
            bool success = swapData.SwapToInput0(1.0f);

            // Assert
            Assert.IsTrue(success, "Swap to input 0 should succeed.");
            Assert.IsTrue(mixer.IsInputConnected(0), "Input 0 should be connected after swap.");
            Assert.IsFalse(mixer.IsInputConnected(1), "Input 1 should be disconnected after swap.");
        }

        [Test]
        public void SwapData_ToggleInput_SwitchesBetweenInputs()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);

            mixer.AddInput(input0, 0, 1.0f);

            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Act - First toggle
            bool success1 = swapData.ToggleInput(1.0f);

            // Assert - After first toggle
            Assert.IsTrue(success1, "First toggle should succeed.");
            Assert.IsTrue(swapData.IsUsingInput1(), "Should be using input 1 after first toggle.");

            // Act - Second toggle
            bool success2 = swapData.ToggleInput(1.0f);

            // Assert - After second toggle
            Assert.IsTrue(success2, "Second toggle should succeed.");
            Assert.IsFalse(swapData.IsUsingInput1(), "Should be using input 0 after second toggle.");
        }

        [Test]
        public void SwapData_TryGetInput0Weight_ReturnsCorrectWeight()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            float expectedWeight = 0.75f;
            mixer.AddInput(input0, 0, expectedWeight);

            // Act
            bool success = swapData.TryGetInput0Weight(out float actualWeight);

            // Assert
            Assert.IsTrue(success, "Should successfully get input 0 weight.");
            Assert.AreEqual(expectedWeight, actualWeight, 0.001f, "Weight should match the set value.");
        }

        [Test]
        public void SwapData_TryGetInput1Weight_ReturnsCorrectWeight()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            float expectedWeight = 0.85f;
            mixer.AddInput(input1, 1, expectedWeight);

            // Act
            bool success = swapData.TryGetInput1Weight(out float actualWeight);

            // Assert
            Assert.IsTrue(success, "Should successfully get input 1 weight.");
            Assert.AreEqual(expectedWeight, actualWeight, 0.001f, "Weight should match the set value.");
        }

        [Test]
        public void SwapData_SetInput0Weight_WeightIsSet()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            mixer.AddInput(input0, 0, 0.0f);

            float expectedWeight = 0.6f;

            // Act
            swapData.SetInput0Weight(expectedWeight);

            // Assert
            float actualWeight = mixer.GetInputWeight(0);
            Assert.AreEqual(expectedWeight, actualWeight, 0.001f, "Weight should match the set value.");
        }

        [Test]
        public void SwapData_SetInput1Weight_WeightIsSet()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            mixer.AddInput(input1, 1, 0.0f);

            float expectedWeight = 0.7f;

            // Act
            swapData.SetInput1Weight(expectedWeight);

            // Assert
            float actualWeight = mixer.GetInputWeight(1);
            Assert.AreEqual(expectedWeight, actualWeight, 0.001f, "Weight should match the set value.");
        }

        [Test]
        public void SwapData_IsUsingInput1_ReturnsCorrectState()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Act & Assert - Initially should be false
            Assert.IsFalse(swapData.IsUsingInput1(), "Initially should not be using input 1.");

            // Act - Swap to input 1
            swapData.SwapToInput1(1.0f);

            // Assert - Should now be true
            Assert.IsTrue(swapData.IsUsingInput1(), "Should be using input 1 after swap.");
        }

        [Test]
        public void SwapData_IsInput0Connected_ReturnsCorrectStatus()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Act & Assert - Initially not connected
            Assert.IsFalse(swapData.IsInput0Connected(), "Input 0 should not be connected initially.");

            // Act - Connect input 0
            mixer.AddInput(input0, 0, 1.0f);

            // Assert - Should now be connected
            Assert.IsTrue(swapData.IsInput0Connected(), "Input 0 should be connected after adding.");
        }

        [Test]
        public void SwapData_IsInput1Connected_ReturnsCorrectStatus()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Act & Assert - Initially not connected
            Assert.IsFalse(swapData.IsInput1Connected(), "Input 1 should not be connected initially.");

            // Act - Connect input 1
            mixer.AddInput(input1, 1, 1.0f);

            // Assert - Should now be connected
            Assert.IsTrue(swapData.IsInput1Connected(), "Input 1 should be connected after adding.");
        }

        [Test]
        public void SwapData_DisconnectAll_DisconnectsBothInputs()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            mixer.AddInput(input0, 0, 1.0f);
            mixer.AddInput(input1, 1, 1.0f);

            // Act
            swapData.DisconnectAll();

            // Assert
            Assert.IsFalse(mixer.IsInputConnected(0), "Input 0 should be disconnected.");
            Assert.IsFalse(mixer.IsInputConnected(1), "Input 1 should be disconnected.");
        }

        [Test]
        public void SwapConnectionOps_DisconnectInput0_DisconnectsSuccessfully()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            mixer.AddInput(input0, 0, 1.0f);

            // Act
            SwapConnectionOps.DisconnectInput0(in mixer);

            // Assert
            Assert.IsFalse(mixer.IsInputConnected(0), "Input 0 should be disconnected.");
        }

        [Test]
        public void SwapConnectionOps_DisconnectInput1_DisconnectsSuccessfully()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            mixer.AddInput(input1, 1, 1.0f);

            // Act
            SwapConnectionOps.DisconnectInput1(in mixer);

            // Assert
            Assert.IsFalse(mixer.IsInputConnected(1), "Input 1 should be disconnected.");
        }

        [Test]
        public void SwapConnectionOps_ConnectInput0_ConnectsSuccessfully()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);

            // Act
            bool success = SwapConnectionOps.ConnectInput0(in mixer, in input0, 0.8f);

            // Assert
            Assert.IsTrue(success, "Connection should succeed.");
            Assert.IsTrue(mixer.IsInputConnected(0), "Input 0 should be connected.");
            Assert.AreEqual(0.8f, mixer.GetInputWeight(0), 0.001f, "Weight should be set correctly.");
        }

        [Test]
        public void SwapConnectionOps_ConnectInput1_ConnectsSuccessfully()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);

            // Act
            bool success = SwapConnectionOps.ConnectInput1(in mixer, in input1, 0.9f);

            // Assert
            Assert.IsTrue(success, "Connection should succeed.");
            Assert.IsTrue(mixer.IsInputConnected(1), "Input 1 should be connected.");
            Assert.AreEqual(0.9f, mixer.GetInputWeight(1), 0.001f, "Weight should be set correctly.");
        }

        [Test]
        public void SwapConnectionOps_HardSwapToInput1_SwapsSuccessfully()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);

            mixer.AddInput(input0, 0, 1.0f);

            // Act
            bool success = SwapConnectionOps.HardSwapToInput1(in mixer, in input1, 1.0f);

            // Assert
            Assert.IsTrue(success, "Hard swap should succeed.");
            Assert.IsFalse(mixer.IsInputConnected(0), "Input 0 should be disconnected.");
            Assert.IsTrue(mixer.IsInputConnected(1), "Input 1 should be connected.");
        }

        [Test]
        public void SwapConnectionOps_HardSwapToInput0_SwapsSuccessfully()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);

            mixer.AddInput(input1, 1, 1.0f);

            // Act
            bool success = SwapConnectionOps.HardSwapToInput0(in mixer, in input0, 1.0f);

            // Assert
            Assert.IsTrue(success, "Hard swap should succeed.");
            Assert.IsTrue(mixer.IsInputConnected(0), "Input 0 should be connected.");
            Assert.IsFalse(mixer.IsInputConnected(1), "Input 1 should be disconnected.");
        }

        [Test]
        public void SwapConnectionOps_HasValidInputPorts_ReturnsTrueForValidMixer()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);

            // Act
            bool isValid = SwapConnectionOps.HasValidInputPorts(in mixer);

            // Assert
            Assert.IsTrue(isValid, "Mixer with 2 inputs should be valid.");
        }

        [Test]
        public void SwapConnectionOps_HasValidInputPorts_ReturnsFalseForInvalidMixer()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 1);

            // Act
            bool isValid = SwapConnectionOps.HasValidInputPorts(in mixer);

            // Assert
            Assert.IsFalse(isValid, "Mixer with 1 input should not be valid for swapping.");
        }

        [Test]
        public void SwapData_MultipleSwaps_AllSucceed()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Act - Perform multiple swaps
            bool success1 = swapData.SwapToInput0(1.0f);
            bool success2 = swapData.SwapToInput1(1.0f);
            bool success3 = swapData.SwapToInput0(0.5f);
            bool success4 = swapData.SwapToInput1(0.5f);

            // Assert
            Assert.IsTrue(success1, "First swap should succeed.");
            Assert.IsTrue(success2, "Second swap should succeed.");
            Assert.IsTrue(success3, "Third swap should succeed.");
            Assert.IsTrue(success4, "Fourth swap should succeed.");
        }

        [Test]
        public void SwapData_SwapWithDifferentWeights_WeightsAreCorrect()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            float testWeight = 0.65f;

            // Act
            swapData.SwapToInput1(testWeight);

            // Assert
            float actualWeight = mixer.GetInputWeight(1);
            Assert.AreEqual(testWeight, actualWeight, 0.001f, "Weight should match the swap weight.");
        }

        [Test]
        public void SwapData_InvalidSwapOperation_DoesNotCrash()
        {
            // Arrange
            var mixer = AnimationMixerPlayable.Create(graph, 2);
            var input0 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var input1 = ScriptPlayable.Create<EmptyPlayableBehaviour>(graph);
            var swapData = new Day14SwapData();
            swapData.Initialize(in graph, in mixer, in input0, in input1);

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() => swapData.SwapToInput1(1.5f));
            Assert.DoesNotThrow(() => swapData.SwapToInput0(-0.5f));
        }

        // Helper class for creating test playables
        private class EmptyPlayableBehaviour : PlayableBehaviour
        {
            // Empty behaviour for testing
        }
    }
}
