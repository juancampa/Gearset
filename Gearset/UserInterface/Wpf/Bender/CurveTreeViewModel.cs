using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset.Components.CurveEditorControl;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Gearset.UserInterface.Wpf.Bender;

namespace Gearset.Components
{
    class CurveTreeViewModel : INotifyPropertyChanged
    {
        public CurveEditorControl2 Control { get; private set; }

        private CurveWrapper selectedCurve;
        ///// <summary>
        ///// The currently selected curve. This value must be set by the owner
        ///// window of the control.
        ///// </summary>
        //public CurveWrapper SelectedCurve
        //{
        //    get
        //    {
        //        return selectedCurve;
        //    }
        //    set
        //    {
        //        selectedCurve = value;
        //        OnPropertyChanged("SelectedCurve");
        //    }
        //}

        private CurveTreeNode root;
        public CurveTreeNode Root { get { return root; } private set { root = value; OnPropertyChanged("Root"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        

        /// <summary>
        /// A random used to generate colors, used with a predefined seed so multiple
        /// runs of the same code path will yield the same colors.
        /// </summary>
        private readonly Random random = new Random(32154);


        public CurveTreeViewModel(CurveEditorControl2 control)
        {
            this.Control = control;
            this.Root = new CurveTreeNode(String.Empty, null);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Removes the provided Curve from Bender.
        /// </summary>
        internal void RemoveCurve(Curve curve)
        {
            CurveTreeLeaf leaf = FindContainerLeaf(Root, curve);
            if (leaf != null)
            {
                RemoveNodeAndCurves(leaf);
                leaf.Parent.Children.Remove(leaf);
            }
        }

        private CurveTreeLeaf FindContainerLeaf(CurveTreeNode node, Curve curve)
        {
            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                CurveTreeLeaf leaf = node.Children[i] as CurveTreeLeaf;
                if (leaf != null && Object.ReferenceEquals(curve, leaf.Curve.Curve))
                {
                    return leaf;
                }
                // Call recursively on this child;
                leaf = FindContainerLeaf(node.Children[i], curve);
                if (leaf != null)
                    return leaf;
            }
            return null;
        }

        internal void RemoveCurveOrGroup(String name)
        {
            String[] path = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            CurveTreeNode child;
            CurveTreeNode node = Root;
            for (int i = 0; i < path.Length - 1; i++)
            {
                if ((child = GetChildNode(node, path[i])) == null)
                {
                    GearsetResources.Console.Log("Gearset", "Bender error. Curve or Group '{0}' doesn't exists.", name);
                    return;
                }
                node = child;
            }

            // Check if the last item actually exists, and finnally remove it.
            String curveName = path[path.Length - 1];
            if ((child = GetChildNode(node, curveName)) == null)
            {
                GearsetResources.Console.Log("Gearset", "Bender error. Curve or Group '{0}' doesn't exists.", name);
                return;
            }
            RemoveNodeAndCurves(child);
            node.Children.Remove(child);
        }

        /// <summary>
        /// Will remove the whole subtree of nodes below the provided node (from the control).
        /// The provided node must be removed from the tree by the calling code.
        /// </summary>
        private void RemoveNodeAndCurves(CurveTreeNode node)
        {
            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                RemoveNodeAndCurves(node.Children[i]);
            }
            CurveTreeLeaf leaf = node as CurveTreeLeaf;
            if (leaf != null)
                Control.Curves.Remove(leaf.Curve);
        }

        /// <summary>
        /// Adds the curve to the TreeView and also the CurveEditorControl.
        /// </summary>
        internal void AddCurve(string name, Curve curve)
        {
            // Check if the curve is already added.
            var container = FindContainerLeaf(Root, curve);
            if (container != null)
            {
                container.IsSelected = true;
                container.IsVisible = true;
                return;
            }

            // Add to tree
            String[] path = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            CurveTreeNode node = Root;
            for (int i = 0; i < path.Length - 1; i++)
            {
                CurveTreeNode child;
                if ((child = GetChildNode(node, path[i])) == null)
                {
                    child = new CurveTreeNode(path[i], node);
                    node.Children.Add(child);
                }
                node = child;
            }

            // Add to control
            String curveName = path[path.Length - 1];
            CurveWrapper wrapper = new CurveWrapper(curve, curveName, Control, GetColor(curveName));
            Control.Curves.Add(wrapper);
            // Add to tree
            node.Children.Add(new CurveTreeLeaf(node, wrapper));

            //for (int i = 0; i < 20; i++)
            //{
            //    wrapper.AddKey(new CurveKey((float)random.NextDouble(), (float)random.NextDouble()));
            //}
        }

        private CurveTreeNode GetChildNode(CurveTreeNode node, string name)
        {
            foreach (var item in node.Children)
            {
                if (item.Name == name)
                    return item;
            }
            return null;
        }

        private System.Windows.Media.Color GetColor(string name)
        {
            name = name.ToUpper();
            if (name == "X" || name == "R" || name == "RED")
                return System.Windows.Media.Color.FromRgb(200, 80, 80);
            if (name == "Y" || name == "G" || name == "GREEN")
                return System.Windows.Media.Color.FromRgb(80, 200, 80);
            if (name == "Z" || name == "B" || name == "BLUE")
                return System.Windows.Media.Color.FromRgb(80, 80, 200);
            if (name == "W" || name == "A" || name == "ALPHA")
                return System.Windows.Media.Color.FromRgb(220, 220, 220);
            else
                return System.Windows.Media.Color.FromRgb((byte)(random.Next(155) + 100), (byte)(random.Next(155) + 100), (byte)(random.Next(155) + 100));
        }


    }
}
