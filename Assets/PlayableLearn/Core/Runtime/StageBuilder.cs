using UnityEngine;

namespace PlayableLearn.Core
{
    /// <summary>
    /// The Stage Architect - Builds a professional lighting and camera setup instantly.
    /// </summary>
    public static class StageBuilder
    {
        public static void RebuildStage()
        {
            // 1. Cleanup old junk
            foreach (var go in Object.FindObjectsOfType<GameObject>())
            {
                if (go.name.StartsWith("[Stage]")) Object.DestroyImmediate(go);
            }

            // 2. Camera Setup
            var camGo = new GameObject("[Stage] Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            camGo.transform.position = new Vector3(0, 3f, -5f);
            camGo.transform.LookAt(Vector3.up * 1f);

            // Set nice background
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.12f); // Dark Slate

            // 3. Lighting (Three Point Setup)
            CreateLight("Key Light", new Vector3(5, 5, -5), 1.2f, new Color(1f, 0.95f, 0.9f));
            CreateLight("Fill Light", new Vector3(-5, 2, -5), 0.5f, new Color(0.8f, 0.9f, 1f));
            CreateLight("Back Light", new Vector3(0, 5, 5), 0.8f, new Color(1f, 0.6f, 0.4f));

            // 4. The Floor
            CreateFloor();
        }

        private static void CreateLight(string name, Vector3 pos, float intensity, Color col)
        {
            var go = new GameObject($"[Stage] {name}");
            var l = go.AddComponent<Light>();
            l.type = LightType.Directional;
            l.intensity = intensity;
            l.color = col;
            l.shadows = LightShadows.Soft;
            go.transform.position = pos;
            go.transform.LookAt(Vector3.zero);
        }

        private static void CreateFloor()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "[Stage] Floor";
            floor.transform.localScale = Vector3.one * 5;

            // Procedural Material
            var mat = SmartAssets.CreateMaterial(Color.white, "FloorMat");
            mat.mainTexture = SmartAssets.GenerateGridTexture(
                new Color(0.15f, 0.15f, 0.15f),
                new Color(0.3f, 0.3f, 0.3f)
            );
            floor.GetComponent<Renderer>().material = mat;
        }

        public static GameObject SpawnActor(string name = "Actor")
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = Vector3.up * 0.5f;

            // "Learn Blue" Material
            var mat = SmartAssets.CreateMaterial(new Color(0.0f, 0.4f, 0.8f), "ActorMat");
            go.GetComponent<Renderer>().material = mat;

            return go;
        }
    }
}
