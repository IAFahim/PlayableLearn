using UnityEngine;
using UnityEngine.Playables;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// The Main Component - The only thing you put on the object.
    /// Now with procedural animation generation and visual debugging.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Day01_Lifecycle : MonoBehaviour
    {
        private PlayableGraph _graph;

        void Start()
        {
            // 1. Procedural Animation Clip (No assets needed!)
            var clip = CreateProceduralClip();

            // 2. Build Graph
            _graph = PlayableGraph.Create("Day01_Graph");
            _graph.CreateLessonGraph(clip, GetComponent<Animator>());

            // 3. Play
            _graph.Play();
        }

        void OnDestroy()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }

        /// <summary>
        /// Generates a simple "Floating" animation code-side.
        /// No AnimationClip asset required - pure procedural magic.
        /// </summary>
        private AnimationClip CreateProceduralClip()
        {
            var clip = new AnimationClip { name = "CodeGeneratedFloat" };
            clip.legacy = false;

            // Animate Y position with a Sine wave approximation
            var curve = new AnimationCurve();
            for(float t=0; t<=2.0f; t+=0.1f)
            {
                curve.AddKey(t, Mathf.Sin(t * Mathf.PI) * 1.0f);
            }

            clip.SetCurve("", typeof(Transform), "localPosition.y", curve);
            clip.wrapMode = WrapMode.Loop;
            return clip;
        }

        /// <summary>
        /// 200 IQ Visual Debugging - Shows graph status in-game.
        /// </summary>
        void OnGUI()
        {
            GUI.color = Color.green;
            GUILayout.BeginArea(new Rect(20, 20, 300, 100));
            GUILayout.Label("<b>DAY 01: GRAPH ACTIVE</b>");
            GUILayout.Label($"Graph Valid: {_graph.IsValid()}");
            GUILayout.Label($"Output Count: {_graph.GetOutputCount()}");
            GUILayout.EndArea();
        }
    }
}
