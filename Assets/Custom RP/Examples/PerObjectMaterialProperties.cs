using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    private static int baseColorId = Shader.PropertyToID("_BaseColor");
    private static int cutOffId = Shader.PropertyToID("_CutOff");
    private static int metallicId = Shader.PropertyToID("_Metallic");
    private static int smoothnessId = Shader.PropertyToID("_Smoothness");
    private static MaterialPropertyBlock block;
    
    [SerializeField]
    Color baseColor = Color.white;

    [SerializeField]
    float cutOff = 0.5f, metallic = 0.5f, smoothness = 0.5f;
    private void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }

        block.SetFloat(metallicId,metallic);
        block.SetFloat(smoothnessId,smoothness);
        block.SetColor(baseColorId, baseColor);
        block.SetFloat(cutOffId,cutOff);
        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    private void Awake()
    {
        OnValidate();
    }
}
