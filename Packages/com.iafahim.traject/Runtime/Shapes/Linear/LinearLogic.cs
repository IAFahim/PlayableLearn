using System.Runtime.CompilerServices;
using AV.Traject.Runtime.Core;
using Unity.Mathematics;

namespace AV.Traject.Runtime.Shapes.Linear
{
    /// <summary>
    /// Core logic for linear trajectory evaluation.
    /// Static methods with strict in/out patterns following DOD architecture.
    /// </summary>
    public static class LinearLogic
    {
        /// <summary>
        /// Evaluates a linear trajectory at normalized time t.
        /// Movement is along the Forward axis of the basis.
        /// </summary>
        /// <param name="basis">The coordinate basis for evaluation.</param>
        /// <param name="range">The distance along the forward direction.</param>
        /// <param name="t">Normalized time [0, 1].</param>
        /// <param name="position">Output: World space position.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Evaluate(
            in TrajectBasis basis,
            in float range,
            in float t,
            out float3 position)
        {
            float forwardDist = range * t;
            TrajectMath.ResolvePositionInBasis(in basis, in forwardDist, 0f, 0f, out position);
        }
    }
}
