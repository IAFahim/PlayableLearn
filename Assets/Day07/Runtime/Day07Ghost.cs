using UnityEngine;

namespace AV.Day07.Runtime
{
    public class Day07Ghost : MonoBehaviour
    {
        // ?? PRIVATE & NON-SERIALIZED (Unity won't save this, Inspector won't see it)
        [System.NonSerialized] 
        private float _secretOpacity = 1f;

        [SerializeField] private Renderer _renderer;

        // ? THE BRIDGE (The Timeline talks to this)
        public float Opacity
        {
            get => _secretOpacity;
            set
            {
                _secretOpacity = Mathf.Clamp01(value);
                Debug.Log(value);
                ApplyToMaterial(_secretOpacity); 
            }
        }

        private void ApplyToMaterial(float val)
        {
            if (_renderer != null && _renderer.material != null)
            {
                Color color = _renderer.material.color;
                color.a = val;
                _renderer.material.color = color;
            }
        }

        private void OnValidate()
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
        }
    }
}
