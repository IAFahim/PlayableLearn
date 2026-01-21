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

namespace PlayableLearn.Day07.Tests
{
    public class Day07Tests
    {
        private Day01GraphHandle graphHandle;
        private Day02OutputHandle outputHandle;
        private Day04RotatorData rotatorData;
        private Day05SpeedData speedData;
        private Day06PlayStateData playStateData;
        private Day07ReverseData reverseData;
        private GameObject testGameObject;

        [SetUp]
        public void SetUp()
        {
            graphHandle = new Day01GraphHandle();
            outputHandle = new Day02OutputHandle();
            rotatorData = new Day04RotatorData();
            speedData = new Day05SpeedData();
            playStateData = new Day06PlayStateData();
            reverseData = new Day07ReverseData();
            testGameObject = new GameObject("TestCube");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up in reverse order
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

        [Test]
        public void ReverseData_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Assert
            Assert.IsTrue(reverseData.IsActive, "Reverse time control should be active after initialization");
            Assert.IsTrue(reverseData.IsValidReverseControl(), "Reverse time control should be valid");
        }

        [Test]
        public void ReverseData_WithReverseEnabled_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act - Initialize with reverse time enabled
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Assert
            Assert.IsTrue(reverseData.IsActive, "Reverse time control should be active");
            Assert.IsTrue(reverseData.EnableReverseTime, "Reverse time should be enabled");
            Assert.IsTrue(reverseData.EnableTimeWrapping, "Time wrapping should be enabled");
        }

        [Test]
        public void ReverseData_WithReverseDisabled_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act - Initialize with reverse time disabled
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", false, false);

            // Assert
            Assert.IsTrue(reverseData.IsActive, "Reverse time control should be active");
            Assert.IsFalse(reverseData.EnableReverseTime, "Reverse time should be disabled");
            Assert.IsFalse(reverseData.EnableTimeWrapping, "Time wrapping should be disabled");
        }

        [Test]
        public void ReverseData_DisposesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act
            reverseData.Dispose();

            // Assert
            Assert.IsFalse(reverseData.IsActive, "Reverse time control should not be active after disposal");
        }

        [Test]
        public void ReverseData_WithInvalidGraph_DoesNotInitialize()
        {
            // Arrange - Invalid graph
            PlayableGraph invalidGraph = default;

            // Act
            reverseData.Initialize(in invalidGraph, "TestReverseControl", true, true);

            // Assert
            Assert.IsFalse(reverseData.IsActive, "Reverse time control should not be active with invalid graph");
        }

        [Test]
        public void ReverseData_SetWrapLimit_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act
            reverseData.SetWrapLimit(20.0);

            // Assert
            Assert.AreEqual(20.0, reverseData.GetWrapLimit(), 0.001, "Wrap limit should be set to 20.0");
        }

        [Test]
        public void ReverseData_UpdateTimeTracking_TracksTimeForward()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);
            reverseData.SetCurrentSpeed(1.0f);

            // Act - Update time tracking with 1 second delta
            reverseData.UpdateTimeTracking(1.0f);

            // Assert
            Assert.AreEqual(1.0, reverseData.GetAccumulatedTime(), 0.001, "Accumulated time should be 1.0s");
        }

        [Test]
        public void ReverseData_UpdateTimeTracking_TracksTimeReverse()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);
            reverseData.SetCurrentSpeed(-1.0f);

            // Act - Update time tracking with 1 second delta
            reverseData.UpdateTimeTracking(1.0f);

            // Assert
            Assert.AreEqual(-1.0, reverseData.GetAccumulatedTime(), 0.001, "Accumulated time should be -1.0s");
        }

        [Test]
        public void ReverseData_IsReversing_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act & Assert - Forward speed
            reverseData.SetCurrentSpeed(1.0f);
            Assert.IsFalse(reverseData.IsReversing(), "Should not be reversing with positive speed");

            // Act & Assert - Reverse speed
            reverseData.SetCurrentSpeed(-1.0f);
            Assert.IsTrue(reverseData.IsReversing(), "Should be reversing with negative speed");
        }

        [Test]
        public void ReverseData_IsForward_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act & Assert - Forward speed
            reverseData.SetCurrentSpeed(1.0f);
            Assert.IsTrue(reverseData.IsForward(), "Should be forward with positive speed");

            // Act & Assert - Reverse speed
            reverseData.SetCurrentSpeed(-1.0f);
            Assert.IsFalse(reverseData.IsForward(), "Should not be forward with negative speed");
        }

        [Test]
        public void ReverseData_IsStopped_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act & Assert - Stopped speed
            reverseData.SetCurrentSpeed(0.0f);
            Assert.IsTrue(reverseData.IsStopped(), "Should be stopped with zero speed");

            // Act & Assert - Moving speed
            reverseData.SetCurrentSpeed(1.0f);
            Assert.IsFalse(reverseData.IsStopped(), "Should not be stopped with positive speed");
        }

        [Test]
        public void ReverseData_ClampSpeed_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act & Assert - Clamp within bounds
            float clamped1 = reverseData.ClampSpeed(-1.0f, 1.0f, 0.5f);
            Assert.AreEqual(0.5f, clamped1, 0.001f, "Should not clamp within bounds");

            // Act & Assert - Clamp above max
            float clamped2 = reverseData.ClampSpeed(-1.0f, 1.0f, 2.0f);
            Assert.AreEqual(1.0f, clamped2, 0.001f, "Should clamp to max");

            // Act & Assert - Clamp below min
            float clamped3 = reverseData.ClampSpeed(-1.0f, 1.0f, -2.0f);
            Assert.AreEqual(-1.0f, clamped3, 0.001f, "Should clamp to min");
        }

        [Test]
        public void ReverseData_GetSpeedMagnitude_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act & Assert - Positive speed
            reverseData.SetCurrentSpeed(2.0f);
            Assert.AreEqual(2.0f, reverseData.GetSpeedMagnitude(), 0.001f, "Magnitude should be 2.0");

            // Act & Assert - Negative speed
            reverseData.SetCurrentSpeed(-2.0f);
            Assert.AreEqual(2.0f, reverseData.GetSpeedMagnitude(), 0.001f, "Magnitude should be 2.0");
        }

        [Test]
        public void ReverseData_GetSpeedDirection_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act & Assert - Forward direction
            reverseData.SetCurrentSpeed(1.0f);
            Assert.AreEqual(1, reverseData.GetSpeedDirection(), "Direction should be 1 (forward)");

            // Act & Assert - Reverse direction
            reverseData.SetCurrentSpeed(-1.0f);
            Assert.AreEqual(-1, reverseData.GetSpeedDirection(), "Direction should be -1 (reverse)");

            // Act & Assert - Stopped
            reverseData.SetCurrentSpeed(0.0f);
            Assert.AreEqual(0, reverseData.GetSpeedDirection(), "Direction should be 0 (stopped)");
        }

        [Test]
        public void ReverseData_GetDirectionSymbol_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act & Assert - Forward symbol
            reverseData.SetCurrentSpeed(1.0f);
            Assert.AreEqual(">>", reverseData.GetDirectionSymbol(), "Symbol should be '>>'");

            // Act & Assert - Reverse symbol
            reverseData.SetCurrentSpeed(-1.0f);
            Assert.AreEqual("<<", reverseData.GetDirectionSymbol(), "Symbol should be '<<'");

            // Act & Assert - Stopped symbol
            reverseData.SetCurrentSpeed(0.0f);
            Assert.AreEqual("||", reverseData.GetDirectionSymbol(), "Symbol should be '||'");
        }

        [Test]
        public void ReverseData_ResetTime_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);
            reverseData.SetCurrentSpeed(1.0f);
            reverseData.UpdateTimeTracking(5.0f);

            // Act
            reverseData.ResetTime();

            // Assert
            Assert.AreEqual(0.0, reverseData.GetAccumulatedTime(), 0.001, "Time should be reset to 0.0");
        }

        [Test]
        public void ReverseData_SetReverseEnabled_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act
            reverseData.SetReverseEnabled(false);

            // Assert
            Assert.IsFalse(reverseData.EnableReverseTime, "Reverse time should be disabled");
        }

        [Test]
        public void ReverseData_SetTimeWrappingEnabled_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act
            reverseData.SetTimeWrappingEnabled(false);

            // Assert
            Assert.IsFalse(reverseData.EnableTimeWrapping, "Time wrapping should be disabled");
        }

        [Test]
        public void ReverseData_IsNearWrapLimit_WorksCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);
            reverseData.SetWrapLimit(10.0);
            reverseData.SetCurrentSpeed(1.0f);
            reverseData.UpdateTimeTracking(9.5f); // 9.5s accumulated

            // Act & Assert - Near limit
            Assert.IsTrue(reverseData.IsNearWrapLimit(1.0), "Should be near wrap limit");

            // Act & Assert - Not near limit
            Assert.IsFalse(reverseData.IsNearWrapLimit(0.1), "Should not be near wrap limit with small threshold");
        }

        [Test]
        public void ReverseData_LogReverseInfo_LogsCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            reverseData.Initialize(in graphHandle.Graph, "TestReverseControl", true, true);

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() => reverseData.LogReverseInfo("TestControl"), "LogReverseInfo should not throw");
        }

        [Test]
        public void ReverseTimeOps_IsReversing_WorksCorrectly()
        {
            // Arrange
            double forwardSpeed = 1.0;
            double reverseSpeed = -1.0;
            double stoppedSpeed = 0.0;

            // Act
            bool isReversing1 = ReverseTimeOps.IsReversing(forwardSpeed);
            bool isReversing2 = ReverseTimeOps.IsReversing(reverseSpeed);
            bool isReversing3 = ReverseTimeOps.IsReversing(stoppedSpeed);

            // Assert
            Assert.IsFalse(isReversing1, "Forward speed should not be reversing");
            Assert.IsTrue(isReversing2, "Negative speed should be reversing");
            Assert.IsFalse(isReversing3, "Zero speed should not be reversing");
        }

        [Test]
        public void ReverseTimeOps_IsForward_WorksCorrectly()
        {
            // Arrange
            double forwardSpeed = 1.0;
            double reverseSpeed = -1.0;
            double stoppedSpeed = 0.0;

            // Act
            bool isForward1 = ReverseTimeOps.IsForward(forwardSpeed);
            bool isForward2 = ReverseTimeOps.IsForward(reverseSpeed);
            bool isForward3 = ReverseTimeOps.IsForward(stoppedSpeed);

            // Assert
            Assert.IsTrue(isForward1, "Positive speed should be forward");
            Assert.IsFalse(isForward2, "Negative speed should not be forward");
            Assert.IsFalse(isForward3, "Zero speed should not be forward");
        }

        [Test]
        public void ReverseTimeOps_IsStopped_WorksCorrectly()
        {
            // Arrange
            double forwardSpeed = 1.0;
            double reverseSpeed = -1.0;
            double stoppedSpeed = 0.0;

            // Act
            bool isStopped1 = ReverseTimeOps.IsStopped(forwardSpeed);
            bool isStopped2 = ReverseTimeOps.IsStopped(reverseSpeed);
            bool isStopped3 = ReverseTimeOps.IsStopped(stoppedSpeed);

            // Assert
            Assert.IsFalse(isStopped1, "Positive speed should not be stopped");
            Assert.IsFalse(isStopped2, "Negative speed should not be stopped");
            Assert.IsTrue(isStopped3, "Zero speed should be stopped");
        }

        [Test]
        public void ReverseTimeOps_ClampSpeed_WorksCorrectly()
        {
            // Arrange
            float minSpeed = -1.0f;
            float maxSpeed = 1.0f;

            // Act
            float clamped1 = ReverseTimeOps.ClampSpeed(minSpeed, maxSpeed, 0.5f);
            float clamped2 = ReverseTimeOps.ClampSpeed(minSpeed, maxSpeed, 2.0f);
            float clamped3 = ReverseTimeOps.ClampSpeed(minSpeed, maxSpeed, -2.0f);

            // Assert
            Assert.AreEqual(0.5f, clamped1, 0.001f, "Should not clamp within bounds");
            Assert.AreEqual(1.0f, clamped2, 0.001f, "Should clamp to max");
            Assert.AreEqual(-1.0f, clamped3, 0.001f, "Should clamp to min");
        }

        [Test]
        public void ReverseTimeOps_WrapTime_WorksCorrectly()
        {
            // Arrange
            double wrapLimit = 10.0;

            // Act - Wrap forward
            double wrapped1 = ReverseTimeOps.WrapTime(12.0, wrapLimit);

            // Act - Wrap backward
            double wrapped2 = ReverseTimeOps.WrapTime(-12.0, wrapLimit);

            // Act - No wrap needed
            double wrapped3 = ReverseTimeOps.WrapTime(5.0, wrapLimit);

            // Assert
            Assert.AreEqual(-8.0, wrapped1, 0.001, "Should wrap from +12 to -8");
            Assert.AreEqual(8.0, wrapped2, 0.001, "Should wrap from -12 to +8");
            Assert.AreEqual(5.0, wrapped3, 0.001, "Should not wrap within bounds");
        }

        [Test]
        public void ReverseTimeOps_DidDirectionChange_DetectsChanges()
        {
            // Arrange
            double forwardSpeed = 1.0;
            double reverseSpeed = -1.0;
            double sameSpeed = 1.0;

            // Act
            bool changed = ReverseTimeOps.DidDirectionChange(forwardSpeed, reverseSpeed);
            bool notChanged = ReverseTimeOps.DidDirectionChange(forwardSpeed, sameSpeed);

            // Assert
            Assert.IsTrue(changed, "Should detect direction change");
            Assert.IsFalse(notChanged, "Should not detect change when directions are the same");
        }

        [Test]
        public void ReverseTimeOps_IsTransitionToReverse_DetectsTransition()
        {
            // Arrange
            double forwardSpeed = 1.0;
            double reverseSpeed = -1.0;
            double otherSpeed = 1.0;

            // Act
            bool isReverseTransition = ReverseTimeOps.IsTransitionToReverse(forwardSpeed, reverseSpeed);
            bool isNotReverseTransition = ReverseTimeOps.IsTransitionToReverse(forwardSpeed, otherSpeed);

            // Assert
            Assert.IsTrue(isReverseTransition, "Should detect forward -> reverse transition");
            Assert.IsFalse(isNotReverseTransition, "Should not detect forward -> forward transition");
        }

        [Test]
        public void ReverseTimeOps_IsTransitionToForward_DetectsTransition()
        {
            // Arrange
            double reverseSpeed = -1.0;
            double forwardSpeed = 1.0;
            double otherSpeed = -1.0;

            // Act
            bool isForwardTransition = ReverseTimeOps.IsTransitionToForward(reverseSpeed, forwardSpeed);
            bool isNotForwardTransition = ReverseTimeOps.IsTransitionToForward(reverseSpeed, otherSpeed);

            // Assert
            Assert.IsTrue(isForwardTransition, "Should detect reverse -> forward transition");
            Assert.IsFalse(isNotForwardTransition, "Should not detect reverse -> reverse transition");
        }

        [Test]
        public void ReverseTimeOps_GetSpeedMagnitude_WorksCorrectly()
        {
            // Arrange
            float positiveSpeed = 2.0f;
            float negativeSpeed = -2.0f;

            // Act
            float magnitude1 = ReverseTimeOps.GetSpeedMagnitude(positiveSpeed);
            float magnitude2 = ReverseTimeOps.GetSpeedMagnitude(negativeSpeed);

            // Assert
            Assert.AreEqual(2.0f, magnitude1, 0.001f, "Magnitude should be 2.0");
            Assert.AreEqual(2.0f, magnitude2, 0.001f, "Magnitude should be 2.0");
        }

        [Test]
        public void ReverseTimeOps_GetSpeedDirection_WorksCorrectly()
        {
            // Arrange
            double forwardSpeed = 1.0;
            double reverseSpeed = -1.0;
            double stoppedSpeed = 0.0;

            // Act
            int direction1 = ReverseTimeOps.GetSpeedDirection(forwardSpeed);
            int direction2 = ReverseTimeOps.GetSpeedDirection(reverseSpeed);
            int direction3 = ReverseTimeOps.GetSpeedDirection(stoppedSpeed);

            // Assert
            Assert.AreEqual(1, direction1, "Forward direction should be 1");
            Assert.AreEqual(-1, direction2, "Reverse direction should be -1");
            Assert.AreEqual(0, direction3, "Stopped direction should be 0");
        }

        [Test]
        public void ReverseTimeOps_CalculateNormalizedProgress_WorksCorrectly()
        {
            // Arrange
            double wrapLimit = 10.0;

            // Act
            float progress1 = ReverseTimeOps.CalculateNormalizedProgress(0.0, wrapLimit);
            float progress2 = ReverseTimeOps.CalculateNormalizedProgress(5.0, wrapLimit);
            float progress3 = ReverseTimeOps.CalculateNormalizedProgress(10.0, wrapLimit);

            // Assert
            Assert.AreEqual(0.5f, progress1, 0.001f, "0.0s should map to 0.5 (center of range)");
            Assert.IsTrue(progress2 > 0.5f && progress2 < 1.0f, "5.0s should be in upper half");
            Assert.AreEqual(0.5f, progress3, 0.001f, "10.0s should wrap to 0.5 (center of range)");
        }

        [Test]
        public void CompleteFlow_AllDay07Components_WorksCorrectly()
        {
            // Act - Create the complete chain
            graphHandle.Initialize("CompleteTestGraph");
            outputHandle.Initialize(in graphHandle.Graph, "CompleteTestOutput");
            rotatorData.Initialize(in graphHandle.Graph, "CompleteTestRotator", testGameObject.transform, 90.0f, Vector3.up);
            speedData.Initialize(in graphHandle.Graph, "CompleteTestSpeedControl", 2.0f, true, 2.0f);
            playStateData.Initialize(in graphHandle.Graph, "CompleteTestPlayStateControl", true);
            reverseData.Initialize(in graphHandle.Graph, "CompleteTestReverseTimeControl", true, true);

            // Connect speed control to output
            speedData.ConnectToOutput(in outputHandle.Output);

            // Assert - Verify all components are valid
            Assert.IsTrue(graphHandle.IsActive, "Graph should be active");
            Assert.IsTrue(outputHandle.IsValidOutput(), "Output should be valid");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.IsTrue(playStateData.IsValidControl(), "PlayState control should be valid");
            Assert.IsTrue(reverseData.IsValidReverseControl(), "Reverse time control should be valid");

            // Verify the connection
            Playable source = outputHandle.Output.GetSourcePlayable();
            Assert.IsTrue(source.IsValid(), "Output should have a valid source");

            // Verify play state
            Assert.IsTrue(playStateData.IsPlaying(), "Graph should be playing with auto-play");

            // Verify reverse time is enabled
            Assert.IsTrue(reverseData.EnableReverseTime, "Reverse time should be enabled");
        }
    }
}
