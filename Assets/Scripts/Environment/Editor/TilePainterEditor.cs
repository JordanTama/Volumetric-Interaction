using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilePainter))]
public class TilePainterEditor : Editor
{
    private TilePainter[] _scripts;
    
    private void OnEnable()
    {
        _scripts = targets as TilePainter[];
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Generate Map"))
        {
            foreach (TilePainter script in _scripts)
                script.GenerateMap();
        }

        if (GUILayout.Button("Clear Map"))
        {
            foreach (TilePainter script in _scripts)
                script.ClearMap();
        }
    }
}
