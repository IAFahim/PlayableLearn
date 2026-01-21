using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day03;

namespace PlayableLearn.Day03.Tests
{
    public class Day03Tests
    {
        private Day01GraphHandle graphHandle;
        private Day02OutputHandle outputHandle;
        private Day03NodeHandle nodeHandle;

        [SetUp]
        public void SetUp()
        {
            graphHandle = new Day01GraphHandle();
            outputHandle = new Day02OutputHandle();
            nodeHandle = new Day03NodeHandle();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up in reverse order
            if (nodeHandle.IsActive)
            {
                nodeHandle.Dispose();
            }

            if (outputHandle.IsActive)
            {
                outputHandle.Dispose();
            }

            if (graphHandle.IsActive)
            {
                graphHandle.Dispose();
            }
        }

        [Test]
        public void GraphHandle_InitializesCorrectly()
        {
            // Act
            graphHandle.Initialize("TestGraph");

            // Assert
            Assert.IsTrue(graphHandle.IsActive, "Graph should be active after initialization");
            Assert.IsTrue(graphHandle.Graph.IsValid(), "Graph should be valid");
        }

        [Test]
        public void OutputHandle_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act
            outputHandle.Initialize(in graphHandle.Graph, "TestOutput");

            // Assert
            Assert.IsTrue(outputHandle.IsActive, "Output should be active after initialization");
            Assert.IsTrue(outputHandle.IsValidOutput(), "Output should be valid");
        }

        [Test]
        public void NodeHandle_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act
            nodeHandle.Initialize(in graphHandle.Graph, "TestNode");

            // Assert
            Assert.IsTrue(nodeHandle.IsActive, "Node should be active after initialization");
            Assert.IsTrue(nodeHandle.IsValidNode(), "Node should be valid");
        }

        [Test]
        public void NodeHandle_ConnectsToOutputCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            outputHandle.Initialize(in graphHandle.Graph, "TestOutput");
            nodeHandle.Initialize(in graphHandle.Graph, "TestNode");

            // Act
            nodeHandle.ConnectToOutput(in outputHandle.Output);

            // Assert
            Assert.IsTrue(nodeHandle.IsValidNode(), "Node should still be valid after connection");
            Assert.IsTrue(outputHandle.IsValidOutput(), "Output should still be valid after connection");

            // Verify the connection by checking the output source
            Playable source = outputHandle.Output.GetSourcePlayable();
            Assert.IsTrue(source.IsValid(), "Output source should be valid");
            Assert.AreEqual(nodeHandle.Node, source, "Output source should be the connected node");
        }

        [Test]
        public void NodeHandle_ReturnsCorrectPortInfo()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            nodeHandle.Initialize(in graphHandle.Graph, "TestNode");

            // Act
            bool hasPortInfo = nodeHandle.TryGetPortInfo(out int inputCount, out int outputCount);

            // Assert
            Assert.IsTrue(hasPortInfo, "Should be able to get port info");
            Assert.AreEqual(0, inputCount, "Day 03 node should have 0 input ports");
            Assert.AreEqual(0, outputCount, "Day 03 node should have 0 output ports");
        }

        [Test]
        public void NodeHandle_ReturnsCorrectNodeType()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            nodeHandle.Initialize(in graphHandle.Graph, "TestNode");

            // Act
            bool hasNodeType = nodeHandle.TryGetNodeType(out PlayableType nodeType);

            // Assert
            Assert.IsTrue(hasNodeType, "Should be able to get node type");
            Assert.AreEqual(PlayableType.ScriptPlayable, nodeType, "Node type should be ScriptPlayable");
        }

        [Test]
        public void NodeHandle_DisposesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            nodeHandle.Initialize(in graphHandle.Graph, "TestNode");

            // Act
            nodeHandle.Dispose();

            // Assert
            Assert.IsFalse(nodeHandle.IsActive, "Node should not be active after disposal");
        }

        [Test]
        public void CompleteFlow_GraphOutputNode_WorksCorrectly()
        {
            // Act - Create the complete chain
            graphHandle.Initialize("CompleteTestGraph");
            outputHandle.Initialize(in graphHandle.Graph, "CompleteTestOutput");
            nodeHandle.Initialize(in graphHandle.Graph, "CompleteTestNode");
            nodeHandle.ConnectToOutput(in outputHandle.Output);

            // Assert - Verify all components are valid
            Assert.IsTrue(graphHandle.IsActive, "Graph should be active");
            Assert.IsTrue(outputHandle.IsValidOutput(), "Output should be valid");
            Assert.IsTrue(nodeHandle.IsValidNode(), "Node should be valid");

            // Verify the connection
            Playable source = outputHandle.Output.GetSourcePlayable();
            Assert.IsTrue(source.IsValid(), "Output should have a valid source");
        }

        [Test]
        public void NodeHandle_WithInvalidGraph_DoesNotInitialize()
        {
            // Arrange - Invalid graph
            PlayableGraph invalidGraph = default;

            // Act
            nodeHandle.Initialize(in invalidGraph, "TestNode");

            // Assert
            Assert.IsFalse(nodeHandle.IsActive, "Node should not be active with invalid graph");
        }

        [Test]
        public void NodeHandle_WithInvalidOutput_DoesNotConnect()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            nodeHandle.Initialize(in graphHandle.Graph, "TestNode");
            PlayableOutput invalidOutput = default;

            // Act - This should not crash, just fail gracefully
            nodeHandle.ConnectToOutput(in invalidOutput);

            // Assert - Node should still be valid
            Assert.IsTrue(nodeHandle.IsValidNode(), "Node should remain valid even if connection fails");
        }
    }
}
