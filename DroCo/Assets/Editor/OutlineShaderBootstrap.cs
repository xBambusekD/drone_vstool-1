#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[InitializeOnLoad]
public static class OutlineShaderBootstrap {
    static OutlineShaderBootstrap() {
        TryReimportShader("Assets/Submodules/Unity-URP-Outlines/Outlines/ShaderGraphs/ViewSpaceNormals.shadergraph");
        TryReimportShader("Assets/Submodules/Unity-URP-Outlines/Outlines/ShaderGraphs/Outlines.shadergraph");
        TryReimportShader("Assets/Submodules/Unity-URP-Outlines/Outlines/ShaderGraphs/UnlitColor.shadergraph");
    }

    private static void TryReimportShader(string path) {
        var shader = Shader.Find("Hidden/" + System.IO.Path.GetFileNameWithoutExtension(path));
        if (shader == null && System.IO.File.Exists(path)) {
            Debug.Log($"[Bootstrap] Reimporting {path} so URP features can find it...");
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
#endif
