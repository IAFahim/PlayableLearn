using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Mathematics;
using PlayableLearn.Day01;
using PlayableLearn.Day02;
using PlayableLearn.Day03;
using PlayableLearn.Day04;

namespace PlayableLearn.Day04.Tests
{
    public class Day04Tests
    {
        private Day01GraphHandle graphHandle;
        private Day02OutputHandle outputHandle;
        private Day04RotatorData rotatorData;
        private GameObject testGameObject;

        [SetUp]
        public void SetUp()
        {
            graphHandle = new Day01GraphHandle();
            outputHandle = new Day02OutputHandle();
            rotatorData = new Day04RotatorData();
            testGameObject = new GameObject("TestCube");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up in reverse order
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
        public void RotatorData_InitializesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            Transform targetTransform = testGameObject.transform;
            float rotationSpeed = 90.0f;
            Vector3 rotationAxis = Vector3.up;

            // Act
            rotatorData.Initialize(in graphHandle.Graph, "TestRotator", targetTransform, rotationSpeed, rotationAxis);

            // Assert
            Assert.IsTrue(rotatorData.IsActive, "Rotator should be active after initialization");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");
            Assert.AreEqual(targetTransform, rotatorData.TargetTransform, "Target transform should match");

            // Note: Rotation speed and axis are now stored in the Behaviour, not the Data struct
            if (rotatorData.Node.TryGetBehaviour(out Day04RotatorBehaviour behaviour))
            {
                Assert.AreEqual(rotationSpeed, behaviour.RotationSpeed, 0.001f, "Rotation speed should match");
                Assert.AreEqual(rotationAxis, behaviour.RotationAxis, "Rotation axis should match");
            }
            else
            {
                Assert.Fail("Behaviour should be available");
            }
        }

        [Test]
        public void RotatorData_ConnectsToOutputCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            outputHandle.Initialize(in graphHandle.Graph, "TestOutput");
            rotatorData.Initialize(in graphHandle.Graph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);

            // Act
            rotatorData.ConnectToOutput(in outputHandle.Output);

            // Assert
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should still be valid after connection");
            Assert.IsTrue(outputHandle.IsValidOutput(), "Output should still be valid after connection");

            // Verify the connection
            Playable source = outputHandle.Output.GetSourcePlayable();
            Assert.IsTrue(source.IsValid(), "Output source should be valid");
            Assert.AreEqual(rotatorData.Node, source, "Output source should be the connected rotator");
        }

        [Test]
        public void RotatorLogic_CalculateRotation_WorksCorrectly()
        {
            // Arrange
            float3 currentEuler = new float3(0, 0, 0);
            float3 axis = new float3(0, 1, 0);
            float speed = 90.0f; // 90 degrees per second
            float deltaTime = 1.0f; // 1 second

            // Act
            RotatorLogic.CalculateRotation(in currentEuler, in axis, speed, deltaTime, out float3 newEuler);

            // Assert
            Assert.AreEqual(0, newEuler.x, 0.001f, "X rotation should not change");
            Assert.AreEqual(90.0f, newEuler.y, 0.001f, "Y rotation should increase by 90 degrees");
            Assert.AreEqual(0, newEuler.z, 0.001f, "Z rotation should not change");
        }

        [Test]
        public void RotatorLogic_CalculateRotation_WithDifferentAxis_WorksCorrectly()
        {
            // Arrange
            float3 currentEuler = new float3(0, 0, 0);
            float3 axis = new float3(1, 0, 0); // X axis
            float speed = 45.0f;
            float deltaTime = 2.0f; // 2 seconds

            // Act
            RotatorLogic.CalculateRotation(in currentEuler, in axis, speed, deltaTime, out float3 newEuler);

            // Assert
            Assert.AreEqual(90.0f, newEuler.x, 0.001f, "X rotation should increase by 90 degrees (45 * 2)");
            Assert.AreEqual(0, newEuler.y, 0.001f, "Y rotation should not change");
            Assert.AreEqual(0, newEuler.z, 0.001f, "Z rotation should not change");
        }

        [Test]
        public void RotatorLogic_NormalizeAxis_WorksCorrectly()
        {
            // Arrange
            float3 nonNormalizedAxis = new float3(0, 5, 0); // Not unit length

            // Act
            RotatorLogic.NormalizeAxis(in nonNormalizedAxis, out float3 normalizedAxis);

            // Assert
            Assert.AreEqual(0, normalizedAxis.x, 0.001f, "X component should be 0");
            Assert.AreEqual(1, normalizedAxis.y, 0.001f, "Y component should be 1 (normalized)");
            Assert.AreEqual(0, normalizedAxis.z, 0.001f, "Z component should be 0");
        }

        [Test]
        public void RotatorLogic_Vector3ToFloat3_WorksCorrectly()
        {
            // Arrange
            Vector3 vector3 = new Vector3(1, 2, 3);

            // Act
            RotatorLogic.Vector3ToFloat3(in vector3, out float3 float3Value);

            // Assert
            Assert.AreEqual(1, float3Value.x, 0.001f, "X component should match");
            Assert.AreEqual(2, float3Value.y, 0.001f, "Y component should match");
            Assert.AreEqual(3, float3Value.z, 0.001f, "Z component should match");
        }

        [Test]
        public void RotatorLogic_Float3ToVector3_WorksCorrectly()
        {
            // Arrange
            float3 float3Value = new float3(4, 5, 6);

            // Act
            RotatorLogic.Float3ToVector3(in float3Value, out Vector3 vector3);

            // Assert
            Assert.AreEqual(4, vector3.x, 0.001f, "X component should match");
            Assert.AreEqual(5, vector3.y, 0.001f, "Y component should match");
            Assert.AreEqual(6, vector3.z, 0.001f, "Z component should match");
        }

        [Test]
        public void RotatorLogic_ClampRotationSpeed_WorksCorrectly()
        {
            // Arrange
            float tooHighSpeed = 1000.0f;
            float maxSpeed = 360.0f;
            float minSpeed = 0.0f;

            // Act
            RotatorLogic.ClampRotationSpeed(tooHighSpeed, minSpeed, maxSpeed, out float clampedSpeed);

            // Assert
            Assert.AreEqual(maxSpeed, clampedSpeed, 0.001f, "Speed should be clamped to maximum");
        }

        [Test]
        public void RotatorLogic_IsValidRotationSpeed_WorksCorrectly()
        {
            // Arrange
            float validSpeed = 90.0f;
            float invalidSpeed = float.NaN;

            // Act
            bool isValid = RotatorLogic.IsValidRotationSpeed(validSpeed);
            bool isInvalid = RotatorLogic.IsValidRotationSpeed(invalidSpeed);

            // Assert
            Assert.IsTrue(isValid, "Valid speed should return true");
            Assert.IsFalse(isInvalid, "NaN speed should return false");
        }

        [Test]
        public void RotatorData_SetRotationSpeed_UpdatesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            rotatorData.Initialize(in graphHandle.Graph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);
            float newSpeed = 180.0f;

            // Act
            rotatorData.SetRotationSpeed(newSpeed);

            // Assert
            // Note: Rotation speed is now stored in the Behaviour, not the Data struct
            if (rotatorData.Node.TryGetBehaviour(out Day04RotatorBehaviour behaviour))
            {
                Assert.AreEqual(newSpeed, behaviour.RotationSpeed, 0.001f, "Rotation speed should be updated");
            }
            else
            {
                Assert.Fail("Behaviour should be available");
            }
        }

        [Test]
        public void RotatorData_SetRotationAxis_UpdatesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            rotatorData.Initialize(in graphHandle.Graph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);
            Vector3 newAxis = Vector3.right;

            // Act
            rotatorData.SetRotationAxis(newAxis);

            // Assert
            // Note: Rotation axis is now stored in the Behaviour, not the Data struct
            if (rotatorData.Node.TryGetBehaviour(out Day04RotatorBehaviour behaviour))
            {
                Assert.AreEqual(newAxis, behaviour.RotationAxis, "Rotation axis should be updated");
            }
            else
            {
                Assert.Fail("Behaviour should be available");
            }
        }

        [Test]
        public void RotatorData_DisposesCorrectly()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            rotatorData.Initialize(in graphHandle.Graph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);

            // Act
            rotatorData.Dispose();

            // Assert
            Assert.IsFalse(rotatorData.IsActive, "Rotator should not be active after disposal");
            Assert.IsNull(rotatorData.TargetTransform, "Target transform should be null after disposal");
        }

        [Test]
        public void RotatorData_WithInvalidGraph_DoesNotInitialize()
        {
            // Arrange - Invalid graph
            PlayableGraph invalidGraph = default;

            // Act
            rotatorData.Initialize(in invalidGraph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);

            // Assert
            Assert.IsFalse(rotatorData.IsActive, "Rotator should not be active with invalid graph");
        }

        [Test]
        public void RotatorData_WithNullTransform_DoesInitialize()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");

            // Act - Initialize with null transform (should be allowed)
            rotatorData.Initialize(in graphHandle.Graph, "TestRotator", null, 90.0f, Vector3.up);

            // Assert - Rotator should still initialize, but just won't rotate anything
            Assert.IsTrue(rotatorData.IsActive, "Rotator should be active even with null transform");
            Assert.IsNull(rotatorData.TargetTransform, "Target transform should be null");
        }

        [Test]
        public void CompleteFlow_GraphOutputRotator_WorksCorrectly()
        {
            // Act - Create the complete chain
            graphHandle.Initialize("CompleteTestGraph");
            outputHandle.Initialize(in graphHandle.Graph, "CompleteTestOutput");
            rotatorData.Initialize(in graphHandle.Graph, "CompleteTestRotator", testGameObject.transform, 90.0f, Vector3.up);
            rotatorData.ConnectToOutput(in outputHandle.Output);

            // Assert - Verify all components are valid
            Assert.IsTrue(graphHandle.IsActive, "Graph should be active");
            Assert.IsTrue(outputHandle.IsValidOutput(), "Output should be valid");
            Assert.IsTrue(rotatorData.IsValidRotator(), "Rotator should be valid");

            // Verify the connection
            Playable source = outputHandle.Output.GetSourcePlayable();
            Assert.IsTrue(source.IsValid(), "Output should have a valid source");
        }

        [Test]
        public void RotatorBehaviour_WithValidTarget_HasCorrectProperties()
        {
            // Arrange
            graphHandle.Initialize("TestGraph");
            rotatorData.Initialize(in graphHandle.Graph, "TestRotator", testGameObject.transform, 90.0f, Vector3.up);

            // Act - Get the behaviour
            bool hasBehaviour = rotatorData.Node.TryGetBehaviour(out Day04RotatorBehaviour behaviour);

            // Assert
            Assert.IsTrue(hasBehaviour, "Should be able to get behaviour");
            Assert.IsNotNull(behaviour, "Behaviour should not be null");
            Assert.AreEqual(testGameObject.transform, behaviour.TargetTransform, "Behaviour target should match");
            Assert.AreEqual(90.0f, behaviour.RotationSpeed, 0.001f, "Behaviour speed should match");
            Assert.AreEqual(Vector3.up, behaviour.RotationAxis, "Behaviour axis should match");
        }
    }
}
