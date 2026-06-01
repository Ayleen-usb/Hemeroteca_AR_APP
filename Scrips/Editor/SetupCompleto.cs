using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// SETUP COMPLETO AUTOMÁTICO — Hemeroteca AR
/// Menú: Tools → SETUP COMPLETO HEMEROTECA
/// Hace TODO de una vez: destinos, flechas, prefabs, asignaciones.
/// </summary>
public class SetupCompleto : EditorWindow
{
    [MenuItem("Tools/⚡ SETUP COMPLETO HEMEROTECA")]
    public static void EjecutarSetup()
    {
        Debug.Log("=== INICIANDO SETUP COMPLETO ===");

        CrearPrefabFlecha();
        CrearDestinosEnNavMesh();
        AsignarReferencias();
        ConfigurarARCamera();

        // Guardar la escena automáticamente
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        Debug.Log("=== ✅ SETUP COMPLETO TERMINADO — Revisa la Hierarchy ===");
        EditorUtility.DisplayDialog(
            "✅ Setup Completo",
            "Todo listo:\n\n" +
            "✓ 10 destinos de computadores creados\n" +
            "✓ 6 destinos de salas creados\n" +
            "✓ Prefab de flecha creado\n" +
            "✓ Referencias asignadas\n" +
            "✓ Escena guardada\n\n" +
            "Ajusta las posiciones de los destinos en la escena según el plano real.",
            "OK"
        );
    }

    // ═══════════════════════════════════════════════════════════
    // 1. CREAR PREFAB DE FLECHA
    // ═══════════════════════════════════════════════════════════
    static GameObject arrowPrefabAsset;

    static void CrearPrefabFlecha()
    {
        string prefabPath = "Assets/Prefabricados/Flecha_AR.prefab";

        // Si ya existe, usarlo
        arrowPrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (arrowPrefabAsset != null)
        {
            Debug.Log("[Setup] Prefab de flecha ya existe, reutilizando.");
            return;
        }

        // Crear flecha con primitivas
        GameObject root = new GameObject("Flecha_AR");

        // Cuerpo de la flecha (cilindro horizontal)
        GameObject cuerpo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cuerpo.name = "Cuerpo";
        cuerpo.transform.SetParent(root.transform);
        cuerpo.transform.localPosition = new Vector3(0, 0, 0);
        cuerpo.transform.localEulerAngles = new Vector3(90, 0, 0);
        cuerpo.transform.localScale = new Vector3(0.08f, 0.25f, 0.08f);
        DestroyCollider(cuerpo);

        // Punta de la flecha (esfera achatada)
        GameObject punta = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        punta.name = "Punta";
        punta.transform.SetParent(root.transform);
        punta.transform.localPosition = new Vector3(0, 0, 0.3f);
        punta.transform.localScale = new Vector3(0.2f, 0.12f, 0.25f);
        DestroyCollider(punta);

        // Material verde brillante
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.1f, 0.9f, 0.3f);
        mat.SetFloat("_Metallic", 0.2f);
        mat.SetFloat("_Glossiness", 0.8f);

        // Guardar material
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        AssetDatabase.CreateAsset(mat, "Assets/Materials/Mat_Flecha.mat");

        cuerpo.GetComponent<Renderer>().sharedMaterial = mat;
        punta.GetComponent<Renderer>().sharedMaterial = mat;

        // Guardar como prefab
        if (!AssetDatabase.IsValidFolder("Assets/Prefabricados"))
            AssetDatabase.CreateFolder("Assets", "Prefabricados");

        arrowPrefabAsset = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        Object.DestroyImmediate(root);

        Debug.Log("[Setup] ✓ Prefab de flecha creado en: " + prefabPath);
    }

    static void DestroyCollider(GameObject obj)
    {
        var col = obj.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);
    }

    // ═══════════════════════════════════════════════════════════
    // 2. CREAR DESTINOS EN EL NAVMESH
    // ═══════════════════════════════════════════════════════════
    static List<GameObject> pcDestinos = new List<GameObject>();
    static List<GameObject> salaDestinos = new List<GameObject>();

    static void CrearDestinosEnNavMesh()
    {
        pcDestinos.Clear();
        salaDestinos.Clear();

        // Encontrar bounds del modelo de la hemeroteca
        Bounds bounds = ObtenerBoundsHemeroteca();
        float cx = bounds.center.x;
        float cz = bounds.center.z;
        float w = bounds.size.x * 0.35f;
        float d = bounds.size.z * 0.35f;

        // Limpiar destinos anteriores si existen
        LimpiarDestinos("Destinos_Computadores");
        LimpiarDestinos("Destinos_Salas");

        // ── Computadores (2 filas de 5) ──────────────────────
        GameObject padrePC = new GameObject("Destinos_Computadores");
        Undo.RegisterCreatedObjectUndo(padrePC, "Crear Destinos PC");

        Vector3[] posicionesPC = new Vector3[]
        {
            new Vector3(cx - w * 0.8f, 0, cz - d * 0.5f),
            new Vector3(cx - w * 0.4f, 0, cz - d * 0.5f),
            new Vector3(cx,             0, cz - d * 0.5f),
            new Vector3(cx + w * 0.4f, 0, cz - d * 0.5f),
            new Vector3(cx + w * 0.8f, 0, cz - d * 0.5f),
            new Vector3(cx - w * 0.8f, 0, cz + d * 0.5f),
            new Vector3(cx - w * 0.4f, 0, cz + d * 0.5f),
            new Vector3(cx,             0, cz + d * 0.5f),
            new Vector3(cx + w * 0.4f, 0, cz + d * 0.5f),
            new Vector3(cx + w * 0.8f, 0, cz + d * 0.5f),
        };

        for (int i = 0; i < 10; i++)
        {
            GameObject pc = new GameObject("PC" + (i + 1) + "_Destino");
            pc.transform.SetParent(padrePC.transform);
            pc.transform.position = SnapToNavMesh(posicionesPC[i]);

            // Ícono visual para ver en escena
            AddEditorIcon(pc, i < 5 ? "sv_label_1" : "sv_label_2");
            pcDestinos.Add(pc);
        }

        // ── Salas de estudio (1 fila de 6) ───────────────────
        GameObject padreSala = new GameObject("Destinos_Salas");
        Undo.RegisterCreatedObjectUndo(padreSala, "Crear Destinos Sala");

        for (int i = 0; i < 6; i++)
        {
            float xPos = cx - w + (i * w * 0.4f);
            Vector3 rawPos = new Vector3(xPos, 0, cz - d * 1.2f);

            GameObject sala = new GameObject("Sala" + (i + 1) + "_Destino");
            sala.transform.SetParent(padreSala.transform);
            sala.transform.position = SnapToNavMesh(rawPos);

            AddEditorIcon(sala, "sv_label_5");
            salaDestinos.Add(sala);
        }

        Debug.Log("[Setup] ✓ Destinos creados: 10 PCs + 6 Salas");
    }

    static Bounds ObtenerBoundsHemeroteca()
    {
        // Buscar el modelo de la hemeroteca por nombre
        GameObject hemeroteca = GameObject.Find("hemeroteca2");
        if (hemeroteca != null)
        {
            Renderer[] renderers = hemeroteca.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds b = renderers[0].bounds;
                foreach (var r in renderers) b.Encapsulate(r.bounds);
                return b;
            }
        }

        // Fallback: bounds genérico
        Debug.LogWarning("[Setup] No se encontró hemeroteca2, usando posiciones genéricas.");
        return new Bounds(Vector3.zero, new Vector3(10, 2, 8));
    }

    static Vector3 SnapToNavMesh(Vector3 pos)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, 5f, NavMesh.AllAreas))
            return hit.position + Vector3.up * 0.05f;
        return pos + Vector3.up * 0.05f;
    }

    static void LimpiarDestinos(string nombre)
    {
        GameObject viejo = GameObject.Find(nombre);
        if (viejo != null)
        {
            Object.DestroyImmediate(viejo);
            Debug.Log("[Setup] Limpiando destinos anteriores: " + nombre);
        }
    }

    static void AddEditorIcon(GameObject obj, string iconName)
    {
        var icon = EditorGUIUtility.IconContent(iconName).image as Texture2D;
        if (icon != null)
            EditorGUIUtility.SetIconForObject(obj, icon);
    }

    // ═══════════════════════════════════════════════════════════
    // 3. ASIGNAR REFERENCIAS EN LOS SCRIPTS
    // ═══════════════════════════════════════════════════════════
    static void AsignarReferencias()
    {
        // Buscar SpaceManager
        SpaceManager spaceManager = Object.FindObjectOfType<SpaceManager>();
        if (spaceManager == null)
        {
            Debug.LogError("[Setup] No se encontró SpaceManager en la escena.");
            return;
        }

        // Inicializar listas si están vacías
        if (spaceManager.computers == null || spaceManager.computers.Count == 0)
        {
            spaceManager.computers = new System.Collections.Generic.List<SpaceManager.Space>();
            for (int i = 0; i < 10; i++)
            {
                spaceManager.computers.Add(new SpaceManager.Space
                {
                    id = "PC" + (i + 1),
                    displayName = "Computador " + (i + 1),
                    type = SpaceManager.SpaceType.Computer,
                    isAvailable = true,
                    locationTransform = i < pcDestinos.Count ? pcDestinos[i].transform : null
                });
            }
        }
        else
        {
            // Solo asignar transforms si ya existen los spaces
            for (int i = 0; i < spaceManager.computers.Count && i < pcDestinos.Count; i++)
                spaceManager.computers[i].locationTransform = pcDestinos[i].transform;
        }

        if (spaceManager.studyRooms == null || spaceManager.studyRooms.Count == 0)
        {
            spaceManager.studyRooms = new System.Collections.Generic.List<SpaceManager.Space>();
            for (int i = 0; i < 6; i++)
            {
                spaceManager.studyRooms.Add(new SpaceManager.Space
                {
                    id = "Sala" + (i + 1),
                    displayName = "Sala " + (i + 1),
                    type = SpaceManager.SpaceType.StudyRoom,
                    isAvailable = (i % 2 == 0),
                    locationTransform = i < salaDestinos.Count ? salaDestinos[i].transform : null
                });
            }
        }
        else
        {
            for (int i = 0; i < spaceManager.studyRooms.Count && i < salaDestinos.Count; i++)
                spaceManager.studyRooms[i].locationTransform = salaDestinos[i].transform;
        }

        EditorUtility.SetDirty(spaceManager);

        // Asignar prefab de flecha al ArrowGuide
        ArrowGuide arrowGuide = Object.FindObjectOfType<ArrowGuide>();
        if (arrowGuide != null && arrowPrefabAsset != null)
        {
            arrowGuide.arrowPrefab = arrowPrefabAsset;
            EditorUtility.SetDirty(arrowGuide);
            Debug.Log("[Setup] ✓ Prefab de flecha asignado al ArrowGuide");
        }

        // Asignar hemeroteca al AppManager
        AppManager appManager = Object.FindObjectOfType<AppManager>();
        if (appManager != null)
        {
            GameObject hemeroteca = GameObject.Find("hemeroteca2");
            if (hemeroteca != null)
            {
                appManager.hemerotecaModel = hemeroteca;
                EditorUtility.SetDirty(appManager);
                Debug.Log("[Setup] ✓ Modelo hemeroteca asignado al AppManager");
            }
        }

        Debug.Log("[Setup] ✓ Referencias asignadas correctamente");
    }

    // ═══════════════════════════════════════════════════════════
    // 4. CONFIGURAR AR CAMERA
    // ═══════════════════════════════════════════════════════════
    static void ConfigurarARCamera()
    {
        GameObject arCameraObj = GameObject.Find("ARCamera");
        if (arCameraObj == null)
        {
            Debug.LogWarning("[Setup] No se encontró ARCamera — asigna manualmente.");
            return;
        }

        Camera arCamera = arCameraObj.GetComponent<Camera>();

        AppManager appManager = Object.FindObjectOfType<AppManager>();
        if (appManager != null && arCamera != null)
        {
            appManager.arCamera = arCamera;
            EditorUtility.SetDirty(appManager);
        }

        NavigationManager navManager = Object.FindObjectOfType<NavigationManager>();
        if (navManager != null)
        {
            navManager.userTransform = arCameraObj.transform;
            EditorUtility.SetDirty(navManager);
        }

        Debug.Log("[Setup] ✓ ARCamera asignada a AppManager y NavigationManager");
    }
}
