using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
using UnityEditor.Rendering.CustomRenderTexture.ShaderGraph;
using Meta.WitAi;

public class WindowCutter : MonoBehaviour
{

    // Start is called before the first frame update
 
    void Start()
    {
        GameObject wallObject = this.transform.parent.gameObject;
        GameObject wallCube = this.transform.GetChild(0).gameObject;
        MRUKAnchor wallAnchor = wallObject.GetComponent<MRUKAnchor>();
        foreach (MRUKAnchor childAnchor in wallAnchor.ChildAnchors)
        {
            GameObject windowObj = childAnchor.gameObject;
            GameObject windowprefab = windowObj.transform.GetChild(0).gameObject;
            GameObject windowCube = windowprefab.transform.GetChild(0).gameObject;

            Model subtraction = CSG.Subtract(wallCube, windowCube);

            var composite = new GameObject();
            composite.AddComponent<MeshFilter>().sharedMesh = subtraction.mesh;
            composite.AddComponent<MeshRenderer>().sharedMaterials = subtraction.materials.ToArray();

            Destroy(wallCube.gameObject);
            Destroy(windowCube.gameObject);
            composite.transform.SetParent(this.transform);

            wallCube = this.transform.GetChild(1).gameObject;
        }

    }

}
