using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RV.Player
{
    public class PlayerColors : MonoBehaviour
    {
        private Player _player;

        private void Start()
        {
            _player = GetComponentInParent<Player>();
        
        }

        private void OnTriggerStay(Collider other) 
        {
            if (other.gameObject != null && other.gameObject.layer == 6)
            {
                _player.ChangeColor(this.gameObject.name);
            }
        }
    }
}