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

namespace PlayableLearn.Day11.Tests
{
    public class Day11Tests
    {
        private Day10DisposableGraph disposableGraph;
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

        #region AnimOutputOps Tests

        [Test]
        public void AnimOutputOps_Create_WithValidGraph_CreatesOutput()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);

            // Act
            AnimOutputOps.Create(in graph, "TestAnimOutput", out PlayableOutput output);

            // Assert
            Assert.IsTrue(output.IsOutputValid(), "Output should be valid after creation");

            // Cleanup
            AnimOutputOps.Destroy(in graph, in output);
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void AnimOutputOps_Create_WithInvalidGraph_ReturnsNullOutput()
        {
            // Arrange
            PlayableGraph invalidGraph = default;

            // Act
            AnimOutputOps.Create(in invalidGraph, "TestAnimOutput", out PlayableOutput output);

            // Assert
            Assert.IsFalse(output.IsOutputValid(), "Output should be invalid when graph is invalid");
        }

        [Test]
        public void AnimOutputOps_Destroy_WithValidOutput_DestroysOutput()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            AnimOutputOps.Create(in graph, "TestAnimOutput", out PlayableOutput output);
            Assert.IsTrue(output.IsOutputValid(), "Output should be valid before destruction");

            // Act
            AnimOutputOps.Destroy(in graph, in output);

            // Assert
            Assert.IsFalse(output.IsOutputValid(), "Output should be invalid after destruction");

            // Cleanup
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void AnimOutputOps_IsValid_WithValidOutput_ReturnsTrue()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            AnimOutputOps.Create(in graph, "TestAnimOutput", out PlayableOutput output);

            // Act
            bool isValid = AnimOutputOps.IsValid(in output);

            // Assert
            Assert.IsTrue(isValid, "Valid output should return true");

            // Cleanup
            AnimOutputOps.Destroy(in graph, in output);
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void AnimOutputOps_IsValid_WithInvalidOutput_ReturnsFalse()
        {
            // Arrange
            PlayableOutput invalidOutput = default;

            // Act
            bool isValid = AnimOutputOps.IsValid(in invalidOutput);

            // Assert
            Assert.IsFalse(isValid, "Invalid output should return false");
        }

        [Test]
        public void AnimOutputOps_GetOutputType_WithValidOutput_ReturnsAnimationType()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            AnimOutputOps.Create(in graph, "TestAnimOutput", out PlayableOutput output);

            // Act
            AnimOutputOps.GetOutputType(in output, out AnimOutputType outputType);

            // Assert
            Assert.AreEqual(AnimOutputType.AnimationOutput, outputType, "Output type should be AnimationOutput");

            // Cleanup
            AnimOutputOps.Destroy(in graph, in output);
            DisposalOps.DisposeGraph(in graph);
        }

        [Test]
        public void AnimOutputOps_GetOutputInfo_WithValidOutput_ReturnsCorrectInfo()
        {
            // Arrange
            GraphOps.Create("TestGraph", out PlayableGraph graph);
            AnimOutputOps.Create(in graph, "TestAnimOutput", out PlayableOutput output);

            // Act
            AnimOutputOps.GetOutputInfo(in output, out AnimOutputType type, out bool isValid);

            // Assert
            Assert.IsTrue(isValid, "Output should be valid");
            Assert.AreEqual(AnimOutputType.AnimationOutput, type, "Type should be AnimationOutput");

            // Cleanup
            AnimOutputOps.Destroy(in graph, in output);
            DisposalOps.DisposeGraph(in graph);
        }

        #endregion

        #region Day11AnimOutputHandle Tests

        [Test]
        public void Day11AnimOutputHandle_Initialize_WithValidAnimator_CreatesOutput()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            // Act
            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            // Assert
            Assert.IsTrue(animOutputHandle.IsActive, "Output should be active after initialization");
            Assert.IsNotNull(animOutputHandle.Animator, "Animator should be set");
            Assert.AreEqual(testAnimator, animOutputHandle.Animator, "Animator should match the provided one");
            Assert.AreNotEqual(0, animOutputHandle.OutputId, "Output ID should be set");
        }

        [Test]
        public void Day11AnimOutputHandle_Initialize_Twice_ShowsWarning()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var animOutput2 = animOutputHandle;
                animOutput2.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
                animOutputHandle = animOutput2;
            }, "Initializing twice should not throw");
        }

        [Test]
        public void Day11AnimOutputHandle_Initialize_WithNullAnimator_DoesNotCreateOutput()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            // Act
            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", null);
            animOutputHandle = animOutput;

            // Assert
            Assert.IsFalse(animOutputHandle.IsActive, "Output should not be active with null Animator");
        }

        [Test]
        public void Day11AnimOutputHandle_Dispose_DisposesOutput()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            // Act
            animOutputHandle.Dispose();

            // Assert
            Assert.IsFalse(animOutputHandle.IsActive, "Output should be inactive after disposal");
            Assert.IsNull(animOutputHandle.Animator, "Animator should be cleared");
            Assert.AreEqual(0, animOutputHandle.OutputId, "Output ID should be cleared");
        }

        [Test]
        public void Day11AnimOutputHandle_Dispose_Twice_IsIdempotent()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;
            animOutputHandle.Dispose();

            // Act & Assert
            Assert.DoesNotThrow(() => animOutputHandle.Dispose(),
                "Disposing twice should not throw");
        }

        #endregion

        #region Day11AnimOutputExtensions Tests

        [Test]
        public void Day11AnimOutputExtensions_Initialize_WithValidData_CreatesValidOutput()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            // Act
            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            // Assert
            Assert.IsTrue(animOutputHandle.IsValidOutput(), "Output should be valid");
            Assert.IsTrue(animOutputHandle.IsActive, "Output should be active");
        }

        [Test]
        public void Day11AnimOutputExtensions_TryGetOutputType_WithValidOutput_ReturnsAnimationType()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            // Act
            bool success = animOutputHandle.TryGetOutputType(out AnimOutputType outputType);

            // Assert
            Assert.IsTrue(success, "TryGetOutputType should succeed");
            Assert.AreEqual(AnimOutputType.AnimationOutput, outputType, "Type should be AnimationOutput");
        }

        [Test]
        public void Day11AnimOutputExtensions_TryGetOutputType_WithInvalidOutput_ReturnsFalse()
        {
            // Arrange
            Day11AnimOutputHandle invalidHandle = new Day11AnimOutputHandle();

            // Act
            bool success = invalidHandle.TryGetOutputType(out AnimOutputType outputType);

            // Assert
            Assert.IsFalse(success, "TryGetOutputType should fail for invalid handle");
            Assert.AreEqual(AnimOutputType.Null, outputType, "Type should be Null");
        }

        [Test]
        public void Day11AnimOutputExtensions_IsValidOutput_WithValidOutput_ReturnsTrue()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            // Act
            bool isValid = animOutputHandle.IsValidOutput();

            // Assert
            Assert.IsTrue(isValid, "Valid output should return true");
        }

        [Test]
        public void Day11AnimOutputExtensions_IsValidOutput_WithInvalidOutput_ReturnsFalse()
        {
            // Arrange
            Day11AnimOutputHandle invalidHandle = new Day11AnimOutputHandle();

            // Act
            bool isValid = invalidHandle.IsValidOutput();

            // Assert
            Assert.IsFalse(isValid, "Invalid output should return false");
        }

        [Test]
        public void Day11AnimOutputExtensions_TryGetAnimator_WithValidOutput_ReturnsAnimator()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            // Act
            bool success = animOutputHandle.TryGetAnimator(out Animator animator);

            // Assert
            Assert.IsTrue(success, "TryGetAnimator should succeed");
            Assert.AreEqual(testAnimator, animator, "Animator should match");
        }

        [Test]
        public void Day11AnimOutputExtensions_TryGetAnimator_WithInvalidOutput_ReturnsFalse()
        {
            // Arrange
            Day11AnimOutputHandle invalidHandle = new Day11AnimOutputHandle();

            // Act
            bool success = invalidHandle.TryGetAnimator(out Animator animator);

            // Assert
            Assert.IsFalse(success, "TryGetAnimator should fail for invalid handle");
            Assert.IsNull(animator, "Animator should be null");
        }

        [Test]
        public void Day11AnimOutputExtensions_TryGetPlayableOutput_WithValidOutput_ReturnsOutput()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("TestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            // Act
            bool success = animOutputHandle.TryGetPlayableOutput(out PlayableOutput output);

            // Assert
            Assert.IsTrue(success, "TryGetPlayableOutput should succeed");
            Assert.IsTrue(output.IsOutputValid(), "Returned output should be valid");
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Day11Integration_WithAllPreviousDays_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("IntegrationTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

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
            Assert.IsTrue(animOutputHandle.IsValidOutput(), "Animation output should be valid");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState should be valid");
            Assert.IsTrue(reverseData.IsValidReverseControl(), "Reverse control should be valid");
            Assert.IsTrue(namedGraphData.IsValidControl(), "Named graph should be valid");
        }

        [Test]
        public void Day11Integration_AnimOutputVsScriptOutput_BothWork()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("ComparisonTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            // Test AnimationPlayableOutput
            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            // Test ScriptPlayableOutput
            var scriptOutput = scriptOutputHandle;
            scriptOutput.Initialize(in unityGraph, "TestScriptOutput");
            scriptOutputHandle = scriptOutput;

            // Act & Assert
            Assert.IsTrue(animOutputHandle.IsValidOutput(), "Animation output should be valid");
            Assert.IsTrue(scriptOutputHandle.IsValidOutput(), "Script output should be valid");
            Assert.IsTrue(animOutputHandle.TryGetAnimator(out Animator animator), "Animation output should have Animator");
            Assert.AreEqual(testAnimator, animator, "Animator should match");
        }

        [Test]
        public void Day11Integration_Dispose_CleansUpAllResources()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("IntegrationTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            var rotator = rotatorData;
            rotator.Initialize(in unityGraph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);
            rotatorData = rotator;

            // Act - Dispose in reverse order
            rotatorData.Dispose();
            animOutputHandle.Dispose();

            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;

            // Assert
            Assert.IsFalse(disposableGraph.IsValid(), "Disposable graph should be disposed");
            Assert.IsFalse(rotatorData.IsActive, "Rotator should be inactive");
            Assert.IsFalse(animOutputHandle.IsActive, "Animation output should be inactive");
        }

        [Test]
        public void Day11Integration_WithDay09Visualizer_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("VisualizerTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            var visualizer = visualizerData;
            visualizer.Initialize(in unityGraph, VisualizationDetailLevel.Basic, VisualizerColorScheme.Default);
            visualizerData = visualizer;

            // Act & Assert
            Assert.IsTrue(disposableGraph.IsValid(), "Disposable graph should be valid");
            Assert.IsTrue(animOutputHandle.IsValidOutput(), "Animation output should be valid");
            Assert.IsTrue(visualizerData.IsValidVisualizer(), "Visualizer should be valid");

            // Cleanup
            visualizerData.Dispose();
            animOutputHandle.Dispose();
            var temp = disposableGraph;
            temp.Dispose();
            disposableGraph = temp;
        }

        [Test]
        public void Day11Integration_ConnectPlayableToOutput_WorksCorrectly()
        {
            // Arrange
            var graph = disposableGraph;
            graph.Initialize("ConnectionTestGraph");
            disposableGraph = graph;

            PlayableGraph unityGraph = disposableGraph.Graph;

            var animOutput = animOutputHandle;
            animOutput.Initialize(in unityGraph, "TestAnimOutput", testAnimator);
            animOutputHandle = animOutput;

            var rotator = rotatorData;
            rotator.Initialize(in unityGraph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);
            rotatorData = rotator;

            // Act
            bool canGetOutput = animOutputHandle.TryGetPlayableOutput(out PlayableOutput output);

            // Assert
            Assert.IsTrue(canGetOutput, "Should be able to get PlayableOutput");
            Assert.IsTrue(output.IsOutputValid(), "Output should be valid");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should remain valid");
        }

        #endregion
    }
}
