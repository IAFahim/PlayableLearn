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

namespace PlayableLearn.Day08.Tests
{
    public class Day08Tests
    {
        private Day01GraphHandle graphHandle;
        private Day02OutputHandle outputHandle;
        private Day04RotatorData rotatorData;
        private Day05SpeedData speedData;
        private Day06PlayStateData playStateData;
        private Day07ReverseData reverseData;
        private Day08NamedGraphData namedGraphData;
        private GameObject testGameObject;

        [SetUp]
        public void SetUp()
        {
            // Reset controller ID counter for consistent testing
            Day08NamedGraphExtensions.ResetControllerIdCounter();

            graphHandle = new Day01GraphHandle();
            outputHandle = new Day02OutputHandle();
            rotatorData = new Day04RotatorData();
            speedData = new Day05SpeedData();
            playStateData = new Day06PlayStateData();
            reverseData = new Day07ReverseData();
            namedGraphData = new Day08NamedGraphData();
            testGameObject = new GameObject("TestCube");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up in reverse order
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

            if (outputHandle.IsActive)
            {
                outputHandle.Dispose();
            }

            if (graphHandle.IsActive)
            {
                graphHandle.Dispose();
            }

            if (testGameObject != null)
            {
                Object.DestroyImmediate(testGameObject);
            }
        }

        #region GraphNamingOps Tests

        [Test]
        public void GraphNamingOps_IsValidName_WithValidName_ReturnsTrue()
        {
            // Arrange
            string validName = "MyPlayableGraph";

            // Act
            bool result = GraphNamingOps.IsValidName(validName);

            // Assert
            Assert.IsTrue(result, "Valid name should return true");
        }

        [Test]
        public void GraphNamingOps_IsValidName_WithNullName_ReturnsFalse()
        {
            // Arrange
            string nullName = null;

            // Act
            bool result = GraphNamingOps.IsValidName(nullName);

            // Assert
            Assert.IsFalse(result, "Null name should return false");
        }

        [Test]
        public void GraphNamingOps_IsValidName_WithEmptyName_ReturnsFalse()
        {
            // Arrange
            string emptyName = "";

            // Act
            bool result = GraphNamingOps.IsValidName(emptyName);

            // Assert
            Assert.IsFalse(result, "Empty name should return false");
        }

        [Test]
        public void GraphNamingOps_AreNamesEqual_WithSameNames_ReturnsTrue()
        {
            // Arrange
            string name1 = "MyGraph";
            string name2 = "MyGraph";

            // Act
            bool result = GraphNamingOps.AreNamesEqual(name1, name2);

            // Assert
            Assert.IsTrue(result, "Same names should be equal");
        }

        [Test]
        public void GraphNamingOps_AreNamesEqual_WithDifferentCase_ReturnsTrue()
        {
            // Arrange
            string name1 = "MyGraph";
            string name2 = "mygraph";

            // Act
            bool result = GraphNamingOps.AreNamesEqual(name1, name2);

            // Assert
            Assert.IsTrue(result, "Names with different case should be equal (case-insensitive)");
        }

        [Test]
        public void GraphNamingOps_AreNamesEqual_WithDifferentNames_ReturnsFalse()
        {
            // Arrange
            string name1 = "MyGraph";
            string name2 = "OtherGraph";

            // Act
            bool result = GraphNamingOps.AreNamesEqual(name1, name2);

            // Assert
            Assert.IsFalse(result, "Different names should not be equal");
        }

        [Test]
        public void GraphNamingOps_HasNameChanged_WithDifferentNames_ReturnsTrue()
        {
            // Arrange
            string currentName = "NewName";
            string previousName = "OldName";

            // Act
            bool result = GraphNamingOps.HasNameChanged(currentName, previousName);

            // Assert
            Assert.IsTrue(result, "Different names should indicate name has changed");
        }

        [Test]
        public void GraphNamingOps_GenerateUniqueName_WithCounter_GeneratesCorrectName()
        {
            // Arrange
            string baseName = "MyGraph";
            int counter = 5;

            // Act
            string result = GraphNamingOps.GenerateUniqueName(baseName, counter);

            // Assert
            Assert.AreEqual("MyGraph_5", result, "Unique name should be baseName_counter");
        }

        [Test]
        public void GraphNamingOps_SanitizeName_WithInvalidChars_RemovesInvalidChars()
        {
            // Arrange
            string invalidName = "My<Graph>:Name";

            // Act
            string result = GraphNamingOps.SanitizeName(invalidName);

            // Assert
            Assert.AreEqual("My_Graph__Name", result, "Invalid characters should be replaced with underscores");
        }

        [Test]
        public void GraphNamingOps_SanitizeName_WithWhitespace_TrimsWhitespace()
        {
            // Arrange
            string whitespaceName = "  MyGraph  ";

            // Act
            string result = GraphNamingOps.SanitizeName(whitespaceName);

            // Assert
            Assert.AreEqual("MyGraph", result, "Whitespace should be trimmed");
        }

        [Test]
        public void GraphNamingOps_TruncateName_WithLongName_TruncatesToMaxLength()
        {
            // Arrange
            string longName = "ThisIsAVeryLongGraphNameThatExceedsTheMaximumLength";
            int maxLength = 20;

            // Act
            string result = GraphNamingOps.TruncateName(longName, maxLength);

            // Assert
            Assert.AreEqual(20, result.Length, "Name should be truncated to maximum length");
            Assert.AreEqual("ThisIsAVeryLongGra", result, "Name should be correctly truncated");
        }

        [Test]
        public void GraphNamingOps_FormatName_WithPrefixAndSuffix_FormatsCorrectly()
        {
            // Arrange
            string prefix = "Prefix";
            string baseName = "MyGraph";
            string suffix = "Suffix";

            // Act
            string result = GraphNamingOps.FormatName(prefix, baseName, suffix);

            // Assert
            Assert.AreEqual("Prefix_MyGraph_Suffix", result, "Name should be formatted with prefix and suffix");
        }

        [Test]
        public void GraphNamingOps_IncrementNameCount_IncrementsCount()
        {
            // Arrange
            int currentCount = 5;

            // Act
            int result = GraphNamingOps.IncrementNameCount(currentCount);

            // Assert
            Assert.AreEqual(6, result, "Name count should be incremented");
        }

        #endregion

        #region Day08NamedGraphExtensions Tests

        [Test]
        public void NamedGraphData_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act
            namedGraphData.Initialize(in graphHandle.Graph, "TestNamedGraph");

            // Assert
            Assert.IsTrue(namedGraphData.IsActive, "Named graph should be active after initialization");
            Assert.IsTrue(namedGraphData.IsValidControl(), "Named graph should be valid");
            Assert.IsTrue(namedGraphData.IsNameSet(), "Graph name should be set");
        }

        [Test]
        public void NamedGraphData_WithInvalidGraph_DoesNotInitialize()
        {
            // Arrange
            PlayableGraph invalidGraph = default;

            // Act
            namedGraphData.Initialize(in invalidGraph, "TestNamedGraph");

            // Assert
            Assert.IsFalse(namedGraphData.IsActive, "Named graph should not be active with invalid graph");
            Assert.IsFalse(namedGraphData.IsValidControl(), "Named graph should not be valid");
        }

        [Test]
        public void NamedGraphData_SetName_SetsNameCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "OriginalName");

            // Act
            namedGraphData.SetName("NewName");

            // Assert
            Assert.AreEqual("NewName", namedGraphData.GetName(), "Graph name should be updated to new name");
            Assert.AreEqual("OriginalName", namedGraphData.GetPreviousName(), "Previous name should be stored");
            Assert.AreEqual(2, namedGraphData.GetNameChangeCount(), "Name change count should be incremented");
        }

        [Test]
        public void NamedGraphData_GetName_ReturnsCorrectName()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "TestName");

            // Act
            string name = namedGraphData.GetName();

            // Assert
            Assert.AreEqual("TestName", name, "GetName should return the correct graph name");
        }

        [Test]
        public void NamedGraphData_HasNameChanged_WhenNameChanged_ReturnsTrue()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "OriginalName");
            namedGraphData.SetName("NewName");

            // Act
            bool hasChanged = namedGraphData.HasNameChanged();

            // Assert
            Assert.IsTrue(hasChanged, "HasNameChanged should return true when name has changed");
        }

        [Test]
        public void NamedGraphData_SetFormattedName_FormatsNameCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "OriginalName");

            // Act
            namedGraphData.SetFormattedName("Prefix", "Base", "Suffix");

            // Assert
            Assert.AreEqual("Prefix_Base_Suffix", namedGraphData.GetName(), "Name should be formatted correctly");
        }

        [Test]
        public void NamedGraphData_SetUniqueName_SetsUniqueNameWithCounter()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "OriginalName");

            // Act
            namedGraphData.SetUniqueName("MyGraph", 42);

            // Assert
            Assert.AreEqual("MyGraph_42", namedGraphData.GetName(), "Unique name should include counter");
        }

        [Test]
        public void NamedGraphData_Dispose_DisposesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "TestName");

            // Act
            namedGraphData.Dispose();

            // Assert
            Assert.IsFalse(namedGraphData.IsActive, "Named graph should not be active after disposal");
            Assert.IsFalse(namedGraphData.IsValidControl(), "Named graph should not be valid after disposal");
        }

        [Test]
        public void NamedGraphData_GetControllerId_ReturnsCorrectId()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "TestName");

            // Act
            int controllerId = namedGraphData.GetControllerId();

            // Assert
            Assert.AreEqual(0, controllerId, "First named graph should have controller ID 0");
        }

        [Test]
        public void NamedGraphData_MultipleNamedGraphs_HaveUniqueControllerIds()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "FirstGraph");

            // Act
            Day08NamedGraphData secondNamedGraph = new Day08NamedGraphData();
            secondNamedGraph.Initialize(in graphHandle.Graph, "SecondGraph");

            // Assert
            Assert.AreNotEqual(namedGraphData.GetControllerId(), secondNamedGraph.GetControllerId(),
                "Multiple named graphs should have unique controller IDs");
        }

        [Test]
        public void NamedGraphData_WithSanitization_SanitizesInvalidName()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            string invalidName = "My<Invalid>Name";

            // Act
            namedGraphData.Initialize(in graphHandle.Graph, invalidName);

            // Assert
            Assert.IsTrue(namedGraphData.GetName().IndexOf('<') == -1, "Invalid character '<' should be removed");
            Assert.IsTrue(namedGraphData.GetName().IndexOf('>') == -1, "Invalid character '>' should be removed");
        }

        [Test]
        public void NamedGraphData_WithLongName_TruncatesName()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            string longName = new string('A', 200); // 200 characters

            // Act
            namedGraphData.Initialize(in graphHandle.Graph, longName);

            // Assert
            Assert.LessOrEqual(namedGraphData.GetName().Length, 128, "Name should be truncated to 128 characters");
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Day08Integration_WithAllPreviousDays_WorksCorrectly()
        {
            // Arrange & Act - Initialize all days
            graphHandle.Initialize("IntegrationTestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "IntegrationNamedGraph");
            outputHandle.Initialize(in graphHandle.Graph, "IntegrationOutput");
            rotatorData.Initialize(in graphHandle.Graph, "IntegrationRotator", testGameObject.transform, 90.0f, Vector3.up);
            speedData.Initialize(in graphHandle.Graph, "IntegrationSpeedControl", 1.0f, true, 2.0f);
            playStateData.Initialize(in graphHandle.Graph, "IntegrationPlayStateController", true);
            reverseData.Initialize(in graphHandle.Graph, "IntegrationReverseControl", true, true);

            // Assert - Verify all components are valid
            Assert.IsTrue(graphHandle.IsActive, "Graph should be active");
            Assert.IsTrue(namedGraphData.IsValidControl(), "Named graph should be valid");
            Assert.IsTrue(outputHandle.IsActive, "Output should be active");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState should be valid");
            Assert.IsTrue(reverseData.IsValidReverseControl(), "Reverse control should be valid");
        }

        [Test]
        public void Day08Integration_WithDynamicRenaming_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("DynamicRenameTest");
            namedGraphData.Initialize(in graphHandle.Graph, "InitialName");
            playStateData.Initialize(in graphHandle.Graph, "PlayStateController", true);

            // Act - Rename based on state
            string stateName = playStateData.IsPlaying() ? "Playing" : "Paused";
            string dynamicName = $"MyGraph_{stateName}";
            namedGraphData.SetName(dynamicName);

            // Assert
            Assert.IsTrue(namedGraphData.GetName().Contains("Playing") || namedGraphData.GetName().Contains("Paused"),
                "Dynamic name should contain state information");
        }

        [Test]
        public void Day08Integration_Cleanup_WorksInCorrectOrder()
        {
            // Arrange - Initialize all components
            graphHandle.Initialize("CleanupTestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "NamedGraph");
            outputHandle.Initialize(in graphHandle.Graph, "Output");
            rotatorData.Initialize(in graphHandle.Graph, "Rotator", testGameObject.transform, 90.0f, Vector3.up);
            speedData.Initialize(in graphHandle.Graph, "SpeedControl", 1.0f, true, 2.0f);
            playStateData.Initialize(in graphHandle.Graph, "PlayStateController", true);
            reverseData.Initialize(in graphHandle.Graph, "ReverseControl", true, true);

            // Act - Cleanup in reverse order
            reverseData.Dispose();
            playStateData.Dispose();
            speedData.Dispose();
            rotatorData.Dispose();
            outputHandle.Dispose();
            namedGraphData.Dispose();
            graphHandle.Dispose();

            // Assert - Verify all components are cleaned up
            Assert.IsFalse(reverseData.IsActive, "Reverse control should be cleaned up");
            Assert.IsFalse(playStateData.IsActive, "PlayState should be cleaned up");
            Assert.IsFalse(speedData.IsActive, "Speed control should be cleaned up");
            Assert.IsFalse(rotatorData.IsActive, "Rotator should be cleaned up");
            Assert.IsFalse(outputHandle.IsActive, "Output should be cleaned up");
            Assert.IsFalse(namedGraphData.IsActive, "Named graph should be cleaned up");
            Assert.IsFalse(graphHandle.IsActive, "Graph should be cleaned up");
        }

        [Test]
        public void Day08Integration_WithNameChangeTracking_TracksChangesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TrackingTest");
            namedGraphData.Initialize(in graphHandle.Graph, "Name1");

            // Act - Change name multiple times
            namedGraphData.SetName("Name2");
            namedGraphData.SetName("Name3");
            namedGraphData.SetName("Name4");

            // Assert
            Assert.AreEqual(4, namedGraphData.GetNameChangeCount(),
                "Name change count should reflect total number of changes including initialization");
            Assert.AreEqual("Name3", namedGraphData.GetPreviousName(),
                "Previous name should be the second-to-last name set");
        }

        #endregion
    }
}
