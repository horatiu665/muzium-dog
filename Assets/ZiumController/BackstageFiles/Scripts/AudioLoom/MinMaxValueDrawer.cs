#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

// IngredientDrawer
[CustomPropertyDrawer(typeof(MinMaxValue))]
public class MinMaxValueDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

      
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var textMinRect = new Rect(position.x, position.y, 30, position.height);
        var minRect = new Rect(position.x+30, position.y, 60, position.height);

        var textMaxRect = new Rect(position.x+110, position.y, 30, position.height);
        var maxRect = new Rect(position.x + 140, position.y, 60, position.height);



        EditorGUI.LabelField(textMinRect, "Min");
        EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"), GUIContent.none);
        EditorGUI.LabelField(textMaxRect, "Max");
        EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), GUIContent.none);
        

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

#endif