using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class HealthClip : PlayableAsset, ITimelineClipAsset
{
    public float StartHealth;
    public float EndHealth;

    public ClipCaps clipCaps => ClipCaps.Blending; // Allow blending

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<HealthBehaviour>.Create(graph); // Create logic container
        var behaviour = playable.GetBehaviour(); // Access logic
        
        behaviour.StartHealth = StartHealth; // Inject data
        behaviour.EndHealth = EndHealth; // Inject data
        
        return playable; // Return handle
    }
}