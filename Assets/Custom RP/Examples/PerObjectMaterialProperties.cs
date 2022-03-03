using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    private static int baseColorId = Shader.PropertyToID("_BaseColor");
    private static int cutOffId = Shader.PropertyToID("_CutOff");
    private static MaterialPropertyBlock block;
    
    [SerializeField]
    Color baseColor = Color.white;

    [SerializeField]
    float cutOff = 0.5f;
    
    private void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }

        block.SetColor(baseColorId, baseColor);
        block.SetFloat(cutOffId,cutOff);
        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    private void Awake()
    {
        OnValidate();
    }
}
