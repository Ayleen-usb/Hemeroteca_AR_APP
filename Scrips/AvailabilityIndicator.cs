using UnityEngine;

/// <summary>
/// Muestra una esfera flotante sobre cada destino:
/// VERDE = libre, ROJA = ocupado.
/// Se actualiza en tiempo real cuando cambia la disponibilidad.
/// Agrégalo al GameObject "AppManager" o a un objeto vacío en la escena.
/// </summary>
public class AvailabilityIndicator : MonoBehaviour
{
    [Header("Apariencia")]
    public float alturaIndicador = 0.5f;      // Qué tan alto flota sobre el suelo
    public float escalaIndicador = 0.2f;       // Tamaño de la esfera
    public float velocidadFlotacion = 1.5f;    // Velocidad de animación flotante
    public float amplitudFlotacion = 0.06f;    // Cuánto sube y baja

    [Header("Colores")]
    public Color colorLibre    = new Color(0.1f, 0.9f, 0.3f);
    public Color colorOcupado  = new Color(0.95f, 0.2f, 0.2f);

    // Referencia interna a cada indicador creado
    private struct Indicador
    {
        public GameObject objeto;
        public Renderer render;
        public string spaceId;
        public float offsetFase; // Para que no floten todas igual
    }

    private System.Collections.Generic.List<Indicador> indicadores
        = new System.Collections.Generic.List<Indicador>();

    private Material matLibre;
    private Material matOcupado;

    void Start()
    {
        // Crear materiales
        matLibre   = CrearMaterial(colorLibre);
        matOcupado = CrearMaterial(colorOcupado);

        // Esperar un frame para que SpaceManager esté listo
        Invoke(nameof(CrearIndicadores), 0.2f);
    }

    void CrearIndicadores()
    {
        if (SpaceManager.Instance == null)
        {
            Debug.LogWarning("[AvailabilityIndicator] SpaceManager no encontrado.");
            return;
        }

        float fase = 0f;

        // Computadores
        foreach (var space in SpaceManager.Instance.GetAllComputers())
            CrearIndicador(space, ref fase);

        // Salas
        foreach (var space in SpaceManager.Instance.GetAllStudyRooms())
            CrearIndicador(space, ref fase);

        Debug.Log("[AvailabilityIndicator] " + indicadores.Count + " indicadores creados.");
    }

    void CrearIndicador(SpaceManager.Space space, ref float fase)
    {
        if (space.locationTransform == null) return;

        // Esfera flotante
        GameObject esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        esfera.name = "Indicador_" + space.id;

        // Posición: encima del destino
        Vector3 pos = space.locationTransform.position;
        pos.y += alturaIndicador;
        esfera.transform.position = pos;
        esfera.transform.localScale = Vector3.one * escalaIndicador;

        // Quitar colisión (no la necesitamos)
        Destroy(esfera.GetComponent<Collider>());

        // Asignar color según disponibilidad
        Renderer rend = esfera.GetComponent<Renderer>();
        rend.material = space.isAvailable ? matLibre : matOcupado;

        // Guardar referencia
        indicadores.Add(new Indicador
        {
            objeto    = esfera,
            render    = rend,
            spaceId   = space.id,
            offsetFase = fase
        });

        fase += 0.7f; // Desfase entre indicadores para que no vayan al unísono
    }

    void Update()
    {
        if (SpaceManager.Instance == null) return;

        float tiempo = Time.time;

        for (int i = 0; i < indicadores.Count; i++)
        {
            var ind = indicadores[i];
            if (ind.objeto == null) continue;

            SpaceManager.Space space = SpaceManager.Instance.GetSpace(ind.spaceId);
            if (space == null || space.locationTransform == null) continue;

            // Flotación suave (seno)
            float y = space.locationTransform.position.y
                    + alturaIndicador
                    + Mathf.Sin((tiempo + ind.offsetFase) * velocidadFlotacion)
                    * amplitudFlotacion;

            ind.objeto.transform.position = new Vector3(
                space.locationTransform.position.x,
                y,
                space.locationTransform.position.z
            );

            // Actualizar color según estado actual
            ind.render.material = space.isAvailable ? matLibre : matOcupado;
        }
    }

    Material CrearMaterial(Color color)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;

        // Emisión para que brille un poco
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", color * 0.4f);
        return mat;
    }

    void OnDestroy()
    {
        // Limpiar indicadores al destruir el componente
        foreach (var ind in indicadores)
            if (ind.objeto != null) Destroy(ind.objeto);
        indicadores.Clear();
    }
}
