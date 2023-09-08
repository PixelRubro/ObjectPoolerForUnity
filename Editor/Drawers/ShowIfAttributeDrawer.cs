using UnityEditor;
using UnityEngine;

namespace VermillionVanguard.ObjectPooler.InspectorAttributes
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfAttributeDrawer : ConditionalAttributeDrawer
    {
        protected override PropertyDrawing GetPropertyDrawing()
        {
            return PropertyDrawing.Show;
        }
    }
}