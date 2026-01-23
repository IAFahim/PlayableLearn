using AV.Day01.Runtime;
using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day02.Runtime
{
    /// <summary>
    /// Day 02: Creating a Graph Manually.
    /// This removes the "Timeline" abstraction and shows the raw engine underneath.
    /// </summary>
    public class Day02ManualGraph : MonoBehaviour
    {
        [Range(0f, 5f)]
        public float TimeScale = 1f;

        private PlayableGraph _graph;
        private ScriptPlayable<Day01LoggerPlayableBehaviour> _loggerPlayable;

        private void Start()
        {
            // 1. The Container: Create the Graph
            _graph = PlayableGraph.Create("Day02_ManualGraph");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // 2. The Node: Create the Playable
            _loggerPlayable = ScriptPlayable<Day01LoggerPlayableBehaviour>.Create(_graph);
            
            // 3. The Output: Create the Endpoint
            var output = ScriptPlayableOutput.Create(_graph, "LoggerOutput");
            output.SetSourcePlayable(_loggerPlayable);

            // 4. Play
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
            if (_graph.IsValid())
            {
                _graph.Destroy();
                Debug.Log("<color=cyan><b>[Day 02]</b> Manual Graph Destroyed.</color>");
            }
        }
    }
}
