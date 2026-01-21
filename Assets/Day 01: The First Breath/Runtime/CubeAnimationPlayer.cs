using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// The Result: Your MonoBehaviour.
    /// Demonstrates the simplest possible PlayableGraph: playing a single animation clip.
    /// 
    /// SETUP INSTRUCTIONS:
    /// 1. Create a Cube in your scene (GameObject -> 3D Object -> Cube)
    /// 2. Add an Animator component to the Cube
    /// 3. Attach this script to the Cube
    /// 4. Create or import any AnimationClip (e.g., a simple rotation animation)
    /// 5. Assign the clip to the 'Clip To Play' field in the Inspector
    /// 6. Press Play
    /// 
    /// THE PAYOFF: The cube will play the animation using ONLY PlayableGraph (no Mecanim state machine)
    /// </summary>
    public sealed class CubeAnimationPlayer : MonoBehaviour
    {
        [Header("Day 01: The First Breath")]
        [Tooltip("The animation clip to play on this cube")]
        public AnimationClip ClipToPlay;

        [Tooltip("Playback speed multiplier (1.0 = normal speed)")]
        [Range(0.1f, 3.0f)]
        public float PlaybackSpeed = 1.0f;

        [Header("Debug Info")]
        [SerializeField] private float _elapsedTime;
        [SerializeField] private bool _isPlaying;

        private PlayableGraph _graph;
        private SimpleClipPlayerBehaviour _playerBehaviour;

        void Start()
        {
            if (ClipToPlay == null)
            {
                Debug.LogError("[Day 01] No animation clip assigned! Please assign a clip to play.", this);
                return;
            }

            if (TryGetComponent(out Animator animator) == false)
            {
                Debug.LogError("[Day 01] No Animator component found! Adding one automatically.", this);
                animator = gameObject.AddComponent<Animator>();
            }

            // Prepare the data
            var clipData = new AnimationClipData
            {
                Clip = ClipToPlay,
                Speed = PlaybackSpeed
            };

            // Create and build the graph
            _graph = PlayableGraph.Create($"Day01_Cube_{gameObject.name}");

            ScriptPlayable<SimpleClipPlayerBehaviour> behaviourNode;
            _graph.CreateClipPlayerGraph(clipData, animator, out behaviourNode);

            _playerBehaviour = behaviourNode.GetBehaviour();

            // Start the graph
            _graph.Play();

            Debug.Log($"[Day 01] Graph created and playing on '{gameObject.name}'. " +
                     $"Clip: '{ClipToPlay.name}', Speed: {PlaybackSpeed}x");
        }

        void Update()
        {
            if (_playerBehaviour != null)
            {
                _elapsedTime = _playerBehaviour.GetElapsedTime();
                _isPlaying = _playerBehaviour.IsPlaying();
            }
        }

        void OnDestroy()
        {
            // CRITICAL: Always destroy the graph to prevent memory leaks
            if (_graph.IsValid())
            {
                _graph.Destroy();
                Debug.Log($"[Day 01] Graph destroyed for '{gameObject.name}'.");
            }
        }

        /// <summary>
        /// Visual debug display in the Scene view
        /// </summary>
        void OnDrawGizmos()
        {
            if (_isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.5f, 0.2f);
                
#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * 2f, 
                    $"Day 01: Playing\nTime: {_elapsedTime:F2}s"
                );
#endif
            }
        }
    }
}
