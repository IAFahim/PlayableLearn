using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PlayableLearn.Core.Editor
{
    /// <summary>
    /// Automated Scene Setup - "The Genie" that sets up the stage.
    /// </summary>
    public static class SceneBootstrapper
    {
        public static void CreateCleanStage(string sceneName)
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Lighting & Environment
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientSkyColor = new Color(0.15f, 0.15f, 0.15f);

            var lightGo = new GameObject("Spotlight");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Spot;
            light.intensity = 1.5f;
            light.range = 30f;
            lightGo.transform.position = new Vector3(0, 5, -3);
            lightGo.transform.rotation = Quaternion.Euler(50, 0, 0);

            // 2. The Floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Stage_Floor";
            floor.transform.localScale = Vector3.one * 2;
            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.1f, 0.1f, 0.15f);
            floor.GetComponent<Renderer>().material = mat;

            // 3. Camera
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            camGo.transform.position = new Vector3(0, 2, -6);
            camGo.transform.LookAt(new Vector3(0, 1, 0));
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.05f);

            Debug.Log($"<color=#44FF44>[Bootstrapper]</color> Stage Ready: {sceneName}");
        }

        public static GameObject CreateActor(string name, Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.0f, 0.6f, 1.0f); // Learn Blue
            go.GetComponent<Renderer>().material = mat;
            return go;
        }
    }
}
