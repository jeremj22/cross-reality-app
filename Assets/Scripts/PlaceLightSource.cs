using Meta.WitAi;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceLightSource : MonoBehaviour
{

    [SerializeField, Tooltip("Default: SecondaryIndexTrigger")]
    private OVRInput.Button _button = OVRInput.Button.SecondaryIndexTrigger;


    [SerializeField]
    private OVRInput.Controller _controller = OVRInput.Controller.Touch;

    [SerializeField]
    public GameObject lightprefab;
    public LineRenderer rayLinePrefab;
    public Transform origin;
    public float maxDist = 1.0f;
    public float lineshowtimer = 1.0f;

    private LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        line = Instantiate(rayLinePrefab);
        line.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, origin.position);
        Vector3 endpoint = origin.position + origin.forward * maxDist;
        line.SetPosition(1, endpoint);
        if (OVRInput.GetDown(_button))
        {
            InstantiateLight(endpoint);
        }
        Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (joystick.y != 0)
        {
            maxDist += joystick.y*0.1f;
        }
    }

    public void InstantiateLight(Vector3 endpoint)
    {
        if (lightprefab != null)
        {
            GameObject loc = new GameObject();
            loc.transform.position = endpoint;
            Instantiate(lightprefab, loc.transform);
        }
    }
}
