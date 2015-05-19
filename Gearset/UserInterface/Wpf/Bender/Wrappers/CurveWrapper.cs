using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Media;
using Gearset.UserInterface.Wpf.Bender;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Wraps a Curve, giving it an Id and a Name.
    /// </summary>
    public class CurveWrapper
    {
        public static long latestId = 0;

        /// <summary>
        /// Gets or sets whether this curve is visible on the control for editing.
        /// </summary>
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;

                // If they're making this curve invisible, make sure we deselect 
                // all its keys.
                if (!visible)
                {
                    Control.DeselectAllKeysOwnedBy(this);
                }
                Control.InvalidateVisual();
            }
        }
        private bool visible;

        /// <summary>
        /// Wraps the Curve's Loop property making sure the control gets invalidated
        /// when its set.
        /// </summary>
        public CurveLoopType PreLoop
        {
            get { return Curve.PreLoop; }
            set { Curve.PreLoop = value; Control.InvalidateVisual(); }
        }

        /// <summary>
        /// Wraps the Curve's Loop property making sure the control gets invalidated
        /// when its set.
        /// </summary>
        public CurveLoopType PostLoop
        {
            get { return Curve.PostLoop; }
            set { Curve.PostLoop = value; Control.InvalidateVisual(); }
        }
        

        /// <summary>
        /// Wrapped curve instance. Do not edit it directly, use the CurveWrapper
        /// methods instead.
        /// </summary>
        public Curve Curve { get; private set; }

        /// <summary>
        /// Id of the curve, this is used by undo commands to reference
        /// a modification to the wrapped key.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Pen used to draw this curve.
        /// </summary>
        public Pen Pen { get; set; }

        /// <summary>
        /// Pen used to draw this curve.
        /// </summary>
        public Pen DashedPen { get; set; }

        /// <summary>
        /// Friendly name of the Wrapped Curve.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The color used to build the pens.
        /// </summary>
        public Brush ColorBrush { get; private set; }

        /// <summary>
        /// Reference to the control that contains this wrapper.
        /// </summary>
        public CurveEditorControl2 Control { get; private set; }

        public CurveWrapper(Curve curve, String name, CurveEditorControl2 control, System.Windows.Media.Color color)
        {
            this.visible = true;
            this.Curve = curve;
            this.Id = latestId++;
            this.Name = name;
            this.Control = control;

            ColorBrush = new SolidColorBrush(color);
            ColorBrush.Freeze();

            Pen = new Pen(ColorBrush, 1);
            Pen.EndLineCap = PenLineCap.Round;
            Pen.StartLineCap = PenLineCap.Round;
            Pen.Freeze();

            DashedPen = new Pen(ColorBrush, 0.5);
            DashedPen.Freeze();
        }

        /// <summary>
        /// This is the method to use when adding a key in the editor. It will create
        /// the corresponding KeyWrapper and add it to the control.
        /// </summary>
        public KeyWrapper AddKey(CurveKey key, long id = -1)
        {
            Curve.Keys.Add(key);
            KeyWrapper newKey = new KeyWrapper(key, this, KeyTangentMode.Smooth, id);
            Control.Keys.Add(newKey);
            ComputeTangents();

            return newKey;
        }

        /// <summary>
        /// This method is only to be used when undoing a RemoveKeys command.
        /// </summary>
        internal void RestoreKey(KeyWrapper key)
        {
            Curve.Keys.Add(key.Key);
            Control.Keys.Add(key);
            ComputeTangents();
        }

        /// <summary>
        /// This is the method to use when removing a key in the editor. It will remove
        /// the corresponding KeyWrapper from the control.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveKey(long id)
        {
            KeyWrapper wrapper = Control.Keys[id];
            Control.Keys.Remove(wrapper);
            Curve.Keys.Remove(wrapper.Key);
            ComputeTangents();
        }

        /// <summary>
        /// Returns the KeyWrapper of the key at the provided position. This is a
        /// relatively expensive method because it iterates over the dictionary's values.
        /// </summary>
        public KeyWrapper GetKeyAt(int index)
        {
            return Control.GetWrapper(Curve.Keys[index]);
        }

        /// <summary>
        /// Wraps the curve's Evaluate method.
        /// </summary>
        public float Evaluate(float position)
        {
            if (Curve.Keys.Count == 0) return 0f;
            if (Curve.Keys.Count == 1) return Curve.Keys[1].Value;

            return Curve.Evaluate(position);
        }

        /// <summary>
        /// Computes the tangents of all automatic keys.
        /// </summary>
        public void ComputeTangents()
        {
            for (int i = 0; i < Curve.Keys.Count; i++)
            {
                KeyWrapper wrapper = Control.GetWrapper(Curve.Keys[i]);
                if (wrapper != null)
                    wrapper.ComputeTangentIfAuto();
            }
        } 
    }
}
