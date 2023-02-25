using UnityEngine;

namespace PixelSparkStudio.ObjectPooler.InspectorAttributes
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public abstract class CustomAttribute : PropertyAttribute
    {
    }
}
