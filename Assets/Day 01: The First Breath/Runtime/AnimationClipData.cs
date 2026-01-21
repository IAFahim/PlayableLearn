using System;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Layer A: Pure Data.
    /// The minimal data required to play an animation clip.
    /// </summary>
    [Serializable]
    public struct AnimationClipData
    {
        public UnityEngine.AnimationClip Clip;
        public float Speed;
    }
}
