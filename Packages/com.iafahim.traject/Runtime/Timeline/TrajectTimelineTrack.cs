using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AV.Traject.Runtime.Timeline
{
    [TrackClipType(typeof(TrajectTimelineClip))]
    [TrackBindingType(typeof(Transform))]
    public class TrajectTimelineTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TrajectTimelineMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            var component = director.GetGenericBinding(this) as Transform;
            if (component == null) return;

            var so = new SerializedObject(component);
            var iterator = so.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (iterator.hasVisibleChildren) continue;

                driver.AddFromName<Transform>(component.gameObject, iterator.propertyPath);
            }
#endif
            base.GatherProperties(director, driver);
        }
    }
}
