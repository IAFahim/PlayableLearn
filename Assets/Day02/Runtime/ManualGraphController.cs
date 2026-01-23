using AV.Day01.Runtime;
using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day02.Runtime
{
    /// <summary>
    /// Day 02: Creating a Graph Manually.
    /// This removes the "Timeline" abstraction and shows the raw engine underneath.
    /// </summary>
    public class ManualGraphController : MonoBehaviour
    {
        [Range(0f, 5f)]
        public float TimeScale = 1f;

        private PlayableGraph _graph;
        private ScriptPlayable<Day01LoggerPlayableBehaviour> _loggerPlayable;

        private void Start()
        {
            // 1. The Container: Create the Graph
            // The Graph is the lifecycle owner. It updates all playables attached to it.
            _graph = PlayableGraph.Create("Day02_ManualGraph");
            
            // Set the update mode (GameTime is standard for animations/gameplay)
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // 2. The Node: Create the Playable
            // We reuse our Day01 logger to verify the lifecycle works the same way.
            _loggerPlayable = ScriptPlayable<Day01LoggerPlayableBehaviour>.Create(_graph);
            
            // Optional: Configure the behaviour directly
            var behaviour = _loggerPlayable.GetBehaviour();
            // (If we had properties to set on the behaviour, we'd do it here)

            // 3. The Output: Create the Endpoint
            // A graph needs an Output to be considered "valid" for processing in many cases,
            // or we just connect it to the graph root.
            var output = ScriptPlayableOutput.Create(_graph, "LoggerOutput");
            
            // Connect the Playable to the Output
            output.SetSourcePlayable(_loggerPlayable);

            // 4. The Switch: Play the Graph
            _graph.Play();
            
            Debug.Log("<color=cyan><b>[Day 02]</b> Manual Graph Created & Playing.</color>");
        }

        private void Update()
        {
            if (_loggerPlayable.IsValid())
            {
                _loggerPlayable.SetSpeed(TimeScale);
            }
        }

        private void OnDestroy()
        {
            // 5. Cleanup: ALWAYS destroy manual graphs.
            // Unlike GameObjects, Graphs are not automatically collected when the scene changes 
            // if they aren't attached to a destroyed object properly (though PlayableGraph.Create usually links to the global list).
            // Explicit destruction is best practice.
            if (_graph.IsValid())
            {
                _graph.Destroy();
                Debug.Log("<color=cyan><b>[Day 02]</b> Manual Graph Destroyed.</color>");
            }
        }
    }
}
