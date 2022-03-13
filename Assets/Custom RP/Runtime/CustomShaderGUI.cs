using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class CustomShaderGUI : ShaderGUI
{
    private MaterialEditor editor;
    private MaterialProperty[] properties;
    private Object[] materials;
    private bool showPresets;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor,properties);
        editor = materialEditor;
        this.properties = properties;
        
        EditorGUILayout.Space();
        showPresets = EditorGUILayout.Foldout(showPresets, "Presets", true);
        if (showPresets)
        {
            OpaquePreset();
            ClipPreset();
            FadePreset();
            TransparentPreset();
        }
    }

    public bool SetProperty(string name, float Value)
    {
        MaterialProperty property = FindProperty(name, properties,false);
        if (property != null)
        {
            property.floatValue = Value;
            return true;
        }
        
        return false;
    }

    public void SetKeyWord(string keyWord, bool enabled)
    {
        if (enabled)
        {
            foreach (Material m in materials)
            {
                m.EnableKeyword(keyWord);
            }
        }
        else
        {
            foreach (Material m in materials)
            {
                m.DisableKeyword(keyWord);
            }
        }
    }

    public void SetProperty(string name,string keyWord, bool Value)
    {
        if (SetProperty(name, Value?1.0f:0.0f))
        {
            SetKeyWord(keyWord, Value);
        }
    }

    bool HasProperty(string name) => FindProperty(name, properties, false) != null;
    
    bool HasPremultiplyAlpha => HasProperty("_PremulAlpha");
    
    bool Clipping {
        set => SetProperty("_Clipping", "_CLIPPING", value);
    }

    bool PremultiplyAlpha {
        set => SetProperty("_PremulAlpha", "_PREMULTIPLY_ALPHA", value);
    }

    BlendMode SrcBlend {
        set => SetProperty("_SrcBlend", (float)value);
    }

    BlendMode DstBlend {
        set => SetProperty("_DstBlend", (float)value);
    }

    bool ZWrite {
        set => SetProperty("_ZWrite", value ? 1f : 0f);
    }
    
    RenderQueue RenderQueue {
        set {
            foreach (Material m in materials) {
                m.renderQueue = (int)value;
            }
        }
    }

    bool PreSetBtn(string name)
    {
        if (GUILayout.Button(name))
        {
            editor.RegisterPropertyChangeUndo(name);
            return true;
        }

        return false;
    }

    void OpaquePreset () {
        if (PreSetBtn("Opaque")) {
            Clipping = false;
            PremultiplyAlpha = false;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.Zero;
            ZWrite = true;
            RenderQueue = RenderQueue.Geometry;
        }
    }
    
    void ClipPreset()
    {
        if (PreSetBtn("Clip"))
        {
            Clipping = true;
            PremultiplyAlpha = false;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.Zero;
            ZWrite = true;
            RenderQueue = RenderQueue.AlphaTest;
        }
    }
    
    void FadePreset () {
        if (PreSetBtn("Fade")) {
            Clipping = false;
            PremultiplyAlpha = false;
            SrcBlend = BlendMode.SrcAlpha;
            DstBlend = BlendMode.OneMinusSrcAlpha;
            ZWrite = false;
            RenderQueue = RenderQueue.Transparent;
        }
    }
    
    void TransparentPreset () {
        if (HasPremultiplyAlpha && PreSetBtn("Transparent")) {
            Clipping = false;
            PremultiplyAlpha = true;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.OneMinusSrcAlpha;
            ZWrite = false;
            RenderQueue = RenderQueue.Transparent;
        }
    }
}
