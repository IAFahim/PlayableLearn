using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Mathematics;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day04;
using PlayableLearn.Day05;
using PlayableLearn.Day06;

namespace PlayableLearn.Day06.Tests
{
    public class Day06Tests
    {
        private Day01GraphHandle graphHandle;
        private Day02OutputHandle outputHandle;
        private Day04RotatorData rotatorData;
        private Day05SpeedData speedData;
        private Day06PlayStateData playStateData;
        private GameObject testGameObject;

        [SetUp]
        public void SetUp()
        {
            graphHandle = new Day01GraphHandle();
            outputHandle = new Day02OutputHandle();
            rotatorData = new Day04RotatorData();
            speedData = new Day05SpeedData();
            playStateData = new Day06PlayStateData();
            testGameObject = new GameObject("TestCube");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up in reverse order
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
        public void PlayStateData_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Assert
            Assert.IsTrue(playStateData.IsActive, "PlayState control should be active after initialization");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState control should be valid");
            Assert.IsTrue(playStateData.IsGraphValid, "Graph should be marked as valid");
        }

        [Test]
        public void PlayStateData_WithAutoPlayOnStart_StartsPlaying()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act - Initialize with auto-play enabled
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Assert
            Assert.IsTrue(playStateData.IsPlaying(), "Graph should be playing with auto-play enabled");
        }

        [Test]
        public void PlayStateData_WithAutoPlayOff_StartsPaused()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act - Initialize with auto-play disabled
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", false);

            // Assert
            Assert.IsTrue(playStateData.IsPaused(), "Graph should be paused with auto-play disabled");
        }

        [Test]
        public void PlayStateData_DisposesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Act
            playStateData.Dispose();

            // Assert
            Assert.IsFalse(playStateData.IsActive, "PlayState control should not be active after disposal");
        }

        [Test]
        public void PlayStateData_Play_StartsGraph()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", false);

            // Act - Start playing
            playStateData.Play();

            // Assert
            Assert.IsTrue(playStateData.IsPlaying(), "Graph should be playing after Play() call");
        }

        [Test]
        public void PlayStateData_Pause_StopsGraph()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Act - Pause the graph
            playStateData.Pause();

            // Assert
            Assert.IsTrue(playStateData.IsPaused(), "Graph should be paused after Pause() call");
        }

        [Test]
        public void PlayStateData_TogglePlayPause_TogglesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Act - Toggle to pause
            playStateData.TogglePlayPause();

            // Assert - Should be paused
            Assert.IsTrue(playStateData.IsPaused(), "Graph should be paused after first toggle");

            // Act - Toggle to play
            playStateData.TogglePlayPause();

            // Assert - Should be playing
            Assert.IsTrue(playStateData.IsPlaying(), "Graph should be playing after second toggle");
        }

        [Test]
        public void PlayStateData_UpdateState_TracksStateChanges()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Act - Pause the graph
            playStateData.Pause();
            playStateData.UpdateState();

            // Assert - State should be updated
            Assert.IsTrue(playStateData.IsPaused(), "State should be updated to paused");
        }

        [Test]
        public void PlayStateData_JustPaused_DetectsPauseTransition()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Act - Pause the graph
            playStateData.Pause();
            playStateData.UpdateState();

            // Assert - Should detect the transition
            Assert.IsTrue(playStateData.JustPaused(), "Should detect transition to paused state");
        }

        [Test]
        public void PlayStateData_JustStartedPlaying_DetectsPlayTransition()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", false);

            // Act - Start playing
            playStateData.Play();
            playStateData.UpdateState();

            // Assert - Should detect the transition
            Assert.IsTrue(playStateData.JustStartedPlaying(), "Should detect transition to playing state");
        }

        [Test]
        public void PlayStateData_GetStateString_ReturnsCorrectString()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Act
            string stateString = playStateData.GetStateString();

            // Assert
            Assert.AreEqual("Playing", stateString, "State string should be 'Playing'");
        }

        [Test]
        public void PlayStateData_GetProgress_ReturnsCorrectValue()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Act
            float progress = playStateData.GetProgress();

            // Assert - Playing should return 1.0
            Assert.AreEqual(1.0f, progress, 0.001f, "Progress should be 1.0 when playing");
        }

        [Test]
        public void PlayStateData_LogStateInfo_LogsCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() => playStateData.LogStateInfo("TestControl"), "LogStateInfo should not throw");
        }

        [Test]
        public void PlayStateOps_IsPlaying_WorksCorrectly()
        {
            // Arrange
            PlayState playingState = PlayState.Playing;
            PlayState pausedState = PlayState.Paused;

            // Act
            bool isPlaying1 = PlayStateOps.IsPlaying(playingState);
            bool isPlaying2 = PlayStateOps.IsPlaying(pausedState);

            // Assert
            Assert.IsTrue(isPlaying1, "Playing state should return true");
            Assert.IsFalse(isPlaying2, "Paused state should return false");
        }

        [Test]
        public void PlayStateOps_IsPaused_WorksCorrectly()
        {
            // Arrange
            PlayState playingState = PlayState.Playing;
            PlayState pausedState = PlayState.Paused;

            // Act
            bool isPaused1 = PlayStateOps.IsPaused(playingState);
            bool isPaused2 = PlayStateOps.IsPaused(pausedState);

            // Assert
            Assert.IsFalse(isPaused1, "Playing state should return false");
            Assert.IsTrue(isPaused2, "Paused state should return true");
        }

        [Test]
        public void PlayStateOps_DidStateChange_DetectsChanges()
        {
            // Arrange
            PlayState fromState = PlayState.Playing;
            PlayState toState = PlayState.Paused;
            PlayState sameState = PlayState.Playing;

            // Act
            bool changed = PlayStateOps.DidStateChange(fromState, toState);
            bool notChanged = PlayStateOps.DidStateChange(fromState, sameState);

            // Assert
            Assert.IsTrue(changed, "Should detect state change");
            Assert.IsFalse(notChanged, "Should not detect change when states are the same");
        }

        [Test]
        public void PlayStateOps_IsTransitionToPause_DetectsPauseTransition()
        {
            // Arrange
            PlayState fromState = PlayState.Playing;
            PlayState toState = PlayState.Paused;
            PlayState otherState = PlayState.Playing;

            // Act
            bool isPauseTransition = PlayStateOps.IsTransitionToPause(fromState, toState);
            bool isNotPauseTransition = PlayStateOps.IsTransitionToPause(fromState, otherState);

            // Assert
            Assert.IsTrue(isPauseTransition, "Should detect playing -> paused transition");
            Assert.IsFalse(isNotPauseTransition, "Should not detect playing -> playing transition");
        }

        [Test]
        public void PlayStateOps_IsTransitionToPlay_DetectsPlayTransition()
        {
            // Arrange
            PlayState fromState = PlayState.Paused;
            PlayState toState = PlayState.Playing;
            PlayState otherState = PlayState.Paused;

            // Act
            bool isPlayTransition = PlayStateOps.IsTransitionToPlay(fromState, toState);
            bool isNotPlayTransition = PlayStateOps.IsTransitionToPlay(fromState, otherState);

            // Assert
            Assert.IsTrue(isPlayTransition, "Should detect paused -> playing transition");
            Assert.IsFalse(isNotPlayTransition, "Should not detect paused -> paused transition");
        }

        [Test]
        public void PlayStateOps_GetToggledState_TogglesCorrectly()
        {
            // Arrange
            PlayState playingState = PlayState.Playing;
            PlayState pausedState = PlayState.Paused;

            // Act
            PlayStateOps.GetToggledState(playingState, out PlayState toggledFromPlaying);
            PlayStateOps.GetToggledState(pausedState, out PlayState toggledFromPaused);

            // Assert
            Assert.AreEqual(PlayState.Paused, toggledFromPlaying, "Should toggle from playing to paused");
            Assert.AreEqual(PlayState.Playing, toggledFromPaused, "Should toggle from paused to playing");
        }

        [Test]
        public void PlayStateOps_IsValidTransition_WorksCorrectly()
        {
            // Arrange
            PlayState playingState = PlayState.Playing;
            PlayState pausedState = PlayState.Paused;

            // Act
            bool validTransition1 = PlayStateOps.IsValidTransition(playingState, pausedState);
            bool validTransition2 = PlayStateOps.IsValidTransition(pausedState, playingState);

            // Assert
            Assert.IsTrue(validTransition1, "Playing -> Paused should be valid");
            Assert.IsTrue(validTransition2, "Paused -> Playing should be valid");
        }

        [Test]
        public void PlayStateOps_CalculateProgress_ReturnsCorrectValue()
        {
            // Arrange
            PlayState playingState = PlayState.Playing;
            PlayState pausedState = PlayState.Paused;

            // Act
            PlayStateOps.CalculateProgress(playingState, out float playingProgress);
            PlayStateOps.CalculateProgress(pausedState, out float pausedProgress);

            // Assert
            Assert.AreEqual(1.0f, playingProgress, 0.001f, "Playing should return 1.0 progress");
            Assert.AreEqual(0.0f, pausedProgress, 0.001f, "Paused should return 0.0 progress");
        }

        [Test]
        public void PlayStateOps_StateToInt_ConvertsCorrectly()
        {
            // Arrange
            PlayState playingState = PlayState.Playing;
            PlayState pausedState = PlayState.Paused;

            // Act
            PlayStateOps.StateToInt(playingState, out int playingInt);
            PlayStateOps.StateToInt(pausedState, out int pausedInt);

            // Assert
            Assert.AreEqual(0, playingInt, "Playing should convert to 0");
            Assert.AreEqual(1, pausedInt, "Paused should convert to 1");
        }

        [Test]
        public void PlayStateOps_ShouldAutoPlay_WorksCorrectly()
        {
            // Arrange
            bool autoPlayEnabled = true;
            bool autoPlayDisabled = false;

            // Act
            bool shouldPlay1 = PlayStateOps.ShouldAutoPlay(autoPlayEnabled);
            bool shouldPlay2 = PlayStateOps.ShouldAutoPlay(autoPlayDisabled);

            // Assert
            Assert.IsTrue(shouldPlay1, "Should auto-play when enabled");
            Assert.IsFalse(shouldPlay2, "Should not auto-play when disabled");
        }

        [Test]
        public void PlayStateOps_CanControlGraph_WorksCorrectly()
        {
            // Arrange
            bool graphValid = true;
            bool graphInvalid = false;

            // Act
            bool canControl1 = PlayStateOps.CanControlGraph(graphValid);
            bool canControl2 = PlayStateOps.CanControlGraph(graphInvalid);

            // Assert
            Assert.IsTrue(canControl1, "Should control when graph is valid");
            Assert.IsFalse(canControl2, "Should not control when graph is invalid");
        }

        [Test]
        public void CompleteFlow_GraphOutputSpeedControlRotatorPlayState_WorksCorrectly()
        {
            // Act - Create the complete chain
            graphHandle.Initialize("CompleteTestGraph");
            outputHandle.Initialize(in graphHandle.Graph, "CompleteTestOutput");
            rotatorData.Initialize(in graphHandle.Graph, "CompleteTestRotator", testGameObject.transform, 90.0f, Vector3.up);
            speedData.Initialize(in graphHandle.Graph, "CompleteTestSpeedControl", 2.0f, true, 2.0f);
            playStateData.Initialize(in graphHandle.Graph, "CompleteTestPlayStateControl", true);

            // Connect speed control to output
            speedData.ConnectToOutput(in outputHandle.Output);

            // Assert - Verify all components are valid
            Assert.IsTrue(graphHandle.IsActive, "Graph should be active");
            Assert.IsTrue(outputHandle.IsValidOutput(), "Output should be valid");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState control should be valid");

            // Verify the connection
            Playable source = outputHandle.Output.GetSourcePlayable();
            Assert.IsTrue(source.IsValid(), "Output should have a valid source");

            // Verify play state
            Assert.IsTrue(playStateData.IsPlaying(), "Graph should be playing with auto-play");
        }

        [Test]
        public void PlayStateData_WithInvalidGraph_DoesNotInitialize()
        {
            // Arrange - Invalid graph
            PlayableGraph invalidGraph = default;

            // Act
            playStateData.Initialize(in invalidGraph, "TestPlayStateControl", true);

            // Assert
            Assert.IsFalse(playStateData.IsActive, "PlayState control should not be active with invalid graph");
        }

        [Test]
        public void PlayStateData_PauseWhenAlreadyPaused_DoesNotError()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", false);

            // Act & Assert - Should not throw even though already paused
            Assert.DoesNotThrow(() => playStateData.Pause(), "Pause when already paused should not throw");
        }

        [Test]
        public void PlayStateData_PlayWhenAlreadyPlaying_DoesNotError()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            playStateData.Initialize(in graphHandle.Graph, "TestPlayStateControl", true);

            // Act & Assert - Should not throw even though already playing
            Assert.DoesNotThrow(() => playStateData.Play(), "Play when already playing should not throw");
        }
    }
}
