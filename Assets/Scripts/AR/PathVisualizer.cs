using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script de Visualización de Camino: Dibuja la ruta calculada por NavMesh
/// en el suelo usando LineRenderer.
/// Este script trabaja en conjunto con NavigationController.
/// </summary>
public class PathVisualizer : MonoBehaviour
{
    [Header("Configuración del Camino")]
    [SerializeField] private LineRenderer pathLineRenderer;
    [SerializeField] private float lineWidth = 0.15f;
    [SerializeField] private Color lineColor = new Color(0, 0.5f, 1, 0.8f); // Azul semitransparente
    [SerializeField] private bool drawArrows = true;
    [SerializeField] private float arrowSpacing = 1f;

    [Header("Posición del Suelo")]
    [SerializeField] private float groundHeightOffset = 0.02f; // Pequeño offset para evitar z-fighting

    private NavMeshAgent agent;
    private bool isNavigating = false;

    void Start()
    {
        // Obtener NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("[PathVisualizer] NavMeshAgent no encontrado en el mismo GameObject");
            return;
        }

        // Crear o configurar LineRenderer
        if (pathLineRenderer == null)
        {
            pathLineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        ConfigureLineRenderer();
    }

    void Update()
    {
        if (agent == null || !agent.hasPath)
        {
            if (isNavigating)
            {
                ClearPath();
                isNavigating = false;
            }
            return;
        }

        // Dibujar el camino
        if (agent.velocity.sqrMagnitude > 0.01f || agent.pathPending)
        {
            DrawPath();
            isNavigating = true;
        }
        else if (isNavigating)
        {
            // Se detuvo de navegar
            ClearPath();
            isNavigating = false;
        }
    }

    /// <summary>
    /// Dibuja la ruta actual del NavMeshAgent en el LineRenderer
    /// </summary>
    private void DrawPath()
    {
        NavMeshPath path = agent.path;
        
        if (path.corners.Length < 2)
        {
            ClearPath();
            return;
        }

        // Configurar número de puntos
        pathLineRenderer.positionCount = path.corners.Length;

        // Dibujar puntos del camino
        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 cornerPos = path.corners[i];
            // Añadir offset de altura para evitar que se vea bajo el terreno
            cornerPos.y += groundHeightOffset;
            pathLineRenderer.SetPosition(i, cornerPos);
        }

        // Dibujar flechas de dirección si está habilitado
        if (drawArrows)
        {
            DrawArrows(path.corners);
        }
    }

    /// <summary>
    /// Dibuja flechas a lo largo del camino para indicar dirección
    /// </summary>
    private void DrawArrows(Vector3[] corners)
    {
        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 start = corners[i] + Vector3.up * groundHeightOffset;
            Vector3 end = corners[i + 1] + Vector3.up * groundHeightOffset;
            Vector3 direction = (end - start).normalized;
            float distance = Vector3.Distance(start, end);

            // Dibujar flechas cada 'arrowSpacing' unidades
            for (float d = 0; d < distance; d += arrowSpacing)
            {
                Vector3 arrowPos = start + direction * d;
                Vector3 arrowTip = arrowPos + direction * 0.3f;

                // Dibujar línea pequeña para la flecha
                Debug.DrawLine(arrowPos, arrowTip, Color.cyan, Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Limpia el camino dibujado
    /// </summary>
    public void ClearPath()
    {
        if (pathLineRenderer != null)
        {
            pathLineRenderer.positionCount = 0;
        }
    }

    /// <summary>
    /// Configura las propiedades del LineRenderer
    /// </summary>
    private void ConfigureLineRenderer()
    {
        pathLineRenderer.startWidth = lineWidth;
        pathLineRenderer.endWidth = lineWidth;
        
        // Crear material
        pathLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        // Configurar colores
        pathLineRenderer.startColor = lineColor;
        pathLineRenderer.endColor = lineColor;
        
        // Configuración adicional
        pathLineRenderer.sortingOrder = 100; // Asegurar que se dibuje encima
        pathLineRenderer.alignment = LineAlignment.View;
        
        Debug.Log("[PathVisualizer] LineRenderer configurado correctamente");
    }

    /// <summary>
    /// Cambia el color del camino en tiempo real
    /// </summary>
    public void SetPathColor(Color newColor)
    {
        lineColor = newColor;
        pathLineRenderer.startColor = newColor;
        pathLineRenderer.endColor = newColor;
    }

    /// <summary>
    /// Cambia el ancho del camino en tiempo real
    /// </summary>
    public void SetPathWidth(float newWidth)
    {
        lineWidth = newWidth;
        pathLineRenderer.startWidth = newWidth;
        pathLineRenderer.endWidth = newWidth;
    }
}
