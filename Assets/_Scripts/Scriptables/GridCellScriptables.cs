using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridCellScriptableObject", menuName = "Scriptables/GridCell")]
public class GridCellScriptables : ScriptableObject
{
    [field: Header("Floating Animation")]
    [field: SerializeField] public float FloatMagnitude {get; private set;}
    [field: SerializeField] public float FloatSpeed {get; private set;}
    [field: SerializeField] public float FloatSpeedSmoothness {get; private set;}
    [field: SerializeField] public float VariationMultiplier {get; set;}
    [field: SerializeField] public float WavePhaseMultiplier {get; private set;}
}