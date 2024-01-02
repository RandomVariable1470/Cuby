using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColors : MonoBehaviour
{
    [SerializeField] private Player _player;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject != null && other.gameObject.layer == 6)
        {
            _player.ChangeColor(this.gameObject.name);
        }
    }
}