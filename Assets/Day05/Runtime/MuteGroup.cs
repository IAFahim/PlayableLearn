using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MuteGroup : MonoBehaviour {
    public PlayableDirector director;
    public string groupName = "MyGroup";

    [ContextMenu(nameof(Mute))]
    void Mute() {
        TimelineAsset timeline = (TimelineAsset)director.playableAsset;
        foreach (TrackAsset track in timeline.GetRootTracks()) {
            if (track.name == groupName && track is GroupTrack) {
                track.muted = true;
                break;
            }
        }
        director.RebuildGraph();
    }
}