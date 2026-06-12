using UnityEngine;

/// <summary>
/// Script de Persistencia: Guarda el ID del destino seleccionado
/// para que no se borre al cambiar entre escenas.
/// Utiliza PlayerPrefs para guardar datos entre sesiones.
/// </summary>
public class DestinationManager : MonoBehaviour
{
    public static DestinationManager Instance { get; private set; }

    private string selectedDestinationID = "";
    private string destinationType = ""; // "Computador", "Sala", "Recepcion"

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Cargar datos guardados
        LoadDestination();
    }

    /// <summary>
    /// Guarda el destino seleccionado
    /// </summary>
    public void SetDestination(string destinationID, string type)
    {
        selectedDestinationID = destinationID;
        destinationType = type;
        
        PlayerPrefs.SetString("SelectedDestinationID", destinationID);
        PlayerPrefs.SetString("DestinationType", type);
        PlayerPrefs.Save();
        
        Debug.Log($"[DestinationManager] Destino guardado: {destinationID} ({type})");
    }

    /// <summary>
    /// Obtiene el ID del destino guardado
    /// </summary>
    public string GetDestinationID()
    {
        return selectedDestinationID;
    }

    /// <summary>
    /// Obtiene el tipo de destino
    /// </summary>
    public string GetDestinationType()
    {
        return destinationType;
    }

    /// <summary>
    /// Verifica si hay un destino seleccionado
    /// </summary>
    public bool HasDestination()
    {
        return !string.IsNullOrEmpty(selectedDestinationID);
    }

    /// <summary>
    /// Carga el destino guardado de PlayerPrefs
    /// </summary>
    private void LoadDestination()
    {
        selectedDestinationID = PlayerPrefs.GetString("SelectedDestinationID", "");
        destinationType = PlayerPrefs.GetString("DestinationType", "");
        
        if (HasDestination())
            Debug.Log($"[DestinationManager] Destino cargado: {selectedDestinationID} ({destinationType})");
    }

    /// <summary>
    /// Limpia el destino guardado
    /// </summary>
    public void ClearDestination()
    {
        selectedDestinationID = "";
        destinationType = "";
        
        PlayerPrefs.DeleteKey("SelectedDestinationID");
        PlayerPrefs.DeleteKey("DestinationType");
        PlayerPrefs.Save();
        
        Debug.Log("[DestinationManager] Destino limpiado");
    }
}
