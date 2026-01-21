using UnityEngine;
using UnityEditor;

namespace PlayableLearn.Day01
{
    /// <summary>
    /// Custom Inspector for Day01Readme that renders interactive documentation.
    /// </summary>
    [CustomEditor(typeof(Day01Readme))]
    public sealed class Day01ReadmeEditor : Editor
    {
        // Style cache
        GUIStyle _titleStyle;
        GUIStyle _subtitleStyle;
        GUIStyle _headingStyle;
        GUIStyle _bodyStyle;
        GUIStyle _boxStyle;
        GUIStyle _primaryButtonStyle;
        GUIStyle _secondaryButtonStyle;
        GUIStyle _labelStyle;

        // Expanded states for collapsible sections
        private readonly System.Collections.Generic.Dictionary<string, bool> _expandedSections = new();

        private Day01Readme Readme => (Day01Readme)target;

        public override void OnInspectorGUI()
        {
            InitializeStylesIfNeeded();

            DrawBanner();
            DrawTitle();
            DrawOverview();
            DrawSections();
            DrawQuickLinks();
            DrawFooter();
        }

        private void DrawBanner()
        {
            if (Readme.bannerIcon != null)
            {
                var width = EditorGUIUtility.currentViewWidth - 40;
                var ratio = (float)Readme.bannerIcon.height / Readme.bannerIcon.width;
                var height = width * ratio;

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(Readme.bannerIcon, GUILayout.Width(width), GUILayout.Height(height));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                // Fallback colored banner
                var bannerRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 80);
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUI.DrawRect(bannerRect, Readme.bannerColor);
                }

                GUI.color = Color.white;
                EditorGUI.DropShadowLabel(
                    bannerRect,
                    "<size=40><b>PlayableLearn</b></size>",
                    new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 40,
                        fontStyle = FontStyle.Bold,
                        richText = true
                    }
                );
                GUI.color = Color.white;
            }

            GUILayout.Space(20);
        }

        private void DrawTitle()
        {
            GUILayout.Label(Readme.lessonTitle, _titleStyle);
            GUILayout.Label(Readme.subtitle, _subtitleStyle);
            GUILayout.Space(15);
        }

        private void DrawOverview()
        {
            EditorGUILayout.BeginVertical(_boxStyle);
            GUILayout.Label("Overview", _headingStyle);
            GUILayout.Label(Readme.overview, _bodyStyle);
            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
        }

        private void DrawSections()
        {
            if (Readme.sections == null) return;

            foreach (var section in Readme.sections)
            {
                if (string.IsNullOrEmpty(section.heading) && string.IsNullOrEmpty(section.content))
                    continue;

                string sectionKey = section.heading ?? "UnnamedSection";

                // Initialize expanded state if not exists
                if (!_expandedSections.ContainsKey(sectionKey))
                {
                    _expandedSections[sectionKey] = section.startExpanded;
                }

                // Draw collapsible header
                if (section.isCollapsible)
                {
                    _expandedSections[sectionKey] = EditorGUILayout.BeginFoldoutHeaderGroup(
                        _expandedSections[sectionKey],
                        section.heading,
                        _headingStyle
                    );

                    if (_expandedSections[sectionKey])
                    {
                        EditorGUILayout.BeginVertical(_boxStyle);
                        GUILayout.Label(section.content, _bodyStyle);
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                else
                {
                    // Non-collapsible section
                    if (!string.IsNullOrEmpty(section.heading))
                    {
                        GUILayout.Label(section.heading, _headingStyle);
                    }

                    if (!string.IsNullOrEmpty(section.content))
                    {
                        EditorGUILayout.BeginVertical(_boxStyle);
                        GUILayout.Label(section.content, _bodyStyle);
                        EditorGUILayout.EndVertical();
                    }
                }

                GUILayout.Space(10);
            }
        }

        private void DrawQuickLinks()
        {
            if (Readme.quickLinks == null || Readme.quickLinks.Length == 0)
                return;

            GUILayout.Label("Quick Actions", _headingStyle);
            GUILayout.Space(5);

            foreach (var link in Readme.quickLinks)
            {
                if (string.IsNullOrEmpty(link.buttonText) || string.IsNullOrEmpty(link.url))
                    continue;

                var buttonStyle = link.isPrimary ? _primaryButtonStyle : _secondaryButtonStyle;

                if (GUILayout.Button(link.buttonText, buttonStyle, GUILayout.Height(30)))
                {
                    HandleLinkClick(link.url);
                }

                GUILayout.Space(5);
            }

            GUILayout.Space(15);
        }

        private void DrawFooter()
        {
            GUILayout.Space(20);
            EditorGUILayout.BeginVertical(_boxStyle);
            GUILayout.Label(
                "<size=11><i>Need to edit this documentation?</i></size>\n<size=10>Switch Inspector to Debug mode (right-click Inspector tab) to modify content.</size>",
                _labelStyle
            );
            EditorGUILayout.EndVertical();
        }

        private void HandleLinkClick(string url)
        {
            if (url.StartsWith("asset://"))
            {
                // Handle asset selection
                var assetPath = url.Replace("asset://", "");
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                if (asset != null)
                {
                    Selection.activeObject = asset;
                    EditorGUIUtility.PingObject(asset);
                }
            }
            else if (url.StartsWith("command://"))
            {
                // Handle editor commands
                var command = url.Replace("command://", "");
                HandleCommand(command);
            }
            else
            {
                // Open external URL
                Application.OpenURL(url);
            }
        }

        private void HandleCommand(string command)
        {
            switch (command)
            {
                case "create-cube":
                    CreateDemoCube();
                    break;
                case "select-folder":
                    SelectDay01Folder();
                    break;
                case "open-documentation":
                    Application.OpenURL("https://docs.unity3d.com/ScriptReference/Playables.PlayableGraph.html");
                    break;
            }
        }

        private void CreateDemoCube()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Day01_Cube";
            var animator = cube.AddComponent<Animator>();
            animator.applyRootMotion = false;

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(
                "Assets/Day 01: The First Breath/Scripts/CubeAnimationPlayer.cs"
            );

            if (script != null)
            {
                var scriptType = script.GetClass();
                if (scriptType != null)
                {
                    cube.AddComponent(scriptType);
                }
            }

            Selection.activeObject = cube;
            Debug.Log("[Day 01] Created demo cube with Animator and CubeAnimationPlayer.");
        }

        private void SelectDay01Folder()
        {
            var folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                "Assets/Day 01: The First Breath"
            );
            if (folder != null)
            {
                Selection.activeObject = folder;
                EditorGUIUtility.PingObject(folder);
            }
        }

        private void InitializeStylesIfNeeded()
        {
            if (_titleStyle != null) return;

            // Title Style
            _titleStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                margin = new RectOffset(10, 10, 10, 5)
            };

            // Subtitle Style
            _subtitleStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
                richText = true,
                margin = new RectOffset(10, 10, 0, 10)
            };

            // Heading Style
            _headingStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                richText = true,
                margin = new RectOffset(0, 0, 10, 5)
            };

            // Body Style
            _bodyStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 13,
                wordWrap = true,
                richText = true,
                margin = new RectOffset(10, 10, 5, 5)
            };

            // Box Style
            _boxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                margin = new RectOffset(5, 5, 5, 5),
                padding = new RectOffset(10, 10, 10, 10)
            };

            // Primary Button Style
            _primaryButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(15, 15, 8, 8),
                richText = true
            };

            // Secondary Button Style
            _secondaryButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 13,
                padding = new RectOffset(15, 15, 6, 6),
                richText = true
            };

            // Label Style (for footer)
            _labelStyle = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };
        }
    }
}
