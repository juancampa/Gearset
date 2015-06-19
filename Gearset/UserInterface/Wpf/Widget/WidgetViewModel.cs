#if WPF
    using System;
    using System.Collections.ObjectModel;

    namespace Gearset.UserInterface.Wpf.Widget
    {
        public class WidgetViewModel
        {
            public class ActionItem
            {
                internal Action Action;
                internal String Name;
                public override string ToString()
                {
                    return Name;
                }
            }

            public readonly ObservableCollection<ActionItem> ButtonActions = new ObservableCollection<ActionItem>();

            public void AddAction(string name, Action action)
            {
                // Search for an action with that name.
                foreach (var t in ButtonActions)
                {
                    if (t.Name != name)
                        continue;

                    t.Action = action;
                    return;
                }

                // New action
                if (name != null && action != null)
                    ButtonActions.Add(new ActionItem { Name = name, Action = action });
            }
        }
    }
#endif