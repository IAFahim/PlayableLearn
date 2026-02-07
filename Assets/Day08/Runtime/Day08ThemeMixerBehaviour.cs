using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day08.Runtime
{
    public class Day08ThemeMixerBehaviour : PlayableBehaviour
    {
        private MaterialPropertyBlock _propBlock;
        private static readonly int _colorId = Shader.PropertyToID("_Color");
        private Color _defaultColor;
        private bool _firstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var renderer = playerData as Renderer;
            if (renderer == null) return;

            if (_propBlock == null)
                _propBlock = new MaterialPropertyBlock();

            if (!_firstFrameHappened)
            {
                _defaultColor = renderer.sharedMaterial.color;
                _firstFrameHappened = true;
            }

            int inputCount = playable.GetInputCount();
            Color finalColor = Color.black;
            float totalWeight = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                if (inputWeight <= 0.001f) continue;

                var inputPlayable = (ScriptPlayable<Day08ThemePlayableBehaviour>)playable.GetInput(i);
                var behaviour = inputPlayable.GetBehaviour();

                finalColor += behaviour.color * inputWeight;
                totalWeight += inputWeight;
            }

            float remainingWeight = 1f - totalWeight;
            if (remainingWeight > 0)
                finalColor += _defaultColor * remainingWeight;

            renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(_colorId, finalColor);
            renderer.SetPropertyBlock(_propBlock);
        }
    }
}
