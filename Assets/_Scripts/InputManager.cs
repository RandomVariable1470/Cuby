using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    #region Events

    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;

    public delegate void EndTouch(Vector2 position, float time);
    public event StartTouch OnEndTouch;

    #endregion

    private PlayerControls _playerControls;
    private Camera _cameraMain;

    private void Awake() 
    {
        _playerControls = new PlayerControls();
        _cameraMain = Camera.main;
    }

    private void OnEnable() 
    {
        _playerControls.Enable();
    }

    private void OnDisable() 
    {
        _playerControls.Disable();
    }

    private void Start() 
    {
        _playerControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        _playerControls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null)
        {
            OnStartTouch(Extension.ScreenToWorld(_cameraMain, _playerControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
        }
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null)
        {
            OnEndTouch(Extension.ScreenToWorld(_cameraMain, _playerControls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.time);
        }
    }

    public Vector2 PrimaryPosition()
    {
        return Extension.ScreenToWorld(_cameraMain, _playerControls.Touch.PrimaryPosition.ReadValue<Vector2>());
    }
}