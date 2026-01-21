using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PlayableLearn.Core.Editor
{
    /// <summary>
    /// The Curriculum Hub - A dedicated window to manage your progress through 100 days.
    /// </summary>
    public class CurriculumHub : EditorWindow
    {
        [MenuItem("PlayableLearn/Open Curriculum Hub", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow<CurriculumHub>("Graph Warlord");
        }

        private Vector2 _scroll;

        private void OnGUI()
        {
            DrawHeader();

            _scroll = GUILayout.BeginScrollView(_scroll);

            DrawSection("Phase 1: The Architect", 1, 10);
            // Future phases...

            GUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
            GUILayout.Space(10);
            GUILayout.Label("GRAPH WARLORD", style);
            GUILayout.Label("Master the Playable API", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Space(20);

            if (GUILayout.Button("Reset Sandbox Scene", GUILayout.Height(30)))
            {
                CreateNewSandbox();
            }
            GUILayout.Space(20);
        }

        private void DrawSection(string title, int startDay, int endDay)
        {
            GUILayout.Label(title, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            for (int i = startDay; i <= endDay; i++)
            {
                DrawDayItem(i);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
        }

        private void DrawDayItem(int day)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Day {day:00}", GUILayout.Width(60));

            if (GUILayout.Button("Load Lesson", EditorStyles.miniButton))
            {
                LoadLesson(day);
            }

            if (GUILayout.Button("Documentation", EditorStyles.miniButton))
            {
                SelectReadme(day);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreateNewSandbox()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            StageBuilder.RebuildStage();
        }

        private void LoadLesson(int day)
        {
            CreateNewSandbox();

            // 200 IQ: Reflection to find the "Bootstrapper" for that day
            string className = $"PlayableLearn.Day{day:00}.Day{day:00}_Bootstrapper";
            var type = System.Type.GetType(className + ", Day" + day.ToString("00"));

            if (type != null)
            {
                var method = type.GetMethod("Initialize");
                if (method != null)
                {
                    method.Invoke(null, null);
                    Debug.Log($"<color=#00FF00>Day {day} Loaded Successfully.</color>");
                }
                else Debug.LogError($"Initialize method missing in {className}");
            }
            else
            {
                Debug.LogError($"Could not find class: {className}. Make sure the Assembly Definition is named 'Day{day:00}'");
            }
        }

        private void SelectReadme(int day)
        {
            string path = $"Assets/PlayableLearn/Day{day:00}/Day{day:00}_Readme.asset";
            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if(obj) Selection.activeObject = obj;
            else Debug.LogWarning("Readme not found at " + path);
        }
    }
}
