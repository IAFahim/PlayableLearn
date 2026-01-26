using TweenPlayables;
using UnityEngine;

namespace AV.Day05.Runtime
{
    public class HealthBehaviour : TweenAnimationBehaviour<HealthComponent>
    {
        [SerializeField] private FloatTweenParameter health;

        public override void OnTweenInitialize(HealthComponent playerData)
        {
            health.SetInitialValue(playerData, playerData.health);
        }
    }
}