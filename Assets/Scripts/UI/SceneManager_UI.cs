using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script de Gestión de Escenas (UI): Maneja todos los botones del Canvas
/// Proporciona dos funciones principales:
/// 1. Cambiar de escena simplemente
/// 2. Cambiar de escena guardando un destino específico
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
    /// Función simple: Cambiar a una escena por nombre
    /// Uso: Button > On Click () > SceneManager_UI.GoToScene(string)
    /// Parámetro: nombre de la escena (ej: "06-Recorrido")
    /// </summary>
    public void GoToScene(string sceneName)
    {
        Debug.Log($"[SceneManager_UI] Ir a escena: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Función con destino: Selecciona un destino Y cambia de escena
    /// Uso: Button > On Click () > SceneManager_UI.SelectDestinationAndGo(string)
    /// Parámetro: nombre único del destino (ej: "Computador_1" o "Sala_2")
    /// 
    /// Nota: El tipo se detecta automáticamente por prefijo:
    /// - "Computador_" → tipo "Computador"
    /// - "Sala_" → tipo "Sala"
    /// - "Recepcion" → tipo "Recepcion"
    /// </summary>
    public void SelectDestinationAndGo(string destinationID)
    {
        // Detectar tipo automáticamente
        string type = DetectDestinationType(destinationID);
        
        // Guardar el destino
        DestinationManager.Instance.SetDestination(destinationID, type);
        
        // Cambiar a la escena de recorrido
        GoToScene("06-Recorrido");
    }

    /// <summary>
    /// Función alternativa: Si prefieres especificar el tipo explícitamente
    /// </summary>
    public void SelectDestinationWithTypeAndGo(string destinationID, string destinationType)
    {
        DestinationManager.Instance.SetDestination(destinationID, destinationType);
        GoToScene("06-Recorrido");
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
    /// Cerrar sesión (limpiar datos y volver a login)
    /// </summary>
    public void LogOut()
    {
        Debug.Log("[SceneManager_UI] Cerrando sesión");
        DestinationManager.Instance.ClearDestination();
        GoToScene("00-Inicio de sesion");
    }

    /// <summary>
    /// Detecta el tipo de destino por su nombre
    /// </summary>
    private string DetectDestinationType(string destinationID)
    {
        if (destinationID.StartsWith("Computador_"))
            return "Computador";
        else if (destinationID.StartsWith("Sala_"))
            return "Sala";
        else if (destinationID.StartsWith("Recepcion"))
            return "Recepcion";
        else
            return "Desconocido";
    }
}
