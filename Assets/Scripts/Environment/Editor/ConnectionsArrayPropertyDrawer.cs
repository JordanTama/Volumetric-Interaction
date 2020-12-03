using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ConnectionsArray))]
public class ConnectionsArrayPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get properties
        SerializedProperty data = property.FindPropertyRelative("data");
        SerializedProperty rows = property.FindPropertyRelative("rows");
        SerializedProperty columns = property.FindPropertyRelative("columns");

        // Configure rects
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        Rect toggleRect = new Rect(position.x, position.y, 18, 18);
        
        // Store indent level
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Draw indentLevel
        for (int y = 0; y < rows.intValue; y++)
        {
            toggleRect.x = position.x;
            
            for (int x = 0; x < columns.intValue; x++)
            {
                if (y == (rows.intValue / 2) && x == (columns.intValue / 2))
                    EditorGUI.Toggle(toggleRect, true);
                else
                    EditorGUI.PropertyField(toggleRect, data.GetArrayElementAtIndex(y * rows.intValue + x), GUIContent.none);
                
                toggleRect.x += toggleRect.size.x;
            }

            toggleRect.y += toggleRect.size.y;
        }

        // Reassign indent level
        EditorGUI.indentLevel = indent;
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * property.FindPropertyRelative("rows").intValue;
    }
}
