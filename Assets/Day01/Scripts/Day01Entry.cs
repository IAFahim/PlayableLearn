using UnityEngine;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// The MonoBehaviour is JUST the entry point.
    /// It holds the Data (Layer A) and calls the Extensions (Layer C).
    /// </summary>
    public class Day01Entry : MonoBehaviour
    {
        // LAYER A: Data
        [SerializeField]
        private Day01GraphHandle day01GraphHandle;

        // LAYER C: Adapter Calls
        private void OnEnable()
        {
            var handle = day01GraphHandle;
            handle.Initialize(gameObject.name);
            day01GraphHandle = handle;
            Debug.Log($"[Day 01] Engine Started. Graph Valid: {day01GraphHandle.IsActive}");
        }

        private void OnDisable()
        {
            var handle = day01GraphHandle;
            handle.Dispose();
            day01GraphHandle = handle;
            Debug.Log("[Day 01] Engine Stopped.");
        }

        private void Update()
        {
            // PURE VIEW LOGIC
            // We ask the handle for data. We do not touch the inner graph.
            if (day01GraphHandle.TryGetTime(out float graphTime))
            {
                // Visual feedback: Pulse size based on Graph Time
                float scale = 1.0f + Mathf.Sin(graphTime * 3.0f) * 0.25f;
                transform.localScale = Vector3.one * scale;
            }
        }
    }
}
