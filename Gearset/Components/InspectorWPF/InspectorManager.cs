using System;
using System.Reflection;
using System.ComponentModel;
using Gearset.UserInterface;

namespace Gearset.Components.InspectorWPF
{
    public class InspectorManager : Gear
    {
        readonly IUserInterface _userInterface;

        /// <summary>
        /// Gets the Inspector's config.
        /// </summary>
        public InspectorConfig Config { get; private set; }

        /// <summary>
        /// Constructor, creates the inspector logger.
        /// </summary>
        public InspectorManager(IUserInterface userInterface)
            : base(GearsetSettings.Instance.InspectorConfig)
        {
            Config = GearsetSettings.Instance.InspectorConfig;
            
            _userInterface = userInterface;
            _userInterface.CreateInspector(this);
        }

        protected override void OnVisibleChanged()
        {
            _userInterface.InspectorVisible = Visible;
        }

        /// <summary>
        /// Adds the object to the inspector logger so the user
        /// can inspect its fields, properties and methods. The node
        /// will be autoExpanded.
        /// </summary>
        /// <param name="name">A friendly name to use in the Inspector Tree.</param>
        /// <param name="o">The object to inspect.</param>
        public void Inspect(String name, Object o)
        {
            Inspect(name, o, false);
        }

        /// <summary>
        /// Adds the object to the inspector logger so the user
        /// can inspect its fields, properties and methods.
        /// </summary>
        /// <param name="name">A friendly name to use in the Inspector Tree.</param>
        /// <param name="o">The object to inspect.</param>
        /// <param name="autoExpand">Determines whether the node should automatically expand when added to the Inspector Tree.</param>
        public void Inspect(String name, Object o, bool autoExpand)
        {
            if (String.IsNullOrEmpty(name))
                name = "(unnamed object)";
            if (o == null)
                return;

            var t = o.GetType();
            if (t.IsValueType)
            {
                //GearsetResources.Console.Log("Gearset", "ValueTypes cannot be directly inspected. Ignoring {0} ({1}).", name, t.Name);
                //return;

                t = Type.GetType("Gearset.Components.InspectorWPF.ValueTypeWrapper`1").MakeGenericType(t);
                var wrapper = Activator.CreateInstance(t);
                var wrapperType = wrapper.GetType();
                wrapperType.GetProperty("Value").SetValue(wrapper, o, null);
                o = wrapper;
            }
            if (t == typeof(String))
            {
                GearsetResources.Console.Log("Gearset", "Strings cannot be directly inspected. Ignoring {0} ({1}).", name, t.Name);
                return;
            }
            if (o == null)
            {
                GearsetResources.Console.Log("Gearset", "Object to inspect cannot be null. Ignoring {0} ({1}).", name, t.Name);
                return;
            }
            if (t.IsNestedPrivate)
            {
                GearsetResources.Console.Log("Gearset", "Cannot inspect inner (nested) types that are private. Ignoring {0} ({1}).", name, t.Name);
                return;
            }

            _userInterface.Inspect(name, o, autoExpand, t);
        }

        public bool FilterPredicate(object item) 
        { 
            return _userInterface.FilterPredicate(item);
        }

        public object SelectedItem
        {
            get { return _userInterface.InspectorSelectedItem; }
        }

        /// <summary>
        /// Adds the object to the inspector logger so the user can inspect its fields, properties and methods.
        /// </summary>
        /// <param name="node"></param>
        internal void Watch(InspectorNode node)
        {
            _userInterface.Watch(node);
        }

        /// <summary>
        /// Remove the object from the inspector, if exist.
        /// </summary>
        /// <param name="o">The object to remove.</param>
        public void RemoveInspect(Object o)
        {
            _userInterface.RemoveInspect(o);
        }
     
        public void CraftMethodCall(MethodInfo info)
        {
            _userInterface.CraftMethodCall(info);
        }

        public void ClearInspectedObjects()
        {
            _userInterface.ClearInspectedObjects();
        }

        public void ClearMethods()
        {
            _userInterface.ClearMethods();
        }

        internal void AddNotice(string message, string url, string linkText)
        {
            _userInterface.AddNotice(message, url, linkText);
        }
    }

    public class ValueTypeWrapper<T> where T:struct
    {
        public T Value { get; set; }

        public override string ToString()
        {
            return Value + " [copy]";
        }
    }
}