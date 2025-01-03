using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceLightSource : MonoBehaviour
{

    [SerializeField, Tooltip("Default: SecondaryIndexTrigger")]
    private OVRInput.Button _button = OVRInput.Button.SecondaryIndexTrigger;
    private OVRInput.Button _deletebutton = OVRInput.Button.Two;

    // would like to expose this to the editor but there's no way to force it through the setter
    private float _scaleFactor = 1;
    public float ScaleFactor
    {
        get => _scaleFactor;
        set
        {
            _scaleFactor = Math.Max(value, 0);
            objectPreview[0].transform.localScale = Scale;
        }
    }

    private Vector3 BaseScale => objectprefab.transform.localScale;
    public Vector3 Scale => BaseScale * ScaleFactor;

    /// <summary>Defaults to correct amount of nulls</summary>
    public List<GameObject> PreviewPrefabs = new() { null, null, null };

    private int _previewIndex = 0;
    public int PreviewIndex
    {
        get => _previewIndex;
        set
        {
            if (_previewIndex == value) return;

            _previewIndex = value;
            changeSpawnobject(value);
        }
    }

    private GameObject objectprefab;
    public LineRenderer rayLinePrefab;
    public Transform origin;
    public float startDist = 0.1f;
    public float lineshowtimer = 1.0f;
    public float collisionRadius = 0.1f;

    public bool floating = true;

    private LineRenderer line;
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

        objectprefab = PreviewPrefabs[0];
        line = Instantiate(rayLinePrefab);
        line.positionCount = 2;
        allColliders = Physics.OverlapSphere(origin.position, 200);
        loc = new GameObject();
        objectPreview[0] = Instantiate(objectprefab, loc.transform);

        objectPreview[0].layer = LayerMask.NameToLayer("Ignore Raycast");
        //hope they have no recursive children...
        for (int i = 0; i < objectPreview[0].transform.childCount; i++)
        {
            objectPreview[0].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        objectPreview[0].transform.localScale *= ScaleFactor;
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

        if (OVRInput.GetDown(_deletebutton))
        {
            RaycastHit hit;
            GameObject obj;
            if (Physics.Raycast(origin.position, origin.forward, out hit, startDist, LayerMask.GetMask("Default")))
            {
                obj = hit.transform.GameObject();
                if (obj.CompareTag("PlacedObj")){
                    obj.SetActive(false);
                }
            }
        }
    }

    public void UpdateFloatingPlacement()
    {
        line.SetPosition(0, origin.position);
        //RaycastHit hit;
        //if (Physics.Raycast(origin.position, origin.forward, out hit, startDist, LayerMask.GetMask("Default")))
        //{
        //    startDist = hit.distance;
        //}
        Vector3 endpoint = origin.position + origin.forward * startDist;
        loc.transform.position = endpoint;

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
        //RaycastHit hit;
        //if (Physics.Raycast(origin.position, origin.forward, out hit, startDist, LayerMask.GetMask("Default")))
        //{
        //    startDist = hit.distance;
        //}
        Vector3 endpoint = origin.position + origin.forward * startDist;
        endpoint.y = 0;
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
        if (joystickLeft.x != 0 && Math.Abs(joystickLeft.x) > Math.Abs(joystickLeft.y))
        {
            Quaternion curRot = objectPreview[0].transform.rotation;
            objectPreview[0].transform.rotation = curRot * Quaternion.Euler(0, 4 * joystickLeft.x, 0);
        }
        else if (joystickLeft.y != 0)
        {
            ScaleFactor += joystickLeft.y * 0.04f;
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
        objectprefab = PreviewPrefabs[0];
    }

    public void InstantiateObject(Transform trans)
    {
        if (objectprefab != null)
        {
            Vector3 pos = trans.position;
            trans.GetLocalPositionAndRotation(out Vector3 posloc, out Quaternion rot);

            var spawned = Instantiate(objectprefab, pos, rot);
            spawned.transform.localScale *= ScaleFactor;
            spawned.tag = "PlacedObj";

            spawnedObjects[spawnedobjectcounter] = spawned;
            spawnedobjectcounter++;
        }
    }

    public void changeSpawnobject(int objectid)
    {
        Destroy(objectPreview[0]);
        objectprefab = PreviewPrefabs[objectid];

        objectPreview[0] = Instantiate(objectprefab, loc.transform);

        objectPreview[0].layer = LayerMask.NameToLayer("Ignore Raycast");
        //hope they have no recursive children...
        for (int i = 0; i < objectPreview[0].transform.childCount; i++)
        {
            objectPreview[0].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        objectPreview[0].transform.localScale *= ScaleFactor;
    }
}
