using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// UI Manager - Gestiona todos los paneles y navegación
/// Se asigna automáticamente por UIBuilder
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Paneles")]
    public GameObject panelLogin;
    public GameObject panelMenu;
    public GameObject panelComputadores;
    public GameObject panelSalas;
    public GameObject panelDisponibilidad;
    public GameObject panelNotificaciones;
    public GameObject panelRecorrido;
    public GameObject panelReservas;

    [Header("Bottom Navigation")]
    public Button btnNavInicio;
    public Button btnNavReservas;
    public Button btnNavNotificacion;
    public Button btnNavSalir;

    [Header("Menu - Servicios Grid")]
    public Button btnServicios_Computadores;
    public Button btnServicios_Salas;
    public Button btnServicios_Recorrido;
    public Button btnServicios_Preguntas;

    [Header("Computadores - Botones")]
    public Transform computadoresGrid;
    public Button[] computadoresButtons = new Button[10];

    [Header("Salas - Botones")]
    public Transform salasGrid;
    public Button[] salasButtons = new Button[6];

    [Header("Buttons - Volver")]
    public Button btnVolverComputadores;
    public Button btnVolverSalas;
    public Button btnVolverDisponibilidad;
    public Button btnVolverNotificaciones;
    public Button btnVolverReservas;

    void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
        { 
            Destroy(gameObject); 
            return; 
        }
    }

    void Start()
    {
        ConnectAllButtons();
        ShowPanel(panelLogin);
        Debug.Log("[UIManager] UI initialized.");
    }

    void ConnectAllButtons()
    {
        // Bottom Navigation
        if (btnNavInicio != null) btnNavInicio.onClick.AddListener(() => ShowPanel(panelMenu));
        if (btnNavReservas != null) btnNavReservas.onClick.AddListener(() => ShowPanel(panelReservas));
        if (btnNavNotificacion != null) btnNavNotificacion.onClick.AddListener(() => ShowPanel(panelNotificaciones));
        if (btnNavSalir != null) btnNavSalir.onClick.AddListener(OnClickSalir);

        // Menu - Servicios
        if (btnServicios_Computadores != null) btnServicios_Computadores.onClick.AddListener(() => ShowComputadoresPanel());
        if (btnServicios_Salas != null) btnServicios_Salas.onClick.AddListener(() => ShowSalasPanel());
        if (btnServicios_Recorrido != null) btnServicios_Recorrido.onClick.AddListener(() => ShowPanel(panelRecorrido));
        if (btnServicios_Preguntas != null) btnServicios_Preguntas.onClick.AddListener(() => Debug.Log("[UIManager] Preguntas y sugerencias seleccionadas"));

        // Volver buttons
        if (btnVolverComputadores != null) btnVolverComputadores.onClick.AddListener(() => ShowPanel(panelMenu));
        if (btnVolverSalas != null) btnVolverSalas.onClick.AddListener(() => ShowPanel(panelMenu));
        if (btnVolverDisponibilidad != null) btnVolverDisponibilidad.onClick.AddListener(() => ShowPanel(panelMenu));
        if (btnVolverNotificaciones != null) btnVolverNotificaciones.onClick.AddListener(() => ShowPanel(panelMenu));
        if (btnVolverReservas != null) btnVolverReservas.onClick.AddListener(() => ShowPanel(panelMenu));

        // Computadores (1-10)
        for (int i = 0; i < 10; i++)
        {
            int computerNumber = i + 1;
            if (computadoresButtons[i] != null)
                computadoresButtons[i].onClick.AddListener(() => OnComputadorSelected(computerNumber));
        }

        // Salas (1-6)
        for (int i = 0; i < 6; i++)
        {
            int roomNumber = i + 1;
            if (salasButtons[i] != null)
                salasButtons[i].onClick.AddListener(() => OnSalaSelected(roomNumber));
        }
    }

    void ShowPanel(GameObject target)
    {
        GameObject[] allPanels = {
            panelLogin, panelMenu, panelComputadores, panelSalas,
            panelDisponibilidad, panelNotificaciones, panelRecorrido, panelReservas
        };

        foreach (var p in allPanels)
            if (p != null) p.SetActive(false);

        if (target != null) target.SetActive(true);
    }

    void ShowComputadoresPanel()
    {
        ShowPanel(panelComputadores);
        RefreshComputadoresAvailability();
    }

    void ShowSalasPanel()
    {
        ShowPanel(panelSalas);
        RefreshSalasAvailability();
    }

    void RefreshComputadoresAvailability()
    {
        // Si SpaceManager existe, usa datos reales
        // Por ahora solo habilita todos
        for (int i = 0; i < 10; i++)
        {
            if (computadoresButtons[i] != null)
            {
                computadoresButtons[i].interactable = true;
                Image img = computadoresButtons[i].GetComponent<Image>();
                if (img != null)
                    img.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Disponible
            }
        }
    }

    void RefreshSalasAvailability()
    {
        // Si SpaceManager existe, usa datos reales
        // Por ahora solo habilita todos
        for (int i = 0; i < 6; i++)
        {
            if (salasButtons[i] != null)
            {
                salasButtons[i].interactable = true;
                Image img = salasButtons[i].GetComponent<Image>();
                if (img != null)
                    img.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Disponible
            }
        }
    }

    void OnComputadorSelected(int computerNumber)
    {
        string computerId = "PC" + computerNumber;
        Debug.Log($"[UIManager] Selected computer: {computerId}");
        
        // Intenta navegar si NavigationManager existe
        if (NavigationManager.Instance != null)
            NavigationManager.Instance.NavigateTo(computerId);
        
        ShowPanel(panelRecorrido);
    }

    void OnSalaSelected(int roomNumber)
    {
        string roomId = "Sala" + roomNumber;
        Debug.Log($"[UIManager] Selected room: {roomId}");
        
        // Intenta navegar si NavigationManager existe
        if (NavigationManager.Instance != null)
            NavigationManager.Instance.NavigateTo(roomId);
        
        ShowPanel(panelRecorrido);
    }

    void OnClickSalir()
    {
        Debug.Log("[UIManager] Cerrando sesión...");
        ShowPanel(panelLogin);
    }

    public void ShowLoginPanel() => ShowPanel(panelLogin);
    public void ShowMenuPanel() => ShowPanel(panelMenu);
}
