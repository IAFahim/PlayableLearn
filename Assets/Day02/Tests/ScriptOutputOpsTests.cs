using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using PlayableLearn.Day02;

namespace PlayableLearn.Day02.Tests
{
    public class ScriptOutputOpsTests
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
        public void ScriptOutputOps_Create_ReturnsValidOutput()
        {
            // Arrange
            string outputName = "TestOutput";

            // Act
            ScriptOutputOps.Create(in graph, in outputName, out PlayableOutput output);

            // Assert
            Assert.IsTrue(output.IsValid(), "Output should be valid after creation.");
            Assert.IsFalse(output.IsNull(), "Output should not be null after creation.");

            // Cleanup
            ScriptOutputOps.Destroy(in output);
        }

        [Test]
        public void ScriptOutputOps_Create_WithInvalidGraph_ReturnsNullOutput()
        {
            // Arrange
            PlayableGraph invalidGraph = default;
            string outputName = "TestOutput";

            // Act
            ScriptOutputOps.Create(in invalidGraph, in outputName, out PlayableOutput output);

            // Assert
            Assert.IsTrue(output.IsNull(), "Output should be null when graph is invalid.");
        }

        [Test]
        public void ScriptOutputOps_Destroy_RemovesOutput()
        {
            // Arrange
            string outputName = "TestOutput";
            ScriptOutputOps.Create(in graph, in outputName, out PlayableOutput output);

            // Act
            ScriptOutputOps.Destroy(in output);

            // Assert
            Assert.IsFalse(output.IsValid(), "Output should not be valid after destruction.");
        }

        [Test]
        public void ScriptOutputOps_Destroy_WithInvalidOutput_DoesNotThrow()
        {
            // Arrange
            PlayableOutput invalidOutput = default;

            // Act & Assert
            Assert.DoesNotThrow(() => ScriptOutputOps.Destroy(in invalidOutput),
                "Destroying an invalid output should not throw an exception.");
        }

        [Test]
        public void ScriptOutputOps_IsValid_ReturnsTrueForValidOutput()
        {
            // Arrange
            string outputName = "TestOutput";
            ScriptOutputOps.Create(in graph, in outputName, out PlayableOutput output);

            // Act
            bool isValid = ScriptOutputOps.IsValid(in output);

            // Assert
            Assert.IsTrue(isValid, "Output should be valid.");

            // Cleanup
            ScriptOutputOps.Destroy(in output);
        }

        [Test]
        public void ScriptOutputOps_IsValid_ReturnsFalseForInvalidOutput()
        {
            // Arrange
            PlayableOutput invalidOutput = default;

            // Act
            bool isValid = ScriptOutputOps.IsValid(in invalidOutput);

            // Assert
            Assert.IsFalse(isValid, "Invalid output should not be valid.");
        }

        [Test]
        public void ScriptOutputOps_GetOutputType_ReturnsScriptOutputType()
        {
            // Arrange
            string outputName = "TestOutput";
            ScriptOutputOps.Create(in graph, in outputName, out PlayableOutput output);

            // Act
            string outputType = ScriptOutputOps.GetOutputType(in output);

            // Assert
            Assert.IsNotNull(outputType, "Output type should not be null.");
            Assert.IsNotEmpty(outputType, "Output type should not be empty.");

            // Cleanup
            ScriptOutputOps.Destroy(in output);
        }

        [Test]
        public void ScriptOutputOps_GetOutputType_WithInvalidOutput_ReturnsNull()
        {
            // Arrange
            PlayableOutput invalidOutput = default;

            // Act
            string outputType = ScriptOutputOps.GetOutputType(in invalidOutput);

            // Assert
            Assert.AreEqual("Null", outputType, "Invalid output should return 'Null' type.");
        }

        [Test]
        public void ScriptOutputOps_LogToConsole_LogsOutputInfo()
        {
            // Arrange
            string outputName = "TestOutput";
            ScriptOutputOps.Create(in graph, in outputName, out PlayableOutput output);

            // Act & Assert
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"\[ScriptOutput\]"));
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"Name:"));
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"Type:"));
            ScriptOutputOps.LogToConsole(in output, in outputName);

            // Cleanup
            ScriptOutputOps.Destroy(in output);
        }

        [Test]
        public void ScriptOutputOps_LogToConsole_WithInvalidOutput_LogsError()
        {
            // Arrange
            PlayableOutput invalidOutput = default;
            string outputName = "TestOutput";

            // Act & Assert
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(@"not valid"));
            ScriptOutputOps.LogToConsole(in invalidOutput, in outputName);
        }

        [Test]
        public void ScriptOutputOps_CreateMultiple_OutputsAreIndependent()
        {
            // Arrange
            string outputName1 = "TestOutput1";
            string outputName2 = "TestOutput2";

            // Act
            ScriptOutputOps.Create(in graph, in outputName1, out PlayableOutput output1);
            ScriptOutputOps.Create(in graph, in outputName2, out PlayableOutput output2);

            // Assert
            Assert.IsTrue(output1.IsValid(), "First output should be valid.");
            Assert.IsTrue(output2.IsValid(), "Second output should be valid.");
            Assert.AreNotEqual(output1.GetPlayableOutputInternal(0), output2.GetPlayableOutputInternal(0),
                "Outputs should be independent.");

            // Cleanup
            ScriptOutputOps.Destroy(in output1);
            ScriptOutputOps.Destroy(in output2);
        }
    }
}
