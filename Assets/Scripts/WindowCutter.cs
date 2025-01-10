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
        var windows = wallAnchor.ChildAnchors.Where(IsWindow);
        foreach (MRUKAnchor childAnchor in windows)
        {
            GameObject windowPrefab = childAnchor.transform.GetChild(0).gameObject;
            GameObject windowCube = windowPrefab.transform.GetChild(0).gameObject;

            Model subtraction = CSG.Subtract(wallCube, windowCube);

            var composite = new GameObject("WindowCut");

            FixPivot(composite, subtraction, wallCube.transform.position);

            Destroy(wallCube);
            Destroy(windowCube);

            wallCube = composite;
        }

        if (windows.Any())
            wallCube.transform.SetParent(wallAnchor.Room.transform);

    }

    private static bool IsWindow(MRUKAnchor anchor)
        => anchor.Label == MRUKAnchor.SceneLabels.WINDOW_FRAME;

    private static void FixPivot(GameObject target, Model model, Vector3 targetPivot)
    {
        ProBuilderMesh pbMesh = target.AddComponent<ProBuilderMesh>();

        // Adding ProBuilderMesh also already puts those 2 there
        target.GetComponent<MeshFilter>().sharedMesh = model.mesh;
        target.GetComponent<MeshRenderer>().sharedMaterials = model.materials.ToArray();

        var importer = new MeshImporter(target);
        importer.Import();

        // Convert the target pivot from world space to local space
        Vector3 localPivot = target.transform.InverseTransformPoint(targetPivot);

        // Apply the pivot using ProBuilderMesh's SetPivot method
        pbMesh.SetPivot(localPivot);

        // Refresh the mesh data and apply changes
        pbMesh.ToMesh();
        pbMesh.Refresh();
    }

}
