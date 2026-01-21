using UnityEngine;
using System;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Interactive documentation for Day 1: The First Breath.
    /// This ScriptableObject stores the lesson data displayed in a custom Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "Day01_Readme", menuName = "PlayableLearn/Day 01 Readme", order = 1)]
    public sealed class Day01Readme : ScriptableObject
    {
        [Header("Visual Assets")]
        public Texture2D bannerIcon;
        public Color bannerColor = new Color(0.2f, 0.4f, 0.8f);

        [Header("Lesson Info")]
        public string lessonTitle = "Day 01: The First Breath";
        public string subtitle = "Welcome to the Playables API Curriculum";
        [TextArea(3, 10)]
        public string overview = "Create your first PlayableGraph programmatically and play an animation on a cube without Mecanim.";

        [Header("Lesson Details")]
        public Section[] sections;

        [Header("Quick Links")]
        public LinkButton[] quickLinks;

        [Serializable]
        public class Section
        {
            public string heading;
            [TextArea(5, 20)]
            public string content;
            public bool isCollapsible;
            public bool startExpanded = true;
        }

        [Serializable]
        public class LinkButton
        {
            public string buttonText;
            public string url;
            public bool isPrimary;
        }
    }
}
