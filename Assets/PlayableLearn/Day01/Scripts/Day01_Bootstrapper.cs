using UnityEngine;
using PlayableLearn.Core; // Uses our 200 IQ Core

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Called by Curriculum Hub via Reflection to initialize Day 01.
    /// This bridges the Core and the Lesson.
    /// </summary>
    public static class Day01_Bootstrapper
    {
        // Called by Curriculum Hub
        public static void Initialize()
        {
            // 1. Spawn the standard Actor from Core
            var actor = StageBuilder.SpawnActor("Day01_GraphRunner");

            // 2. Attach the Lesson Component
            var lifecycle = actor.AddComponent<Day01_Lifecycle>();

            // 3. Auto-Setup the Animator if missing
            if (!actor.TryGetComponent<Animator>(out _))
                actor.AddComponent<Animator>();

            Debug.Log("Day 01 Initialized. Check the 'Day01_GraphRunner' object.");
        }
    }
}
