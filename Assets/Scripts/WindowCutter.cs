using Meta.XR.MRUtilityKit;
using Parabox.CSG;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class WindowCutter : MonoBehaviour
{

    // Start is called before the first frame update

    void Start()
    {
        GameObject wallObject = this.transform.parent.gameObject;
        GameObject wallCube = this.transform.GetChild(0).gameObject;
        MRUKAnchor wallAnchor = wallObject.GetComponent<MRUKAnchor>();
        foreach (MRUKAnchor childAnchor in wallAnchor.ChildAnchors.Where(IsWindow))
        {
            GameObject windowprefab = childAnchor.transform.GetChild(0).gameObject;
            GameObject windowCube = windowprefab.transform.GetChild(0).gameObject;

            Model subtraction = CSG.Subtract(wallCube, windowCube);

            var composite = new GameObject("WindowCut");
            composite.AddComponent<MeshFilter>().sharedMesh = subtraction.mesh;
            composite.AddComponent<MeshRenderer>().sharedMaterials = subtraction.materials.ToArray();

            Destroy(wallCube);
            Destroy(windowCube);
            composite.transform.SetParent(this.transform);

            FixPivot(composite, subtraction.mesh, wallCube.transform.position);

            wallCube = composite;
        }

    }

    private static bool IsWindow(MRUKAnchor anchor)
        => anchor.Label == MRUKAnchor.SceneLabels.WINDOW_FRAME;

    private static void FixPivot(GameObject target, Mesh mesh, Vector3 targetPivot)
    {
        ProBuilderMesh pbMesh = target.AddComponent<ProBuilderMesh>();
        target.GetComponent<MeshFilter>().sharedMesh = mesh;

        var importer = new MeshImporter(target);
        importer.Import();

        Debug.Log("ProBuilderMesh successfully imported from Unity Mesh.");

        // Convert the target pivot from world space to local space
        Vector3 localPivot = target.transform.InverseTransformPoint(targetPivot);

        // Apply the pivot using ProBuilderMesh's SetPivot method
        pbMesh.SetPivot(localPivot);

        // Refresh the mesh data and apply changes
        pbMesh.ToMesh();
        pbMesh.Refresh();
    }

}
