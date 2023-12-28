using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [field:Header("UI")]
    [field:SerializeField] public TextMeshProUGUI colorText {get; private set;}
}