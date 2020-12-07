using Environment.TilePainting;
using UnityEditor;
using UnityEngine;

namespace Environment.TilePainting.Editor
{
    [CustomEditor(typeof(TilePainter)), CanEditMultipleObjects]
    public class TilePainterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
        
            if (GUILayout.Button("Generate Map"))
            {
                foreach (Object o in targets)
                {
                    var script = (TilePainter) o;
                    script.GenerateMap();
                }
            }

            if (GUILayout.Button("Clear Map"))
            {
                foreach (Object o in targets)
                {
                    var script = (TilePainter) o;
                    script.ClearMap();
                }
            }
        }
    }
}
