﻿using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class MyLightingShaderGUI : ShaderGUI
{
    enum RenderingMode
    {
        Opaque, Cutout, Fade, Transparent
    }

    struct RenderingSettings
    {
        public RenderQueue queue;
        public string renderType;
        public BlendMode srcBlend, dstBlend;
        public bool zWrite;

        public static RenderingSettings[] modes = {
            new RenderingSettings() {
                queue = RenderQueue.Geometry,
                renderType = "",
                srcBlend = BlendMode.One,
                dstBlend = BlendMode.Zero,
                zWrite = true
            },
            new RenderingSettings() {
                queue = RenderQueue.AlphaTest,
                renderType = "TransparentCutout",
                srcBlend = BlendMode.One,
                dstBlend = BlendMode.Zero,
                zWrite = true
            },
            new RenderingSettings() {
                queue = RenderQueue.Transparent,
                renderType = "Transparent",
                srcBlend = BlendMode.SrcAlpha,
                dstBlend = BlendMode.OneMinusSrcAlpha,
                zWrite = false
            },
			new RenderingSettings() {
				queue = RenderQueue.Transparent,
				renderType = "Transparent",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = false
			}
        };
    }

    enum SmoothnessSource
    {
        Uniform, Albedo, Metallic
    }

    Material target;
    MaterialEditor editor;
    MaterialProperty[] properties;
    bool shouldShowAlphaCutoff;

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.target = editor.target as Material;
        this.editor = editor;
        this.properties = properties;
        DoRenderingMode();
        DoMain();
        DoSecondary();

    }
    MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, properties);
    }

    void SetKeyword(string keyword, bool state)
    {
        if (state)
        {
            foreach (Material m in editor.targets)
            {
                m.EnableKeyword(keyword);
            }
        }
        else
        {
            foreach (Material m in editor.targets)
            {
                m.DisableKeyword(keyword);
            }
        }
    }

    bool IsKeywordEnabled(string keyword)
    {
        return target.IsKeywordEnabled(keyword);
    }

    static GUIContent staticLabel = new GUIContent();

    static GUIContent MakeLabel(string text, string tooltip = null)
    {
        staticLabel.text = text;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }

    static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
    {
        staticLabel.text = property.displayName;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }

    void RecordAction(string label)
    {
        editor.RegisterPropertyChangeUndo(label);
    }

    void DoMain()
    {
        GUILayout.Label("Main Maps", EditorStyles.boldLabel);

        MaterialProperty mainTex = FindProperty("_MainTex");
        MaterialProperty tint = FindProperty("_Tint");
        GUIContent albedoLabel = MakeLabel(mainTex, "Albedo (RGB)");
        editor.TexturePropertySingleLine(albedoLabel, mainTex, tint);
        DoMetallic();
        DoSmoothness();
        DoEmission();
        DoNormals();
        DoDetailMask();
        DoOcclusion();
        if (shouldShowAlphaCutoff)
        {
            DoAlphaCutoff();
        }

        editor.TextureScaleOffsetProperty(mainTex);

    }

    void DoRenderingMode()
    {
        RenderingMode mode = RenderingMode.Opaque;
        shouldShowAlphaCutoff = false;
        if (IsKeywordEnabled("_RENDERING_CUTOUT"))
        {
            shouldShowAlphaCutoff = true;
            mode = RenderingMode.Cutout;
        }
        else if (IsKeywordEnabled("_RENDERING_FADE"))
        {
            mode = RenderingMode.Fade;
        }		
        else if (IsKeywordEnabled("_RENDERING_TRANSPARENT")) {
			mode = RenderingMode.Transparent;
		}

        EditorGUI.BeginChangeCheck();
        mode = (RenderingMode)EditorGUILayout.EnumPopup(
            MakeLabel("Rendering Mode"), mode
        );
        if (EditorGUI.EndChangeCheck())
        {
            RecordAction("Rendering Mode");
            SetKeyword("_RENDERING_CUTOUT", mode == RenderingMode.Cutout);
            SetKeyword("_RENDERING_FADE", mode == RenderingMode.Fade);
			SetKeyword(
				"_RENDERING_TRANSPARENT", mode == RenderingMode.Transparent
			);
            RenderingSettings settings = RenderingSettings.modes[(int)mode];
            foreach (Material m in editor.targets)
            {
                m.renderQueue = (int)settings.queue;
                m.SetOverrideTag("RenderType", settings.renderType);
                m.SetInt("_SrcBlend", (int)settings.srcBlend);
                m.SetInt("_DstBlend", (int)settings.dstBlend);
                m.SetInt("_ZWrite", settings.zWrite?1:0);
            }
        }
    }

    void DoAlphaCutoff()
    {
        MaterialProperty slider = FindProperty("_AlphaCutoff");
        EditorGUI.indentLevel += 2;
        editor.ShaderProperty(slider, MakeLabel(slider));
        EditorGUI.indentLevel -= 2;
    }
    void DoNormals()
    {
        MaterialProperty map = FindProperty("_NormalMap");
        Texture tex = map.textureValue;
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(MakeLabel(map), map,
            tex ? FindProperty("_BumpScale") : null);
        if (EditorGUI.EndChangeCheck() && tex != map.textureValue)
        {
            SetKeyword("_NORMAL_MAP", map.textureValue);
        }
    }

    void DoOcclusion()
    {
        MaterialProperty map = FindProperty("_OcclusionMap");
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(
            MakeLabel(map, "Occlusion (G)"), map,
            map.textureValue ? FindProperty("_OcclusionStrength") : null
        );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_OCCLUSION_MAP", map.textureValue);
        }
    }

    void DoDetailMask()
    {
        MaterialProperty mask = FindProperty("_DetailMask");
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(
            MakeLabel(mask, "Detail Mask (A)"), mask
        );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_DETAIL_MASK", mask.textureValue);
        }
    }

    void DoMetallic()
    {
        MaterialProperty map = FindProperty("_MetallicMap");
        editor.TexturePropertySingleLine(
            MakeLabel(map, "Metallic (R)"), map,
            map.textureValue ? null : FindProperty("_Metallic")
        );
        SetKeyword("_METALLIC_MAP", map.textureValue);
    }

    void DoEmission()
    {
        MaterialProperty map = FindProperty("_EmissionMap");
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertyWithHDRColor(
            MakeLabel(map, "Emission (RGB)"), map, FindProperty("_Emission"),
            false
        );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_EMISSION_MAP", map.textureValue);
        }
    }

    void DoSmoothness()
    {
        SmoothnessSource source = SmoothnessSource.Uniform;
        if (IsKeywordEnabled("_SMOOTHNESS_ALBEDO"))
        {
            source = SmoothnessSource.Albedo;
        }
        else if (IsKeywordEnabled("_SMOOTHNESS_METALLIC"))
        {
            source = SmoothnessSource.Metallic;
        }
        MaterialProperty slider = FindProperty("_Smoothness");
        EditorGUI.indentLevel += 2;
        editor.ShaderProperty(slider, MakeLabel(slider));
        EditorGUI.indentLevel += 1;
        EditorGUI.BeginChangeCheck();
        source = (SmoothnessSource)EditorGUILayout.EnumPopup(MakeLabel("Source"), source);
        if (EditorGUI.EndChangeCheck())
        {
            RecordAction("Smoothness Source");
            SetKeyword("_SMOOTHNESS_ALBEDO", source == SmoothnessSource.Albedo);
            SetKeyword(
                "_SMOOTHNESS_METALLIC", source == SmoothnessSource.Metallic
            );
        }
        EditorGUI.indentLevel -= 3;
    }

    void DoSecondary()
    {
        GUILayout.Label("Secondary Maps", EditorStyles.boldLabel);

        MaterialProperty detailTex = FindProperty("_DetailTex");
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(
            MakeLabel(detailTex, "Albedo (RGB) multiplied by 2"), detailTex
        );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_DETAIL_ALBEDO_MAP", detailTex.textureValue);
        }
        DoSecondaryNormals();
        editor.TextureScaleOffsetProperty(detailTex);
    }

    void DoSecondaryNormals()
    {
        MaterialProperty map = FindProperty("_DetailNormalMap");
        EditorGUI.BeginChangeCheck();
        editor.TexturePropertySingleLine(
            MakeLabel(map), map,
            map.textureValue ? FindProperty("_DetailBumpScale") : null
        );
        if (EditorGUI.EndChangeCheck())
        {
            SetKeyword("_DETAIL_NORMAL_MAP", map.textureValue);
        }
    }
}