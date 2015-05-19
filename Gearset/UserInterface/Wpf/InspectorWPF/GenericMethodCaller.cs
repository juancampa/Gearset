using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Forms.Integration;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace Gearset.Components.InspectorWPF
{
    public class GenericMethodCaller : MethodCaller
    {
        /// <summary>
        /// TargetObject for instance methods.
        /// </summary>
        private Object invocationTarget;

        /// <summary>
        /// True if the method is an instance method.
        /// </summary>
        private bool IsStatic { get { return methodInfo.IsStatic; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodInfo"></param>
        public GenericMethodCaller(MethodInfo methodInfo, Object target)
            : base(methodInfo)
        {
            this.invocationTarget = target;
        }

        /// <summary>
        /// Calls the method with the established parameters.
        /// </summary>
        public override void CallMethod()
        {
            methodInfo.Invoke(invocationTarget, (from i in Parameters select ((InspectorNode)i.Parameter).Property as Object).ToArray());
        }
    }
}
