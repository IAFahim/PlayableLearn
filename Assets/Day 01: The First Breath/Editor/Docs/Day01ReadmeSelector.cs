using UnityEditor;
using UnityEngine;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Automatically selects the Day 01 Readme when the project opens or when this folder is first accessed.
    /// </summary>
    [InitializeOnLoad]
    public sealed class Day01ReadmeSelector
    {
        private const string EditorKey = "PlayableLearn_Day01_ReadmeShown";

        static Day01ReadmeSelector()
        {
            EditorApplication.delayCall += ShowReadmeIfNeeded;
        }

        private static void ShowReadmeIfNeeded()
        {
            // Only show on first access to Day 01 folder
            if (EditorPrefs.GetBool(EditorKey, false))
                return;

            // Check if we're in Day 01 folder
            var selected = Selection.activeObject;
            if (selected != null)
            {
                var path = AssetDatabase.GetAssetPath(selected);
                if (!string.IsNullOrEmpty(path) && path.Contains("Day 01"))
                {
                    SelectReadmeAsset();
                    EditorPrefs.SetBool(EditorKey, true);
                }
            }
        }

        [MenuItem("Assets/PlayableLearn/Show Day 01 Documentation", false, 1)]
        private static void SelectReadmeAsset()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(Day01Readme)}");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var readme = AssetDatabase.LoadAssetAtPath<Day01Readme>(path);
                if (readme != null)
                {
                    Selection.activeObject = readme;
                }
            }
            else
            {
                Debug.LogWarning("[Day 01] No Readme asset found. Create one via Right-click > Create > PlayableLearn > Day 01 Readme");
            }
        }

        /// <summary>
        /// Reset the "shown" flag to make the readme appear again on next folder access.
        /// </summary>
        [MenuItem("Assets/PlayableLearn/Reset Readme Auto-Show")]
        private static void ResetReadmeFlag()
        {
            EditorPrefs.DeleteKey(EditorKey);
            Debug.Log("[Day 01] Readme auto-show flag reset. It will appear again when you access the Day 01 folder.");
        }
    }
}
