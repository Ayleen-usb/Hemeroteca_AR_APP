using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script de Navegación en el Área: 
/// Detecta el destino guardado, usa NavMeshAgent para calcular la ruta
/// y coordina con PathVisualizer para mostrar el camino en el suelo.
/// </summary>
public class NavigationController : MonoBehaviour
{
    [Header("Configuración de Navegación")]
    [SerializeField] private float destinationThreshold = 0.5f; // Distancia para considerar que llegó
    [SerializeField] private bool autoStartNavigation = true; // Iniciar navegación automáticamente al entrar

    [Header("Referencias")]
    [SerializeField] private Transform destinationsContainer; // Carpeta con todos los puntos de destino
    [SerializeField] private PathVisualizer pathVisualizer; // Script de visualización del camino

    private NavMeshAgent agent;
    private Transform currentDestination;
    private bool isNavigating = false;

    void Start()
    {
        InitializeNavigation();

        if (autoStartNavigation && DestinationManager.Instance.HasDestination())
        {
            StartNavigation();
        }
    }

    void Update()
    {
        if (!isNavigating || agent == null)
            return;

        // Verificar si llegó al destino
        if (!agent.pathPending && agent.remainingDistance <= destinationThreshold)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                OnDestinationReached();
            }
        }
    }

    /// <summary>
    /// Inicializa el NavMeshAgent y PathVisualizer
    /// </summary>
    private void InitializeNavigation()
    {
        // Obtener o crear NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            Debug.Log("[NavigationController] NavMeshAgent creado");
        }

        // Obtener o crear PathVisualizer
        if (pathVisualizer == null)
        {
            pathVisualizer = GetComponent<PathVisualizer>();
            if (pathVisualizer == null)
            {
                pathVisualizer = gameObject.AddComponent<PathVisualizer>();
                Debug.Log("[NavigationController] PathVisualizer creado");
            }
        }

        // Buscar contenedor de destinos si no está asignado
        if (destinationsContainer == null)
        {
            destinationsContainer = transform.Find("Destinations");
            if (destinationsContainer == null)
            {
                Debug.LogWarning("[NavigationController] Carpeta 'Destinations' no encontrada en Hierarchy");
            }
        }

        Debug.Log("[NavigationController] Inicialización completada");
    }

    /// <summary>
    /// Inicia la navegación hacia el destino guardado
    /// </summary>
    public void StartNavigation()
    {
        if (!DestinationManager.Instance.HasDestination())
        {
            Debug.LogWarning("[NavigationController] No hay destino seleccionado");
            return;
        }

        string destinationID = DestinationManager.Instance.GetDestinationID();
        NavigateToDestination(destinationID);
    }

    /// <summary>
    /// Navega hacia un destino específico
    /// </summary>
    public void NavigateToDestination(string destinationID)
    {
        if (destinationsContainer == null)
        {
            Debug.LogError("[NavigationController] Contenedor de destinos no asignado");
            return;
        }

        // Buscar el punto de destino
        Transform destination = destinationsContainer.Find(destinationID);

        if (destination == null)
        {
            Debug.LogError($"[NavigationController] Destino no encontrado: {destinationID}");
            return;
        }

        NavigateToTransform(destination);
    }

    /// <summary>
    /// Navega hacia un Transform específico
    /// </summary>
    public void NavigateToTransform(Transform destinationTransform)
    {
        if (agent == null)
        {
            Debug.LogError("[NavigationController] NavMeshAgent no inicializado");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("[NavigationController] NavMeshAgent no está en NavMesh");
            return;
        }

        currentDestination = destinationTransform;
        agent.SetDestination(destinationTransform.position);
        isNavigating = true;

        Debug.Log($"[NavigationController] Navegando a: {destinationTransform.name} ({destinationTransform.position})");
    }

    /// <summary>
    /// Se llama cuando se alcanza el destino
    /// </summary>
    private void OnDestinationReached()
    {
        isNavigating = false;
        
        if (currentDestination != null)
        {
            Debug.Log($"[NavigationController] ¡Destino alcanzado: {currentDestination.name}!");
        }

        // Limpiar visualización del camino
        if (pathVisualizer != null)
        {
            pathVisualizer.ClearPath();
        }

        // Aquí puedes añadir eventos o animaciones cuando se llega al destino
        OnNavigationComplete();
    }

    /// <summary>
    /// Evento que se dispara cuando se completa la navegación
    /// Puedes sobrescribir esto en subclases o conectar eventos
    /// </summary>
    protected virtual void OnNavigationComplete()
    {
        // Aquí puedes añadir sonidos, animaciones, etc.
    }

    /// <summary>
    /// Detiene la navegación actual
    /// </summary>
    public void StopNavigation()
    {
        if (agent != null)
        {
            agent.ResetPath();
        }

        isNavigating = false;

        if (pathVisualizer != null)
        {
            pathVisualizer.ClearPath();
        }

        Debug.Log("[NavigationController] Navegación detenida");
    }

    /// <summary>
    /// Obtiene si está navegando actualmente
    /// </summary>
    public bool IsNavigating()
    {
        return isNavigating;
    }

    /// <summary>
    /// Obtiene la distancia restante al destino
    /// </summary>
    public float GetRemainingDistance()
    {
        if (agent != null && agent.hasPath)
        {
            return agent.remainingDistance;
        }
        return -1;
    }

    /// <summary>
    /// Obtiene el destino actual
    /// </summary>
    public Transform GetCurrentDestination()
    {
        return currentDestination;
    }
}
