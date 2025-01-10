using Meta.XR.MRUtilityKit;
using Oculus.Interaction;
using UnityEngine;

public class ToggleWorlds : MonoBehaviour, IActiveState
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

    /// <summary>
    /// Implement interface and pass along virtuality to disable TP in AR
    /// </summary>
    public bool Active => IsVirtual;

    void Awake()
    {
        _manager = FindObjectOfType<OVRManager>();
        _camera = Camera.main;
        _passthrough = FindObjectOfType<OVRPassthroughLayer>();
        _roomTracker = FindAnyObjectByType<MRUK>();

        _roomTracker.RoomCreatedEvent.AddListener(_ => SetVisibilities(_isVirtual));
    }

    // Update is called once per frame
    void Update()
    {
        bool down = OVRInput.GetDown(_button, _controller);

        if (!down)
            return;

        IsVirtual = !IsVirtual;
    }

    public void ToggleVirtual()
        => IsVirtual = !IsVirtual;

    private void SetVisibilities(bool isVirtual)
        => _roomTracker.Rooms.ForEach(a => a.gameObject.SetActive(isVirtual));

    void OnToggle(bool isVirtual)
    {
        _manager.isInsightPassthroughEnabled = !isVirtual;
        _camera.clearFlags = isVirtual ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
        _camera.backgroundColor = Color.clear;
        _passthrough.overlayType = isVirtual ? OVROverlay.OverlayType.None : OVROverlay.OverlayType.Underlay;

        SetVisibilities(isVirtual);
    }
}
