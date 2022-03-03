using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Profiling;

public partial class CameraRenderer
{
    partial void PrepareForSceneWindow();
    partial void DrawGizmos();
    partial void DrawUnsupportedShader();
    partial void PrepareBuffer();
    
#if UNITY_EDITOR
    static ShaderTagId[] legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };

    private static Material errorMaterial;
    private string SampleName { get; set; }

    partial void PrepareForSceneWindow()
    {
        if (camera.cameraType == CameraType.SceneView)
        {
            //将UI到场景视图中进行渲染。
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }
    
    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera,GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera,GizmoSubset.PostImageEffects);
        }
    }

    partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        buffer.name = SampleName = camera.name;
        Profiler.EndSample();
    }
    
    partial void DrawUnsupportedShader()
    {
        if (errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        var drawingSettings = new DrawingSettings(legacyShaderTagIds[0],new SortingSettings(camera))
        {
            overrideMaterial = errorMaterial
        };
        for (int i = 1; i < legacyShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i,legacyShaderTagIds[i]);
        }
        var filterSetting = FilteringSettings.defaultValue;
        context.DrawRenderers(cullResult,ref drawingSettings, ref  filterSetting);
    }
#else
    const string SampleName = bufferName;
#endif
}
