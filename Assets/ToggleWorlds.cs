using Meta.XR.MRUtilityKit;
using System.Linq;
using UnityEngine;

public class ToggleWorlds : MonoBehaviour
{
    [SerializeField, Tooltip("Default: Three (X)")]
    private OVRInput.Button _button = OVRInput.Button.Three;

    [SerializeField]
    private OVRInput.Controller _controller = OVRInput.Controller.Touch;
    
    [SerializeField]
    private OVRManager _manager;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private OVRPassthroughLayer _passthrough;

    [SerializeField]
    private MRUK _roomTracker;

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
        _roomTracker = FindAnyObjectByType<MRUK>();
    }

    // Update is called once per frame
    void Update()
    {
        bool down = OVRInput.GetDown(_button, _controller);

        if (!down)
            return;
        
        IsVirtual = !IsVirtual;
        _prevDown = down;
    }

    public void ToggleVirtual()
        => IsVirtual = !IsVirtual;

    void OnToggle(bool isVirtual)
    {
        _manager.isInsightPassthroughEnabled = !isVirtual;
        _camera.clearFlags = isVirtual ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
        _passthrough.overlayType = isVirtual ? OVROverlay.OverlayType.None : OVROverlay.OverlayType.Underlay;

        _roomTracker.Rooms.Single().Anchors.ForEach(a => a.enabled = isVirtual);
    }
}
