using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// AUTO UI BUILDER - Genera TODO el UI siguiendo el Figma automáticamente
/// Attach a Canvas y presiona Play una sola vez.
/// </summary>
public class UIBuilder : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform canvasRect;
    private UIManager uiManager;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        
        if (canvas == null)
        {
            Debug.LogError("[UIBuilder] Canvas no encontrado");
            return;
        }

        Debug.Log("[UIBuilder] Iniciando construcción del UI...");
        
        BuildUI();
        AssignUIManager();
        
        Debug.Log("[UIBuilder] ✅ UI construido exitosamente. Elimina este script.");
        Destroy(this);
    }

    void BuildUI()
    {
        // Elimina paneles viejos si existen
        DestroyOldPanels();

        // Crea Bottom NavBar (prefab)
        GameObject bottomNavPrefab = CreateBottomNavBar();

        // Crea todos los paneles
        CreatePanelLogin(bottomNavPrefab);
        CreatePanelMenu(bottomNavPrefab);
        CreatePanelComputadores(bottomNavPrefab);
        CreatePanelSalas(bottomNavPrefab);
        CreatePanelDisponibilidad(bottomNavPrefab);
        CreatePanelNotificaciones(bottomNavPrefab);
        CreatePanelRecorrido(bottomNavPrefab);
        CreatePanelReservas(bottomNavPrefab);
    }

    void DestroyOldPanels()
    {
        string[] panelNames = { "PanelLogin", "PanelMenu", "PanelComputadores", "PanelSalas", 
                               "PanelDisponibilidad", "PanelNotificaciones", "PanelRecorrido", "PanelReservas" };
        
        foreach (var name in panelNames)
        {
            Transform old = canvas.transform.Find(name);
            if (old != null) DestroyImmediate(old.gameObject);
        }
    }

    GameObject CreateBottomNavBar()
    {
        GameObject navBar = CreatePanel("BottomNavBar", 0, -540, 1920, 100, new Color(0.8f, 0.8f, 0.8f, 1f));
        navBar.transform.SetParent(canvas.transform, false);

        // Layout
        HorizontalLayoutGroup hlg = navBar.AddComponent<HorizontalLayoutGroup>();
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;
        hlg.spacing = 10;
        hlg.padding = new RectOffset(10, 10, 5, 5);

        // 4 Buttons
        CreateButton(navBar, "btnNavInicio", "Inicio", 0, 0, 400, 90);
        CreateButton(navBar, "btnNavReservas", "Reservas", 420, 0, 400, 90);
        CreateButton(navBar, "btnNavNotificacion", "Notificación", 840, 0, 400, 90);
        CreateButton(navBar, "btnNavSalir", "Salir", 1260, 0, 400, 90);

        return navBar;
    }

    void CreatePanelLogin(GameObject navBarPrefab)
    {
        GameObject panel = CreatePanel("PanelLogin", 0, 0, 1920, 1080, Color.white);
        panel.transform.SetParent(canvas.transform, false);

        // Textos
        CreateText(panel, "Bienvenido", "", 960, 200, 1000, 100, 80, TextAlignmentOptions.Center);
        CreateText(panel, "SubtitleLogin", "¡Es un gusto tenerte de vuelta!", 960, 80, 800, 60, 30, TextAlignmentOptions.Center);

        // Input Fields
        CreateInputField(panel, "InputUsuario", "Nombre de usuario", 960, -100, 600, 80);
        CreateInputField(panel, "InputPassword", "Contraseña", 960, -220, 600, 80);

        // Button Login
        CreateButton(panel, "btnLogin", "Iniciar sesión", 960, -380, 600, 80);

        // BottomNav
        GameObject nav = Instantiate(navBarPrefab, panel.transform);
        nav.name = "BottomNavBar";
    }

    void CreatePanelMenu(GameObject navBarPrefab)
    {
        GameObject panel = CreatePanel("PanelMenu", 0, 0, 1920, 1080, Color.white);
        panel.transform.SetParent(canvas.transform, false);
        panel.SetActive(false);

        // Header
        CreateText(panel, "HeaderMenu", "Hola! ¿Qué necesitas hoy?", 960, 450, 1000, 80, 40, TextAlignmentOptions.Center);

        // Servicios Grid (2x2)
        GameObject serviciosGrid = CreatePanel("ServiciosGrid", 960, 100, 1000, 600, Color.white);
        serviciosGrid.transform.SetParent(panel.transform, false);

        GridLayoutGroup glg = serviciosGrid.AddComponent<GridLayoutGroup>();
        glg.constraintCount = 2;
        glg.constraintType = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.cellSize = new Vector2(400, 250);
        glg.spacing = new Vector2(20, 20);

        CreateButton(serviciosGrid, "btnServicios_Computadores", "Computadores", 0, 0, 400, 250);
        CreateButton(serviciosGrid, "btnServicios_Salas", "Salas de estudio", 0, 0, 400, 250);
        CreateButton(serviciosGrid, "btnServicios_Recorrido", "Recorrido", 0, 0, 400, 250);
        CreateButton(serviciosGrid, "btnServicios_Preguntas", "Preguntas", 0, 0, 400, 250);

        // Card Recorrido Próximo
        GameObject card = CreatePanel("CardRecorrido", 960, -200, 800, 150, new Color(0.9f, 0.9f, 0.9f, 1f));
        card.transform.SetParent(panel.transform, false);
        CreateText(card, "TextoCard", "Recorrido próximo - Computador 20", 0, 0, 700, 100, 25, TextAlignmentOptions.Center);

        // BottomNav
        GameObject nav = Instantiate(navBarPrefab, panel.transform);
        nav.name = "BottomNavBar";
    }

    void CreatePanelComputadores(GameObject navBarPrefab)
    {
        GameObject panel = CreatePanel("PanelComputadores", 0, 0, 1920, 1080, Color.white);
        panel.transform.SetParent(canvas.transform, false);
        panel.SetActive(false);

        // Header con volver
        GameObject header = CreatePanel("Header", 960, 480, 1800, 80, new Color(0.8f, 0.8f, 0.8f, 1f));
        header.transform.SetParent(panel.transform, false);
        
        HorizontalLayoutGroup hlg = header.AddComponent<HorizontalLayoutGroup>();
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        CreateButton(header, "btnVolverComputadores", "←", -900, 0, 80, 80);
        CreateText(header, "TitleComputadores", "Área de computadores", 0, 0, 1000, 80, 35, TextAlignmentOptions.Center);

        // Descripción
        CreateText(panel, "DescComputadores", "Zona equipada con equipos disponibles para consulta, investigación y desarrollo de actividades académicas.", 960, 350, 1600, 80, 20, TextAlignmentOptions.Center);

        // Disponibilidad Button
        CreateButton(panel, "btnDisponibilidad", "Disponibilidad", 960, 250, 400, 60);

        // Guía
        CreateText(panel, "GuiaComputadores", "Presiona y sigue las flechas indicadoras que te guiarán directamente hasta tu destino.", 960, 150, 1600, 80, 18, TextAlignmentOptions.Center);

        // Recepción Button
        CreateButton(panel, "btnRecepcion", "Recepción", 960, 50, 400, 60);

        // Grid de Computadores (5x2)
        GameObject compGrid = CreatePanel("ComputadoresGrid", 960, -150, 1100, 450, Color.white);
        compGrid.transform.SetParent(panel.transform, false);

        GridLayoutGroup ggrid = compGrid.AddComponent<GridLayoutGroup>();
        ggrid.constraintCount = 5;
        ggrid.constraintType = GridLayoutGroup.Constraint.FixedColumnCount;
        ggrid.cellSize = new Vector2(180, 180);
        ggrid.spacing = new Vector2(15, 15);

        for (int i = 1; i <= 10; i++)
        {
            CreateButton(compGrid, $"btnComputador{i}", i.ToString(), 0, 0, 180, 180);
        }

        // BottomNav
        GameObject nav = Instantiate(navBarPrefab, panel.transform);
        nav.name = "BottomNavBar";
    }

    void CreatePanelSalas(GameObject navBarPrefab)
    {
        GameObject panel = CreatePanel("PanelSalas", 0, 0, 1920, 1080, Color.white);
        panel.transform.SetParent(canvas.transform, false);
        panel.SetActive(false);

        // Header
        GameObject header = CreatePanel("Header", 960, 480, 1800, 80, new Color(0.8f, 0.8f, 0.8f, 1f));
        header.transform.SetParent(panel.transform, false);
        CreateButton(header, "btnVolverSalas", "←", -900, 0, 80, 80);
        CreateText(header, "TitleSalas", "Salas de estudio", 0, 0, 1000, 80, 35, TextAlignmentOptions.Center);

        // Descripción
        CreateText(panel, "DescSalas", "Espacios destinados a trabajo colaborativo, asesorías, reuniones, discusión de ideas y actividades en grupo.", 960, 350, 1600, 80, 20, TextAlignmentOptions.Center);

        // Disponibilidad
        CreateButton(panel, "btnDisponibilidad", "Disponibilidad", 960, 250, 400, 60);

        // Guía
        CreateText(panel, "GuiaSalas", "Presiona y sigue las flechas indicadoras que te guiarán directamente hasta tu destino.", 960, 150, 1600, 80, 18, TextAlignmentOptions.Center);

        // Recepción
        CreateButton(panel, "btnRecepcion", "Recepción", 960, 50, 400, 60);

        // Grid Salas (2x3)
        GameObject salasGrid = CreatePanel("SalasGrid", 960, -150, 700, 450, Color.white);
        salasGrid.transform.SetParent(panel.transform, false);

        GridLayoutGroup sgrid = salasGrid.AddComponent<GridLayoutGroup>();
        sgrid.constraintCount = 2;
        sgrid.constraintType = GridLayoutGroup.Constraint.FixedColumnCount;
        sgrid.cellSize = new Vector2(250, 150);
        sgrid.spacing = new Vector2(20, 20);

        for (int i = 1; i <= 6; i++)
        {
            CreateButton(salasGrid, $"btnSala{i}", $"Sala {i}", 0, 0, 250, 150);
        }

        // BottomNav
        GameObject nav = Instantiate(navBarPrefab, panel.transform);
        nav.name = "BottomNavBar";
    }

    void CreatePanelDisponibilidad(GameObject navBarPrefab)
    {
        GameObject panel = CreatePanel("PanelDisponibilidad", 0, 0, 1920, 1080, Color.white);
        panel.transform.SetParent(canvas.transform, false);
        panel.SetActive(false);

        CreateButton(panel, "btnVolverDisponibilidad", "←", 100, 480, 80, 80);
        CreateText(panel, "TitleDisponibilidad", "Disponibilidad", 960, 450, 600, 80, 40, TextAlignmentOptions.Center);

        // BottomNav
        GameObject nav = Instantiate(navBarPrefab, panel.transform);
        nav.name = "BottomNavBar";
    }

    void CreatePanelNotificaciones(GameObject navBarPrefab)
    {
        GameObject panel = CreatePanel("PanelNotificaciones", 0, 0, 1920, 1080, Color.white);
        panel.transform.SetParent(canvas.transform, false);
        panel.SetActive(false);

        CreateButton(panel, "btnVolverNotificaciones", "←", 100, 480, 80, 80);
        CreateText(panel, "TitleNotificaciones", "Notificación", 960, 450, 600, 80, 40, TextAlignmentOptions.Center);

        // BottomNav
        GameObject nav = Instantiate(navBarPrefab, panel.transform);
        nav.name = "BottomNavBar";
    }

    void CreatePanelRecorrido(GameObject navBarPrefab)
    {
        GameObject panel = CreatePanel("PanelRecorrido", 0, 0, 1920, 1080, new Color(0.2f, 0.2f, 0.2f, 1f));
        panel.transform.SetParent(canvas.transform, false);
        panel.SetActive(false);

        CreateText(panel, "TitleRecorrido", "Recorrido AR", 960, 450, 600, 80, 40, TextAlignmentOptions.Center);
        CreateText(panel, "InfoRecorrido", "Vista de realidad aumentada activa", 960, 300, 800, 100, 25, TextAlignmentOptions.Center);

        // BottomNav
        GameObject nav = Instantiate(navBarPrefab, panel.transform);
        nav.name = "BottomNavBar";
    }

    void CreatePanelReservas(GameObject navBarPrefab)
    {
        GameObject panel = CreatePanel("PanelReservas", 0, 0, 1920, 1080, Color.white);
        panel.transform.SetParent(canvas.transform, false);
        panel.SetActive(false);

        CreateButton(panel, "btnVolverReservas", "←", 100, 480, 80, 80);
        CreateText(panel, "TitleReservas", "Reservas", 960, 450, 600, 80, 40, TextAlignmentOptions.Center);

        // BottomNav
        GameObject nav = Instantiate(navBarPrefab, panel.transform);
        nav.name = "BottomNavBar";
    }

    // ========== HELPERS ==========

    GameObject CreatePanel(string name, float x, float y, float width, float height, Color color)
    {
        GameObject panel = new GameObject(name);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Image img = panel.AddComponent<Image>();
        img.color = color;

        return panel;
    }

    Button CreateButton(GameObject parent, string name, string text, float x, float y, float width, float height)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent.transform, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 40;
        tmp.color = Color.black;

        return btn;
    }

    TextMeshProUGUI CreateText(GameObject parent, string name, string text, float x, float y, float width, float height, float fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.alignment = alignment;
        tmp.fontSize = fontSize;
        tmp.color = Color.black;

        return tmp;
    }

    void CreateInputField(GameObject parent, string name, string placeholder, float x, float y, float width, float height)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent.transform, false);

        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Image img = inputObj.AddComponent<Image>();
        img.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        InputField input = inputObj.AddComponent<InputField>();
        input.textComponent = CreateText(inputObj, "Placeholder", placeholder, 0, 0, width - 20, height, 30, TextAlignmentOptions.MiddleLeft).GetComponent<TextMeshProUGUI>();
        input.text = "";
    }

    void AssignUIManager()
    {
        UIManager uiManager = canvas.GetComponent<UIManager>();
        if (uiManager == null)
            uiManager = canvas.gameObject.AddComponent<UIManager>();

        // Asigna Paneles
        uiManager.panelLogin = FindPanel("PanelLogin");
        uiManager.panelMenu = FindPanel("PanelMenu");
        uiManager.panelComputadores = FindPanel("PanelComputadores");
        uiManager.panelSalas = FindPanel("PanelSalas");
        uiManager.panelDisponibilidad = FindPanel("PanelDisponibilidad");
        uiManager.panelNotificaciones = FindPanel("PanelNotificaciones");
        uiManager.panelRecorrido = FindPanel("PanelRecorrido");
        uiManager.panelReservas = FindPanel("PanelReservas");

        // Bottom Nav
        uiManager.btnNavInicio = FindButton("btnNavInicio");
        uiManager.btnNavReservas = FindButton("btnNavReservas");
        uiManager.btnNavNotificacion = FindButton("btnNavNotificacion");
        uiManager.btnNavSalir = FindButton("btnNavSalir");

        // Menu Servicios
        uiManager.btnServicios_Computadores = FindButton("btnServicios_Computadores");
        uiManager.btnServicios_Salas = FindButton("btnServicios_Salas");
        uiManager.btnServicios_Recorrido = FindButton("btnServicios_Recorrido");
        uiManager.btnServicios_Preguntas = FindButton("btnServicios_Preguntas");

        // Volver buttons
        uiManager.btnVolverComputadores = FindButton("btnVolverComputadores");
        uiManager.btnVolverSalas = FindButton("btnVolverSalas");
        uiManager.btnVolverDisponibilidad = FindButton("btnVolverDisponibilidad");
        uiManager.btnVolverNotificaciones = FindButton("btnVolverNotificaciones");
        uiManager.btnVolverReservas = FindButton("btnVolverReservas");

        // Computadores Buttons (1-10)
        uiManager.computadoresButtons = new Button[10];
        for (int i = 1; i <= 10; i++)
            uiManager.computadoresButtons[i - 1] = FindButton($"btnComputador{i}");

        // Salas Buttons (1-6)
        uiManager.salasButtons = new Button[6];
        for (int i = 1; i <= 6; i++)
            uiManager.salasButtons[i - 1] = FindButton($"btnSala{i}");

        // Login
        Button btnLogin = FindButton("btnLogin");
        if (btnLogin != null)
            btnLogin.onClick.AddListener(() => uiManager.ShowMenuPanel());

        Debug.Log("[UIBuilder] ✅ UIManager configurado correctamente");
    }

    GameObject FindPanel(string name)
    {
        Transform t = canvas.transform.Find(name);
        return t != null ? t.gameObject : null;
    }

    Button FindButton(string name)
    {
        Button[] allButtons = canvas.GetComponentsInChildren<Button>();
        foreach (var btn in allButtons)
            if (btn.gameObject.name == name)
                return btn;
        return null;
    }
}
