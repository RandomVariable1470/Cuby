using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RV.Configs
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Configs/Player")]
    public class PlayerScriptable : ScriptableObject
    {
        [field: Header("Movement Settings")]
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public float GroundCheck { get; private set; }
        [field: SerializeField] public float SwipeCooldownTime { get; private set;}

        [field: Space(5)]
        [field: Header("Effects")]
        [field: SerializeField] public GameObject[] JumpParticle { get; private set; }
        [field: SerializeField] public GameObject LandParticle { get; private set; }

        [field: Space(5)]
        [field: Header("Offsets")]
        [field: SerializeField] public Vector3 LeftOffset { get; private set; }
        [field: SerializeField] public Vector3 RightOffset { get; private set; }
        [field: SerializeField] public Vector3 BackOffset { get; private set; }
        [field: SerializeField] public Vector3 FrontOffset { get; private set; }

        [field: Space(5)]
        [field: Header("Ground Check and Physics")]
        [field: SerializeField] public float Gravity { get; private set; }
        [field: SerializeField] public LayerMask GridCellLayerMask { get; private set; }

        [field: Space(5)]
        [field: Header("Color")]
        [field: SerializeField] public Color RedColor { get; private set; }
        [field: SerializeField] public Color CyanColor { get; private set; }
        [field: SerializeField] public Color YellowColor { get; private set; }
        [field: SerializeField] public Color BlueColor { get; private set; }
        [field: SerializeField] public Color GreenColor { get; private set; }
        [field: SerializeField] public Color OrangeColor { get; private set; }
    }
}