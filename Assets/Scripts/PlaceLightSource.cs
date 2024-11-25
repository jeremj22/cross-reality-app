using Meta.WitAi;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceLightSource : MonoBehaviour
{

    [SerializeField, Tooltip("Default: SecondaryIndexTrigger")]
    private OVRInput.Button _button = OVRInput.Button.SecondaryIndexTrigger;
    private OVRInput.Button _deletebutton = OVRInput.Button.Two;



    [SerializeField]
    public GameObject lightprefab;
    public GameObject cursor;
    public LineRenderer rayLinePrefab;
    public Transform origin;
    public float startDist = 0.1f;
    public float lineshowtimer = 1.0f;
    public float collisionRadius = 0.1f;

    private LineRenderer line;
    private GameObject cursorP;
    private Collider[] allColliders;
    private GameObject[] lightsources;
    // Start is called before the first frame update
    void Start()
    {
        line = Instantiate(rayLinePrefab);
        cursorP = Instantiate(cursor);
        line.positionCount = 2;
        allColliders = Physics.OverlapSphere(origin.position, 200);
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, origin.position);
        Vector3 endpoint = origin.position + origin.forward * startDist;
       

        RaycastHit hit;
        if (Physics.Raycast(origin.position, endpoint-origin.position,out hit,startDist))
        {
            endpoint = hit.point;
        }

        cursorP.transform.position = new Vector3(endpoint.x, endpoint.y, endpoint.z);
        line.SetPosition(1, endpoint);
        Collider[] nearColliders = Physics.OverlapSphere(endpoint, collisionRadius);

        if (OVRInput.GetDown(_button))
        {
            //{foreach (Collider collider in nearColliders)
            //{
            //    if (collider.gameObject.tag == "Lightsource")
            //    {
            //        Destroy(collider.gameObject);
            //        nearColliders = Physics.OverlapSphere(endpoint, deleteRadius);
            //    }
            //}}
            InstantiateLight(endpoint);
        }

        lightsources = GameObject.FindGameObjectsWithTag("Lightsource");
        if (OVRInput.GetDown(_deletebutton))
        {
            foreach (GameObject l in lightsources)
            {
               l.SetActive(false);
            }
        }

            Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (joystick.y != 0)
        {
            //if (nearColliders.Length == 0) {
               
            //}
            startDist += joystick.y * 0.05f;
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
