using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

/// <summary>
/// Configura automáticamente todos los ajustes para build Android + Vuforia.
/// Menú: Tools → 📱 Configurar Build Android
/// </summary>
public class AndroidBuildSetup : EditorWindow
{
    [MenuItem("Tools/📱 Configurar Build Android")]
    public static void ConfigurarAndroid()
    {
        // Cambiar plataforma a Android
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            bool cambiar = EditorUtility.DisplayDialog(
                "Cambiar plataforma",
                "Se va a cambiar la plataforma a Android.\nEsto puede tardar unos minutos.",
                "Sí, cambiar", "Cancelar");

            if (!cambiar) return;
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android, BuildTarget.Android);
        }

        // ── Player Settings ───────────────────────────────────
        PlayerSettings.companyName = "Universidad Simon Bolivar";
        PlayerSettings.productName = "HemerotecaAR";
        PlayerSettings.bundleVersion = "1.0";
        PlayerSettings.Android.bundleVersionCode = 1;

        // Nombre del paquete (identificador único de la app)
        PlayerSettings.SetApplicationIdentifier(
            BuildTargetGroup.Android, "com.unisimon.hemerotecaar");

        // API mínima: Android 8.0 (Vuforia requiere mínimo 24)
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

        // Arquitectura: ARM64 (requerida por Vuforia moderno)
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        // Orientación: Portrait (vertical, como un celular normal)
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

        // Internet requerido (para Vuforia license check)
        PlayerSettings.Android.forceInternetPermission = true;

        

        // Input System: Both (nuevo y viejo simultáneamente)
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,
            ScriptingImplementation.IL2CPP);

        // Resolución y pantalla
        PlayerSettings.runInBackground = true;
        PlayerSettings.Android.preferredInstallLocation =
            AndroidPreferredInstallLocation.Auto;

        // ── Calidad gráfica ───────────────────────────────────
        // Desactivar sombras en móvil para mejor rendimiento
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.antiAliasing = 2;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        // ── Escenas en el Build ───────────────────────────────
        string scenePath = "Assets/Scenes/04-Computadores/04-Computadores.unity";
        bool escenaExiste = System.IO.File.Exists(scenePath);

        if (escenaExiste)
        {
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
            Debug.Log("[AndroidSetup] ✓ Escena agregada al build: " + scenePath);
        }
        else
        {
            Debug.LogWarning("[AndroidSetup] No se encontró la escena en: " + scenePath +
                "\nAgrégala manualmente en File → Build Settings");
        }

        // Guardar cambios
        AssetDatabase.SaveAssets();

        Debug.Log("[AndroidSetup] ✅ Configuración Android completa.");
        EditorUtility.DisplayDialog("✅ Android Configurado",
            "Ajustes aplicados:\n\n" +
            "• Plataforma: Android\n" +
            "• API mínima: Android 8.0 (API 24)\n" +
            "• Arquitectura: ARM64\n" +
            "• Orientación: Portrait\n" +
            "• Scripting Backend: IL2CPP\n" +
            "• Bundle ID: com.unisimon.hemerotecaar\n\n" +
            "Siguiente paso:\n" +
            "File → Build Settings → Build And Run",
            "OK");
    }

    [MenuItem("Tools/📱 Configurar Build Android", true)]
    static bool ValidarMenu()
    {
        return true; // Siempre disponible
    }
}
