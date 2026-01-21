using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// The MonoBehaviour is JUST the entry point.
    /// It holds the Data (Layer A) and calls the Extensions (Layer C).
    /// </summary>
    public class Day01Entry : MonoBehaviour
    {
        // 1. DATA
        [SerializeField]
        private Day01GraphLifecycleState _graphState;

        // 2. INITIALIZATION
        private void OnEnable()
        {
            _graphState.InitializeEngine(gameObject.name);
            _graphState.Play();

            Debug.Log($"[Day 01] Graph Created: {_graphState.MainGraphHandle.GetEditorName()}");
        }

        // 3. CLEANUP (Crucial!)
        private void OnDisable()
        {
            _graphState.DisposeEngine();
            Debug.Log("[Day 01] Graph Destroyed");
        }

        // 4. VISUALIZATION
        private void Update()
        {
            if (_graphState.IsCreated && _graphState.MainGraphHandle.IsValid())
            {
                // This proves the graph is alive and updating its own time
                // Don't call this variable "t". Call it what it is.
                double accumulatedTime = _graphState.MainGraphHandle.GetTimeUpdateMode() == DirectorUpdateMode.GameTime
                    ? Time.time
                    : _graphState.MainGraphHandle.GetRootPlayable(0).GetTime(); // (Won't work yet as no root, but concept stands)

                // Simple oscillation to show activity in Inspector
                transform.localScale = Vector3.one * (1.0f + Mathf.Sin((float)Time.time * 2f) * 0.2f);
            }
        }
    }
}
