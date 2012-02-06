using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Windows.Data;

namespace Gearset.Components.InspectorWPF
{
    public class InspectorNode : INotifyPropertyChanged
    {
        /// <summary>
        /// This list contains a list of Types that 
        /// contain extension methods that we will call
        /// on types.
        /// </summary>
        public static List<Type> ExtensionMethodTypes = new List<Type>();

        /// <summary>
        /// So we can notify when a bound property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The TreeViewNode which holds this node.
        /// </summary>
        public TreeViewItem UIContainer { 
            get { return uiContainer; } 
            set {
                if (uiContainer != null)
                {
                    uiContainer.ItemContainerGenerator.StatusChanged -= new EventHandler(ItemContainerGenerator_StatusChanged);
                    wasPreviousContainerExpanded = uiContainer.IsExpanded;
                }
                uiContainer = value;
                if (uiContainer != null)
                {
                    uiContainer.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
                    uiContainer.IsExpanded = wasPreviousContainerExpanded;
                    ItemContainerGenerator_StatusChanged(this, EventArgs.Empty);
                }
            } 
        }
        private TreeViewItem uiContainer;

        // Since containers change when filtering, we save here if the previous container was expaneded
        // so we can reexpand to restore the previous state.
        private bool wasPreviousContainerExpanded = false;

        /// <summary>
        /// Returns the root of the tree.
        /// </summary>
        public InspectorNode Root
        {
            get
            {
                var n = this;
                while (n.Parent != null) 
                    n = n.Parent;
                return n;
            }
        }
        /// <summary>
        /// This value will only be set in the root node.
        /// </summary>
        internal Object RootTarget { get { return rootTarget; }
            set { rootTarget = value; Type = value.GetType(); }
        }
        private Object rootTarget;

        /// <summary>
        /// Returns true if the node is an ExtraNode, that means that it does not correspond 
        /// to any field or property of the parent but is added as an extra control to better
        /// manipulate the parent.
        /// </summary>
        public bool IsExtraNode { get; private set; }

        /// <summary>
        /// Will turn true when the user modify this property through the UI.
        /// </summary>
        public bool UserModified { get { return userModified; } private set { userModified = value; OnPropertyChanged("UserModified"); } }
        private bool userModified;

        /// <summary>
        /// The VisualItem that can Update the variable 
        /// and Update the UI
        /// </summary>
        internal VisualItemBase VisualItem;

        /// <summary>
        /// If true, the UI will be updated every frame to reflect
        /// the node value.
        /// </summary>
        public bool Updating { get { return updating; } 
            set { updating = value; if (updating) Force = false; OnPropertyChanged("Updating"); } }
        private bool updating;

        /// <summary>
        /// If true, the value set in the UI will be set to
        /// this node every frame.
        /// </summary>
        public bool Force { get { return force; } set { force = value; if (force) Updating = false; OnPropertyChanged("Force"); } }
        private bool force;

        /// <summary>
        /// If false, the "Cant write" icon won't be showed. This property is to be binded by WPF.
        /// </summary>
        public bool ShowCantWriteIcon { get { return !hideCantWriteIcon && (!CanWrite); } }
        private bool hideCantWriteIcon;

        /// <summary>
        /// Gets or sets whether the private children of this node are shown or not.
        /// </summary>
        public bool IsShowingPrivate { 
            get { return isShowingPrivate; } 
            set { 
                isShowingPrivate = value;

                // Don't show private members of our own code.
                if (Type.Assembly != typeof(InspectorNode).Assembly &&
                    Type.Assembly != typeof(Gearset.Component.GraphicsDeviceManager).Assembly)
                    Expand(true, isShowingPrivate); 
            } 
        }
        private bool isShowingPrivate;

        /// <summary>
        /// The target object (always represented by the root of the tree)
        /// so it is looked up recursively.
        /// </summary>
        public Object Target
        {
            get
            {
                if (Parent == null)
                    return RootTarget;
                else
                    return Parent.Target;
            }
        }

        public InspectorNode Parent { get; private set; }

        /// <summary>
        /// Used to reference this node in XAML
        /// </summary>
        public InspectorNode Itself { get { return this; } }

        /// <summary>
        /// The name of the field this node represents.
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Name of this node shown in Inspector tree.
        /// </summary>
        public String FriendlyName
        {
            get
            {
                if (friendlyName != null)
                    return friendlyName;
                return Name;
            }
            set
            {
                friendlyName = value;
                OnPropertyChanged("FriendlyName");
            }
        }
        private String friendlyName;

        private Type type;
        /// <summary>
        /// The type of the field this node represents.
        /// </summary>
        public Type Type { get { return type; } private set { type = value; OnPropertyChanged("Type"); } }

        /// <summary>
        /// A list that contains nodes that represent the
        /// fields of the object represented by this node.
        /// </summary>
        public ObservableCollection<InspectorNode> Children { get; private set; }

        /// <summary>
        /// The children as must be viewed by the UI layer.
        /// </summary>
        public CollectionViewSource ChildrenView { get { return childrenView; } private set { childrenView = value; OnPropertyChanged("ChildrenView"); } }
        private CollectionViewSource childrenView;

        /// <summary>
        /// A list that contains methods available for this object.
        /// TODO: a global Dictionary(Type, List(Method)) might be better.
        /// </summary>
        public ICollection<InspectorNode> Methods { get; private set; }

        /// <summary>
        /// True if the value this node represents can be set.
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return Parent == null ? true : Setter != null;
            }
        }

        /// <summary>
        /// True if the value this node represents can be get.
        /// </summary>
        public bool CanRead 
        {
            get
            {
                return Parent == null ? true : Getter != null;
            }
        }

        /// <summary>
        /// True if this node represents a property, otherwise
        /// it represents a field. We store this information
        /// in order to know when a ValueType is encapsulated 
        /// so we should instanciate it.
        /// </summary>
        public bool IsProperty { get; set; }

        private bool isPrivate;
        /// <summary>
        /// Gets whether this node is declared private inside its parent.
        /// </summary>
        public bool IsPrivate { get { return isPrivate; } set { isPrivate = value; OnPropertyChanged("IsPrivate"); } }

        /// <summary>
        /// True if Parent equals null.
        /// </summary>
        public bool IsRoot { get { return Parent == null; } }

        /// <summary>
        /// True for nodes that will be auto-expanded when the UI
        /// for it is generated. Setting this value will only have
        /// effect if the UI hasn't been created.
        /// </summary>
        public bool AutoExpand { get; internal set; }

        // Helper methods
        private Setter Setter;
        private Getter Getter;

        /// <summary>
        /// The object this node represents.
        /// This will produce boxing/unboxing.
        /// </summary>
        public Object Property
        {
            set {
                try
                {
                    if (Setter != null)
                    {
                        Setter(Target, value);

                        // Set the UserModified flag all the way to the root.
                        if (!UserModified)
                        {
                            InspectorNode n = this;
                            while (n != null)
                            {
                                n.UserModified = true;
                                n = n.Parent;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // TODO: Change this behavior: instead disable the control and show the exception. Add a button to re-enable it.
                    GearsetResources.Console.Log("Gearset", "An error occured while setting the value of the property " + this + ". This value won't be inspectable anymore.");
                    GearsetResources.Console.Log("Gearset", "Exception Thrown:");
                    GearsetResources.Console.Log("Gearset", "   " + e.Message.Replace("\n", "\n   "));
                    Setter = null;
                }
            }
            get {
                try
                {
                    if (IsRoot)
                        return Target;
                    if (Getter != null)
                        return Getter(Target);

                    return null;
                }
                catch (Exception e)
                {
                    GearsetResources.Console.Log("Gearset", "An error occured while getting the value of the property " + this + ". This value won't be inspectable anymore.");
                    GearsetResources.Console.Log("Gearset", "Exception Thrown:");
                    GearsetResources.Console.Log("Gearset", "   " + e.Message.Replace("\n", "\n   "));
                    Getter = null;
                    return null;
                }
            }
        }

        /// <summary>
        /// Use this constructor to create child nodes.
        /// </summary>
        /// <param name="type">The type of the field this node represets</param>
        /// <param name="name">The name of the field this node represets</param>
        /// <param name="setter">Helper delegate to set the value of the field.</param>
        /// <param name="getter">Helper delegate to get the value of the field.</param>
        public InspectorNode(InspectorNode parent, Type type, String name, Setter setter, Getter getter, bool hideCanWriteIcon)
        {
            Children = new ObservableCollection<InspectorNode>();
            Methods = new ObservableCollection<InspectorNode>();
            Name = name;
            Type = type;
            Setter = setter;
            Getter = getter;
            Parent = parent;

            Updating = true;
            this.hideCantWriteIcon = hideCanWriteIcon;
        }

        /// <summary>
        /// Use this constructor to create the root node
        /// </summary>
        /// <param name="type">The type of the field this node represets</param>
        /// <param name="name">The name of the field this node represets</param>
        /// <param name="setter">Helper delegate to set the value of the field.</param>
        /// <param name="getter">Helper delegate to get the value of the field.</param>
        public InspectorNode(Object target, String name, bool autoExpand)
        {
            Children = new ObservableCollection<InspectorNode>();
            Methods = new ObservableCollection<InspectorNode>();
            RootTarget = target;
            Name = name;
            Type = target.GetType();

            // The root object is direcly accesable
            IsProperty = false;

            AutoExpand = autoExpand;
        }

        public void Expand()
        {
            Expand(false, false);
        }
        /// <summary>
        /// Fills the list of Children with nodes.
        /// </summary>
        /// <param name="force">if set to <c>true</c> the children, if any, will be deleted and the node reexpanded.</param>
        public virtual void Expand(bool force, bool includePrivate)
        {
            Dictionary<MemberInfo, InspectorReflectionHelper.SetterGetterPair> setterGetterDict;

            // Root will never be null, so we check if a child is null
            // or unreadable before trying to expanding it
            if (IsExtraNode)
                return;
            if (Parent!=null && (!CanRead || Property == null))
                return;
            if (Children.Count != 0)
                if (force)
                    Children.Clear();
                else// Already expanded?
                    return;

            ChildrenView = new CollectionViewSource();
            ChildrenView.Source = Children;
            ChildrenView.Filter += new FilterEventHandler(CollectionViewSource_Filter);

            // If there's no UI container created for us yet then we can't expand
            if (UIContainer == null)
                return;

            try
            {
                 setterGetterDict = InspectorReflectionHelper.GetSetterGetterDict3(this);
            }
            catch (CompileErrorException)
            {
                GearsetResources.Console.Log("Gearset", "A compiler error occured, try verifying that the class you're inspecting is private");
                return;
            }

            List<FieldInfo> fields = new List<FieldInfo>(Type.GetInstanceFields(!includePrivate));
            List<PropertyInfo> properties = new List<PropertyInfo>(Type.GetInstanceProperties(!includePrivate));
            List<MethodInfo> methods = new List<MethodInfo>(Type.GetInstanceMethods());
            List<InspectorNode> sortedChildren = new List<InspectorNode>(fields.Count + properties.Count);

            InspectorReflectionHelper.SetterGetterPair pair;

            foreach (var field in fields)
            {
                if (field.GetCustomAttributes(typeof(InspectorIgnoreAttribute), true).Length > 0)
                    continue;
                if (field.FieldType.IsPointer)
                    continue;
                try
                {
                    pair = setterGetterDict[field];
                }
                catch
                {
                    GearsetResources.Console.Log("Gearset", "Field {0} could not be inspected.", field.Name);
                    continue;
                }

                // Do we have a friendly name?
                String friendlyName = field.Name;
                bool hideCantWriteIcon = false;
                foreach (InspectorAttribute attribute in field.GetCustomAttributes(typeof(InspectorAttribute), true))
                {
                    if (attribute.FriendlyName != null)
                        friendlyName = attribute.FriendlyName;
                    hideCantWriteIcon = attribute.HideCantWriteIcon;
                }
                sortedChildren.Add(new InspectorNode(this, field.FieldType, field.Name, pair.Setter, pair.Getter, hideCantWriteIcon) 
                { 
                    IsProperty = false, 
                    FriendlyName = friendlyName,
                    IsPrivate = field.IsPrivate || field.IsFamilyOrAssembly || field.IsFamily || field.IsAssembly || field.IsFamilyAndAssembly,
                });
            }

            foreach (var property in properties)
            {
                if (property.GetCustomAttributes(typeof(InspectorIgnoreAttribute), true).Length > 0)
                    continue;
                if (property.PropertyType.IsPointer)
                    continue;
                try
                {
                    pair = setterGetterDict[property];
                }
                catch
                {
                    GearsetResources.Console.Log("Gearset", "Property {0} could not be inspected.", property.Name);
                    continue;
                }

                // Do we have a friendly name?
                String friendlyName = property.Name;
                bool hideCantWriteIcon = false;
                foreach (InspectorAttribute attribute in property.GetCustomAttributes(typeof(InspectorAttribute), true))
                {
                    if (attribute.FriendlyName != null)
                        friendlyName = attribute.FriendlyName;
                    hideCantWriteIcon = attribute.HideCantWriteIcon;
                }

                MethodInfo getMethod = property.GetGetMethod(true);
                MethodInfo setMethod = property.GetSetMethod(true);
                bool privateGet = getMethod != null ? getMethod.IsPrivate || getMethod.IsFamilyOrAssembly || getMethod.IsFamily || getMethod.IsAssembly || getMethod.IsFamilyAndAssembly : false;
                bool privateSet = setMethod != null ? setMethod.IsPrivate || setMethod.IsFamilyOrAssembly || setMethod.IsFamily || setMethod.IsAssembly || setMethod.IsFamilyAndAssembly : false;

                // If there's one that's not private, add it in the public part.
                if ((!privateGet && getMethod != null) || (!privateSet && setMethod != null))
                {
                    sortedChildren.Add(new InspectorNode(this,
                            property.PropertyType,
                            property.Name,
                            privateSet ? null : pair.Setter,
                            privateGet ? null : pair.Getter,
                            hideCantWriteIcon)
                    {
                        IsProperty = true,
                        FriendlyName = friendlyName,
                        IsPrivate = false,
                    });
                }

                // If on accessor is private, we have to add it again in the private part with full access.
                if (includePrivate && (privateGet || privateSet))
                {
                    sortedChildren.Add(new InspectorNode(this,
                            property.PropertyType,
                            property.Name,
                            pair.Setter,
                            pair.Getter,
                            hideCantWriteIcon)
                    {
                        IsProperty = true,
                        FriendlyName = friendlyName,
                        IsPrivate = true,
                    });
                }
                
            }

            // HACK: this could be done in the UI layer using the ListView.View property.
            //sortedChildren.Sort(AlphabeticalComparison);
            foreach (InspectorNode child in sortedChildren)
                Children.Add(child);

            // Special markers to add children to special types
            // TODO: make this extensible.
            // EXTRAS:
            if (typeof(IEnumerable).IsAssignableFrom(Type))
                Children.Add(new InspectorNode(this, typeof(CollectionMarker), String.Empty, null, Getter != null ? Getter : (x) => { return RootTarget; }, false) { IsExtraNode = true });
            if (typeof(Texture2D).IsAssignableFrom(Type))
                Children.Add(new InspectorNode(this, typeof(Texture2DMarker), String.Empty, null, Getter != null ? Getter : (x) => { return RootTarget; }, false) { IsExtraNode = true });
            if (typeof(Vector2).IsAssignableFrom(Type))
                Children.Add(new InspectorNode(this, typeof(float), "Vector Length",
                    null, // Setter
                    (x) => ((Vector2)this.Property).Length(), true)                                              // Getter
                    { IsExtraNode = true });
            if (typeof(Vector3).IsAssignableFrom(Type))
                Children.Add(new InspectorNode(this, typeof(float), "Vector Length",
                    null, // Setter
                    (x) => ((Vector3)this.Property).Length(), true)                                              // Getter
                    { IsExtraNode = true });

            // Add methods that don't take any params
            Methods.Clear();
            foreach (var method in methods)
            {
                if (method.GetParameters().Length != 0)
                    continue;
                if (method.IsDefined(typeof(CompilerGeneratedAttribute), true))
                    continue;
                if (method.IsSpecialName)
                    continue;
                if (method.DeclaringType == typeof(Object))
                    continue;

                // Do we have a friendly name?
                String friendlyName = method.Name;
                foreach (InspectorMethodAttribute attribute in method.GetCustomAttributes(typeof(InspectorMethodAttribute), true))
                {
                    if (attribute.FriendlyName != null)
                    {
                        friendlyName = attribute.FriendlyName;
                        InspectorNode methodNodec = new InspectorNode(this, typeof(void), method.Name, null, null, false) { FriendlyName = friendlyName };
                        methodNodec.Method = method;
                        Children.Add(methodNodec);
                    } 
                }
                InspectorNode methodNode = new InspectorNode(this, typeof(void), method.Name, null, null, false) { FriendlyName = friendlyName };
                methodNode.Method = method;
                Methods.Add(methodNode);
            }

            // Add extension methods (if any)
            foreach (var t in ExtensionMethodTypes)
            {
                foreach (var method in t.GetStaticMethods())
                {
                    if (method.GetParameters().Length != 1)
                        continue;
                    else
                    {
                        // Do we have a friendly name for the method?
                        String friendlyName = method.Name;
                        foreach (InspectorMethodAttribute attribute in method.GetCustomAttributes(typeof(InspectorMethodAttribute), true))
                        {
                            if (attribute.FriendlyName != null)
                            {
                                friendlyName = attribute.FriendlyName;
                                InspectorNode methodNodec = new InspectorNode(this, typeof(void), method.Name, null, null, false) { FriendlyName = friendlyName };
                                methodNodec.Method = method;
                                Children.Add(methodNodec);
                            }    
                        }
                        Type paramType = method.GetParameters()[0].ParameterType;
                        if (paramType.IsAssignableFrom(this.Type))
                        {
                            InspectorNode methodNode = new InspectorNode(this, typeof(void), method.Name, null, null, true) { FriendlyName = friendlyName };
                            methodNode.Method = method;
                            Methods.Add(methodNode);
                        }
                    }
                }
            }
        }

        Random random = new Random();
        void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = GearsetResources.Console.Inspector.FilterPredicate(e.Item);
        }

        private static int AlphabeticalComparison(InspectorNode a, InspectorNode b)
        {
            return String.Compare(a.Name, b.Name);
        }

        /// <summary>
        /// Get the TreeViewItems (containers) and let the InspectorTreeNodes
        /// know where they are.
        /// </summary>
        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (UIContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                int i = 0;
                foreach (var item in Children)
                {
                    InspectorNode child = (InspectorNode)item;
                    if (child.UIContainer == null || (child.UIContainer != null && child.UIContainer.Header != null && child.UIContainer.Header.ToString().Equals("{DisconnectedItem}")))
                    {
                        child.UIContainer = (TreeViewItem)UIContainer.ItemContainerGenerator.ContainerFromItem(child);

                        if (GearsetResources.Console.Inspector.Config.ModifiedOnly && child.UIContainer != null)
                            child.UIContainer.IsExpanded = true;
                    }
                    i++;
                }
            }
        }

        /// <summary>
        /// Only set for Nodes of Type Void (methods) (no getter nor setter).
        /// </summary>
        public MethodInfo Method;

        /// <summary>
        /// Returns the path that leads to this node from the
        /// Target object with and added point at the end.
        /// </summary>
        internal String GetPath()
        {
            if (Parent == null) return Name;
            return Parent.GetPath() + "." + this.Name;
        }

        public override string ToString()
        {
            return GetPath();
        }

        /// <summary>
        /// Method to rise the event.
        /// </summary>
        protected void OnPropertyChanged(string name)
          {
              PropertyChangedEventHandler handler = PropertyChanged;
              if (handler != null)
              {
                  handler(this, new PropertyChangedEventArgs(name));
              }
          }

        /// <summary>
        /// Calls the update callback so the UI can get updated
        /// and recursively call Update on it's children.
        /// </summary>
        public void Update()
        {
            if (UIContainer == null)
                return;
            bool visuallyExpanded = ((TreeViewItem)UIContainer).IsExpanded;

            // Check if property suddenly became null.
            if (CanRead && !IsRoot && Property==null)
            {
                Children.Clear();
            }

            // Update if the update button is pressed and if we're expanded (vissually) and if there's a getter
            //if (VisualItem != null && Updating && (!visuallyExpanded || VisualItem.UpdateIfExpanded) && CanRead)
            if (VisualItem != null && Updating && CanRead)
                VisualItem.UpdateUI(Property);

            // It makes no sense to force the value while updating.
            if (!Updating && Force)
                VisualItem.UpdateVariable();


            // Only update children if we're visually expanded.
            if (UIContainer != null && visuallyExpanded)
            {
                if (ChildrenView != null && ChildrenView.View != null)
                {
                    foreach (InspectorNode child in ChildrenView.View)
                    {
                        child.Update();
                    }
                }
            }
        }

    }
}
