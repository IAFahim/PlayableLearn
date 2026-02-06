using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AV.Day04.Runtime
{
    [CustomStyle("BookmarkRed")]
    [Serializable]
    public class Day04NotificationMarker : Marker, INotification
    {
        public string notificationName;
        public PropertyName id => new(notificationName);
    }
}