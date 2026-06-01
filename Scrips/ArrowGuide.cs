using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns and manages 3D arrows along the NavMesh navigation path.
/// Attach to a GameObject called "ArrowGuide" in the scene.
/// Assign an Arrow prefab (a 3D arrow pointing forward on Z axis).
/// </summary>
public class ArrowGuide : MonoBehaviour
{
    public static ArrowGuide Instance { get; private set; }

    [Header("Arrow Settings")]
    [Tooltip("Prefab of the 3D arrow. Should point forward (Z+). Create a simple arrow in the scene.")]
    public GameObject arrowPrefab;

    [Tooltip("Distance between each arrow along the path")]
    public float arrowSpacing = 0.6f;

    [Tooltip("Height offset above the floor")]
    public float heightOffset = 0.05f;

    [Tooltip("Scale of each arrow")]
    public Vector3 arrowScale = new Vector3(0.3f, 0.3f, 0.3f);

    private List<GameObject> activeArrows = new List<GameObject>();
    private bool isActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void OnEnable()
    {
        // Subscribe to navigation events
        if (NavigationManager.Instance != null)
        {
            NavigationManager.Instance.OnPathUpdated += DrawArrows;
            NavigationManager.Instance.OnNavigationStopped += ClearArrows;
            NavigationManager.Instance.OnArrived += ClearArrows;
        }
    }

    void OnDisable()
    {
        if (NavigationManager.Instance != null)
        {
            NavigationManager.Instance.OnPathUpdated -= DrawArrows;
            NavigationManager.Instance.OnNavigationStopped -= ClearArrows;
            NavigationManager.Instance.OnArrived -= ClearArrows;
        }
    }

    /// <summary>
    /// Draw arrows along the given path corners.
    /// </summary>
    public void DrawArrows(Vector3[] pathCorners)
    {
        ClearArrows();

        if (arrowPrefab == null)
        {
            Debug.LogWarning("[ArrowGuide] No arrow prefab assigned!");
            return;
        }

        if (pathCorners == null || pathCorners.Length < 2) return;

        // Walk along path segments and place arrows at intervals
        float distanceCovered = 0f;
        float nextArrowAt = arrowSpacing / 2f; // Start halfway in

        for (int i = 0; i < pathCorners.Length - 1; i++)
        {
            Vector3 segStart = pathCorners[i];
            Vector3 segEnd = pathCorners[i + 1];
            float segLength = Vector3.Distance(segStart, segEnd);
            Vector3 segDir = (segEnd - segStart).normalized;

            while (distanceCovered + segLength >= nextArrowAt)
            {
                float t = nextArrowAt - distanceCovered;
                Vector3 arrowPos = segStart + segDir * t;
                arrowPos.y += heightOffset;

                // Rotate to point along path direction (flat on floor)
                Quaternion arrowRot = Quaternion.LookRotation(
                    new Vector3(segDir.x, 0, segDir.z),
                    Vector3.up
                );

                GameObject arrow = Instantiate(arrowPrefab, arrowPos, arrowRot);
                arrow.transform.localScale = arrowScale;
                activeArrows.Add(arrow);

                nextArrowAt += arrowSpacing;
            }

            distanceCovered += segLength;
        }

        isActive = true;
    }

    /// <summary>
    /// Remove all arrows from the scene.
    /// </summary>
    public void ClearArrows()
    {
        foreach (var arrow in activeArrows)
            if (arrow != null) Destroy(arrow);

        activeArrows.Clear();
        isActive = false;
    }

    public bool IsActive => isActive;
    public int ArrowCount => activeArrows.Count;

    // ── Fallback: create a primitive arrow if no prefab is assigned ──────────
    // Call this from Start() if you don't have a prefab yet, for testing.
    public GameObject CreateDefaultArrowPrefab()
    {
        GameObject root = new GameObject("Arrow_Default");

        // Body (cylinder)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        body.transform.SetParent(root.transform);
        body.transform.localPosition = new Vector3(0, 0, 0);
        body.transform.localScale = new Vector3(0.05f, 0.2f, 0.05f);
        body.transform.localEulerAngles = new Vector3(90, 0, 0);

        // Head (cone-like using a scaled capsule)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0, 0, 0.25f);
        head.transform.localScale = new Vector3(0.15f, 0.15f, 0.2f);

        // Color it green
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0f, 0.8f, 0.2f);
        body.GetComponent<Renderer>().material = mat;
        head.GetComponent<Renderer>().material = mat;

        return root;
    }
}
