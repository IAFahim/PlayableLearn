using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Mathematics;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day03;
using PlayableLearn.Day04;
using PlayableLearn.Day05;

namespace PlayableLearn.Day05.Tests
{
    public class Day05Tests
    {
        private Day01GraphHandle graphHandle;
        private Day02OutputHandle outputHandle;
        private Day04RotatorData rotatorData;
        private Day05SpeedData speedData;
        private GameObject testGameObject;

        [SetUp]
        public void SetUp()
        {
            graphHandle = new Day01GraphHandle();
            outputHandle = new Day02OutputHandle();
            rotatorData = new Day04RotatorData();
            speedData = new Day05SpeedData();
            testGameObject = new GameObject("TestCube");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up in reverse order
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
        public void SpeedData_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            float speedMultiplier = 2.0f;
            bool enableTimeDilation = true;
            float interpolationSpeed = 2.0f;

            // Act
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", speedMultiplier, enableTimeDilation, interpolationSpeed);

            // Assert
            Assert.IsTrue(speedData.IsActive, "Speed control should be active after initialization");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");
            Assert.AreEqual(speedMultiplier, speedData.SpeedMultiplier, 0.001f, "Speed multiplier should match");
            Assert.AreEqual(enableTimeDilation, speedData.EnableTimeDilation, "Time dilation enabled should match");
            Assert.AreEqual(interpolationSpeed, speedData.InterpolationSpeed, 0.001f, "Interpolation speed should match");
        }

        [Test]
        public void SpeedData_WithDefaultSpeed_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act - Initialize with default parameters
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl");

            // Assert
            Assert.IsTrue(speedData.IsActive, "Speed control should be active");
            Assert.AreEqual(1.0f, speedData.SpeedMultiplier, 0.001f, "Default speed should be 1.0");
            Assert.IsTrue(speedData.EnableTimeDilation, "Time dilation should be enabled by default");
        }

        [Test]
        public void SpeedData_DisposesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 2.0f, true, 2.0f);

            // Act
            speedData.Dispose();

            // Assert
            Assert.IsFalse(speedData.IsActive, "Speed control should not be active after disposal");
        }

        [Test]
        public void SpeedData_SetTargetSpeed_UpdatesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 1.0f, true, 2.0f);
            float newTargetSpeed = 0.5f;

            // Act
            speedData.SetTargetSpeed(newTargetSpeed);

            // Assert
            Assert.AreEqual(newTargetSpeed, speedData.TargetSpeed, 0.001f, "Target speed should be updated");
        }

        [Test]
        public void SpeedData_SetSpeedImmediate_UpdatesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 1.0f, true, 2.0f);
            float newSpeed = 0.0f;

            // Act
            speedData.SetSpeedImmediate(newSpeed);

            // Assert
            Assert.AreEqual(newSpeed, speedData.SpeedMultiplier, 0.001f, "Speed multiplier should be updated immediately");
            Assert.AreEqual(newSpeed, speedData.TargetSpeed, 0.001f, "Target speed should also be updated");
        }

        [Test]
        public void SpeedData_SetTimeDilationEnabled_TogglesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 1.0f, true, 2.0f);

            // Act
            speedData.SetTimeDilationEnabled(false);

            // Assert
            Assert.IsFalse(speedData.EnableTimeDilation, "Time dilation should be disabled");

            // Act - Enable again
            speedData.SetTimeDilationEnabled(true);

            // Assert
            Assert.IsTrue(speedData.EnableTimeDilation, "Time dilation should be enabled");
        }

        [Test]
        public void SpeedData_IsPaused_DetectsPausedState()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 0.0f, true, 2.0f);

            // Act
            bool isPaused = speedData.IsPaused();

            // Assert
            Assert.IsTrue(isPaused, "Speed of 0 should be detected as paused");
        }

        [Test]
        public void SpeedData_IsReversed_DetectsReversedState()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", -1.0f, true, 2.0f);

            // Act
            bool isReversed = speedData.IsReversed();

            // Assert
            Assert.IsTrue(isReversed, "Negative speed should be detected as reversed");
        }

        [Test]
        public void SpeedData_IsFastForward_DetectsFastForwardState()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 2.5f, true, 2.0f);

            // Act
            bool isFastForward = speedData.IsFastForward();

            // Assert
            Assert.IsTrue(isFastForward, "Speed > 1.0 should be detected as fast forward");
        }

        [Test]
        public void SpeedData_IsSlowMotion_DetectsSlowMotionState()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 0.5f, true, 2.0f);

            // Act
            bool isSlowMotion = speedData.IsSlowMotion();

            // Assert
            Assert.IsTrue(isSlowMotion, "Speed between 0 and 1.0 should be detected as slow motion");
        }

        [Test]
        public void SpeedOps_CalculateInterpolatedSpeed_WorksCorrectly()
        {
            // Arrange
            float currentSpeed = 1.0f;
            float targetSpeed = 2.0f;
            float interpolationSpeed = 1.0f;
            float deltaTime = 0.5f; // 50% interpolation

            // Act
            SpeedOps.CalculateInterpolatedSpeed(currentSpeed, targetSpeed, interpolationSpeed, deltaTime, out float newSpeed);

            // Assert
            Assert.AreEqual(1.5f, newSpeed, 0.001f, "Speed should interpolate 50% towards target");
        }

        [Test]
        public void SpeedOps_ClampSpeed_ClampsHighValues()
        {
            // Arrange
            float tooHighSpeed = 100.0f;
            float maxSpeed = 10.0f;
            float minSpeed = 0.0f;

            // Act
            SpeedOps.ClampSpeed(tooHighSpeed, minSpeed, maxSpeed, out float clampedSpeed);

            // Assert
            Assert.AreEqual(maxSpeed, clampedSpeed, 0.001f, "Speed should be clamped to maximum");
        }

        [Test]
        public void SpeedOps_ClampSpeed_ClampsLowValues()
        {
            // Arrange
            float negativeSpeed = -1.0f;
            float maxSpeed = 10.0f;
            float minSpeed = 0.0f;

            // Act
            SpeedOps.ClampSpeed(negativeSpeed, minSpeed, maxSpeed, out float clampedSpeed);

            // Assert
            Assert.AreEqual(minSpeed, clampedSpeed, 0.001f, "Speed should be clamped to minimum");
        }

        [Test]
        public void SpeedOps_IsValidSpeed_WorksCorrectly()
        {
            // Arrange
            float validSpeed = 1.5f;
            float invalidSpeedNaN = float.NaN;
            float invalidSpeedInf = float.PositiveInfinity;

            // Act
            bool isValid = SpeedOps.IsValidSpeed(validSpeed);
            bool isInvalidNaN = SpeedOps.IsValidSpeed(invalidSpeedNaN);
            bool isInvalidInf = SpeedOps.IsValidSpeed(invalidSpeedInf);

            // Assert
            Assert.IsTrue(isValid, "Valid speed should return true");
            Assert.IsFalse(isInvalidNaN, "NaN speed should return false");
            Assert.IsFalse(isInvalidInf, "Infinity speed should return false");
        }

        [Test]
        public void SpeedOps_CalculateEffectiveDeltaTime_WorksCorrectly()
        {
            // Arrange
            float deltaTime = 0.1f;
            float speedMultiplier = 2.0f;

            // Act
            SpeedOps.CalculateEffectiveDeltaTime(deltaTime, speedMultiplier, out float effectiveDeltaTime);

            // Assert
            Assert.AreEqual(0.2f, effectiveDeltaTime, 0.001f, "Effective delta time should be scaled by speed multiplier");
        }

        [Test]
        public void SpeedOps_ShouldApplyTimeDilation_WorksCorrectly()
        {
            // Arrange - Case 1: Enabled and speed != 1.0
            bool enabled1 = true;
            float speed1 = 2.0f;

            // Act
            bool shouldApply1 = SpeedOps.ShouldApplyTimeDilation(enabled1, speed1);

            // Assert
            Assert.IsTrue(shouldApply1, "Should apply time dilation when enabled and speed != 1.0");

            // Arrange - Case 2: Disabled
            bool enabled2 = false;
            float speed2 = 2.0f;

            // Act
            bool shouldApply2 = SpeedOps.ShouldApplyTimeDilation(enabled2, speed2);

            // Assert
            Assert.IsFalse(shouldApply2, "Should not apply time dilation when disabled");

            // Arrange - Case 3: Enabled but speed = 1.0
            bool enabled3 = true;
            float speed3 = 1.0f;

            // Act
            bool shouldApply3 = SpeedOps.ShouldApplyTimeDilation(enabled3, speed3);

            // Assert
            Assert.IsFalse(shouldApply3, "Should not apply time dilation when speed is approximately 1.0");
        }

        [Test]
        public void SpeedOps_CalculateDampedSpeed_WorksCorrectly()
        {
            // Arrange
            float currentSpeed = 1.0f;
            float targetSpeed = 2.0f;
            float dampingFactor = 1.0f;
            float deltaTime = 0.5f;

            // Act
            SpeedOps.CalculateDampedSpeed(currentSpeed, targetSpeed, dampingFactor, deltaTime, out float newSpeed);

            // Assert
            Assert.IsTrue(newSpeed > currentSpeed && newSpeed <= targetSpeed, "Speed should move towards target");
        }

        [Test]
        public void SpeedOps_IsPaused_WorksCorrectly()
        {
            // Arrange
            float pausedSpeed = 0.0f;
            float normalSpeed = 1.0f;
            float slowSpeed = 0.001f;

            // Act
            bool isPaused1 = SpeedOps.IsPaused(pausedSpeed);
            bool isPaused2 = SpeedOps.IsPaused(normalSpeed);
            bool isPaused3 = SpeedOps.IsPaused(slowSpeed);

            // Assert
            Assert.IsTrue(isPaused1, "Speed of 0 should be paused");
            Assert.IsFalse(isPaused2, "Speed of 1 should not be paused");
            Assert.IsFalse(isPaused3, "Speed of 0.001 should not be paused");
        }

        [Test]
        public void SpeedOps_IsReversed_WorksCorrectly()
        {
            // Arrange
            float reversedSpeed = -1.0f;
            float normalSpeed = 1.0f;

            // Act
            bool isReversed1 = SpeedOps.IsReversed(reversedSpeed);
            bool isReversed2 = SpeedOps.IsReversed(normalSpeed);

            // Assert
            Assert.IsTrue(isReversed1, "Negative speed should be reversed");
            Assert.IsFalse(isReversed2, "Positive speed should not be reversed");
        }

        [Test]
        public void SpeedOps_IsFastForward_WorksCorrectly()
        {
            // Arrange
            float fastSpeed = 2.0f;
            float normalSpeed = 1.0f;
            float slowSpeed = 0.5f;

            // Act
            bool isFast1 = SpeedOps.IsFastForward(fastSpeed);
            bool isFast2 = SpeedOps.IsFastForward(normalSpeed);
            bool isFast3 = SpeedOps.IsFastForward(slowSpeed);

            // Assert
            Assert.IsTrue(isFast1, "Speed > 1.0 should be fast forward");
            Assert.IsFalse(isFast2, "Speed of 1.0 should not be fast forward");
            Assert.IsFalse(isFast3, "Speed < 1.0 should not be fast forward");
        }

        [Test]
        public void SpeedOps_IsSlowMotion_WorksCorrectly()
        {
            // Arrange
            float slowSpeed = 0.5f;
            float normalSpeed = 1.0f;
            float fastSpeed = 2.0f;

            // Act
            bool isSlow1 = SpeedOps.IsSlowMotion(slowSpeed);
            bool isSlow2 = SpeedOps.IsSlowMotion(normalSpeed);
            bool isSlow3 = SpeedOps.IsSlowMotion(fastSpeed);

            // Assert
            Assert.IsTrue(isSlow1, "Speed between 0 and 1.0 should be slow motion");
            Assert.IsFalse(isSlow2, "Speed of 1.0 should not be slow motion");
            Assert.IsFalse(isSlow3, "Speed > 1.0 should not be slow motion");
        }

        [Test]
        public void SpeedOps_NormalizeSpeed_InvalidSpeed_ReturnsDefault()
        {
            // Arrange
            float invalidSpeed = float.NaN;

            // Act
            SpeedOps.NormalizeSpeed(invalidSpeed, out float normalizedSpeed);

            // Assert
            Assert.AreEqual(1.0f, normalizedSpeed, 0.001f, "Invalid speed should default to 1.0");
        }

        [Test]
        public void SpeedBehaviour_WithValidParameters_HasCorrectProperties()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 2.5f, true, 3.0f);

            // Act - Get the behaviour
            bool hasBehaviour = speedData.Node.TryGetBehaviour(out Day05SpeedBehaviour behaviour);

            // Assert
            Assert.IsTrue(hasBehaviour, "Should be able to get behaviour");
            Assert.IsNotNull(behaviour, "Behaviour should not be null");
            Assert.AreEqual(2.5f, behaviour.SpeedMultiplier, 0.001f, "Behaviour speed should match");
            Assert.IsTrue(behaviour.EnableTimeDilation, "Time dilation should be enabled");
            Assert.AreEqual(3.0f, behaviour.InterpolationSpeed, 0.001f, "Interpolation speed should match");
        }

        [Test]
        public void CompleteFlow_GraphOutputSpeedControlRotator_WorksCorrectly()
        {
            // Act - Create the complete chain
            graphHandle.Initialize("CompleteTestGraph");
            outputHandle.Initialize(in graphHandle.Graph, "CompleteTestOutput");
            rotatorData.Initialize(in graphHandle.Graph, "CompleteTestRotator", testGameObject.transform, 90.0f, Vector3.up);
            speedData.Initialize(in graphHandle.Graph, "CompleteTestSpeedControl", 2.0f, true, 2.0f);

            // Connect speed control to output
            speedData.ConnectToOutput(in outputHandle.Output);

            // Assert - Verify all components are valid
            Assert.IsTrue(graphHandle.IsActive, "Graph should be active");
            Assert.IsTrue(outputHandle.IsValidOutput(), "Output should be valid");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.IsTrue(speedData.IsValidSpeedControl(), "Speed control should be valid");

            // Verify the connection
            Playable source = outputHandle.Output.GetSourcePlayable();
            Assert.IsTrue(source.IsValid(), "Output should have a valid source");
        }

        [Test]
        public void SpeedData_GetCurrentSpeed_ReturnsCorrectValue()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 1.5f, true, 2.0f);

            // Act
            float currentSpeed = speedData.GetCurrentSpeed();

            // Assert
            Assert.AreEqual(1.5f, currentSpeed, 0.001f, "Current speed should match initial speed");
        }

        [Test]
        public void SpeedData_WithInvalidGraph_DoesNotInitialize()
        {
            // Arrange - Invalid graph
            PlayableGraph invalidGraph = default;

            // Act
            speedData.Initialize(in invalidGraph, "TestSpeedControl", 1.0f, true, 2.0f);

            // Assert
            Assert.IsFalse(speedData.IsActive, "Speed control should not be active with invalid graph");
        }

        [Test]
        public void SpeedData_LogSpeedInfo_LogsCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            speedData.Initialize(in graphHandle.Graph, "TestSpeedControl", 0.5f, true, 2.0f);

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() => speedData.LogSpeedInfo("TestControl"), "LogSpeedInfo should not throw");
        }
    }
}
