using System.Collections.Generic;
using UnityEngine;

public class PlaceLightSource : MonoBehaviour
{

    [SerializeField, Tooltip("Default: SecondaryIndexTrigger")]
    private OVRInput.Button _button = OVRInput.Button.SecondaryIndexTrigger;
    private OVRInput.Button _deletebutton = OVRInput.Button.Two;


    /// <summary>Defaults to correct amount of nulls</summary>
    public List<GameObject> Previews = new() { null, null, null };

    private PreviewType _type = PreviewType.Light;
    public PreviewType Type
    {
        get => _type;
        set
        {
            if (_type == value) return;

            _type = value;
            changeSpawnobject((int)value);
        }
    }

    private GameObject objectprefab;
    public GameObject cursor;
    public LineRenderer rayLinePrefab;
    public Transform origin;
    public float startDist = 0.1f;
    public float lineshowtimer = 1.0f;
    public float collisionRadius = 0.1f;

    public bool floating = true;

    private LineRenderer line;
    private GameObject cursorP;
    private Collider[] allColliders;
    private GameObject[] lightsources;

    private GameObject loc;
    private GameObject[] objectPreview;
    private GameObject[] spawnedObjects;
    private int spawnedobjectcounter = 0;
    // Start is called before the first frame update
    void Start()
    {

        objectPreview = new GameObject[2];
        spawnedObjects = new GameObject[100];

        objectprefab = Previews[0];
        line = Instantiate(rayLinePrefab);
        cursorP = Instantiate(cursor);
        line.positionCount = 2;
        allColliders = Physics.OverlapSphere(origin.position, 200);
        loc = new GameObject();
        objectPreview[0] = Instantiate(objectprefab, loc.transform);
        Vector3 inc = objectPreview[0].transform.position;
        objectPreview[0].transform.position = new Vector3(inc.x, inc.y + 0.8f, inc.z);
        //objectPreview.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (floating)
        {
            UpdateFloatingPlacement();
        }
        else
        {
            UpdateFloorPlacement();
        }

        lightsources = GameObject.FindGameObjectsWithTag("Lightsource");
        if (OVRInput.GetDown(_deletebutton))
        {
            foreach (GameObject l in lightsources)
            {
                l.SetActive(false);
            }
        }
    }

    public void UpdateFloatingPlacement()
    {
        line.SetPosition(0, origin.position);
        Vector3 endpoint = origin.position + origin.forward * startDist;
        loc.transform.position = endpoint;
        RaycastHit hit;
        if (Physics.Raycast(origin.position, endpoint - origin.position, out hit, startDist))
        {
            endpoint = hit.point;
        }

        //cursorP.transform.position = new Vector3(endpoint.x, endpoint.y, endpoint.z);
        line.SetPosition(1, endpoint);

        if (OVRInput.GetDown(_button))
        {
            InstantiateObject(objectPreview[0].transform);
        }

        Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (joystick.y != 0)
        {
            startDist += joystick.y * 0.05f;
        }
    }

    public void UpdateFloorPlacement()
    {
        line.SetPosition(0, origin.position);
        Vector3 endpoint = origin.position + origin.forward * startDist;
        endpoint.y = 0;
        //cursorP.transform.position = new Vector3(endpoint.x, endpoint.y, endpoint.z);
        line.SetPosition(1, endpoint);

        loc.transform.position = endpoint;

        if (OVRInput.GetDown(_button))
        {
            InstantiateObject(objectPreview[0].transform);
        }

        Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (joystick.y != 0)
        {
            startDist += joystick.y * 0.05f;
        }

        Vector2 joystickLeft = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        if (joystickLeft.x != 0 && joystickLeft.x > joystickLeft.y)
        {
            Quaternion curRot = objectPreview[0].transform.rotation;
            objectPreview[0].transform.rotation = curRot * Quaternion.Euler(0, 5 * joystickLeft.x, 0);
        }
        else if (joystickLeft.y != 0)
        {
            // TODO: manipulate scale here instead
            Quaternion curRot = objectPreview[0].transform.rotation;
            objectPreview[0].transform.rotation = curRot * Quaternion.Euler(0, 5 * joystickLeft.x, 0);
        }
    }
    public void FloatingOff()
    {
        floating = false;
        objectPreview[0].SetActive(true);
    }
    public void FloatingOn()
    {
        floating = true;
        objectprefab = Previews[0];
    }

    public void InstantiateObject(Transform trans)
    {
        if (objectprefab != null)
        {
            Vector3 pos = trans.position;
            trans.GetLocalPositionAndRotation(out Vector3 posloc, out Quaternion rot);
            spawnedObjects[spawnedobjectcounter] = Instantiate(objectprefab, pos, rot);
            spawnedobjectcounter++;
        }
    }

    public void changeSpawnobject(int objectid)
    {
        Destroy(objectPreview[0]);
        objectprefab = Previews[objectid];

        objectPreview[0] = Instantiate(objectprefab, loc.transform);

        if (objectid != 0)
        {
            Vector3 inc = objectPreview[0].transform.position;
            objectPreview[0].transform.position = new Vector3(inc.x, inc.y + 0.8f, inc.z);
        }
    }

    public enum PreviewType
    {
        Light = 0,
        Table = 1,
        Wall = 2,
    }
}
