using UnityEngine;

public class SimpleTreeBuilder : MonoBehaviour
{
    [SerializeField] private bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            CreateTree(transform.position);
        }
    }

    public static GameObject CreateTree(Vector3 position)
    {
        var root = new GameObject("SimpleTree");
        root.transform.position = position;

        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(root.transform);
        trunk.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        trunk.transform.localScale = new Vector3(0.3f, 0.8f, 0.3f);
        var trunkRenderer = trunk.GetComponent<Renderer>();
        trunkRenderer.sharedMaterial.color = new Color(0.45f, 0.28f, 0.12f);

        var leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.name = "Canopy";
        leaves.transform.SetParent(root.transform);
        leaves.transform.localPosition = new Vector3(0f, 2.1f, 0f);
        leaves.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        var leavesRenderer = leaves.GetComponent<Renderer>();
        leavesRenderer.sharedMaterial.color = new Color(0.2f, 0.7f, 0.25f);

        return root;
    }
}
