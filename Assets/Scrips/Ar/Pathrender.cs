using UnityEngine;

/// <summary>
/// Dibuja el camino con un LineRenderer basado en los eventos de NavigationManager
/// Adjunta esto a un GameObject en la escena 06-Recorrido
/// </summary>
public class PathRenderer : MonoBehaviour
{
    [Header("Configuración de Línea")]
    [SerializeField] private LineRenderer pathLine;
    [SerializeField] private float lineWidth = 0.15f;
    [SerializeField] private Color lineColor = new Color(0, 0.5f, 1, 0.8f); // Azul

    void Start()
    {
        // Crear LineRenderer si no existe
        if (pathLine == null)
        {
            pathLine = gameObject.AddComponent<LineRenderer>();
        }

        ConfigureLineRenderer();

        // Suscribirse a eventos de NavigationManager
        if (NavigationManager.Instance != null)
        {
            NavigationManager.Instance.OnPathUpdated += DrawPath;
            NavigationManager.Instance.OnNavigationStopped += ClearPath;
        }
    }

    private void DrawPath(Vector3[] corners)
    {
        if (corners == null || corners.Length < 2)
        {
            ClearPath();
            return;
        }

        // Configurar posiciones del LineRenderer
        pathLine.positionCount = corners.Length;
        for (int i = 0; i < corners.Length; i++)
        {
            // Pequeño offset en Y para que no esté pegado al suelo
            Vector3 pos = corners[i];
            pos.y += 0.02f;
            pathLine.SetPosition(i, pos);
        }
    }

    private void ClearPath()
    {
        if (pathLine != null)
        {
            pathLine.positionCount = 0;
        }
    }

    private void ConfigureLineRenderer()
    {
        pathLine.startWidth = lineWidth;
        pathLine.endWidth = lineWidth;
        pathLine.material = new Material(Shader.Find("Sprites/Default"));
        pathLine.startColor = lineColor;
        pathLine.endColor = lineColor;
        pathLine.sortingOrder = 100;
        pathLine.alignment = LineAlignment.View;
    }

    void OnDestroy()
    {
        if (NavigationManager.Instance != null)
        {
            NavigationManager.Instance.OnPathUpdated -= DrawPath;
            NavigationManager.Instance.OnNavigationStopped -= ClearPath;
        }
    }
}