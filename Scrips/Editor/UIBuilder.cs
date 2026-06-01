using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

/// <summary>
/// Construye toda la UI automáticamente: paneles, botones, textos.
/// Menú: Tools → 🎨 Construir UI Completa
/// </summary>
public class UIBuilder : EditorWindow
{
    [MenuItem("Tools/🎨 Construir UI Completa")]
    public static void ConstruirUI()
    {
        Canvas canvas = FindOrCreateCanvas();
        if (canvas == null) return;

        Transform canvasT = canvas.transform;

        // Limpiar paneles anteriores
        for (int i = canvasT.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(canvasT.GetChild(i).gameObject);

        // Colores del sistema
        Color colorVerde    = new Color(0.13f, 0.77f, 0.37f);
        Color colorRojo     = new Color(0.90f, 0.26f, 0.21f);
        Color colorAzul     = new Color(0.13f, 0.49f, 0.97f);
        Color colorOscuro   = new Color(0.10f, 0.11f, 0.13f, 0.92f);
        Color colorGris     = new Color(0.18f, 0.20f, 0.24f, 0.95f);

        // ── 1. PANEL INICIO ──────────────────────────────────
        GameObject pInicio = CrearPanel(canvasT, "PanelInicio", colorOscuro);
        CrearTexto(pInicio.transform, "TxtBienvenido", "Bienvenido\nHemeroteca UniSimón",
            new Vector2(0, 80), 28, Color.white, FontStyles.Bold);
        CrearTexto(pInicio.transform, "TxtSub", "Sistema de Navegación AR",
            new Vector2(0, 30), 16, new Color(0.7f, 0.7f, 0.7f));
        CrearBoton(pInicio.transform, "BtnEntrar", "ENTRAR", new Vector2(0, -60),
            new Vector2(220, 55), colorAzul, "OnClickLogin");

        // ── 2. PANEL MENÚ PRINCIPAL ──────────────────────────
        GameObject pMenu = CrearPanel(canvasT, "PanelMenu", colorOscuro);
        CrearTexto(pMenu.transform, "TxtMenu", "¿Qué necesitas?",
            new Vector2(0, 140), 22, Color.white, FontStyles.Bold);
        CrearBoton(pMenu.transform, "BtnComputadores", "💻 Computadores",
            new Vector2(-65, 50), new Vector2(200, 70), colorAzul, "OnClickComputadores");
        CrearBoton(pMenu.transform, "BtnSalas", "📚 Salas de Estudio",
            new Vector2(65, 50), new Vector2(200, 70), colorVerde, "OnClickSalas");
        CrearBoton(pMenu.transform, "BtnRecorrido", "🗺 Recorrido General",
            new Vector2(0, -40), new Vector2(200, 60), colorGris, "OnClickRecorrido");

        // ── 3. PANEL COMPUTADORES ────────────────────────────
        GameObject pPC = CrearPanel(canvasT, "PanelComputadores", colorOscuro);
        CrearTexto(pPC.transform, "TxtTituloPC", "Computadores",
            new Vector2(0, 170), 22, Color.white, FontStyles.Bold);
        CrearTexto(pPC.transform, "TxtSubPC", "Selecciona un computador disponible",
            new Vector2(0, 140), 13, new Color(0.7f, 0.7f, 0.7f));

        // Grid de 10 botones (2 columnas x 5 filas)
        GameObject gridPC = CrearScrollView(pPC.transform, "GridPC",
            new Vector2(0, -10), new Vector2(320, 260));
        Transform contentPC = gridPC.transform.Find("Viewport/Content");
        if (contentPC != null)
        {
            for (int i = 1; i <= 10; i++)
            {
                string id = "PC" + i;
                GameObject btn = CrearBotonEspacio(contentPC, id,
                    "Computador " + i, true, colorVerde, colorRojo);
            }
        }
        CrearBoton(pPC.transform, "BtnVolverPC", "← Volver",
            new Vector2(0, -170), new Vector2(160, 45), colorGris, "OnClickBackToMenu");

        // ── 4. PANEL SALAS ───────────────────────────────────
        GameObject pSala = CrearPanel(canvasT, "PanelSalas", colorOscuro);
        CrearTexto(pSala.transform, "TxtTituloSala", "Salas de Estudio",
            new Vector2(0, 170), 22, Color.white, FontStyles.Bold);
        CrearTexto(pSala.transform, "TxtSubSala", "Selecciona una sala disponible",
            new Vector2(0, 140), 13, new Color(0.7f, 0.7f, 0.7f));

        GameObject gridSala = CrearScrollView(pSala.transform, "GridSala",
            new Vector2(0, -10), new Vector2(320, 200));
        Transform contentSala = gridSala.transform.Find("Viewport/Content");
        if (contentSala != null)
        {
            bool[] disponibles = { true, false, true, true, false, true };
            for (int i = 1; i <= 6; i++)
            {
                CrearBotonEspacio(contentSala, "Sala" + i,
                    "Sala " + i, disponibles[i - 1], colorVerde, colorRojo);
            }
        }
        CrearBoton(pSala.transform, "BtnVolverSala", "← Volver",
            new Vector2(0, -170), new Vector2(160, 45), colorGris, "OnClickBackToMenu");

        // ── 5. PANEL AR (NAVEGACIÓN ACTIVA) ──────────────────
        GameObject pAR = CrearPanel(canvasT, "PanelAR", new Color(0, 0, 0, 0.3f));
        GameObject lblDestino = CrearTexto(pAR.transform, "TxtDestino",
            "Navegando a: Computador 1",
            new Vector2(0, 180), 18, Color.white, FontStyles.Bold);
        // Barra de estado superior
        Image barraSuperior = CrearImagen(pAR.transform, "BarraSuperior",
            new Vector2(0, 190), new Vector2(360, 50), colorOscuro);
        CrearBoton(pAR.transform, "BtnDetener", "✕ Detener navegación",
            new Vector2(0, -185), new Vector2(200, 45), colorRojo, "StopNavigation");

        // ── 6. PANEL LLEGADA ─────────────────────────────────
        GameObject pLlegada = CrearPanel(canvasT, "PanelArrived",
            new Color(0.1f, 0.7f, 0.3f, 0.92f));
        CrearTexto(pLlegada.transform, "TxtLlegaste", "✓ ¡Llegaste!",
            new Vector2(0, 30), 32, Color.white, FontStyles.Bold);
        CrearTexto(pLlegada.transform, "TxtMensajeLlegada", "Has llegado a tu destino",
            new Vector2(0, -15), 16, Color.white);

        // ── Activar solo PanelInicio al iniciar ──────────────
        pInicio.SetActive(true);
        pMenu.SetActive(false);
        pPC.SetActive(false);
        pSala.SetActive(false);
        pAR.SetActive(false);
        pLlegada.SetActive(false);

        // ── Asignar paneles al UIController ──────────────────
        UIController uiCtrl = Object.FindObjectOfType<UIController>();
        if (uiCtrl != null)
        {
            uiCtrl.panelInicio         = pInicio;
            uiCtrl.panelMenu           = pMenu;
            uiCtrl.panelComputadores   = pPC;
            uiCtrl.panelSalas          = pSala;
            uiCtrl.panelAR             = pAR;
            uiCtrl.panelArrived        = pLlegada;
            uiCtrl.txtDestinationLabel = lblDestino.GetComponent<TextMeshProUGUI>();
            EditorUtility.SetDirty(uiCtrl);
        }

        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("[UIBuilder] ✅ UI construida y asignada.");
        EditorUtility.DisplayDialog("✅ UI Lista",
            "Todos los paneles fueron creados:\n\n" +
            "• PanelInicio\n• PanelMenu\n• PanelComputadores\n" +
            "• PanelSalas\n• PanelAR\n• PanelArrived\n\n" +
            "Revisa el Canvas en la Hierarchy.", "OK");
    }

    // ── Helpers ───────────────────────────────────────────────

    static Canvas FindOrCreateCanvas()
    {
        Canvas c = Object.FindObjectOfType<Canvas>();
        if (c != null) return c;
        GameObject go = new GameObject("Canvas");
        c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<CanvasScaler>().uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.AddComponent<GraphicRaycaster>();
        return c;
    }

    static GameObject CrearPanel(Transform parent, string nombre, Color color)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        Image img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    static GameObject CrearTexto(Transform parent, string nombre, string texto,
        Vector2 pos, int size, Color color, FontStyles style = FontStyles.Normal)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(300, 60);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = style;
        return go;
    }

    static GameObject CrearBoton(Transform parent, string nombre, string label,
        Vector2 pos, Vector2 size, Color color, string metodo)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Image img = go.AddComponent<Image>();
        img.color = color;
        Button btn = go.AddComponent<Button>();

        // Texto del botón
        GameObject txtGo = new GameObject("Text");
        txtGo.transform.SetParent(go.transform, false);
        RectTransform txtRt = txtGo.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
        TextMeshProUGUI tmp = txtGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 15;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        // Conectar al UIController
        UIController uiCtrl = Object.FindObjectOfType<UIController>();
        if (uiCtrl != null && !string.IsNullOrEmpty(metodo))
        {
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(
                btn.onClick,
                uiCtrl.SendMessage,
                metodo
            );
        }
        return go;
    }

    static GameObject CrearBotonEspacio(Transform parent, string id,
        string label, bool disponible, Color colorLibre, Color colorOcupado)
    {
        Color color = disponible ? colorLibre : colorOcupado;
        string estado = disponible ? "✓ Libre" : "✗ Ocupado";

        GameObject go = new GameObject("Btn_" + id);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(140, 55);
        Image img = go.AddComponent<Image>();
        img.color = color;
        Button btn = go.AddComponent<Button>();
        btn.interactable = disponible;

        GameObject txtGo = new GameObject("Text");
        txtGo.transform.SetParent(go.transform, false);
        RectTransform txtRt = txtGo.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
        TextMeshProUGUI tmp = txtGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label + "\n" + estado;
        tmp.fontSize = 13;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    static GameObject CrearScrollView(Transform parent, string nombre,
        Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        ScrollRect scroll = go.AddComponent<ScrollRect>();
        Image img = go.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.2f);

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(go.transform, false);
        RectTransform vrt = viewport.AddComponent<RectTransform>();
        vrt.anchorMin = Vector2.zero; vrt.anchorMax = Vector2.one;
        vrt.offsetMin = vrt.offsetMax = Vector2.zero;
        viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0);
        viewport.AddComponent<Mask>().showMaskGraphic = false;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform crt = content.AddComponent<RectTransform>();
        crt.anchorMin = new Vector2(0, 1); crt.anchorMax = new Vector2(1, 1);
        crt.pivot = new Vector2(0.5f, 1);
        crt.anchoredPosition = Vector2.zero;
        crt.sizeDelta = new Vector2(0, 400);

        GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(140, 55);
        grid.spacing = new Vector2(8, 8);
        grid.padding = new RectOffset(10, 10, 10, 10);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 2;

        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = vrt;
        scroll.content = crt;
        scroll.horizontal = false;
        scroll.vertical = true;

        return go;
    }

    static Image CrearImagen(Transform parent, string nombre,
        Vector2 pos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Image img = go.AddComponent<Image>();
        img.color = color;
        return img;
    }
}
