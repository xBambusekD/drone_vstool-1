#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[InitializeOnLoad]
public static class OutlineShaderBootstrap {
    static OutlineShaderBootstrap() {
        TryReimportShader("Assets/Submodules/Unity-URP-Outlines/Outlines/ShaderGraphs/ViewSpaceNormals.shader");
        TryReimportShader("Assets/Submodules/Unity-URP-Outlines/Outlines/ShaderGraphs/Outlines.shader");
        TryReimportShader("Assets/Submodules/Unity-URP-Outlines/Outlines/ShaderGraphs/UnlitColor.shader");
    }

    private static void TryReimportShader(string path) {
        var shader = Shader.Find(System.IO.Path.GetFileNameWithoutExtension(path));
        if (shader == null && System.IO.File.Exists(path)) {
            Debug.Log($"[Bootstrap] Reimporting {path} so URP features can find it...");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
#endif
