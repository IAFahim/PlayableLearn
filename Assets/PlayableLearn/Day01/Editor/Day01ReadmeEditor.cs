using UnityEngine;
using UnityEditor;
using PlayableLearn.Core.Editor;

namespace PlayableLearn.Day01.Editor
{
    [CustomEditor(typeof(Day01Readme))]
    public class Day01ReadmeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Header
            var styleHeader = new GUIStyle(EditorStyles.boldLabel) { fontSize = 24, alignment = TextAnchor.MiddleCenter };
            var styleBody = new GUIStyle(EditorStyles.label) { wordWrap = true, fontSize = 13 };
            var styleBox = new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 10, 10) };

            GUILayout.Space(10);
            GUILayout.Label("Day 01: The First Breath", styleHeader);
            GUILayout.Space(10);

            // Intro
            EditorGUILayout.BeginVertical(styleBox);
            GUILayout.Label("Welcome to graph-based animation. Today we break free from the Animator Controller.", styleBody);
            GUILayout.Label("\nWe will create a PlayableGraph that plays a raw AnimationClip on a cube.", styleBody);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);

            // ACTION BUTTON
            GUI.backgroundColor = new Color(0.4f, 1.0f, 0.6f);
            if (GUILayout.Button("SETUP SCENE & START LESSON", GUILayout.Height(40)))
            {
                SetupLesson();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Clicking this creates a new scene, sets up lighting, creates the Actor, and attaches the Day01 script.", MessageType.Info);

            GUILayout.Space(20);

            // Navigation
            GUILayout.Label("Key Files:", EditorStyles.boldLabel);
            if (GUILayout.Button("Day01_Lifecycle.cs (The Setup)")) OpenScript("Day01_Lifecycle");
            if (GUILayout.Button("Day01_Binder.cs (The Graph)")) OpenScript("Day01_Binder");
            if (GUILayout.Button("Day01_Logic.cs (The Brain)")) OpenScript("Day01_Logic");
        }

        private void SetupLesson()
        {
            // 1. Build Stage
            SceneBootstrapper.CreateCleanStage("Day01_Sandbox");

            // 2. Build Actor
            var actor = SceneBootstrapper.CreateActor("Graph_Actor", new Vector3(0, 0.5f, 0));

            // 3. Attach Script
            // We use string reflection to avoid tight coupling if scripts fail to compile
            var scriptType = System.Type.GetType("PlayableLearn.Day01.Day01_Lifecycle, Day01");
            if (scriptType != null)
            {
                actor.AddComponent(scriptType);
                Selection.activeGameObject = actor;
            }
            else
            {
                Debug.LogError("Could not find Day01_Lifecycle script. Did you copy the files correctly?");
            }

            Debug.Log("Day 01 Lesson Ready. Press Play!");
        }

        private void OpenScript(string name)
        {
            var guids = AssetDatabase.FindAssets(name);
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 1);
            }
        }
    }
}
