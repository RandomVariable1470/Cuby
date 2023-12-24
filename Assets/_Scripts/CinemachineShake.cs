using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : Singleton<CinemachineShake>
{
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private float _shakeTimer;
    private float _shakeTimerTotal;
    private float _startingIntensity;

    private void Start() 
    {
        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }   

    public void ShakeCamera(float intensity, float timer)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        _startingIntensity = intensity;
        _shakeTimerTotal = timer;
        _shakeTimer = timer;
    }

    private void Update() 
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;

            if (_shakeTimer <= 0)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(_startingIntensity , 0f, 1- (_shakeTimer / _shakeTimerTotal));
            }
        }
    }

}