using UnityEngine;
using UnityEngine.Rendering;

namespace PlayableLearn.Core
{
    /// <summary>
    /// The Asset Fabricator - Generates textures, materials, and grids via code at runtime.
    /// Solves "Missing Materials" and pink objects forever.
    /// </summary>
    public static class SmartAssets
    {
        private static Shader _cachedShader;

        /// <summary>
        /// Automatically finds a working shader for URP, HDRP, or Built-in.
        /// </summary>
        public static Shader GetSafeShader()
        {
            if (_cachedShader != null) return _cachedShader;

            string[] shaderNames = new[] {
                "Universal Render Pipeline/Lit",
                "Universal Render Pipeline/Simple Lit",
                "Standard",
                "Mobile/Diffuse"
            };

            foreach (var name in shaderNames)
            {
                var s = Shader.Find(name);
                if (s != null)
                {
                    _cachedShader = s;
                    return s;
                }
            }
            return Shader.Find("Hidden/InternalErrorShader");
        }

        public static Material CreateMaterial(Color color, string name = "ProcMat")
        {
            var mat = new Material(GetSafeShader());
            mat.name = name;

            // Handle URP vs Standard property names
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);

            return mat;
        }

        /// <summary>
        /// Generates a cool "VR Training Room" grid texture in memory.
        /// </summary>
        public static Texture2D GenerateGridTexture(Color bg, Color line)
        {
            int res = 512;
            int gridSize = 64;
            var tex = new Texture2D(res, res);
            tex.filterMode = FilterMode.Trilinear;

            Color[] pixels = new Color[res * res];
            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    bool isLine = (x % gridSize < 2) || (y % gridSize < 2);
                    pixels[y * res + x] = isLine ? line : bg;
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}
