using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles NavMesh pathfinding from user position to selected destination.
/// Attach to the "AppManager" GameObject. Requires NavMesh baked on scene.
/// </summary>
public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance { get; private set; }

    [Header("Navigation Settings")]
    [Tooltip("How close the user must be to consider 'arrived' (meters)")]
    public float arrivalDistance = 1.0f;

    [Tooltip("How often to recalculate the path (seconds)")]
    public float pathUpdateInterval = 0.5f;

    [Header("User Position")]
    [Tooltip("Transform representing the user's current position. Assign AR Camera here.")]
    public Transform userTransform;

    // Current navigation state
    private string currentDestinationId;
    private Transform currentDestinationTransform;
    private NavMeshPath currentPath;
    private bool isNavigating = false;
    private float pathUpdateTimer = 0f;

    // Events
    public System.Action<Vector3[]> OnPathUpdated;   // Called when path recalculates
    public System.Action OnArrived;                   // Called when user reaches destination
    public System.Action OnNavigationStarted;
    public System.Action OnNavigationStopped;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        currentPath = new NavMeshPath();
    }

    void Update()
    {
        if (!isNavigating || userTransform == null || currentDestinationTransform == null)
            return;

        // Check arrival
        float dist = Vector3.Distance(userTransform.position, currentDestinationTransform.position);
        if (dist <= arrivalDistance)
        {
            Arrive();
            return;
        }

        // Recalculate path periodically
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            pathUpdateTimer = 0f;
            RecalculatePath();
        }
    }

    /// <summary>
    /// Start navigation to a destination by Space ID.
    /// </summary>
    public void NavigateTo(string spaceId)
    {
        SpaceManager.Space space = SpaceManager.Instance?.GetSpace(spaceId);
        if (space == null)
        {
            Debug.LogWarning($"[NavigationManager] Space '{spaceId}' not found.");
            return;
        }

        if (space.locationTransform == null)
        {
            Debug.LogWarning($"[NavigationManager] Space '{spaceId}' has no location assigned in Inspector.");
            return;
        }

        currentDestinationId = spaceId;
        currentDestinationTransform = space.locationTransform;
        isNavigating = true;
        pathUpdateTimer = pathUpdateInterval; // Force immediate recalculation

        OnNavigationStarted?.Invoke();
        Debug.Log($"[NavigationManager] Navigating to {space.displayName}");
    }

    /// <summary>
    /// Navigate directly to a Transform (use if Space is not in SpaceManager).
    /// </summary>
    public void NavigateToTransform(Transform destination, string label = "destination")
    {
        currentDestinationTransform = destination;
        currentDestinationId = label;
        isNavigating = true;
        pathUpdateTimer = pathUpdateInterval;
        OnNavigationStarted?.Invoke();
    }

    public void StopNavigation()
    {
        isNavigating = false;
        currentDestinationId = null;
        currentDestinationTransform = null;
        currentPath = new NavMeshPath();
        OnNavigationStopped?.Invoke();
    }

    private void RecalculatePath()
    {
        if (userTransform == null || currentDestinationTransform == null) return;

        bool success = NavMesh.CalculatePath(
            userTransform.position,
            currentDestinationTransform.position,
            NavMesh.AllAreas,
            currentPath
        );

        if (success && currentPath.status == NavMeshPathStatus.PathComplete)
        {
            OnPathUpdated?.Invoke(currentPath.corners);
        }
        else
        {
            Debug.LogWarning("[NavigationManager] Path incomplete or failed. Check NavMesh bake.");
        }
    }

    private void Arrive()
    {
        Debug.Log($"[NavigationManager] Arrived at {currentDestinationId}!");
        StopNavigation();
        OnArrived?.Invoke();
    }

    public bool IsNavigating => isNavigating;
    public string CurrentDestination => currentDestinationId;

    // Draw path in Scene view for debugging
    void OnDrawGizmos()
    {
        if (currentPath == null || currentPath.corners.Length < 2) return;
        Gizmos.color = Color.green;
        for (int i = 0; i < currentPath.corners.Length - 1; i++)
            Gizmos.DrawLine(currentPath.corners[i], currentPath.corners[i + 1]);
    }
}
