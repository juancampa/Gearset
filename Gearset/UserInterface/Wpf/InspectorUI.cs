
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using Gearset.Components;
using Gearset.Components.InspectorWPF;

namespace Gearset.UserInterface.Wpf
{
    public class InspectorUI : INotifyPropertyChanged
    {
        readonly Inspector _inspectorWindow;
        public InspectorConfig Config { get; private set; }

        bool _inspectorLocationJustChanged;

        readonly BackgroundWorker _filterWorker = new BackgroundWorker();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Objects being inspected.
        /// </summary>
        public ObservableCollection<InspectorNode> InspectedObjects;

        /// <summary>
        /// Nodes shown in the Watch window.
        /// </summary>
        public ObservableCollection<InspectorNode> WatchedNodes;

        /// <summary>
        /// Methods Callers
        /// </summary>
        public ObservableCollection<MethodCaller> MethodCallers;

        /// <summary>
        /// List of notices to show.
        /// </summary>
        private ObservableCollection<NoticeViewModel> _notices;
        public ObservableCollection<NoticeViewModel> Notices
        {
            get { return _notices; }
            set { _notices = value; OnPropertyChanged("Notices"); }
        }

        public string[] SearchTerms;

        /// <summary>
        /// It is set to the seconds to wait between the user stop typing and the filtering
        /// is actually performed. It is reset to some value with every keystroke.
        /// </summary>
        float _updateSearchFilteringDelay;

        public InspectorUI(InspectorConfig config, ICollection<IWindow> windows)
        {
            Config = config;
            Notices = new ObservableCollection<NoticeViewModel>();

            _inspectorWindow = new Inspector
            {
                Top = Config.Top,
                Left = Config.Left,
                Width = Config.Width,
                Height = Config.Height,
                DataContext = this
            };

            Config.ModifiedOnlyChanged += Config_ModifiedOnlyChanged;
            Config.SearchTextChanged += Config_SearchTextChanged;

            _updateSearchFilteringDelay = 0.25f;

            if (Config.Visible)
                _inspectorWindow.Show();


            windows.Add(_inspectorWindow);

            WindowHelper.EnsureOnScreen(_inspectorWindow);

            // This is needed in order to correctly handle keyboard input.
            ElementHost.EnableModelessKeyboardInterop(_inspectorWindow);

            InspectorNode.ExtensionMethodTypes.Add(typeof(InspectorExtensionsMethods));

            // Create both item sources.
            InspectedObjects = new ObservableCollection<InspectorNode>();
            WatchedNodes = new ObservableCollection<InspectorNode>();
            MethodCallers = new ObservableCollection<MethodCaller>();

            var source = new CollectionViewSource();
            source.Source = InspectedObjects;
            source.View.Filter = FilterPredicate;
            _inspectorWindow.TreeView1.ItemsSource = source.View;

            _inspectorWindow.TreeView1.ItemContainerGenerator.StatusChanged += ItemContainerGenerator1_StatusChanged;
            _inspectorWindow.TreeView1.SelectedItemChanged += TreeView1_SelectedItemChanged;

            _inspectorWindow.plots.DataContext = GearsetResources.Console.Plotter.Plots;

            _inspectorWindow.LocationChanged += (sender, args) => _inspectorLocationJustChanged = true;
            _inspectorWindow.SizeChanged += (sender, args) => _inspectorLocationJustChanged = true;
            _inspectorWindow.IsVisibleChanged += (sender, args) => Config.Visible = _inspectorWindow.IsVisible;
        }

        public void Update(double deltaTime)
        {
            if (_inspectorLocationJustChanged)
            {
                _inspectorLocationJustChanged = false;
                Config.Top = _inspectorWindow.Top;
                Config.Left = _inspectorWindow.Left;
                Config.Width = _inspectorWindow.Width;
                Config.Height = _inspectorWindow.Height;
            }

            foreach (var obj in _inspectorWindow.TreeView1.Items)
            {
                var o = (InspectorNode)obj;
                o.Update();
            }

            foreach (var o in MethodCallers)
            {
                o.Update();
            }

            if (_updateSearchFilteringDelay > 0)
            {
                _updateSearchFilteringDelay -= (float)deltaTime;
                if (_updateSearchFilteringDelay <= 0)
                {
                    _updateSearchFilteringDelay = 0;

                    // There's a chance that the worker is busy. Wait.
                    while (_filterWorker.IsBusy)
                    { }
                    _filterWorker.DoWork -= filterWorker_DoWork;
                    _filterWorker.DoWork += filterWorker_DoWork;
                    _filterWorker.RunWorkerAsync();
                }
            }

            // If the node expansion was generated because the currently selected node
            // dissapeared (because we're adding private fields, for example) then this
            // would generate a conflict with the expansion.
            if (_inspectorWindow.nodeToExpandAfterUpdate != null)
            {
                _inspectorWindow.nodeToExpandAfterUpdate.Expand();
                _inspectorWindow.nodeToExpandAfterUpdate = null;
            }
        }

        public bool Visible
        {
            set { SetWindowVisibility(_inspectorWindow, value); }
        }

        static void SetWindowVisibility(UIElement window, bool isVisible)
        {
            if (window != null)
                window.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        //Inspector

        public void AddNotice(string message, string url, string linkText)
        {
            var notice = new NoticeViewModel
            {
                NoticeText = message,
                NoticeHyperlinkUrl = url,
                NoticeHyperlinkText = linkText
            };
            _inspectorWindow.Dispatcher.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate { Notices.Add(notice); });
        }

        public object InspectorSelectedItem
        {
            get { return _inspectorWindow.TreeView1.SelectedItem; }
        }

        public void Watch(InspectorNode node) 
        {
            if (node == null)
                return;

            WatchedNodes.Add(node);
        }

        public void Inspect(string name, Object o, bool autoExpand, Type t) 
        {
            var insertPosition = Math.Min(2, InspectedObjects.Count);
            foreach (var currentObject in InspectedObjects)
            {
                var currentNode = currentObject;
                if (currentNode.Name == name)
                {
                    if (currentNode.Type != t)
                    {
                        InspectedObjects.Remove(currentNode);
                        currentNode = new InspectorNode(o, name, autoExpand);
                        InspectedObjects.Add(currentNode);
                    }
                    else
                    {
                        currentNode.RootTarget = o;
                    }
                    // This might be null if the window has not oppened yet.
                    if (currentNode.UIContainer != null)
                    {
                        currentNode.UIContainer.IsSelected = true;
                        currentNode.UIContainer.BringIntoView();
                    }
                    return;
                }
            }

            var root = new InspectorNode(o, name, autoExpand);
            InspectedObjects.Insert(insertPosition, root);
        }

        public void RemoveInspect(Object o) 
        {
            InspectorNode container = null;
            foreach (var node in InspectedObjects)
            {
                if (node.Target == o)
                {
                    container = node;
                    break;
                }
            }
            if (container != null)
                InspectedObjects.Remove(container);
        }

        public void CraftMethodCall(MethodInfo info)
        {
            MethodCaller caller = new GenericMethodCaller(info, null);
            MethodCallers.Add(caller);
        }

        public void ClearInspectedObjects()
        {
            for (var i = InspectedObjects.Count - 1; i >= 2; i--)
            {
                InspectedObjects.RemoveAt(i);
            }
        }

        public void ClearMethods()
        {
            MethodCallers.Clear();
        }

        public bool FilterPredicate(Object o)
        {
            // If there's nothing filtering, accept everything.
            if (!Config.ModifiedOnly && String.IsNullOrWhiteSpace(Config.SearchText)) return true;

            var node = o as InspectorNode;
            if (node != null)
            {
                var acceptedByModifiedOnly = (!Config.ModifiedOnly || node.UserModified);
                // HACK: The parent == null condition is to check if it's a root node,
                // this is a hack because a cleaner solution would be to use a different filter
                // predicate for child nodes.
                if (node.Parent == null && SearchTerms != null)
                {
                    for (var i = 0; i < SearchTerms.Length; i++)
                    {
                        if (!(node.Name.ToUpper().Contains(SearchTerms[i]) ||
                            node.Type.Name.ToUpper().Contains(SearchTerms[i])))
                        {
                            // Rejected by search
                            return false;
                        }
                    }
                }
                if (acceptedByModifiedOnly)
                    return true;
            }
            // Rejected by modifiedOnly
            return false;
        }

        /// <summary>
        /// Get the TreeViewItems (containers) and let the InspectorTreeNodes
        /// know where they are.
        /// </summary>
        void ItemContainerGenerator1_StatusChanged(object sender, EventArgs e)
        {
            if (_inspectorWindow.TreeView1.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                foreach (var obj in _inspectorWindow.TreeView1.Items)
                {
                    var o = (InspectorNode)obj;
                    if (o.UIContainer == null || (o.UIContainer != null && o.UIContainer.Header != null && o.UIContainer.Header.ToString().Equals("{DisconnectedItem}")))
                    {
                        var item = _inspectorWindow.TreeView1.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
                        o.UIContainer = item;

                        // If we're in the Modified only view, expand everything.
                        if (Config.ModifiedOnly && o.UIContainer != null)
                            o.UIContainer.IsExpanded = true;


                        // If this item didn't have a UIContainer is because it is new, expand it. 
                        // TODO: This line used to expand every root node, but it makes the inspector really slow to appear. This should be configurable.
                        //o.Expand();
                        // item could be null if we're filtering the collection (there's an item but is not being show).
                        if (InspectedObjects.Count > 2 && item != null)
                        {
                            if (o.AutoExpand)
                            {
                                var treeViewItem = o.UIContainer;
                                if (treeViewItem != null)
                                    treeViewItem.IsExpanded = true;
                            }

                            item.IsSelected = true;
                            item.BringIntoView();
                        }
                    }
                }
            }
        }

        void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var node = e.NewValue as InspectorNode;
            if (node != null)
            {
                _inspectorWindow.methods.DataContext = node.Methods;
                _inspectorWindow.nodeToExpandAfterUpdate = node;
            }
        }

        void filterWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SearchTerms = Config.SearchText.ToUpper().Split(' ');
            var dispatcher = _inspectorWindow.TreeView1.Dispatcher;
            dispatcher.Invoke((Action)delegate { ((CollectionView)_inspectorWindow.TreeView1.ItemsSource).Refresh(); });
        }

        void source_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = FilterPredicate(e.Item);
        }

        public void Config_ModifiedOnlyChanged(object sender, EventArgs e)
        {
            foreach (var o in InspectedObjects)
            {
                UpdateFilterRecursively(o);
            }
            ((CollectionView)_inspectorWindow.TreeView1.ItemsSource).Refresh();
        }

        void Config_SearchTextChanged(object sender, EventArgs e)
        {
            // Wait until the filterworker is done in case it's doing something
            _updateSearchFilteringDelay = 0.25f;
        }

        void UpdateFilterRecursively(InspectorNode node)
        {
            if (node.ChildrenView != null)
            {
                foreach (var child in node.Children)
                {
                    UpdateFilterRecursively(child);
                }
                node.ChildrenView.View.Refresh();
                if (node.UserModified && node.UIContainer != null)
                    node.UIContainer.IsExpanded = true;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
