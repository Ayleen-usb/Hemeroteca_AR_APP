using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Controls all UI panels and connects buttons to navigation/availability logic.
/// Follows the wireframe flow: Inicio → Menu → Computadores/Salas → Disponibilidad → Vista AR
/// Attach to a Canvas GameObject.
/// </summary>
public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    // ── Panels ────────────────────────────────────────────────────────────────
    [Header("Panels (assign in Inspector)")]
    public GameObject panelInicio;
    public GameObject panelMenu;
    public GameObject panelComputadores;
    public GameObject panelSalas;
    public GameObject panelDisponibilidad;
    public GameObject panelAR;
    public GameObject panelArrived;

    // ── Computers panel ───────────────────────────────────────────────────────
    [Header("Computers Panel")]
    [Tooltip("Parent transform where computer buttons will be spawned")]
    public Transform computerButtonContainer;
    public GameObject spaceButtonPrefab; // Prefab with Button + TextMeshPro

    // ── Study Rooms panel ─────────────────────────────────────────────────────
    [Header("Study Rooms Panel")]
    public Transform roomButtonContainer;

    // ── AR Panel ──────────────────────────────────────────────────────────────
    [Header("AR Panel")]
    public TextMeshProUGUI txtDestinationLabel;
    public TextMeshProUGUI txtArrivedMessage;
    public Button btnStopNavigation;

    // ── Arrived Panel ─────────────────────────────────────────────────────────
    [Header("Arrived Message")]
    public float arrivedMessageDuration = 3f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // Wire navigation events
        if (NavigationManager.Instance != null)
        {
            NavigationManager.Instance.OnNavigationStarted += OnNavigationStarted;
            NavigationManager.Instance.OnArrived += OnArrived;
            NavigationManager.Instance.OnNavigationStopped += OnNavigationStopped;
        }

        if (btnStopNavigation != null)
            btnStopNavigation.onClick.AddListener(StopNavigation);

        // Build space buttons
        BuildComputerButtons();
        BuildRoomButtons();

        // Start on Inicio
        ShowPanel(panelInicio);
    }

    // ── Panel navigation ──────────────────────────────────────────────────────

    void ShowPanel(GameObject target)
    {
        GameObject[] all = {
            panelInicio, panelMenu, panelComputadores,
            panelSalas, panelDisponibilidad, panelAR, panelArrived
        };
        foreach (var p in all)
            if (p != null) p.SetActive(false);

        if (target != null) target.SetActive(true);
    }

    // Called by "Iniciar Sesión" button on Inicio panel
    public void OnClickLogin() => ShowPanel(panelMenu);

    // Called by "Computadores" button on Menu panel
    public void OnClickComputadores()
    {
        RefreshSpaceButtons(computerButtonContainer, SpaceManager.Instance?.GetAllComputers());
        ShowPanel(panelComputadores);
    }

    // Called by "Salas de Estudio" button on Menu panel
    public void OnClickSalas()
    {
        RefreshSpaceButtons(roomButtonContainer, SpaceManager.Instance?.GetAllStudyRooms());
        ShowPanel(panelSalas);
    }

    // Called by "Recorrido" button on Menu panel
    public void OnClickRecorrido() => ShowPanel(panelDisponibilidad);

    // Back buttons
    public void OnClickBackToMenu() => ShowPanel(panelMenu);
    public void OnClickBackToInicio() => ShowPanel(panelInicio);

    // ── Space buttons ─────────────────────────────────────────────────────────

    void BuildComputerButtons()
    {
        if (computerButtonContainer == null || spaceButtonPrefab == null) return;
        foreach (Transform child in computerButtonContainer) Destroy(child.gameObject);

        var spaces = SpaceManager.Instance?.GetAllComputers();
        if (spaces == null) return;
        BuildButtons(computerButtonContainer, spaces);
    }

    void BuildRoomButtons()
    {
        if (roomButtonContainer == null || spaceButtonPrefab == null) return;
        foreach (Transform child in roomButtonContainer) Destroy(child.gameObject);

        var spaces = SpaceManager.Instance?.GetAllStudyRooms();
        if (spaces == null) return;
        BuildButtons(roomButtonContainer, spaces);
    }

    void BuildButtons(Transform container, List<SpaceManager.Space> spaces)
    {
        foreach (var space in spaces)
        {
            GameObject btn = Instantiate(spaceButtonPrefab, container);
            string capturedId = space.id;

            // Set label text
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = space.displayName + "\n" + (space.isAvailable ? "✓ Libre" : "✗ Ocupado");

            // Set button color
            var image = btn.GetComponent<Image>();
            if (image != null)
                image.color = space.isAvailable
                    ? new Color(0.2f, 0.8f, 0.3f, 1f)   // green = available
                    : new Color(0.9f, 0.3f, 0.2f, 1f);   // red  = occupied

            // Wire click → navigate only if available
            var button = btn.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = space.isAvailable;
                button.onClick.AddListener(() => OnSpaceSelected(capturedId));
            }
        }
    }

    void RefreshSpaceButtons(Transform container, List<SpaceManager.Space> spaces)
    {
        if (container == null || spaces == null) return;
        foreach (Transform child in container) Destroy(child.gameObject);
        BuildButtons(container, spaces);
    }

    // ── Space selected → start navigation ────────────────────────────────────

    void OnSpaceSelected(string spaceId)
    {
        Debug.Log($"[UIController] Selected space: {spaceId}");
        NavigationManager.Instance?.NavigateTo(spaceId);
    }

    // ── Navigation events ─────────────────────────────────────────────────────

    void OnNavigationStarted()
    {
        ShowPanel(panelAR);
        if (txtDestinationLabel != null)
        {
            var space = SpaceManager.Instance?.GetSpace(NavigationManager.Instance?.CurrentDestination);
            txtDestinationLabel.text = space != null
                ? "Navegando a: " + space.displayName
                : "Navegando...";
        }
    }

    void OnArrived()
    {
        ShowPanel(panelArrived);
        if (txtArrivedMessage != null)
            txtArrivedMessage.text = "¡Llegaste a tu destino!";

        Invoke(nameof(OnNavigationStopped), arrivedMessageDuration);
    }

    void OnNavigationStopped() => ShowPanel(panelMenu);

    void StopNavigation()
    {
        NavigationManager.Instance?.StopNavigation();
        ArrowGuide.Instance?.ClearArrows();
        ShowPanel(panelMenu);
    }
}
