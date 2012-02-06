using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Gearset
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InvokableAttribute : Attribute
    {
        public InvokableAttribute()
        {
        }
    }
}
