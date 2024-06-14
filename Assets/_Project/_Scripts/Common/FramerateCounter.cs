using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RV.Common
{
    public class FramerateCounter : MonoBehaviour
    {
        [Header("Texts")] 
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private TextMeshProUGUI msText;
        
        private int _updateRate = 4;
        private int _frameCount = 0;
        private float _deltaTime = 0f;
        
        private float _fps = 0f;
        private float _ms = 0f;
        
        private const string _msStringFormat = "0.0";

        private void Update()
        {
            _deltaTime += Time.unscaledDeltaTime;

            _frameCount++;

            if(_deltaTime > 1f / _updateRate)
            {
                _fps = _frameCount / _deltaTime;
                _ms = _deltaTime / _frameCount * 1000f;
                
                fpsText.text = $"FPS: {Mathf.RoundToInt(_fps).ToString()}";
                
                msText.text = $"MS: {_ms.ToString(_msStringFormat)}";
                
                _deltaTime = 0f;
                _frameCount = 0;
            }
        }
    }
}