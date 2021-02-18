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
    [SerializeField] private bool depthTest;
    [SerializeField] private string fileName;

    private Material _material;
    
    private static readonly int SamplesName = Shader.PropertyToID("_Samples");
    private static readonly int OpacityName = Shader.PropertyToID("_Opacity");
    private static readonly int DepthTestName = Shader.PropertyToID("_DepthTest");

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
        _material.SetInt(DepthTestName, depthTest ? 1 : 0);
        Graphics.Blit(src, dest, _material);
    }
}
