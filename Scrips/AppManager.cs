using UnityEngine;
#if VUFORIA_PRESENT
using Vuforia;
#endif

/// <summary>
/// Main app manager. Handles AR initialization and app lifecycle.
/// Attach to a GameObject called "AppManager" in the scene.
/// This same GameObject should also have SpaceManager and NavigationManager.
/// </summary>
public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    [Header("AR Camera")]
    [Tooltip("Assign the AR Camera (Vuforia's ARCamera prefab) here")]
    public Camera arCamera;

    [Header("Hemeroteca Model")]
    [Tooltip("The imported hemeroteca2.fbm model in the scene")]
    public GameObject hemerotecaModel;

    [Header("Debug")]
    public bool enableDebugLogs = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        InitVuforia();
        Log("App started. SpaceManager: " + (SpaceManager.Instance != null ? "OK" : "MISSING"));
        Log("NavigationManager: " + (NavigationManager.Instance != null ? "OK" : "MISSING"));
    }

    void InitVuforia()
    {
#if VUFORIA_PRESENT
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
        VuforiaApplication.Instance.OnVuforiaStopped += OnVuforiaStopped;
        Log("Vuforia initialized.");
#else
        Log("Vuforia not detected. Running in non-AR mode (editor testing).");
#endif
    }

#if VUFORIA_PRESENT
    void OnVuforiaStarted()
    {
        Log("Vuforia started — AR tracking active.");

        // Set the AR Camera as the user transform for navigation
        if (arCamera != null && NavigationManager.Instance != null)
            NavigationManager.Instance.userTransform = arCamera.transform;
    }

    void OnVuforiaStopped()
    {
        Log("Vuforia stopped.");
    }
#endif

    void Log(string msg)
    {
        if (enableDebugLogs)
            Debug.Log("[AppManager] " + msg);
    }
}
