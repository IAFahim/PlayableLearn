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
        // Level 1: Just a string field exposed to the editor
        public string notificationName;
        public bool showDebugLog = true;

        // INotification Implementation
        public PropertyName id => new PropertyName(notificationName);
    }
}