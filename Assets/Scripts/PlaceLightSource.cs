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

    public bool floating = true;

    private LineRenderer line;
    private Collider[] allColliders;
    private GameObject[] lightsources;

    private GameObject loc;
    private GameObject[] objectPreview;
    private GameObject[] spawnedObjects;
    private int spawnedobjectcounter = 0;

    private int state = 0;
    /*
     * 0: standby
     * 1: place
     * 2: erase
     */

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
    }

    // Update is called once per frame
    void Update()
    {   
        if (state == 0) { return; }
        else if (state == 1)
        {
            if (floating)
            {
                UpdateFloatingPlacement();
            }
            else
            {
                UpdateFloorPlacement();
            }
        }
        else
        {
            UpdateDeleteMode();
        }
       
    }
    private void UpdateDeleteMode()
    {
        line.SetPosition(0, origin.position);
        Vector3 endpoint = origin.position + origin.forward * startDist;
        line.SetPosition(1, endpoint);

        Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (joystick.y != 0)
        {
            startDist += joystick.y * 0.05f;
        }

        if (OVRInput.GetDown(_button) && !hitsUI())
        {
            RaycastHit hit;
            GameObject obj;
            if (Physics.Raycast(origin.position, origin.forward, out hit, startDist, LayerMask.GetMask("Default")))
            {
                obj = hit.transform.GameObject();
                if (obj.CompareTag("PlacedObj"))
                {
                    Destroy(obj);
                }
            }
        }
    }
    public void UpdateFloatingPlacement()
    {
        line.SetPosition(0, origin.position);
        Vector3 endpoint = origin.position + origin.forward * startDist;
        loc.transform.position = endpoint;

        line.SetPosition(1, endpoint);

        if (OVRInput.GetDown(_button) && !hitsUI())
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
        line.SetPosition(1, endpoint);

        loc.transform.position = endpoint;

        if (OVRInput.GetDown(_button) && !hitsUI())
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
    }
    public void FloatingOn()
    {
        floating = true;
    }

    private bool hitsUI()
    {
        return Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, 10, LayerMask.GetMask("UI"));
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

        if (state == 1)
        {  
            //for reasons this does work but comparing indices does not?
            if (objectprefab.Equals(PreviewPrefabs[objectid]))
            {
                line.gameObject.SetActive(false);
                state = 0;
                return;
            }
        }

        line.gameObject.SetActive(true);
        state = 1;
        objectprefab = PreviewPrefabs[objectid];

        objectPreview[0] = Instantiate(objectprefab, loc.transform);

        objectPreview[0].layer = LayerMask.NameToLayer("Ignore Raycast");
        //hope they have no recursive children...
        for (int i = 0; i < objectPreview[0].transform.childCount; i++)
        {
            objectPreview[0].transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        ScaleFactor = 1.0f;      
    }

    public void deleteAllSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
    }

    public void setEraseState()
    {
        Destroy(objectPreview[0]);
        state = state == 2 ? 0 : 2;
    }
}
