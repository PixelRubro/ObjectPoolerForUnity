﻿using UnityEditor;
using UnityEngine;

namespace PixelRouge.ObjectPooler.InspectorAttributes
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : BasePropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
            EditorGUI.EndDisabledGroup();
        }
    }
}