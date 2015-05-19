//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Windows.Data;
using EmptyKeys.UserInterface.Mvvm;
//using Gearset;
//using Gearset.Helpers;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Implements inspector tree data item
   /// </summary>
   public class InspectorNode// : BindableBase, ITreeDataItem
    {
//        public string Name { get; set; }

//        public bool IsEnabled { get { return true; } }

//        public bool IsSelected { get; set; }

//        public bool IsExpanded { get; set; }

//        public bool HasChildren { get { return Children.Count > 0; } }

//        /// <summary>
//        /// Returns true if the node is an ExtraNode, that means that it does not correspond 
//        /// to any field or property of the parent but is added as an extra control to better
//        /// manipulate the parent.
//        /// </summary>
//        public bool IsExtraNode { get; private set; }

//        public InspectorNode Parent { get; private set; }

//        private List<InspectorNode> _children;
//        public List<InspectorNode> Children
//        {
//            get { return _children; }
//            set { SetProperty(ref _children, value); }
//        }

//        private List<InspectorNode> _methods;
//        public List<InspectorNode> Methods
//        {
//            get { return _methods; }
//            set { SetProperty(ref _methods, value); }
//        }

//        public InspectorNode()
//        {
//            Name = "Tree Item " + Guid.NewGuid();

//            Children = new List<InspectorNode>();
//            Methods = new List<InspectorNode>();
//        }

//        /// <summary>
//        /// True if this node represents a property, otherwise
//        /// it represents a field. We store this information
//        /// in order to know when a ValueType is encapsulated 
//        /// so we should instanciate it.
//        /// </summary>
//        public bool IsProperty { get; set; }

//        private bool isPrivate;
//        /// <summary>
//        /// Gets whether this node is declared private inside its parent.
//        /// </summary>
//        public bool IsPrivate 
//        { 
//            get { return isPrivate; }
//            set { SetProperty(ref isPrivate, value); }
//        }


//        /// <summary>
//        /// Gets the children for tree view item
//        /// </summary>
//        /// <returns></returns>
//        public IEnumerable GetChildren()
//        {
//            return Children;
//        }

//        public override string ToString()
//        {
//            return Name;
//        }

//        public void Expand()
//        {
//            Expand(false, false);
//        }

//        public virtual void Expand(bool force, bool includePrivate)
//        {
//            Dictionary<MemberInfo, InspectorReflectionHelper.SetterGetterPair> setterGetterDict;

//            // Root will never be null, so we check if a child is null
//            // or unreadable before trying to expanding it
//            if (IsExtraNode)
//                return;
//            if (Parent != null && (!CanRead || Property == null))
//                return;
//            if (Children.Count != 0)
//                if (force)
//                    Children.Clear();
//                else// Already expanded?
//                    return;

//            ChildrenView = new CollectionViewSource();
//            ChildrenView.Source = Children;
//            ChildrenView.Filter += new FilterEventHandler(CollectionViewSource_Filter);

//            // If there's no UI container created for us yet then we can't expand
//            if (UIContainer == null)
//                return;

//            try
//            {
//                setterGetterDict = InspectorReflectionHelper.GetSetterGetterDict3(this);
//            }
//            catch (CompileErrorException)
//            {
//                GearsetResources.Console.Log("Gearset", "A compiler error occured, try verifying that the class you're inspecting is private");
//                return;
//            }

//            List<FieldInfo> fields = new List<FieldInfo>(Type.GetInstanceFields(!includePrivate));
//            List<PropertyInfo> properties = new List<PropertyInfo>(Type.GetInstanceProperties(!includePrivate));
//            List<MethodInfo> methods = new List<MethodInfo>(Type.GetInstanceMethods());
//            List<InspectorNode> sortedChildren = new List<InspectorNode>(fields.Count + properties.Count);

//            InspectorReflectionHelper.SetterGetterPair pair;

//            foreach (var field in fields)
//            {
//                if (field.GetCustomAttributes(typeof(InspectorIgnoreAttribute), true).Length > 0)
//                    continue;
//                if (field.FieldType.IsPointer)
//                    continue;
//                try
//                {
//                    pair = setterGetterDict[field];
//                }
//                catch
//                {
//                    GearsetResources.Console.Log("Gearset", "Field {0} could not be inspected.", field.Name);
//                    continue;
//                }

//                // Do we have a friendly name?
//                String friendlyName = field.Name;
//                bool hideCantWriteIcon = false;
//                foreach (InspectorAttribute attribute in field.GetCustomAttributes(typeof(InspectorAttribute), true))
//                {
//                    if (attribute.FriendlyName != null)
//                        friendlyName = attribute.FriendlyName;
//                    hideCantWriteIcon = attribute.HideCantWriteIcon;
//                }
//                sortedChildren.Add(new InspectorNode(this, field.FieldType, field.Name, pair.Setter, pair.Getter, hideCantWriteIcon)
//                {
//                    IsProperty = false,
//                    FriendlyName = friendlyName,
//                    IsPrivate = field.IsPrivate || field.IsFamilyOrAssembly || field.IsFamily || field.IsAssembly || field.IsFamilyAndAssembly,
//                });
//            }

//            foreach (var property in properties)
//            {
//                if (property.GetCustomAttributes(typeof(InspectorIgnoreAttribute), true).Length > 0)
//                    continue;
//                if (property.PropertyType.IsPointer)
//                    continue;
//                try
//                {
//                    pair = setterGetterDict[property];
//                }
//                catch
//                {
//                    GearsetResources.Console.Log("Gearset", "Property {0} could not be inspected.", property.Name);
//                    continue;
//                }

//                // Do we have a friendly name?
//                String friendlyName = property.Name;
//                bool hideCantWriteIcon = false;
//                foreach (InspectorAttribute attribute in property.GetCustomAttributes(typeof(InspectorAttribute), true))
//                {
//                    if (attribute.FriendlyName != null)
//                        friendlyName = attribute.FriendlyName;
//                    hideCantWriteIcon = attribute.HideCantWriteIcon;
//                }

//                MethodInfo getMethod = property.GetGetMethod(true);
//                MethodInfo setMethod = property.GetSetMethod(true);
//                bool privateGet = getMethod != null && (getMethod.IsPrivate || getMethod.IsFamilyOrAssembly || getMethod.IsFamily || getMethod.IsAssembly || getMethod.IsFamilyAndAssembly);
//                bool privateSet = setMethod != null && (setMethod.IsPrivate || setMethod.IsFamilyOrAssembly || setMethod.IsFamily || setMethod.IsAssembly || setMethod.IsFamilyAndAssembly);

//                // If there's one that's not private, add it in the public part.
//                if ((!privateGet && getMethod != null) || (!privateSet && setMethod != null))
//                {
//                    sortedChildren.Add(new InspectorNode(this,
//                            property.PropertyType,
//                            property.Name,
//                            privateSet ? null : pair.Setter,
//                            privateGet ? null : pair.Getter,
//                            hideCantWriteIcon)
//                    {
//                        IsProperty = true,
//                        FriendlyName = friendlyName,
//                        IsPrivate = false,
//                    });
//                }

//                // If on accessor is private, we have to add it again in the private part with full access.
//                if (includePrivate && (privateGet || privateSet))
//                {
//                    sortedChildren.Add(new InspectorNode(this,
//                            property.PropertyType,
//                            property.Name,
//                            pair.Setter,
//                            pair.Getter,
//                            hideCantWriteIcon)
//                    {
//                        IsProperty = true,
//                        FriendlyName = friendlyName,
//                        IsPrivate = true,
//                    });
//                }

//            }

//            // HACK: this could be done in the UI layer using the ListView.View property.
//            //sortedChildren.Sort(AlphabeticalComparison);
//            foreach (InspectorNode child in sortedChildren)
//                Children.Add(child);

//            // Special markers to add children to special types
//            // TODO: make this extensible.
//            // EXTRAS:
//            if (typeof(IEnumerable).IsAssignableFrom(Type))
//                Children.Add(new InspectorNode(this, typeof(CollectionMarker), String.Empty, null, Getter != null ? Getter : (x) => { return RootTarget; }, false) { IsExtraNode = true });
//            if (typeof(Texture2D).IsAssignableFrom(Type))
//                Children.Add(new InspectorNode(this, typeof(Texture2DMarker), String.Empty, null, Getter != null ? Getter : (x) => { return RootTarget; }, false) { IsExtraNode = true });
//            if (typeof(Vector2).IsAssignableFrom(Type))
//                Children.Add(new InspectorNode(this, typeof(float), "Vector Length",
//                    null, // Setter
//                    (x) => ((Vector2)this.Property).Length(), true)                                              // Getter
//                    { IsExtraNode = true });
//            if (typeof(Vector3).IsAssignableFrom(Type))
//                Children.Add(new InspectorNode(this, typeof(float), "Vector Length",
//                    null, // Setter
//                    (x) => ((Vector3)this.Property).Length(), true)                                              // Getter
//                    { IsExtraNode = true });

//            // Add methods that don't take any params
//            Methods.Clear();
//            foreach (var method in methods)
//            {
//                if (method.GetParameters().Length != 0)
//                    continue;
//                if (method.IsDefined(typeof(CompilerGeneratedAttribute), true))
//                    continue;
//                if (method.IsSpecialName)
//                    continue;
//                if (method.DeclaringType == typeof(Object))
//                    continue;

//                // Do we have a friendly name?
//                String friendlyName = method.Name;
//                foreach (InspectorMethodAttribute attribute in method.GetCustomAttributes(typeof(InspectorMethodAttribute), true))
//                {
//                    if (attribute.FriendlyName != null)
//                    {
//                        friendlyName = attribute.FriendlyName;
//                        InspectorNode methodNodec = new InspectorNode(this, typeof(void), method.Name, null, null, false) { FriendlyName = friendlyName };
//                        methodNodec.Method = method;
//                        Children.Add(methodNodec);
//                    }
//                }
//                InspectorNode methodNode = new InspectorNode(this, typeof(void), method.Name, null, null, false) { FriendlyName = friendlyName };
//                methodNode.Method = method;
//                Methods.Add(methodNode);
//            }

//            // Add extension methods (if any)
//            foreach (var t in ExtensionMethodTypes)
//            {
//                foreach (var method in t.GetStaticMethods())
//                {
//                    if (method.GetParameters().Length != 1)
//                        continue;
//                    else
//                    {
//                        // Do we have a friendly name for the method?
//                        String friendlyName = method.Name;
//                        foreach (InspectorMethodAttribute attribute in method.GetCustomAttributes(typeof(InspectorMethodAttribute), true))
//                        {
//                            if (attribute.FriendlyName != null)
//                            {
//                                friendlyName = attribute.FriendlyName;
//                                InspectorNode methodNodec = new InspectorNode(this, typeof(void), method.Name, null, null, false) { FriendlyName = friendlyName };
//                                methodNodec.Method = method;
//                                Children.Add(methodNodec);
//                            }
//                        }
//                        Type paramType = method.GetParameters()[0].ParameterType;
//                        if (paramType.IsAssignableFrom(this.Type))
//                        {
//                            InspectorNode methodNode = new InspectorNode(this, typeof(void), method.Name, null, null, true) { FriendlyName = friendlyName };
//                            methodNode.Method = method;
//                            Methods.Add(methodNode);
//                        }
//                    }
//                }
//            }
//        }
    }
}
