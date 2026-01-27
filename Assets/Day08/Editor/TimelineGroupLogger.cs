using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;

namespace Day08.Editor
{
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

    public static class GroupLogLogic
    {
        private const string Mapper = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatPumlBlock(string content, out string result)
        {
            result = $"@startuml\n\n{content}\n@enduml";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatTrackLine(string name, string alias, out string line)
        {
            line = $"concise \"{name}\" as {alias}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FormatMarkdown(string path, string pumlBlock, string encodedCode, out string md)
        {
            var urlImg = $"https://img.plantuml.biz/plantuml/svg/{encodedCode}";
            var urlEdit = $"https://editor.plantuml.com/uml/{encodedCode}";

            md = $"## File: [{path}]({path})\n\n" +
                 $"```plantuml\n{pumlBlock}\n```\n\n" +
                 $"[![]({urlImg})]({urlEdit})"; 
        }

        public static void EncodeUrl(string puml, out string encoded)
        {
            var bytes = Encoding.UTF8.GetBytes(puml); 
            using var ms = new MemoryStream();

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
    
    [MenuEntry("Create PlantUML Markdown", priority: 1000)]
    public class CreatePlantUmlAction : TrackAction
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