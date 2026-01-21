using System;
using UnityEngine;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Layer A: Pure Data
    /// "What do we need to play a clip?"
    /// </summary>
    [Serializable]
    public struct AnimationConfig
    {
        public AnimationClip Clip;
        [Range(0, 5)] public float Speed;
    }
}
