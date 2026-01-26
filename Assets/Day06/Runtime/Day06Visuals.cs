using UnityEngine;

namespace Day06
{
    // The "Target" that holds the state
    public class Day06Visuals : MonoBehaviour
    {
        [Header("Visual Properties")]
        public Color glowColor = Color.cyan;
        public float glowIntensity = 1.0f;
        public Vector3 localOffset = Vector3.zero;
        public float spinSpeed = 0f;

        // Visual debug to see changes in Scene view without checking Inspector
        private void OnDrawGizmos()
        {
            Gizmos.color = glowColor;
            Gizmos.DrawWireSphere(transform.position + localOffset, 0.5f * glowIntensity);
        }
    }
}