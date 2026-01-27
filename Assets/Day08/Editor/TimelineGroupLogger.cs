using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEditor.Timeline.Actions;

namespace TweenPlayables.Editor.Tools
{
    // LAYER A: DATA
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct GroupLogState
    {
        public string MarkdownContent;
        public string AssetPath;
        public int GroupCount;
        public bool IsGenerated;

        public override string ToString() => $"[LogState] Path: {AssetPath} ({GroupCount} Groups)"; 
    }

    // LAYER B: LOGIC
    public static class GroupLogLogic
    {
        // PlantUML Mapping: 0-9, A-Z, a-z, -, _
        private const string Mapper = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatPumlBlock(string content, out string result)
        {
            result = $"@startuml\n\n{content}\n@enduml"; // Exact spacing
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatTrackLine(string name, string alias, out string line)
        {
            line = $"concise \"{name}\" as {alias}"; // Atomic format
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatMarkdown(string path, string pumlBlock, string encodedCode, out string md)
        {
            // Exact requested format
            var urlImg = $"https://img.plantuml.biz/plantuml/svg/{encodedCode}";
            var urlEdit = $"https://editor.plantuml.com/uml/{encodedCode}";

            md = $"## File: [{path}]({path})\n\n" +
                 $"```plantuml\n{pumlBlock}\n```\n\n" +
                 $"[![]({urlImg})]({urlEdit})"; 
        }

        // --- ENCODING (Deflate + Custom Base64) ---

        public static void EncodeUrl(string puml, out string encoded)
        {
            var bytes = Encoding.UTF8.GetBytes(puml); 
            using var ms = new MemoryStream();
            
            // Deflate Compression (Standard PlantUML requirement)
            using (var deflate = new DeflateStream(ms, CompressionMode.Compress)) 
            {
                deflate.Write(bytes, 0, bytes.Length);
            }
            
            var compressed = ms.ToArray();
            Encode64(compressed, out encoded); 
        }

        private static void Encode64(byte[] data, out string result)
        {
            var sb = new StringBuilder();
            int len = data.Length;
            
            for (int i = 0; i < len; i += 3)
            {
                int b1 = data[i];
                int b2 = (i + 1 < len) ? data[i + 1] : 0;
                int b3 = (i + 2 < len) ? data[i + 2] : 0;
                Append3Bytes(sb, b1, b2, b3); 
            }
            result = sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Append3Bytes(StringBuilder sb, int b1, int b2, int b3)
        {
            int c1 = b1 >> 2;
            int c2 = ((b1 & 0x3) << 4) | (b2 >> 4);
            int c3 = ((b2 & 0xF) << 2) | (b3 >> 6);
            int c4 = b3 & 0x3F;

            sb.Append(Mapper[c1 & 0x3F]);
            sb.Append(Mapper[c2 & 0x3F]);
            sb.Append(Mapper[c3 & 0x3F]);
            sb.Append(Mapper[c4 & 0x3F]);
        }
    }

    // LAYER C: EXTENSIONS
    public static class GroupLogExtensions
    {
        public static bool TryGenerate(ref this GroupLogState state, TimelineAsset timeline, out string result)
        {
            result = string.Empty; // Default
            if (timeline == null) return false; // Guard

            state.AssetPath = AssetDatabase.GetAssetPath(timeline);

            // 1. Build PUML Body
            var sbPuml = new StringBuilder();
            int count = 0;
            for (int i = 0; i < timeline.rootTrackCount; i++)
            {
                var track = timeline.GetRootTrack(i);
                if (track is GroupTrack)
                {
                    count++;
                    // CORRECTED: Uses T01, T02 format
                    GroupLogLogic.FormatTrackLine(track.name, $"T{count:D2}", out var line); 
                    sbPuml.AppendLine(line);
                }
            }
            state.GroupCount = count;

            if (count == 0) return false; // Guard: Empty

            // 2. Wrap PUML
            GroupLogLogic.FormatPumlBlock(sbPuml.ToString(), out var fullPuml);

            // 3. Encode
            GroupLogLogic.EncodeUrl(fullPuml, out var encodedUrl);

            // 4. Final Markdown
            GroupLogLogic.FormatMarkdown(state.AssetPath, fullPuml, encodedUrl, out result);
            
            state.MarkdownContent = result;
            state.IsGenerated = true;

            return true; 
        }

        public static bool TrySaveToDisk(ref this GroupLogState state, out string savedPath)
        {
            savedPath = string.Empty;
            if (!state.IsGenerated) return false; // Guard

            savedPath = System.IO.Path.ChangeExtension(state.AssetPath, ".md"); // Swap extension
            try
            {
                File.WriteAllText(savedPath, state.MarkdownContent); // IO Write
                AssetDatabase.ImportAsset(savedPath); // Refresh DB
                return true;
            }
            catch { return false; }
        }
    }

    // LAYER D: ACTIONS
    [MenuEntry("Create PlantUML Markdown", priority: 1000)]
    public class CreatePlantUMLAction : TrackAction
    {
        public override ActionValidity Validate(IEnumerable<TrackAsset> tracks)
        {
            return (tracks != null && tracks.Any()) ? ActionValidity.Valid : ActionValidity.NotApplicable;
        }

        public override bool Execute(IEnumerable<TrackAsset> tracks)
        {
            var track = tracks.FirstOrDefault();
            if (track == null) return false;

            var timeline = track.timelineAsset;
            var state = new GroupLogState();

            if (state.TryGenerate(timeline, out _))
            {
                if (state.TrySaveToDisk(out var path))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                    EditorGUIUtility.PingObject(asset);

                    Debug.Log($"<color=cyan>[TweenPlayables]</color> Created & Pinged: {path}");
                    return true;
                }
            }

            return false;
        }
    }
}