using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset.Components.CurveEditorControl;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    public class CurveTreeLeaf : CurveTreeNode
    {
        public CurveWrapper Curve { get; private set; }

        public System.Windows.Media.Brush ColorBrush { get { return Curve.ColorBrush; } }

        public override string Name
        {
            get
            {
                return Curve.Name;
            }
            set
            {
                Curve.Name = value;
            }
        }

        public override bool AreParentsVisible
        {
            get
            {
                return base.AreParentsVisible;
            }
            set
            {
                bool prevValue = AreParentsVisible;
                areParentsVisible = value;
                OnParentVisibilityChanged(prevValue);
            }
        }

        public override bool IsVisible
        {
            get
            {
                return base.IsVisible;
            }
            set
            {
                bool becomingVisible = value && !IsVisible && AreParentsVisible;
                bool becomingInvisible = !value && IsVisible && AreParentsVisible;
                if (becomingVisible)
                {
                    IsActuallyVisible = true;
                }
                else if (becomingInvisible)
                {
                    IsActuallyVisible = false;
                }
                isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        public bool IsActuallyVisible
        {
            get { return Curve.Visible; }
            private set
            {
                Curve.Visible = value;
            }
        }

        public CurveLoopType PreLoop
        {
            get { return Curve != null ? Curve.PreLoop : default(CurveLoopType); }
            set { Curve.PreLoop = value; OnPropertyChanged("PreLoop"); }
        }

        public CurveLoopType PostLoop
        {
            get { return Curve != null ? Curve.PostLoop : default(CurveLoopType); }
            set { Curve.PostLoop = value; OnPropertyChanged("PostLoop"); }
        }

        public CurveTreeLeaf(CurveTreeNode parent, CurveWrapper curve)
            : base(parent)
        {
            Curve = curve; 
        }

        private void OnParentVisibilityChanged(bool previousValue)
        {
            // We are visible but some ancestor is not. Hide it.
            if (previousValue == false && AreParentsVisible && IsVisible)
            {
                IsActuallyVisible = true;
            }

            if (previousValue == true && !AreParentsVisible && IsVisible)
            {
                IsActuallyVisible = false;
            }

            OnPropertyChanged("AreParentsVisible");
        }
    }
}
