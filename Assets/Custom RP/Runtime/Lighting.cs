using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    private static int maxDirLightCount = 4;
    private CullingResults cullResult;
    private const string bufferName = "Lighting";

    static int
        directionalLightCount = Shader.PropertyToID("_DirectionalLightCount"),
        directionalLightColorId = Shader.PropertyToID("_DirectionalLightColors"),
        directionalLightDirectionId = Shader.PropertyToID("_DirectionalLightDirs");

    private static Vector4[]
        dirLightColors = new Vector4[maxDirLightCount],
        dirLightDirs = new Vector4[maxDirLightCount];

    private CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    public void SetUp(ScriptableRenderContext context, CullingResults cullResult)
    {
        this.cullResult = cullResult;
        buffer.BeginSample(bufferName);
        //SetUpDirectionalLight();
        SetUpLight();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    private void SetUpLight()
    {
        int dirLightCount = 0;
        NativeArray<VisibleLight> visibleLights = cullResult.visibleLights;
        for (int i = 0; i < visibleLights.Length; i++)
        {
            if (visibleLights[i].lightType == LightType.Directional)
            {
                SetUpDirectionalLight(dirLightCount++,visibleLights[i]);
                if (dirLightCount >= maxDirLightCount)
                    break;
            }
        }
        
        buffer.SetGlobalInt(directionalLightCount,maxDirLightCount);
        buffer.SetGlobalVectorArray(directionalLightColorId,dirLightColors);
        buffer.SetGlobalVectorArray(directionalLightDirectionId,dirLightDirs);
    }

    private void SetUpDirectionalLight(int index, VisibleLight light)
    {
        dirLightColors[index] = light.finalColor;
        dirLightDirs[index] = light.localToWorldMatrix.GetColumn(2);
    }
}