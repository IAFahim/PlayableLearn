using AV.Traject.Runtime.Core;
using AV.Traject.Runtime.Integration;
using Unity.Mathematics;
using UnityEngine;

namespace AV.Traject.Runtime.Shapes.Linear
{
    /// <summary>
    /// Linear trajectory asset.
    /// Creates straight-line movement along forward direction.
    /// </summary>
    [CreateAssetMenu(menuName = "AV/Traject/Linear", fileName = "LinearTraject")]
    public class LinearTraject : TrajectAsset
    {
        /// <summary>
        /// Evaluates the linear trajectory at normalized time t.
        /// </summary>
        public override void Evaluate(in TrajectBasis basis, float range, float t, out float3 pos)
        {
            LinearLogic.Evaluate(in basis, in range, in t, out pos);
        }
    }
}
