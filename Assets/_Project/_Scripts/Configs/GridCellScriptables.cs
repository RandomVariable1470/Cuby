using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RV.Configs
{
    [CreateAssetMenu(fileName = "GridCellData", menuName = "Configs/Grid/Cell")]
    public class GridCellScriptables : ScriptableObject
    {
        [field: Header("Floating Animation")]
        [field: SerializeField] public float FloatMagnitude {get; private set;}
        [field: SerializeField] public float FloatSpeed {get; private set;}
        [field: SerializeField] public float FloatSpeedSmoothness {get; private set;}
        [field: SerializeField] public float VariationMultiplier {get; set;}
        [field: SerializeField] public float WavePhaseMultiplier {get; private set;}
    }
}