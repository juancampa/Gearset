using System;

namespace Gearset
{
    /// <summary>
    /// Add this attribute to fields or properties that need 
    /// to be customized when shown in Gearset's inspector window.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class InspectorIgnoreAttribute : Attribute
    {

    }
}
