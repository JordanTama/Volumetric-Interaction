using Environment.TilePainting;
using UnityEditor;
using UnityEngine;

namespace Environment.TilePainting.Editor
{
    [CustomPropertyDrawer(typeof(ConnectionsArray))]
    public class ConnectionsArrayPropertyDrawer : PropertyDrawer
    {
        private const int ToggleRectSize = 18;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get properties
            SerializedProperty data = property.FindPropertyRelative("data");
            SerializedProperty size = property.FindPropertyRelative("size");
        
            // Configure rects
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            Rect toggleRect = new Rect(position.x, position.y, ToggleRectSize, ToggleRectSize);
        
            // Store indent level
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Draw indentLevel
            for (int y = 0; y < size.intValue; y++)
            {
                toggleRect.x = position.x;
            
                for (int x = 0; x < size.intValue; x++)
                {
                    if (y == (size.intValue / 2) && x == (size.intValue / 2))
                        EditorGUI.Toggle(toggleRect, true);
                    else
                        EditorGUI.PropertyField(toggleRect, data.GetArrayElementAtIndex(y * size.intValue + x), GUIContent.none);
                
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
            return base.GetPropertyHeight(property, label) * property.FindPropertyRelative("size").intValue;
        }
    }
}
