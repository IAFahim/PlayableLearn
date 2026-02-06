using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day04.Runtime
{
    public class Day04NotificationReceiver : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is not Day04NotificationMarker marker) return;
            Debug.Log($"Marker received: {marker.notificationName}");
        }
    }
}