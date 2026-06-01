using UnityEngine;

/// <summary>
/// Anima las flechas de navegación: pulso de escala + cambio de color.
/// Agrégalo al GameObject "ArrowGuide". Se aplica automáticamente
/// a todas las flechas que ArrowGuide crea en escena.
/// </summary>
public class ArrowAnimator : MonoBehaviour
{
    [Header("Animación de Escala")]
    public float velocidadPulso = 2.5f;
    public float amplitudPulso  = 0.15f;   // Qué tanto crece/encoge

    [Header("Animación de Color")]
    public bool animarColor = true;
    public Color colorBase    = new Color(0.1f, 0.9f, 0.3f);   // Verde
    public Color colorBrillo  = new Color(0.6f, 1.0f, 0.7f);   // Verde claro
    public float velocidadColor = 3f;

    [Header("Rotación opcional")]
    public bool rotarFlechas  = false;
    public float velocidadRot = 45f;   // Grados por segundo en Y

    // Cache de las flechas activas
    private ArrowGuide arrowGuide;

    void Start()
    {
        arrowGuide = GetComponent<ArrowGuide>();
        if (arrowGuide == null)
            arrowGuide = FindObjectOfType<ArrowGuide>();
    }

    void Update()
    {
        if (arrowGuide == null) return;

        float t = Time.time;
        float pulso = 1f + Mathf.Sin(t * velocidadPulso) * amplitudPulso;
        Color colorActual = animarColor
            ? Color.Lerp(colorBase, colorBrillo,
                (Mathf.Sin(t * velocidadColor) + 1f) * 0.5f)
            : colorBase;

        // Aplicar a todas las flechas activas en la escena
        // Buscamos por tag o nombre
        GameObject[] flechas = GameObject.FindGameObjectsWithTag("Flecha_AR");
        if (flechas.Length == 0)
        {
            // Fallback: buscar por nombre
            flechas = FindArrowsByName();
        }

        foreach (var flecha in flechas)
        {
            if (flecha == null) continue;

            // Pulso de escala
            Vector3 escalaBase = flecha.transform.localScale;
            flecha.transform.localScale = new Vector3(
                escalaBase.x * pulso / (escalaBase.x > 0
                    ? Mathf.Max(escalaBase.x, 0.001f) : 1f),
                escalaBase.y,
                escalaBase.z
            );

            // Solo modificar X para que el pulso sea lateral
            float escX = 0.3f * pulso;
            float escZ = 0.3f * pulso;
            flecha.transform.localScale = new Vector3(escX, flecha.transform.localScale.y, escZ);

            // Color
            if (animarColor)
            {
                Renderer[] renderers = flecha.GetComponentsInChildren<Renderer>();
                foreach (var rend in renderers)
                {
                    if (rend.material != null)
                        rend.material.color = colorActual;
                }
            }

            // Rotación en Y
            if (rotarFlechas)
                flecha.transform.Rotate(0, velocidadRot * Time.deltaTime, 0);
        }
    }

    GameObject[] FindArrowsByName()
    {
        var lista = new System.Collections.Generic.List<GameObject>();
        foreach (var go in FindObjectsOfType<GameObject>())
        {
            if (go.name.Contains("Flecha_AR") || go.name.Contains("Arrow_Default"))
                lista.Add(go);
        }
        return lista.ToArray();
    }
}
