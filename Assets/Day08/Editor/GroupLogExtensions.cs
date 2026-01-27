using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine.Timeline;

namespace Day08.Editor
{
    public static class GroupLogExtensions
    {
        public static bool TryGenerate(ref this GroupLogState state, TimelineAsset timeline, out string result)
        {
            result = string.Empty;
            if (timeline == null) return false;

            state.AssetPath = AssetDatabase.GetAssetPath(timeline);
            var sbPuml = new StringBuilder();
            
            // Settings
            double fps = timeline.editorSettings.frameRate;
            int trackCount = 0;

            // 1. Gather All Tracks (Recursive)
            var allTracks = new List<TrackAsset>();
            for (int i = 0; i < timeline.rootTrackCount; i++)
            {
                CollectAllTracks(timeline.GetRootTrack(i), allTracks);
            }

            // Dictionary to map Track -> Alias
            var trackAliasMap = new Dictionary<TrackAsset, string>();

            // 2. Define Participants (The Header)
            // We only list tracks that actually have clips
            foreach (var track in allTracks)
            {
                var clips = track.GetClips();
                if (clips != null && clips.Any())
                {
                    trackCount++;
                    string alias = $"T{trackCount:D2}";
                    trackAliasMap[track] = alias;

                    GroupLogLogic.FormatTrackLine(track.name, alias, out var line);
                    sbPuml.AppendLine(line);
                }
            }
            state.GroupCount = trackCount;

            if (trackCount == 0) return false;

            // 3. Define Global Duration Bounds
            sbPuml.AppendLine();
            sbPuml.AppendLine("@0");
            
            double duration = timeline.duration;
            int totalFrames = (duration > 0) ? (int)(duration * fps) : 100;
            sbPuml.AppendLine($"@{totalFrames}");

            // 4. Generate Events Per Track
            foreach (var kvp in trackAliasMap)
            {
                TrackAsset track = kvp.Key;
                string alias = kvp.Value;

                // Switch context to this track
                sbPuml.AppendLine();
                sbPuml.AppendLine($"@{alias}");

                // Sort clips by start time to ensure linear processing
                var sortedClips = track.GetClips().OrderBy(c => c.start).ToList();
                int prevEndFrame = 0;
                bool isFirst = true;

                foreach (var clip in sortedClips)
                {
                    int startFrame = (int)(clip.start * fps);
                    int endFrame = (int)(clip.end * fps);
                    string clipName = clip.displayName.Replace("\"", "'"); // Sanitize quotes

                    // Logic to handle Gaps vs Overlaps
                    if (!isFirst)
                    {
                        if (startFrame < prevEndFrame)
                        {
                            // Overlap detected: Add Blend arrow
                            // From Start of this clip <-> End of previous clip
                            sbPuml.AppendLine($"@{startFrame} <-> @{prevEndFrame} : Blend");
                        }
                        else if (startFrame > prevEndFrame)
                        {
                            // Gap detected: explicitly close previous clip
                            sbPuml.AppendLine($"{prevEndFrame} is {{hidden}}");
                        }
                    }

                    // Define the Clip State
                    sbPuml.AppendLine($"{startFrame} is \"{clipName}\"");

                    prevEndFrame = endFrame;
                    isFirst = false;
                }

                // Always close the last clip of the track so the bar doesn't extend forever
                sbPuml.AppendLine($"{prevEndFrame} is {{hidden}}");
            }

            // 5. Finalize
            GroupLogLogic.FormatPumlBlock(sbPuml.ToString(), out var fullPuml);
            GroupLogLogic.EncodeUrl(fullPuml, out var encodedUrl);
            GroupLogLogic.FormatMarkdown(state.AssetPath, fullPuml, encodedUrl, out result);
            
            state.MarkdownContent = result;
            state.IsGenerated = true;

            return true; 
        }

        public static bool TrySaveToDisk(ref this GroupLogState state, out string savedPath)
        {
            savedPath = string.Empty;
            if (!state.IsGenerated) return false;

            savedPath = Path.ChangeExtension(state.AssetPath, ".md");
            try
            {
                File.WriteAllText(savedPath, state.MarkdownContent);
                AssetDatabase.ImportAsset(savedPath);
                return true;
            }
            catch { return false; }
        }

        private static void CollectAllTracks(TrackAsset track, List<TrackAsset> list)
        {
            if (track == null) return;
            list.Add(track);
            foreach (var child in track.GetChildTracks())
            {
                CollectAllTracks(child, list);
            }
        }
    }
}