using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AV.Day02.Runtime
{
    public class Day02Component : MonoBehaviour, ISimpleSystem
    {
        [SerializeField] private SimpleState _state;

        bool ISimpleSystem.TryActivate()
        {
            return _state.TryActivate(out var time);
        }

        bool ISimpleSystem.TryDeactivate()
        {
            return _state.TryDeactivate(out var time);
        }

        void ISimpleSystem.Initialize(SimpleState initialState)
        {
            _state = initialState;
        }

        [ContextMenu("Debug: Toggle State")]
        private void DebugToggle()
        {
            var success = _state.IsActive ? ((ISimpleSystem)this).TryDeactivate() : ((ISimpleSystem)this).TryActivate();
            Debug.Log(success ? $"<color=cyan>OK:</color> {_state}" : "FAIL"); // Log
        }
    }
}
