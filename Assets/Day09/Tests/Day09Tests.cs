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

namespace PlayableLearn.Day09.Tests
{
    public class Day09Tests
    {
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

        #region VisualizerOps Tests

        [Test]
        public void VisualizerOps_IsValidForVisualization_WithValidGraph_ReturnsTrue()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            PlayableGraph validGraph = graphHandle.Graph;

            // Act
            bool result = VisualizerOps.IsValidForVisualization(in validGraph);

            // Assert
            Assert.IsTrue(result, "Valid graph should return true");
        }

        [Test]
        public void VisualizerOps_IsValidForVisualization_WithInvalidGraph_ReturnsFalse()
        {
            // Arrange
            PlayableGraph invalidGraph = default;

            // Act
            bool result = VisualizerOps.IsValidForVisualization(in invalidGraph);

            // Assert
            Assert.IsFalse(result, "Invalid graph should return false");
        }

        [Test]
        public void VisualizerOps_IsVisualizerAvailable_ReturnsCorrectValue()
        {
            // Act
            bool result = VisualizerOps.IsVisualizerAvailable();

            // Assert
            #if UNITY_EDITOR
            Assert.IsTrue(result, "Visualizer should be available in Unity Editor");
            #else
            Assert.IsFalse(result, "Visualizer should not be available in builds");
            #endif
        }

        [Test]
        public void VisualizerOps_GetDefaultRefreshInterval_WithMinimalDetail_ReturnsOneSecond()
        {
            // Arrange
            VisualizationDetailLevel detailLevel = VisualizationDetailLevel.Minimal;

            // Act
            float interval = VisualizerOps.GetDefaultRefreshInterval(detailLevel);

            // Assert
            Assert.AreEqual(1.0f, interval, 0.001f, "Minimal detail should have 1.0 second interval");
        }

        [Test]
        public void VisualizerOps_GetDefaultRefreshInterval_WithBasicDetail_ReturnsHalfSecond()
        {
            // Arrange
            VisualizationDetailLevel detailLevel = VisualizationDetailLevel.Basic;

            // Act
            float interval = VisualizerOps.GetDefaultRefreshInterval(detailLevel);

            // Assert
            Assert.AreEqual(0.5f, interval, 0.001f, "Basic detail should have 0.5 second interval");
        }

        [Test]
        public void VisualizerOps_GetDefaultRefreshInterval_WithDetailedDetail_ReturnsQuarterSecond()
        {
            // Arrange
            VisualizationDetailLevel detailLevel = VisualizationDetailLevel.Detailed;

            // Act
            float interval = VisualizerOps.GetDefaultRefreshInterval(detailLevel);

            // Assert
            Assert.AreEqual(0.25f, interval, 0.001f, "Detailed detail should have 0.25 second interval");
        }

        [Test]
        public void VisualizerOps_GetDefaultRefreshInterval_WithVerboseDetail_ReturnsTenthSecond()
        {
            // Arrange
            VisualizationDetailLevel detailLevel = VisualizationDetailLevel.Verbose;

            // Act
            float interval = VisualizerOps.GetDefaultRefreshInterval(detailLevel);

            // Assert
            Assert.AreEqual(0.1f, interval, 0.001f, "Verbose detail should have 0.1 second interval");
        }

        [Test]
        public void VisualizerOps_IsValidRefreshInterval_WithValidInterval_ReturnsTrue()
        {
            // Arrange
            float validInterval = 0.5f;

            // Act
            bool result = VisualizerOps.IsValidRefreshInterval(validInterval);

            // Assert
            Assert.IsTrue(result, "Valid interval should return true");
        }

        [Test]
        public void VisualizerOps_IsValidRefreshInterval_WithTooSmallInterval_ReturnsFalse()
        {
            // Arrange
            float tooSmallInterval = 0.001f;

            // Act
            bool result = VisualizerOps.IsValidRefreshInterval(tooSmallInterval);

            // Assert
            Assert.IsFalse(result, "Interval below minimum should return false");
        }

        [Test]
        public void VisualizerOps_IsValidRefreshInterval_WithTooLargeInterval_ReturnsFalse()
        {
            // Arrange
            float tooLargeInterval = 10.0f;

            // Act
            bool result = VisualizerOps.IsValidRefreshInterval(tooLargeInterval);

            // Assert
            Assert.IsFalse(result, "Interval above maximum should return false");
        }

        [Test]
        public void VisualizerOps_CalculateWindowSize_WithMinimalDetail_ReturnsSmallSize()
        {
            // Arrange
            VisualizationDetailLevel detailLevel = VisualizationDetailLevel.Minimal;

            // Act
            int2 size = VisualizerOps.CalculateWindowSize(detailLevel);

            // Assert
            Assert.AreEqual(400, size.x, "Minimal detail width should be 400");
            Assert.AreEqual(300, size.y, "Minimal detail height should be 300");
        }

        [Test]
        public void VisualizerOps_CalculateWindowSize_WithBasicDetail_ReturnsMediumSize()
        {
            // Arrange
            VisualizationDetailLevel detailLevel = VisualizationDetailLevel.Basic;

            // Act
            int2 size = VisualizerOps.CalculateWindowSize(detailLevel);

            // Assert
            Assert.AreEqual(600, size.x, "Basic detail width should be 600");
            Assert.AreEqual(400, size.y, "Basic detail height should be 400");
        }

        [Test]
        public void VisualizerOps_CalculateWindowSize_WithDetailedDetail_ReturnsLargeSize()
        {
            // Arrange
            VisualizationDetailLevel detailLevel = VisualizationDetailLevel.Detailed;

            // Act
            int2 size = VisualizerOps.CalculateWindowSize(detailLevel);

            // Assert
            Assert.AreEqual(800, size.x, "Detailed detail width should be 800");
            Assert.AreEqual(600, size.y, "Detailed detail height should be 600");
        }

        [Test]
        public void VisualizerOps_CalculateWindowSize_WithVerboseDetail_ReturnsExtraLargeSize()
        {
            // Arrange
            VisualizationDetailLevel detailLevel = VisualizationDetailLevel.Verbose;

            // Act
            int2 size = VisualizerOps.CalculateWindowSize(detailLevel);

            // Assert
            Assert.AreEqual(1200, size.x, "Verbose detail width should be 1200");
            Assert.AreEqual(800, size.y, "Verbose detail height should be 800");
        }

        [Test]
        public void VisualizerOps_ShouldAutoRefresh_WithAutoRefreshDisabled_ReturnsFalse()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            visualizerData.SetAutoRefresh(false);

            // Act
            bool result = VisualizerOps.ShouldAutoRefresh(in visualizerData, Time.time + 1.0f);

            // Assert
            Assert.IsFalse(result, "Should not refresh when auto-refresh is disabled");
        }

        [Test]
        public void VisualizerOps_ShouldAutoRefresh_WithAutoRefreshEnabledAndIntervalPassed_ReturnsTrue()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            float futureTime = Time.time + 1.0f;

            // Act
            bool result = VisualizerOps.ShouldAutoRefresh(in visualizerData, futureTime);

            // Assert
            Assert.IsTrue(result, "Should refresh when interval has passed");
        }

        [Test]
        public void VisualizerOps_UpdateRefreshTime_UpdatesTimestamp()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            float newTime = Time.time + 1.5f;

            // Act
            Day09VisualizerData updated = VisualizerOps.UpdateRefreshTime(in visualizerData, newTime);

            // Assert
            Assert.AreEqual(newTime, updated.LastRefreshTime, 0.001f, "Last refresh time should be updated");
        }

        #endregion

        #region Day09VisualizerExtensions Tests

        [Test]
        public void VisualizerData_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);

            // Assert
            Assert.IsTrue(visualizerData.IsActive, "Visualizer should be active after initialization");
            Assert.IsTrue(visualizerData.IsValidVisualizer(), "Visualizer should be valid");
            Assert.AreEqual(VisualizationDetailLevel.Basic, visualizerData.DetailLevel, "Detail level should be Basic");
            Assert.IsTrue(visualizerData.AutoRefresh, "Auto-refresh should be enabled by default");
        }

        [Test]
        public void VisualizerData_WithInvalidGraph_DoesNotInitialize()
        {
            // Arrange
            PlayableGraph invalidGraph = default;

            // Act
            visualizerData.Initialize(in invalidGraph, VisualizationDetailLevel.Basic);

            // Assert
            Assert.IsFalse(visualizerData.IsActive, "Visualizer should not be active with invalid graph");
            Assert.IsFalse(visualizerData.IsValidVisualizer(), "Visualizer should not be valid");
        }

        [Test]
        public void VisualizerData_SetAutoRefresh_UpdatesAutoRefreshState()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);

            // Act
            visualizerData.SetAutoRefresh(false);

            // Assert
            Assert.IsFalse(visualizerData.AutoRefresh, "Auto-refresh should be disabled");
        }

        [Test]
        public void VisualizerData_SetRefreshInterval_UpdatesInterval()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            float newInterval = 1.5f;

            // Act
            visualizerData.SetRefreshInterval(newInterval);

            // Assert
            Assert.AreEqual(newInterval, visualizerData.RefreshInterval, 0.001f, "Refresh interval should be updated");
        }

        [Test]
        public void VisualizerData_SetRefreshInterval_WithInvalidInterval_DoesNotUpdate()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            float originalInterval = visualizerData.RefreshInterval;

            // Act
            visualizerData.SetRefreshInterval(10.0f); // Invalid (too large)

            // Assert
            Assert.AreEqual(originalInterval, visualizerData.RefreshInterval, 0.001f, 
                "Refresh interval should not change with invalid value");
        }

        [Test]
        public void VisualizerData_SetDetailLevel_UpdatesLevelAndWindow()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);

            // Act
            visualizerData.SetDetailLevel(VisualizationDetailLevel.Detailed);

            // Assert
            Assert.AreEqual(VisualizationDetailLevel.Detailed, visualizerData.DetailLevel, 
                "Detail level should be updated");
            Assert.AreEqual(0.25f, visualizerData.RefreshInterval, 0.001f, 
                "Refresh interval should match new detail level");
            Assert.AreEqual(800, visualizerData.WindowSize.x, 
                "Window width should match new detail level");
            Assert.AreEqual(600, visualizerData.WindowSize.y, 
                "Window height should match new detail level");
        }

        [Test]
        public void VisualizerData_SetColorScheme_UpdatesScheme()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);

            // Act
            visualizerData.SetColorScheme(VisualizerColorScheme.Dark);

            // Assert
            Assert.AreEqual(VisualizerColorScheme.Dark, visualizerData.ColorScheme, 
                "Color scheme should be updated");
        }

        [Test]
        public void VisualizerData_TogglePortDetails_TogglesState()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            bool originalState = visualizerData.ShowPortDetails;

            // Act
            visualizerData.TogglePortDetails();

            // Assert
            Assert.AreNotEqual(originalState, visualizerData.ShowPortDetails, 
                "Port details state should be toggled");
        }

        [Test]
        public void VisualizerData_ToggleConnections_TogglesState()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            bool originalState = visualizerData.ShowConnections;

            // Act
            visualizerData.ToggleConnections();

            // Assert
            Assert.AreNotEqual(originalState, visualizerData.ShowConnections, 
                "Connections state should be toggled");
        }

        [Test]
        public void VisualizerData_ForceRefresh_UpdatesLastRefreshTime()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            float timeBeforeForce = visualizerData.LastRefreshTime;

            // Act
            visualizerData.ForceRefresh();

            // Assert
            Assert.Greater(visualizerData.LastRefreshTime, timeBeforeForce, 
                "Last refresh time should be updated after force refresh");
        }

        [Test]
        public void VisualizerData_Dispose_DisposesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);

            // Act
            visualizerData.Dispose();

            // Assert
            Assert.IsFalse(visualizerData.IsActive, "Visualizer should not be active after disposal");
            Assert.IsFalse(visualizerData.IsValidVisualizer(), "Visualizer should not be valid after disposal");
        }

        [Test]
        public void VisualizerData_GetConfigurationString_ReturnsCorrectString()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Detailed, 
                VisualizerColorScheme.Dark);

            // Act
            string config = visualizerData.GetConfigurationString();

            // Assert
            Assert.IsTrue(config.Contains("Detailed"), "Configuration string should contain detail level");
            Assert.IsTrue(config.Contains("Dark"), "Configuration string should contain color scheme");
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Day09Integration_WithAllPreviousDays_WorksCorrectly()
        {
            // Arrange & Act - Initialize all days
            graphHandle.Initialize("IntegrationTestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "IntegrationNamedGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            outputHandle.Initialize(in graphHandle.Graph, "IntegrationOutput");
            rotatorData.Initialize(in graphHandle.Graph, "IntegrationRotator", testGameObject.transform, 90.0f, Vector3.up);
            speedData.Initialize(in graphHandle.Graph, "IntegrationSpeedControl", 1.0f, true, 2.0f);
            playStateData.Initialize(in graphHandle.Graph, "IntegrationPlayStateController", true);
            reverseData.Initialize(in graphHandle.Graph, "IntegrationReverseControl", true, true);

            // Assert - Verify all components are valid
            Assert.IsTrue(graphHandle.IsActive, "Graph should be active");
            Assert.IsTrue(namedGraphData.IsValidControl(), "Named graph should be valid");
            Assert.IsTrue(visualizerData.IsValidVisualizer(), "Visualizer should be valid");
            Assert.IsTrue(outputHandle.IsActive, "Output should be active");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState should be valid");
            Assert.IsTrue(reverseData.IsValidReverseControl(), "Reverse control should be valid");
        }

        [Test]
        public void Day09Integration_VisualizerUpdates_UpdateVisualizerWorks()
        {
            // Arrange
            graphHandle.Initialize("UpdateTest");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
            visualizerData.SetAutoRefresh(true);

            // Act - Simulate multiple updates
            for (int i = 0; i < 10; i++)
            {
                visualizerData.UpdateVisualizer();
            }

            // Assert - Visualizer should still be valid
            Assert.IsTrue(visualizerData.IsValidVisualizer(), "Visualizer should remain valid after updates");
        }

        [Test]
        public void Day09Integration_Cleanup_WorksInCorrectOrder()
        {
            // Arrange - Initialize all components
            graphHandle.Initialize("CleanupTestGraph");
            namedGraphData.Initialize(in graphHandle.Graph, "NamedGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);
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
            visualizerData.Dispose();
            namedGraphData.Dispose();
            graphHandle.Dispose();

            // Assert - Verify all components are cleaned up
            Assert.IsFalse(reverseData.IsActive, "Reverse control should be cleaned up");
            Assert.IsFalse(playStateData.IsActive, "PlayState should be cleaned up");
            Assert.IsFalse(speedData.IsActive, "Speed control should be cleaned up");
            Assert.IsFalse(rotatorData.IsActive, "Rotator should be cleaned up");
            Assert.IsFalse(outputHandle.IsActive, "Output should be cleaned up");
            Assert.IsFalse(visualizerData.IsActive, "Visualizer should be cleaned up");
            Assert.IsFalse(namedGraphData.IsActive, "Named graph should be cleaned up");
            Assert.IsFalse(graphHandle.IsActive, "Graph should be cleaned up");
        }

        [Test]
        public void Day09Integration_MultipleDetailLevels_AllWorkCorrectly()
        {
            // Arrange
            graphHandle.Initialize("DetailLevelTest");
            var visualizers = new Day09VisualizerData[4];
            var detailLevels = new[]
            {
                VisualizationDetailLevel.Minimal,
                VisualizationDetailLevel.Basic,
                VisualizationDetailLevel.Detailed,
                VisualizationDetailLevel.Verbose
            };

            // Act - Initialize with different detail levels
            for (int i = 0; i < detailLevels.Length; i++)
            {
                visualizers[i].Initialize(in graphHandle.Graph, detailLevels[i]);
            }

            // Assert - All should be valid with correct settings
            for (int i = 0; i < detailLevels.Length; i++)
            {
                Assert.IsTrue(visualizers[i].IsValidVisualizer(), 
                    $"Visualizer with {detailLevels[i]} detail level should be valid");
                Assert.AreEqual(detailLevels[i], visualizers[i].DetailLevel, 
                    $"Detail level should be {detailLevels[i]}");
            }

            // Cleanup
            for (int i = 0; i < visualizers.Length; i++)
            {
                var v = visualizers[i];
                v.Dispose();
                visualizers[i] = v;
            }
        }

        [Test]
        public void Day09Integration_VisualizerWithNamedGraph_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("NamedGraphTest");
            namedGraphData.Initialize(in graphHandle.Graph, "TestNamedGraph");
            visualizerData.Initialize(in graphHandle.Graph, VisualizationDetailLevel.Basic);

            // Act & Assert
            Assert.IsTrue(namedGraphData.IsValidControl(), "Named graph should be valid");
            Assert.IsTrue(visualizerData.IsValidVisualizer(), "Visualizer should be valid");
            
            string graphName = namedGraphData.GetName();
            Assert.IsTrue(visualizerData.Graph.IsValid(), "Visualizer should have valid graph");
        }

        #endregion
    }
}