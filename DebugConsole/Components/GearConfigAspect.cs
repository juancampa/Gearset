using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace Gearset.Components
{
    [MulticastAttributeUsage(MulticastTargets.Property, 
        TargetMemberAttributes = 
        MulticastAttributes.Instance | 
        MulticastAttributes.Private | 
        MulticastAttributes.Public | 
        MulticastAttributes.Protected)]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [Serializable]
    internal class GearConfigAspect : LocationInterceptionAspect 
    {
        public override void OnSetValue(LocationInterceptionArgs args)
        {
            GearsetSettings.Save();
        }
    }
}
