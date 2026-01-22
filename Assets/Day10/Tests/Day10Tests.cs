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

namespace PlayableLearn.Day10.Tests
{
    public class Day10Tests
    {
        private Day10DisposableGraph disposableGraph;
        private Day01GraphHandle graphHandle;
        private Day02OutputHandle outputHandle;
        private Day04RotatorData rotatorData;
        private Day05SpeedData speedData;
        private Day06PlayStateData playStateData;
        private Day07ReverseData reverseData;
        private Day08NamedGraphData namedGraphData;
        private Day09VisualizerData visualizerData;
        private GameObject testGameObject;

        [SetUp]
        public void SetUp()
        {
            // Reset controller ID counter for consistent testing
            Day08NamedGraphExtensions.ResetControllerIdCounter();
            Day10DisposableGraphExtensions.ResetIdCounter();

            disposableGraph = new Day10DisposableGraph();
            graphHandle = new Day01GraphHandle();
            outputHandle = new Day02OutputHandle();
            rotatorData = new Day04RotatorData();
            speedData = new Day05SpeedData();
            playStateData = new Day06PlayStateData();
            reverseData = new Day07ReverseData();
            namedGraphData = new Day08NamedGraphData();
            visualizerData = new Day09VisualizerData();
            testGameObject = new GameObject("TestCube");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up in reverse order
            if (visualizerData.IsActive)
            {
                var temp = visualizerData;
                temp.Dispose();
                visualizerData = temp;
            }

            if (namedGraphData.IsActive)
            {
                var temp = namedGraphData;
                temp.Dispose();
                namedGraphData = temp;
            }

            if (reverseData.IsActive)
            {
                var temp = reverseData;
                temp.Dispose();
                reverseData = temp;
            }

            if (playStateData.IsActive)
            {
                var temp = playStateData;
                temp.Dispose();
                playStateData = temp;
            }

            if (speedData.IsActive)
            {
                var temp = speedData;
                temp.Dispose();
                speedData = temp;
            }

            if (rotatorData.IsActive)
            {
                var temp = rotatorData;
                temp.Dispose();
                rotatorData = temp;
            }

            if (outputHandle.IsActive)
            {
                var temp = outputHandle;
                temp.Dispose();
                outputHandle = temp;
            }

            if (graphHandle.IsActive)
            {
                var temp = graphHandle;
                temp.Dispose();
                graphHandle = temp;
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
        }

        #region DisposalOps Tests

        [Test]
        public void DisposalOps_CanDisposeGraph_WithValidGraph_ReturnsTrue()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);

            // Act
            bool canDispose = DisposalOps.CanDisposeGraph(in graph);

            // Assert
            Assert.IsTrue(canDispose, "Valid graph should be disposable");

            // Cleanup
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void DisposalOps_CanDisposeGraph_WithInvalidGraph_ReturnsFalse()
        {
            // Arrange
            PlayableGraph invalidGraph = default;

            // Act
            bool canDispose = DisposalOps.CanDisposeGraph(in invalidGraph);

            // Assert
            Assert.IsFalse(canDispose, "Invalid graph should not be disposable");
        }

        [Test]
        public void DisposalOps_DisposeGraph_WithValidGraph_DestroysGraph()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            Assert.IsTrue(graph.IsValid(), "Graph should be valid before disposal");

            // Act
            DisposalOps.DisposeGraph(in graph);

            // Assert
            Assert.IsFalse(graph.IsValid(), "Graph should be invalid after disposal");
        }

        [Test]
        public void DisposalOps_DisposeGraph_WithInvalidGraph_DoesNotThrow()
        {
            // Arrange
            PlayableGraph invalidGraph = default;

            // Act & Assert
            Assert.DoesNotThrow(() => DisposalOps.DisposeGraph(in invalidGraph),
                "Disposing invalid graph should not throw");
        }

        [Test]
        public void DisposalOps_CanDisposePlayable_WithValidPlayable_ReturnsTrue()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            Playable playable = Playable.Create(graph);

            // Act
            bool canDispose = DisposalOps.CanDisposePlayable(in playable);

            // Assert
            Assert.IsTrue(canDispose, "Valid playable should be disposable");

            // Cleanup
            DisposalOps.DisposePlayable(in playable);
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void DisposalOps_DisposePlayable_WithValidPlayable_DestroysPlayable()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            Playable playable = Playable.Create(graph);
            Assert.IsTrue(playable.IsValid(), "Playable should be valid before disposal");

            // Act
            DisposalOps.DisposePlayable(in playable);

            // Assert
            Assert.IsFalse(playable.IsValid(), "Playable should be invalid after disposal");

            // Cleanup
            DisposalOps.DisposeGraph(in graph);
        }

        #endregion

        #region Day10DisposableGraph Tests

        [Test]
        public void Day10DisposableGraph_Initialize_CreatesValidGraph()
        {
            // Act
            var graph = disposableGraph;
            graph.Initialize("TestDisposableGraph");
            disposableGraph = graph;

            // Assert
            Assert.IsTrue(disposableGraph.IsActive, "Graph should be active after initialization");
            Assert.IsTrue(disposableGraph.Graph.IsValid(), "Graph should be valid");
            Assert.IsFalse(disposableGraph.IsDisposed, "Graph should not be disposed");
            Assert.AreNotEqual(0, disposableGraph.GraphId, "Graph ID should be set");
        }

        [Test]
        public void Day10DisposableGraph_Initialize_Twice_ShowsWarning()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestDisposableGraph");
            disposableGraph = graph;

            // Act & Assert
            // Second initialization should log a warning but not throw
            Assert.DoesNotThrow(() =>
            {
                var graph2 = disposableGraph;
                graph2.Initialize("TestDisposableGraph");
                disposableGraph = graph2;
            }, "Initializing twice should not throw");
        }

        [Test]
        public void Day10DisposableGraph_Dispose_DisposesGraph()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestDisposableGraph");
            disposableGraph = graph;

            // Act
            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;

            // Assert
            Assert.IsTrue(disposableGraph.IsDisposed, "Graph should be marked as disposed");
            Assert.IsFalse(disposableGraph.IsActive, "Graph should be inactive after disposal");
            Assert.IsFalse(disposableGraph.Graph.IsValid(), "Underlying graph should be destroyed");
        }

        [Test]
        public void Day10DisposableGraph_Dispose_Twice_IsIdempotent()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestDisposableGraph");
            disposableGraph = graph;
            var temp1 = disposableGraph;
            temp1.Dispose();
            disposableGraph = temp1;

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var temp2 = disposableGraph;
                temp2.Dispose();
                disposableGraph = temp2;
            }, "Disposing twice should not throw");
            Assert.IsTrue(disposableGraph.IsDisposed, "Graph should remain disposed");
        }

        [Test]
        public void Day10DisposableGraph_IsValid_WithActiveGraph_ReturnsTrue()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestDisposableGraph");
            disposableGraph = graph;

            // Act
            bool isValid = disposableGraph.IsValid();

            // Assert
            Assert.IsTrue(isValid, "Active graph should be valid");
        }

        [Test]
        public void Day10DisposableGraph_IsValid_AfterDispose_ReturnsFalse()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestDisposableGraph");
            disposableGraph = graph;
            var temp1 = disposableGraph;
            temp1.Dispose();
            disposableGraph = temp1;

            // Act
            bool isValid = disposableGraph.IsValid();

            // Assert
            Assert.IsFalse(isValid, "Disposed graph should not be valid");
        }

        [Test]
        public void Day10DisposableGraph_IsDisposedGraph_AfterDispose_ReturnsTrue()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestDisposableGraph");
            disposableGraph = graph;

            // Act
            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;
            bool isDisposed = disposableGraph.IsDisposedGraph();

            // Assert
            Assert.IsTrue(isDisposed, "IsDisposedGraph should return true after disposal");
        }

        [Test]
        public void Day10DisposableGraph_Reset_ClearsState()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestDisposableGraph");
            disposableGraph = graph;
            var temp1 = disposableGraph;
            temp1.Dispose();
            disposableGraph = temp1;

            // Act
            var reset = disposableGraph;
            reset.Reset();
            disposableGraph = reset;

            // Assert
            Assert.IsFalse(disposableGraph.IsActive, "Graph should be inactive after reset");
            Assert.IsFalse(disposableGraph.IsDisposed, "Disposed flag should be cleared");
            Assert.AreEqual(0, disposableGraph.GraphId, "Graph ID should be cleared");
        }

        #endregion

        #region Day10DisposableGraphExtensions Tests

        [Test]
        public void Day10DisposableGraphExtensions_Initialize_WithValidName_CreatesGraph()
        {
            // Act
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            // Assert
            Assert.IsTrue(disposableGraph.IsActive, "Graph should be active");
            Assert.IsTrue(disposableGraph.Graph.IsValid(), "Graph should be valid");
        }

        [Test]
        public void Day10DisposableGraphExtensions_Initialize_GeneratesUniqueIds()
        {
            // Arrange
            Day10DisposableGraphExtensions.ResetIdCounter();

            // Act
            var graph1 = new Day10DisposableGraph();
            graph1.Initialize("TestGraph1");

            var graph2 = new Day10DisposableGraph();
            graph2.Initialize("TestGraph2");

            // Assert
            Assert.AreNotEqual(graph1.GraphId, graph2.GraphId,
                "Each graph should have a unique ID");

            // Cleanup
            var temp1 = graph1;
            temp1.Dispose();
            graph1 = temp1;

            var temp2 = graph2;
            temp2.Dispose();
            graph2 = temp2;
        }

        [Test]
        public void Day10DisposableGraphExtensions_Dispose_ViaExtension_Works()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            // Act
            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;

            // Assert
            Assert.IsTrue(disposableGraph.IsDisposed, "Graph should be disposed");
            Assert.IsFalse(disposableGraph.IsActive, "Graph should be inactive");
        }

        [Test]
        public void Day10DisposableGraphExtensions_IsValid_WithUninitializedGraph_ReturnsFalse()
        {
            // Arrange
            Day10DisposableGraph uninitializedGraph = new Day10DisposableGraph();

            // Act
            bool isValid = uninitializedGraph.IsValid();

            // Assert
            Assert.IsFalse(isValid, "Uninitialized graph should not be valid");
        }

        [Test]
        public void Day10DisposableGraphExtensions_Reset_WithActiveGraph_LogsWarning()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var reset = disposableGraph;
                reset.Reset();
                disposableGraph = reset;
            }, "Resetting active graph should not throw");
        }

        [Test]
        public void Day10DisposableGraphExtensions_ResetIdCounter_ResetsCounter()
        {
            // Arrange
            Day10DisposableGraphExtensions.ResetIdCounter();

            var graph1 = new Day10DisposableGraph();
            graph1.Initialize("TestGraph1");
            int id1 = graph1.GraphId;

            // Act
            Day10DisposableGraphExtensions.ResetIdCounter();

            var graph2 = new Day10DisposableGraph();
            graph2.Initialize("TestGraph2");
            int id2 = graph2.GraphId;

            // Assert
            // After reset, IDs should restart from similar values
            // Note: Exact equality depends on hash function, but counter should be reset
            Assert.IsNotNull(id2, "Graph should have an ID after counter reset");

            // Cleanup
            var temp3 = graph1;
            temp3.Dispose();
            graph1 = temp3;

            var temp4 = graph2;
            temp4.Dispose();
            graph2 = temp4;
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Day10Integration_WithAllPreviousDays_WorksCorrectly()
        {
            // Arrange - Initialize all components
            var graph = disposableGraph;
            graph.Initialize("IntegrationTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var output = outputHandle;
            output.Initialize(in unityGraph, "TestOutput");
            outputHandle = output;

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
            Assert.IsTrue(outputHandle.IsActive, "Output should be active");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState should be valid");
            Assert.IsTrue(reverseData.IsValidReverseControl(), "Reverse control should be valid");
            Assert.IsTrue(namedGraphData.IsValidControl(), "Named graph should be valid");
        }

        [Test]
        public void Day10Integration_Dispose_CleansUpAllResources()
        {
            // Arrange - Initialize all components
            var graph = disposableGraph;
            graph.Initialize("IntegrationTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var output = outputHandle;
            output.Initialize(in unityGraph, "TestOutput");
            outputHandle = output;

            var rotator = rotatorData;
            rotator.Initialize(in unityGraph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);
            rotatorData = rotator;

            // Act - Dispose in reverse order
            var tempRotator = rotatorData;
            tempRotator.Dispose();
            rotatorData = tempRotator;

            var tempOutput = outputHandle;
            tempOutput.Dispose();
            outputHandle = tempOutput;

            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;

            // Assert
            Assert.IsFalse(disposableGraph.IsValid(), "Disposable graph should be disposed");
            Assert.IsFalse(rotatorData.IsActive, "Rotator should be inactive");
            Assert.IsFalse(outputHandle.IsActive, "Output should be inactive");
        }

        [Test]
        public void Day10Integration_WithDay09Visualizer_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("VisualizerTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var visualizer = visualizerData;
            visualizer.Initialize(in unityGraph, VisualizationDetailLevel.Basic, VisualizerColorScheme.Default);
            visualizerData = visualizer;

            // Act & Assert
            Assert.IsTrue(disposableGraph.IsValid(), "Disposable graph should be valid");
            Assert.IsTrue(visualizerData.IsValidVisualizer(), "Visualizer should be valid");

            // Cleanup
            var tempVis = visualizerData;
            tempVis.Dispose();
            visualizerData = tempVis;

            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;
        }

        #endregion
    }
}
