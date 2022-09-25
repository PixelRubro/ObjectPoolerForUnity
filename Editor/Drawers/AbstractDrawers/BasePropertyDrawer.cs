using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using SoftBoiledGames.ObjectPooler.Extensions;

namespace SoftBoiledGames.ObjectPooler.InspectorAttributes
{
    public abstract class BasePropertyDrawer: PropertyDrawer 
    {
        protected void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            // Check for special cases for drawing, if none are met, draw field as simple as possible.
            if (property.GetAttribute<LeftToggleAttribute>() != null)
                DrawFieldWithToggleOnTheLeft(position, property, label);
            else
                DrawPropertySimple(position, property, label);
        }

        protected void DrawErrorMessage(Rect position, string errorMessage)
        {
            var padding = EditorGUIUtility.standardVerticalSpacing;

            var highlightRect = new Rect(position.x - padding, position.y - padding,
                position.width + (padding * 2), position.height + (padding * 2));

            EditorGUI.DrawRect(highlightRect, Color.red);

            var contentColor = GUI.contentColor;
            GUI.contentColor = Color.white;
            EditorGUI.LabelField(position, errorMessage);
            GUI.contentColor = contentColor;
        }

        protected void DrawFieldWithToggleOnTheLeft(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                var message = "ERROR! Not a boolean field.";
                DrawErrorMessage(position, message);
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.ToggleLeft(position, label, property.boolValue);

            if (EditorGUI.EndChangeCheck())
                property.boolValue = value;

            EditorGUI.EndProperty();
        }

        protected void DrawPropertySimple(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }
    }
}