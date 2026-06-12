using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script de Gestión de Escenas (UI): Maneja todos los botones del Canvas
/// Integra con DestinationManager y NavigationManager
/// </summary>
public class SceneManager_UI : MonoBehaviour
{
    public static SceneManager_UI Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Selecciona un destino y navega a la escena de recorrido
    /// Uso: Button > On Click () > SceneManager_UI.SelectDestinationAndGo(string)
    /// Ejemplo: "PC1" o "Sala1"
    /// </summary>
    public void SelectDestinationAndGo(string spaceId)
    {
        Debug.Log($"[SceneManager_UI] Seleccionando destino: {spaceId}");

        // Guardar en DestinationManager
        DestinationManager.Instance.SetDestination(spaceId, "");

        // Cambiar a escena de recorrido
        GoToScene("06-Recorrido");
    }

    /// <summary>
    /// Cambiar a una escena por nombre
    /// </summary>
    public void GoToScene(string sceneName)
    {
        Debug.Log($"[SceneManager_UI] Ir a escena: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Volver a la escena anterior
    /// </summary>
    public void GoBack()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex > 0)
        {
            Debug.Log($"[SceneManager_UI] Volviendo a escena anterior");
            SceneManager.LoadScene(currentIndex - 1);
        }
        else
        {
            Debug.LogWarning("[SceneManager_UI] No hay escena anterior");
        }
    }

    /// <summary>
    /// Cerrar sesión
    /// </summary>
    public void LogOut()
    {
        Debug.Log("[SceneManager_UI] Cerrando sesión");
        DestinationManager.Instance.ClearDestination();
        NavigationManager.Instance?.StopNavigation();
        GoToScene("00-Inicio de sesion");
    }
}
