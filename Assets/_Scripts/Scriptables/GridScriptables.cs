using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridScriptableObject", menuName = "Scriptables/Grid")]
public class GridScriptables : ScriptableObject
{
    [field: Header("Grid Settings")]
    [field: SerializeField] public GameObject GridCellPrefab { get; private set; }
    [field: SerializeField] public int Height { get; private set; }
    [field: SerializeField] public int Width { get; private set; }
    [field: SerializeField] public float GridSpaceSize { get; private set; }
    [field: Space(5)]
    [field: SerializeField] public int XFinalCellCoordinate { get; private set; }
    [field: SerializeField] public int YFinalCellCoordinate { get; private set; }
    [field: SerializeField] public Color FinalCellColor { get; private set; }
    [field: SerializeField] public ColorCode SelectedCellColorCode { get; private set; }
    [field: Space(5)]
    [field: SerializeField] public GameObject PlayerPrefab { get; private set; }
    [field: SerializeField] public int SpawnCellXCoordinate { get; private set; }
    [field: SerializeField] public int SpawnCellYCoordinate { get; private set; }
    [field: Space(10)]
    [field: Header("Creation Settings")]
    [field: SerializeField] public float InitialDelay { get; private set; }
    [field: SerializeField] public float SpeedUpFactor { get; private set; }

    [field: Space(10)]
    [field: Header("Falling Settings")]
    [field: SerializeField] public float FallingSpeed { get; private set; }
}