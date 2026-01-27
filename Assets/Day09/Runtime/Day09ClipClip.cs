using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day09.Runtime
{
    [Serializable]
    public class Day09ClipClip : PlayableBehaviour
    {
        public string logStr;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Debug.Log(logStr);
        }
    }
    
}
