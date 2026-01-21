using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// The Main Component. The only thing you put on the object.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Day01_Lifecycle : MonoBehaviour
    {
        [Header("Layer A: Data")]
        public AnimationConfig Config;

        [Header("Debug View")]
        [SerializeField] private float _runtimeTimer;

        private PlayableGraph _graph;
        private ScriptPlayable<PlaybackMonitorLogic> _logicNode;

        void Start()
        {
            // 0. Safety: Generate a clip if the user was lazy (Magic!)
            if (Config.Clip == null)
            {
                Debug.LogWarning("No Clip assigned! Generating a procedural spin.");
                Config.Clip = GenerateProceduralClip();
                Config.Speed = 1.0f;
            }

            // 1. Create the Graph Container
            _graph = PlayableGraph.Create($"Day01_{gameObject.name}");

            // 2. Build the Graph (Using our Layer C Adapter)
            // We pass 'this.Config' (Data) and get back the Logic Node
            _logicNode = _graph.CreateSimpleGraph(Config, GetComponent<Animator>());

            // 3. Press Play
            _graph.Play();
        }

        void Update()
        {
            // Read data from the Logic Layer
            if (_logicNode.IsValid())
            {
                _runtimeTimer = _logicNode.GetBehaviour().GetTotalRuntime();
            }
        }

        void OnDestroy()
        {
            // MANDATORY: Clean up memory
            if (_graph.IsValid()) _graph.Destroy();
        }

        // Helper to make the lesson "Just Work"
        private AnimationClip GenerateProceduralClip()
        {
            AnimationClip clip = new AnimationClip { name = "ProceduralSpin" };
            clip.legacy = false;

            // Create a curve for Y rotation 0 -> 360
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 2.0f, 360f);
            clip.SetCurve("", typeof(Transform), "localEulerAngles.y", curve);
            clip.wrapMode = WrapMode.Loop;

            return clip;
        }
    }
}
