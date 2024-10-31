using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWorlds : MonoBehaviour
{
    [SerializeField, Tooltip("Default: Three (X)")]
    private OVRInput.Button _button = OVRInput.Button.Three;

    [SerializeField]
    private OVRInput.Controller _controller = OVRInput.Controller.LHand;
    
    [SerializeField]
    private OVRManager _manager;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private OVRPassthroughLayer _passthrough;

    private bool _isVirtual = false;
    public bool IsVirtual
    {
        get => _isVirtual;
        set
        {
            if (value == _isVirtual)
                return;
            
            _isVirtual = value;
            OnToggle(value);
        }
    }

    private bool _prevDown = false;
    
    void Awake()
    {
        _manager = FindObjectOfType<OVRManager>();
        _camera = Camera.main;
        _passthrough = FindObjectOfType<OVRPassthroughLayer>();

        IsVirtual = true;
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();

        bool down = OVRInput.GetDown(_button, _controller);

        if (down == _prevDown)
            return;
        
        IsVirtual = !IsVirtual;
        _prevDown = down;
    }

    void OnToggle(bool isVirtual)
    {
        _manager.isInsightPassthroughEnabled = !isVirtual;
        _camera.clearFlags = isVirtual ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
    }
}
