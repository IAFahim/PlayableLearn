using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day02;

namespace PlayableLearn.Day02.Tests
{
    public class Day02OutputHandleTests
    {
        private PlayableGraph graph;

        [SetUp]
        public void SetUp()
        {
            graph = PlayableGraph.Create("TestGraph");
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        }

        [TearDown]
        public void TearDown()
        {
            if (graph.IsValid())
            {
                graph.Destroy();
            }
        }

        [Test]
        public void OutputHandle_InitialState_IsNotActive()
        {
            // Arrange & Act
            Day02OutputHandle handle = default;

            // Assert
            Assert.IsFalse(handle.IsActive, "Output handle should not be active initially.");
            Assert.AreEqual(0, handle.OutputId, "Output ID should be 0 initially.");
        }

        [Test]
        public void OutputHandle_Initialize_BecomesActive()
        {
            // Arrange
            Day02OutputHandle handle = default;
            string outputName = "TestOutput";

            // Act
            handle.Initialize(in graph, outputName);

            // Assert
            Assert.IsTrue(handle.IsActive, "Output handle should be active after initialization.");
            Assert.AreNotEqual(0, handle.OutputId, "Output ID should be set after initialization.");
        }

        [Test]
        public void OutputHandle_InitializeTwice_LogsWarning()
        {
            // Arrange
            Day02OutputHandle handle = default;
            string outputName = "TestOutput";
            handle.Initialize(in graph, outputName);

            // Act
            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(@"Already initialized"));
            handle.Initialize(in graph, outputName);

            // Assert - Handled by LogAssert
        }

        [Test]
        public void OutputHandle_Dispose_BecomesInactive()
        {
            // Arrange
            Day02OutputHandle handle = default;
            string outputName = "TestOutput";
            handle.Initialize(in graph, outputName);

            // Act
            handle.Dispose();

            // Assert
            Assert.IsFalse(handle.IsActive, "Output handle should not be active after disposal.");
            Assert.AreEqual(0, handle.OutputId, "Output ID should be 0 after disposal.");
        }

        [Test]
        public void OutputHandle_DisposeWithoutInitialize_DoesNotThrow()
        {
            // Arrange
            Day02OutputHandle handle = default;

            // Act & Assert
            Assert.DoesNotThrow(() => handle.Dispose(), 
                "Disposing an uninitialized handle should not throw an exception.");
        }

        [Test]
        public void OutputHandle_WithInvalidGraph_DoesNotInitialize()
        {
            // Arrange
            Day02OutputHandle handle = default;
            PlayableGraph invalidGraph = default;
            string outputName = "TestOutput";

            // Act
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(@"Graph is invalid"));
            handle.Initialize(in invalidGraph, outputName);

            // Assert
            Assert.IsFalse(handle.IsActive, "Output handle should not be active with invalid graph.");
        }

        [Test]
        public void OutputHandle_IsValidOutput_ReturnsTrueWhenActive()
        {
            // Arrange
            Day02OutputHandle handle = default;
            string outputName = "TestOutput";
            handle.Initialize(in graph, outputName);

            // Act
            bool isValid = handle.IsValidOutput();

            // Assert
            Assert.IsTrue(isValid, "Output handle should be valid when active.");
        }

        [Test]
        public void OutputHandle_IsValidOutput_ReturnsFalseWhenInactive()
        {
            // Arrange
            Day02OutputHandle handle = default;

            // Act
            bool isValid = handle.IsValidOutput();

            // Assert
            Assert.IsFalse(isValid, "Output handle should not be valid when inactive.");
        }

        [Test]
        public void OutputHandle_TryGetOutputType_ReturnsTypeWhenActive()
        {
            // Arrange
            Day02OutputHandle handle = default;
            string outputName = "TestOutput";
            handle.Initialize(in graph, outputName);

            // Act
            bool success = handle.TryGetOutputType(out string outputType);

            // Assert
            Assert.IsTrue(success, "TryGetOutputType should succeed when handle is active.");
            Assert.IsNotNull(outputType, "Output type should not be null.");
            Assert.IsNotEmpty(outputType, "Output type should not be empty.");
        }

        [Test]
        public void OutputHandle_TryGetOutputType_ReturnsInvalidWhenInactive()
        {
            // Arrange
            Day02OutputHandle handle = default;

            // Act
            bool success = handle.TryGetOutputType(out string outputType);

            // Assert
            Assert.IsFalse(success, "TryGetOutputType should fail when handle is inactive.");
            Assert.AreEqual("Invalid", outputType, "Output type should be 'Invalid' when handle is inactive.");
        }

        [Test]
        public void OutputHandle_LogToConsole_LogsWhenActive()
        {
            // Arrange
            Day02OutputHandle handle = default;
            string outputName = "TestOutput";
            handle.Initialize(in graph, outputName);

            // Act & Assert
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[ScriptOutput\]"));
            handle.LogToConsole(outputName);
        }

        [Test]
        public void OutputHandle_LogToConsole_LogsWarningWhenInactive()
        {
            // Arrange
            Day02OutputHandle handle = default;
            string outputName = "TestOutput";

            // Act & Assert
            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(@"not active"));
            handle.LogToConsole(outputName);
        }
    }
}
