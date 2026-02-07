using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day07.Runtime
{
    public class Day07SpeedControl : MonoBehaviour
    {
        [SerializeField] private float speedMultiplier = 1.0f;
        private PlayableDirector director;
        private Playable rootPlayable;

        void Awake() => director = GetComponent<PlayableDirector>();

        void OnEnable()
        {
            director.Play();
            rootPlayable = director.playableGraph.GetRootPlayable(0);
            UpdateSpeed();
        }

        void UpdateSpeed()
        {
            if (IsValid())
                rootPlayable.SetSpeed(speedMultiplier);
        }

        public void SetSpeed(double speed)
        {
            if (!IsValid()) return;
            rootPlayable.SetSpeed(speed);
        }

        public void SlowMotion()
        {
            if (!IsValid()) return;
            rootPlayable.SetSpeed(0.5);
        }

        public void FastForward()
        {
            if (!IsValid()) return;
            rootPlayable.SetSpeed(2.0);
        }

        public void Reverse()
        {
            if (!IsValid()) return;
            rootPlayable.SetSpeed(-1.0);
        }

        bool IsValid() => rootPlayable.IsValid();
    }
}
