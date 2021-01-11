using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageEffectAllowedInSceneView, ExecuteAlways, AddComponentMenu("Volumetric Interaction/Visualiser")]
public class Visualiser : MonoBehaviour
{
    // TODO: Recreate material when shader is assigned in custom editor.
    [SerializeField] private Shader shader;
    [SerializeField] private int samples;
    [SerializeField] [Range(0f, 1f)] private float opacity;

    private Material _material;
    
    private static readonly int SamplesName = Shader.PropertyToID("_Samples");
    private static readonly int OpacityName = Shader.PropertyToID("_Opacity");

    private void Start()
    {
        _material = new Material(shader);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!_material)
            _material = new Material(shader);
     
        _material.SetInt(SamplesName, samples);
        _material.SetFloat(OpacityName, opacity);
        Graphics.Blit(src, dest, _material);
    }
}
