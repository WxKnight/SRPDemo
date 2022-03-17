using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    private const string bufferName = "Render Camera";
    private ScriptableRenderContext context;
    private Camera camera;
    private Lighting lighting = new Lighting();
    private CullingResults cullingResult;
    private static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");
    
    private CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };
    
    public void Render(ScriptableRenderContext context,Camera camera,bool useDynamicBatching, bool useGPUInstancing, ShadowSettings shadowSettings)
    {
        this.context = context;
        this.camera = camera;

        PrepareBuffer();
        PrepareForSceneWindow();
        if (!Cull(shadowSettings.maxDistance))
        {
            return;
        }
    
        buffer.BeginSample(SampleName);
        Execute();
        lighting.SetUp(context, cullingResult, shadowSettings);
        buffer.EndSample(SampleName);
        Setup();
        DrawVisibleGeometry(useDynamicBatching,useGPUInstancing);
        DrawUnsupportedShader();
        DrawGizmos();
        lighting.Cleanup();
        Submit();
    }
    

    private bool Cull(float maxShadowDistance)
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters cullingParameters))
        {
            cullingParameters.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);
            cullingResult = context.Cull(ref cullingParameters);
            return true;
        }

        return false;
    }
    private void Setup()
    {
        //Correct clearing  Clear (color+Z+stencil)
        //Draw GL drawing  with the Hidden/InternalClear shader  如果以下两句调换顺序 则不是最高效清楚
        
        //This function sets up view, projection and clipping planes global shader variables.
        context.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        //两个相邻的GL会被合并
        buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth,flags == CameraClearFlags.Color,
            flags == CameraClearFlags.Color ? camera.backgroundColor.linear :Color.clear);
        
        
        //Schedules a performance profiling
        buffer.BeginSample(SampleName);
        Execute();
    }

    private void Submit()
    {
        buffer.EndSample(SampleName);
        Execute();
        context.Submit();
    }

    private void Execute()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    private void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
    {
        var sortSetting = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };

        //If you do not set the LightMode tag in a Pass, URP uses the SRPDefaultUnlit tag value for that Pass.
        var drawSetting = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), sortSetting)
        {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing
        };
        drawSetting.SetShaderPassName(1,litShaderTagId);
        var filterSetting = new FilteringSettings(RenderQueueRange.all);
        
        context.DrawRenderers(cullingResult,ref drawSetting,ref filterSetting);
        context.DrawSkybox(camera);
        filterSetting.renderQueueRange = RenderQueueRange.transparent;
        sortSetting.criteria = SortingCriteria.CommonTransparent;
        drawSetting.sortingSettings = sortSetting;
        context.DrawRenderers(cullingResult,ref drawSetting, ref  filterSetting);
    }
    

}