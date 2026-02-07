using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day07.Runtime
{
    public class Day07SeekButtons : MonoBehaviour
    {
         [SerializeField] PlayableDirector director;
         
        void OnValidate() => director = GetComponent<PlayableDirector>();
        

        [ContextMenu(nameof(SeekToStart))]
        public void SeekToStart()
        {
            director.Pause();
            director.time = 0.0;
            director.Evaluate();
        }

        [ContextMenu(nameof(SeekToMiddle))]
        public void SeekToMiddle()
        {
            director.Pause();
            director.time = director.duration * 0.5;
            director.Evaluate();
        }

        [ContextMenu(nameof(SeekToEnd))]
        public void SeekToEnd()
        {
            director.Pause();
            director.time = director.duration;
            director.Evaluate();
        }
    }
}
