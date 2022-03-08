using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    CameraRenderer render = new CameraRenderer();
    bool useDynamicBatching, useGPUInstancing;
    public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing,bool useSRPBatcher)
    {
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        GraphicsSettings.lightsUseLinearIntensity = true;
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            //每个相机渲染方式可以不同
            render.Render(context,camera,useDynamicBatching,useGPUInstancing);
        }
    }
}