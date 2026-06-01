using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controlador de demo para la presentación del proyecto.
/// Simula cambios de disponibilidad en tiempo real.
/// Agrega este script al AppManager y asigna el Canvas.
/// 
/// Teclas durante el juego (o botones en pantalla):
///   D = Modo Demo automático (cambia disponibilidad cada N segundos)
///   1-9 = Toggle disponibilidad PC1-PC9
///   R = Reset todo a disponible
/// </summary>
public class DemoController : MonoBehaviour
{
    [Header("Modo Demo Automático")]
    public bool demoAutoActivo = false;
    public float intervaloCambio = 4f; // Segundos entre cambios automáticos

    [Header("UI de Demo (opcional)")]
    public GameObject panelDemo;         // Panel overlay con botones de demo
    public TextMeshProUGUI txtEstado;     // Texto que muestra cambios en tiempo real

    private float timerDemo = 0f;
    private int indexActual = 0;
    private string[] todosLosIds;

    void Start()
    {
        // Construir lista de todos los IDs
        var lista = new System.Collections.Generic.List<string>();
        for (int i = 1; i <= 10; i++) lista.Add("PC" + i);
        for (int i = 1; i <= 6; i++) lista.Add("Sala" + i);
        todosLosIds = lista.ToArray();

        // Ocultar panel demo al inicio
        if (panelDemo != null) panelDemo.SetActive(false);
    }

    void Update()
    {
        // Teclas rápidas para la demo
        if (Input.GetKeyDown(KeyCode.D))
            ToggleDemoAuto();

        if (Input.GetKeyDown(KeyCode.R))
            ResetearTodo();

        // Teclas 1-9 para toggle rápido de PCs
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                ToggleEspacio("PC" + i);
        }

        // Demo automático
        if (demoAutoActivo)
        {
            timerDemo += Time.deltaTime;
            if (timerDemo >= intervaloCambio)
            {
                timerDemo = 0f;
                CambiarSiguienteAleatorio();
            }
        }
    }

    /// <summary>Activa/desactiva el modo demo automático.</summary>
    public void ToggleDemoAuto()
    {
        demoAutoActivo = !demoAutoActivo;
        MostrarMensaje(demoAutoActivo
            ? "🟢 Demo automático ACTIVADO"
            : "⏸ Demo automático pausado");
        Debug.Log("[Demo] Modo automático: " + demoAutoActivo);
    }

    /// <summary>Cambia el estado de un espacio por su ID.</summary>
    public void ToggleEspacio(string id)
    {
        if (SpaceManager.Instance == null) return;
        SpaceManager.Space space = SpaceManager.Instance.GetSpace(id);
        if (space == null) return;

        SpaceManager.Instance.ToggleAvailability(id);
        string estado = space.isAvailable ? "🟢 LIBRE" : "🔴 OCUPADO";
        MostrarMensaje(space.displayName + " → " + estado);
    }

    /// <summary>Cambia un espacio aleatorio (para demo automático).</summary>
    void CambiarSiguienteAleatorio()
    {
        if (todosLosIds == null || todosLosIds.Length == 0) return;
        string id = todosLosIds[Random.Range(0, todosLosIds.Length)];
        ToggleEspacio(id);
    }

    /// <summary>Pone todos los espacios como disponibles.</summary>
    public void ResetearTodo()
    {
        if (SpaceManager.Instance == null) return;
        foreach (var s in SpaceManager.Instance.GetAllComputers())
            SpaceManager.Instance.SetAvailability(s.id, true);
        foreach (var s in SpaceManager.Instance.GetAllStudyRooms())
            SpaceManager.Instance.SetAvailability(s.id, true);
        MostrarMensaje("✅ Todo reseteado a DISPONIBLE");
    }

    /// <summary>Ocupa todos los espacios (para demo de problema).</summary>
    public void OcuparTodo()
    {
        if (SpaceManager.Instance == null) return;
        foreach (var s in SpaceManager.Instance.GetAllComputers())
            SpaceManager.Instance.SetAvailability(s.id, false);
        foreach (var s in SpaceManager.Instance.GetAllStudyRooms())
            SpaceManager.Instance.SetAvailability(s.id, false);
        MostrarMensaje("🔴 Todo marcado como OCUPADO");
    }

    /// <summary>Muestra/oculta el panel de controles de demo.</summary>
    public void TogglePanelDemo()
    {
        if (panelDemo != null)
            panelDemo.SetActive(!panelDemo.activeSelf);
    }

    void MostrarMensaje(string msg)
    {
        Debug.Log("[Demo] " + msg);
        if (txtEstado != null)
        {
            txtEstado.text = msg;
            CancelInvoke(nameof(LimpiarMensaje));
            Invoke(nameof(LimpiarMensaje), 2.5f);
        }
    }

    void LimpiarMensaje()
    {
        if (txtEstado != null) txtEstado.text = "";
    }

    // ── Botones UI que se pueden conectar desde el Inspector ──
    public void OnBtnPC1()  => ToggleEspacio("PC1");
    public void OnBtnPC2()  => ToggleEspacio("PC2");
    public void OnBtnPC3()  => ToggleEspacio("PC3");
    public void OnBtnPC4()  => ToggleEspacio("PC4");
    public void OnBtnPC5()  => ToggleEspacio("PC5");
    public void OnBtnSala1() => ToggleEspacio("Sala1");
    public void OnBtnSala2() => ToggleEspacio("Sala2");
    public void OnBtnSala3() => ToggleEspacio("Sala3");
}
