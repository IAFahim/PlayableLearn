using UnityEngine;
using UnityEngine.Playables;

namespace AV.Day02.Runtime
{
    public class Day02SystemMixer : PlayableBehaviour
    {
        private Day02GeneratedComponent _createdComponent;
     
        public void Initialize(GameObject binding)
        {
            _createdComponent ??= binding.AddComponent<Day02GeneratedComponent>();
        }
    }
}