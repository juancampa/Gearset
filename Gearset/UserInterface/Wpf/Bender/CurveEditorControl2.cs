using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Gearset.Components;
using Gearset.Components.CurveEditorControl;
using Microsoft.Xna.Framework;

namespace Gearset.UserInterface.Wpf.Bender
{
    public class CurveEditorControl2 : FrameworkElement
    {
        public static readonly DependencyProperty MinXProperty =
            DependencyProperty.Register("MinX", typeof(float), typeof(CurveEditorControl2), new FrameworkPropertyMetadata(-0.5f, FrameworkPropertyMetadataOptions.None, LimitsChangedCalllBack, CoerceMinX));
        public float MinX { get { return (float)GetValue(MinXProperty); } set { SetValue(MinXProperty, value); } }

        public static readonly DependencyProperty MaxXProperty =
            DependencyProperty.Register("MaxX", typeof(float), typeof(CurveEditorControl2), new FrameworkPropertyMetadata(2f, FrameworkPropertyMetadataOptions.None, LimitsChangedCalllBack, CoerceMaxX));
        public float MaxX { get { return (float)GetValue(MaxXProperty); } set { SetValue(MaxXProperty, value); } }

        public static readonly DependencyProperty MinYProperty =
            DependencyProperty.Register("MinY", typeof(float), typeof(CurveEditorControl2), new FrameworkPropertyMetadata(-0.5f, FrameworkPropertyMetadataOptions.None, LimitsChangedCalllBack, CoerceMinY));
        public float MinY { get { return (float)GetValue(MinYProperty); } set { SetValue(MinYProperty, value); } }

        public static readonly DependencyProperty MaxYProperty =
            DependencyProperty.Register("MaxY", typeof(float), typeof(CurveEditorControl2), new FrameworkPropertyMetadata(2f, FrameworkPropertyMetadataOptions.None, LimitsChangedCalllBack, CoerceMaxY));
        public float MaxY { get { return (float)GetValue(MaxYProperty); } set { SetValue(MaxYProperty, value); } }

        public static readonly DependencyProperty ScaleSpeedProperty =
            DependencyProperty.Register("ScaleSpeed", typeof(float), typeof(CurveEditorControl2), new FrameworkPropertyMetadata(1f, FrameworkPropertyMetadataOptions.None));
        public float ScaleSpeed { get { return (float)GetValue(ScaleSpeedProperty); } set { SetValue(ScaleSpeedProperty, value); } }

        public static void LimitsChangedCalllBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurveEditorControl2 control = d as CurveEditorControl2;
            if (control != null)
                control.InvalidateVisual();
        }

        public static object CoerceMinX(DependencyObject d, object baseValue) { return Math.Min(((CurveEditorControl2)d).MaxX, (float)baseValue); }
        public static object CoerceMaxX(DependencyObject d, object baseValue) { return Math.Max(((CurveEditorControl2)d).MinX, (float)baseValue); }
        public static object CoerceMinY(DependencyObject d, object baseValue) { return Math.Min(((CurveEditorControl2)d).MaxY, (float)baseValue); }
        public static object CoerceMaxY(DependencyObject d, object baseValue) { return Math.Max(((CurveEditorControl2)d).MinY, (float)baseValue); }

        public CurveWrapperCollection Curves { get; private set; }

        /// <summary>
        /// Dictionary of all keys of all curves. The key of the dictionary
        /// represents the Id of the key.
        /// </summary>
        public KeyWrapperCollection Keys { get; private set; }

        /// <summary>
        /// List of selected curve keys.
        /// </summary>
        public KeySelection Selection { get; private set; }

        public CurveEditorControlsViewModel ControlsViewModel { get; set; }

        /// <summary>
        /// Map keys to wrappers for O(1) retrieval. This map must be updated
        /// by wrappers every time their wrapped key changes.
        /// </summary>
        private Dictionary<CurveKey, KeyWrapper> keyToWrapperMap;

        // TODO: Make this dep props.
        private Pen manualTangentPen;
        private Pen autoTangentPen;
        private Pen selectedKeyPen;
        private Pen selectionBorderPen;
        private Pen gridLinePen;
        private Pen gridBoldLinePen;
        private Pen gridZeroLinePen;
        private Pen scaleHandlePen;
        private Brush autoTangentBrush;
        private Brush backgroundBrush;
        private Brush selectedKeyBrush;
        private Brush scaleHandleBrush;

        private struct SelectionBoundingBox
        {
            public System.Windows.Point p1;
            public System.Windows.Point p2;

            public bool IsSelecting;

            public void Expand(double extents)
            {
                p1.X -= extents;
                p1.Y -= extents;
                p2.X += extents;
                p2.Y += extents;
            }

            /// <summary>
            /// Make sure that p1 is min and p2 is max.
            /// </summary>
            public void EnsureMaxMin()
            {
                if (p1.X > p2.X)
                {
                    double aux = p1.X;
                    p1.X = p2.X;
                    p2.X = aux;
                }
                if (p1.Y > p2.Y)
                {
                    double aux = p1.Y;
                    p1.Y = p2.Y;
                    p2.Y = aux;
                }

            }
        }
        private SelectionBoundingBox SelectionBox;

        /// <summary>
        /// Determines whether we're currently drawing a selection box.
        /// </summary>
        private bool SelectingWithBox { get { return dragging && SelectionBox.IsSelecting; } }

        internal struct ScaleBox
        {
            public System.Windows.Point Min;
            public System.Windows.Point Max;
            public ScaleBoxHandle Handle;
        }
        private ScaleBox scaleBox;
        

        internal struct TangentSelectionStruct
        {
            public TangentSelectionMode Mode;
            public KeyWrapper Key;
            public bool IsSelected;
        }
        internal TangentSelectionStruct TangentSelection;

        // Mouse handling.
        private bool mouseLeftDown;
        private bool mouseRightDown;

        private System.Windows.Point mouseDownPos;
        private float downMinX;
        private float downMaxX;
        private float downMinY;
        private float downMaxY;
        private bool dragging;

        // Undo/Redo
        private UndoEngine UndoEngine;

        private const float KeySize = 2;
        private const float SelectedKeySize = 3;
        private const double ClickSelectionExtents = 2;
        public const double TangentHandleLength = 35;

        private MoveKeysCommand currentMover;
        private ScaleKeysCommand currentScaler;
        public double halfDpiX;
        public double halfDpiY;
        private bool guidelinesFixed;
        private ChangeTangentCommand currentTangentChanger;

        private ToolMode toolMode;
        /// <summary>
        /// Gets or set the current tool used by the control.
        /// </summary>
        public ToolMode ToolMode { 
            get 
            {
                return toolMode; 
            } 
            set 
            { 
                toolMode = value;

                if (toolMode == ToolMode.ScaleKeys)
                {
                    ComputeScaleBox();
                }
                InvalidateVisual(true);
                if (ToolModeChanged != null) ToolModeChanged(this, EventArgs.Empty); 
            } 
        }

        public event EventHandler ToolModeChanged;
        public event EventHandler SelectionChanged;
        public event EventHandler SelectedKeysMoved;
        private bool quasiModalTool;

        /// <summary>
        /// Will only have a value after a mouse down when a key was added
        /// and will be reset to null on mouse up.
        /// </summary>
        private long? keyAddedOnMouseDown;

        /// <summary>
        /// Determines if we should move the keys when a mouse move event occurs.
        /// This is MouseDown -> MouseMove communication.
        /// </summary>
        private bool moveKeysWithMouse;

        public CurveEditorControl2()
        {
            Focusable = true;
            FocusVisualStyle = null;
            Curves = new CurveWrapperCollection();
            Keys = new KeyWrapperCollection();
            Selection = new KeySelection();
            UndoEngine = new UndoEngine();
            keyToWrapperMap = new Dictionary<CurveKey, KeyWrapper>(new CurveKeyInstanceComparer());

            var keyToWrapperMap2 = new Dictionary<CurveKey, int>(new CurveKeyInstanceComparer());

            CurveKey k1 = new CurveKey(0, 1);
            keyToWrapperMap2[k1] = 1;

            //Console.WriteLine(keyToWrapperMap2[k1]);
            this.ClipToBounds = true;
            this.ControlsViewModel = new CurveEditorControlsViewModel(this);
            //this.SnapsToDevicePixels = true;
            //this.UseLayoutRounding = true;
            //this.MinActualWidth = 100;
            //this.MinActualHeight = 100;

            MouseWheel += new MouseWheelEventHandler(CurveEditorControl_MouseWheel);
            MouseMove += new MouseEventHandler(CurveEditorControl_MouseMove);
            MouseDown += new MouseButtonEventHandler(CurveEditorControl_MouseDown);
            MouseUp += new MouseButtonEventHandler(CurveEditorControl_MouseUp);
            PreviewKeyDown += new KeyEventHandler(CurveEditorControl2_KeyDown);
            PreviewKeyUp += new KeyEventHandler(CurveEditorControl2_PreviewKeyUp);
            InitializePenAndBrushes();

            Curves.ItemAdded += new EventHandler<ItemAddedEventArgs<CurveWrapper>>(Curves_ItemAdded);
            Curves.ItemRemoved += new EventHandler<ItemRemovedEventArgs<CurveWrapper>>(Curves_ItemRemoved);

            //GearsetResources.Console.Inspect("Dispatcher", Dispatcher);
        }

        private class CurveKeyInstanceComparer : IEqualityComparer<CurveKey>
        {
            public bool Equals(CurveKey x, CurveKey y)
            {
                return Object.ReferenceEquals(x, y);
            }

            public int GetHashCode(CurveKey obj)
            {
                return (int)(obj.Position * 1000);
            }
        }

        private bool invalidateRequested = false;
        public void InvalidateVisual(bool immediate = false)
        {
            //base.InvalidateVisual();
            //return;
            if (immediate)
                base.InvalidateVisual();
            else // Deffered?
                invalidateRequested = true;
        }

        public void UpdateRender()
        {
            
            if (invalidateRequested)
            {
                base.InvalidateVisual();
                invalidateRequested = false;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

            

        void Curves_ItemRemoved(object sender, ItemRemovedEventArgs<CurveWrapper> e)
        {
            // Remove all keys related to the removed curve.
            List<KeyWrapper> keysToRemove = new List<KeyWrapper>();
            foreach (var key in Keys)
            {
                if (key.Curve == e.RemovedItem)
                    keysToRemove.Add(key);
            }
            foreach (var key in keysToRemove)
            {
                Keys.Remove(key);
            }
            InvalidateVisual();
        }

        void Curves_ItemAdded(object sender, ItemAddedEventArgs<CurveWrapper> e)
        {
            // Add the preexisting keys to the control with custom tangent mode.
            foreach (CurveKey key in e.AddedItem.Curve.Keys)
            {
                Keys.Add(new KeyWrapper(key, e.AddedItem, KeyTangentMode.Custom));
            }
            InvalidateVisual();
        }

        void CurveEditorControl2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
                return;
            if ((e.Key == Key.Delete || e.Key == Key.Back) && Selection.Count > 0)
            {
                DeleteKeysCommand keyDeleter = new DeleteKeysCommand(this);
                UndoEngine.Execute(keyDeleter);
                InvalidateVisual();
            }
            else if (e.Key == Key.A)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    // Select all!
                    SelectKeysCommand selector = new SelectKeysCommand(this, (from k in Keys select k.Id).ToList());
                    UndoEngine.Execute(selector);
                    OnSelectionChanged();
                }
                else
                {
                    // Quasimodal Add Keys
                    this.quasiModalTool = true;
                    ToolMode = ToolMode.AddKeys;
                }
            }
            else if (e.Key == Key.Z)
            {
                if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    this.quasiModalTool = true;
                    ToolMode = ToolMode.ZoomBox;
                }
            }
            else if (e.Key == Key.S)
            {
                if (!(Keyboard.IsKeyDown(Key. LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    this.quasiModalTool = true;
                    ToolMode = ToolMode.ScaleKeys;
                }
            }
            else if (e.Key == Key.D && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                SelectKeysCommand selector = new SelectKeysCommand(this, null);
                UndoEngine.Execute(selector);
                OnSelectionChanged();
            }
        }

        /// <summary>
        /// This method is called by KeyWrappers so the map gets updated.
        /// </summary>
        internal void UpdateKeyMap(KeyWrapper wrapper, CurveKey key)
        {
            keyToWrapperMap[key] = wrapper;
        }

        internal void RemoveKeyFromMap(CurveKey key)
        {
            keyToWrapperMap.Remove(key);
        }

        /// <summary>
        /// Gets the KeyWrapper of a given CurveKey.
        /// </summary>
        public KeyWrapper GetWrapper(CurveKey key)
        {
            KeyWrapper result = null;
            keyToWrapperMap.TryGetValue(key, out result);
            return result;
        }

        /// <summary>
        /// Computes the boundaries of the scale box based on the current selection.
        /// </summary>
        private void ComputeScaleBox()
        {
            scaleBox.Min.X = scaleBox.Min.Y = double.MaxValue;
            scaleBox.Max.X = scaleBox.Max.Y = double.MinValue;
            foreach (var key in Selection)
            {
                scaleBox.Min.X = Math.Min(key.Key.Position, scaleBox.Min.X);
                scaleBox.Min.Y = Math.Min(key.Key.Value, scaleBox.Min.Y);
                scaleBox.Max.X = Math.Max(key.Key.Position, scaleBox.Max.X);
                scaleBox.Max.Y = Math.Max(key.Key.Value, scaleBox.Max.Y);
            }

            // Put the anchor in the center by default.
            //scaleBox.Anchor.X = (scaleBox.Min.X + scaleBox.Max.X) * .5;
            //scaleBox.Anchor.Y = (scaleBox.Min.Y + scaleBox.Max.Y) * .5;
        }

        void CurveEditorControl2_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.A || e.Key == Key.Z || e.Key == Key.S) && quasiModalTool)
            {
                quasiModalTool = false;
                ToolMode = ToolMode.SelectMove;

                // A and Z are already covered but S needs the redraw.
                InvalidateVisual(true);
            }
        }

        /// <summary>
        /// Initializes all general pens and brushes.
        /// </summary>
        private void InitializePenAndBrushes()
        {
            manualTangentPen = new Pen(Brushes.Black, 1);
            manualTangentPen.Freeze();

            Brush gridLineBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(38, 38, 38));
            gridLineBrush.Freeze();
            gridLinePen = new Pen(gridLineBrush, 1);
            gridLinePen.Freeze();

            Brush gridBoldLineBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(20, 20, 20));
            gridBoldLineBrush.Freeze();
            gridBoldLinePen = new Pen(gridBoldLineBrush, 1);
            gridZeroLinePen = new Pen(gridBoldLineBrush, 2);
            gridBoldLinePen.Freeze();
            gridZeroLinePen.Freeze();

            autoTangentBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(18, 18, 18));
            autoTangentBrush.Freeze();
            autoTangentPen = new Pen(autoTangentBrush, 1);
            autoTangentPen.DashStyle = new DashStyle(new double[] { 4, 4 }, 0);// DashStyles.Dash;
            autoTangentPen.Freeze();
            selectionBorderPen = new Pen(Brushes.LightGray, 1);
            selectionBorderPen.DashStyle = DashStyles.Dash;
            selectionBorderPen.Freeze();
            selectedKeyPen = new Pen(Brushes.DarkGray, 1);
            selectedKeyPen.Freeze();
            selectedKeyBrush = Brushes.Black;
            backgroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(48, 48, 48));
            backgroundBrush.Freeze();

            scaleHandleBrush = Brushes.White;
            scaleHandlePen = new Pen(Brushes.White, 1);
            scaleHandlePen.Freeze();
        }

        public void Undo()
        {
            UndoEngine.Undo();
            InvalidateVisual();
        }

        public void Redo()
        {
            UndoEngine.Redo();
            InvalidateVisual();
        }

        private void OnSelectionChanged()
        {
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
            InvalidateVisual();
        }

        private void OnSelectedKeysMoved()
        {
            if (SelectedKeysMoved != null)
                SelectedKeysMoved(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the provided KeyTangentMode to the current selection
        /// </summary>
        public void SetKeysContinuity(CurveContinuity continuity)
        {
            if (Selection.Count > 0)
            {
                ChangeContinuityCommand command = new ChangeContinuityCommand(this, continuity);
                UndoEngine.Execute(command);
            }
        }

        /// <summary>
        /// Sets the provided KeyTangentMode to the current selection
        /// </summary>
        public void SetTangentModes(KeyTangentMode? inTangent, KeyTangentMode? outTangent)
        {
            if (Selection.Count > 0)
            {
                ChangeTangentModeCommand command = new ChangeTangentModeCommand(this, inTangent, outTangent);
                UndoEngine.Execute(command);
            }
        }

        int i = 0;
        //DispatcherTimer timer;
        //public new void InvalidateVisual()
        //{
        //    if (timer == null)
        //        timer = new DispatcherTimer();
        //    if (timer.IsEnabled)
        //        return;
        //    else
        //    {
        //        timer.Interval = TimeSpan.FromSeconds(0.033);
        //        timer.Tick += new EventHandler(timer_Tick);
        //        Dispatcher.BeginInvoke(new Action(timer.Start));
        //        //base.InvalidateVisual();
        //    }
        //    //Console.WriteLine("Invalidating");
        //    //if (((i++) % 1) == 0)
        //        //Dispatcher.BeginInvoke(new Action(base.InvalidateVisual), System.Windows.Threading.DispatcherPriority.SystemIdle);
        //        //base.InvalidateVisual();
        //}

        //void timer_Tick(object sender, EventArgs e)
        //{
        //    //Console.WriteLine("Ticking");
        //    base.InvalidateVisual();
        //    //timer.IsEnabled = false;
        //}

        /// <summary>
        /// Deselects all keys (if any) owned by the provided curve.
        /// </summary>
        internal void DeselectAllKeysOwnedBy(CurveWrapper curveWrapper)
        {
            for (int i = Selection.Count - 1; i >= 0; i--)
            {
                if (Object.ReferenceEquals(Selection[i].Curve, curveWrapper))
                {
                    Selection.RemoveAt(i);
                }
            }
            OnSelectionChanged();
        }

        #region Coords transforms
        public System.Windows.Point CurveCoordsToScreen(ref System.Windows.Point point)
        {
            return new System.Windows.Point(((point.X - MinX) / (MaxX - MinX)) * ActualWidth, -((point.Y - MinY) / (MaxY - MinY)) * ActualHeight + ActualHeight);
        }

        public System.Windows.Point CurveCoordsToScreenNormal(ref System.Windows.Point point)
        {
            return new System.Windows.Point(((point.X) / (MaxX - MinX)) * ActualWidth, -((point.Y) / (MaxY - MinY)) * ActualHeight);
        }

        public System.Windows.Point ScreenCoordsToCurve(ref System.Windows.Point point)
        {
            return new System.Windows.Point(((point.X) / (ActualWidth)) * (MaxX - MinX) + MinX, ((-point.Y + ActualHeight) / (ActualHeight)) * (MaxY - MinY) + MinY);
        }

        public System.Windows.Point ScreenCoordsToCurveNormal(ref System.Windows.Point point)
        {
            return new System.Windows.Point(((point.X) / (ActualWidth)) * (MaxX - MinX), ((-point.Y) / (ActualHeight)) * (MaxY - MinY));
        } 
        #endregion

        #region Mouse handling
        void CurveEditorControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownPos = e.GetPosition(this);
            this.Focus();
            
            if (e.ChangedButton == MouseButton.Left && ToolMode == ToolMode.SelectMove)
            {
                mouseLeftDown = true;
                SelectionBox.p1 = mouseDownPos;
                SelectionBox.p2 = mouseDownPos;

                // Make it easier to click-select.
                SelectionBox.Expand(ClickSelectionExtents);

                // Check if we're doing a click-selection.
                IList<long> clickSelection = GetKeysInSelectionBox(true);
                if (clickSelection.Count != 0)
                {
                    // We clicked on something, if it isn't already selected, select it.
                    // otherwise, just keep the current selection.
                    long keyOnMouse = clickSelection[0];

                    // Wait, did we hit a tangent?
                    if (keyOnMouse < 0)
                    {
                        // Do some consistency checks.
                        System.Diagnostics.Debug.Assert(clickSelection.Count > 1);
                        System.Diagnostics.Debug.Assert(Keys[(int)clickSelection[1]].IsSelected == true);

                        TangentSelection.IsSelected = true;
                        TangentSelection.Mode = (TangentSelectionMode)clickSelection[0];
                        TangentSelection.Key = Keys[clickSelection[1]];
                    }
                    else if (!Keys[keyOnMouse].IsSelected)
                    {
                        // We're selecting a new key.
                        clickSelection.Clear();
                        clickSelection.Add(keyOnMouse);

                        // Are we adding a new key to the selection?
                        bool isShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                        if (isShiftDown)
                            clickSelection = clickSelection.Union((from k in Selection select k.Id)).ToList();

                        SelectKeysCommand command = new SelectKeysCommand(this, clickSelection);
                        UndoEngine.Execute(command);
                        OnSelectionChanged();

                        moveKeysWithMouse = true;
                    }
                    else
                    {
                        moveKeysWithMouse = true;
                    }
                }
                else
                {
                    // We didn't clicked on anything, use the selection box.
                    // Setting this variable will set SelectingWithBox to true if
                    // we start dragging.
                    SelectionBox.IsSelecting = true;
                }

                // Remove the box expansion made above.
                SelectionBox.p1 = mouseDownPos;
            }
            else if (e.ChangedButton == MouseButton.Left && ToolMode == ToolMode.AddKeys)
            {
                mouseLeftDown = true;
                moveKeysWithMouse = true;

                // Test add keys.
                var ids = GetCurvesAtPosition(mouseDownPos.X, mouseDownPos.Y);
                if (ids.Count > 0)
                {
                    System.Windows.Point keyPos = ScreenCoordsToCurve(ref mouseDownPos);
                    AddKeyCommand keyAdder = new AddKeyCommand(this, ids[0], (float)keyPos.X, (float)keyPos.Y);
                    UndoEngine.Execute(keyAdder);
                    InvalidateVisual();
                    keyAddedOnMouseDown = keyAdder.KeyId;
                }
            }
            else if (e.ChangedButton == MouseButton.Left && ToolMode == ToolMode.ZoomBox)
            {
                mouseLeftDown = true;
                SelectionBox.IsSelecting = true;
                SelectionBox.p1 = mouseDownPos;
                SelectionBox.p2 = mouseDownPos;
            }
            else if (e.ChangedButton == MouseButton.Left && ToolMode == ToolMode.ScaleKeys)
            {
                // Only set mouseLeftDown if we hit a handle.
                ScaleBoxHandle? handle = GetSelectedScaleBoxHandle(mouseDownPos);
                if (handle.HasValue)
                {
                    mouseLeftDown = true;
                    scaleBox.Handle = handle.Value;
                }
            }
            else
            {
                mouseRightDown = true;

                // Save these values so we can pan around.
                downMinX = MinX;
                downMaxX = MaxX;
                downMinY = MinY;
                downMaxY = MaxY;
            }
        }

        void CurveEditorControl_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(this);

            // Detect a drag.
            if ((mouseLeftDown || mouseRightDown) && !dragging)
            {
                if (Math.Abs(mousePos.X - mouseDownPos.X) > 1 ||// SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(mousePos.Y - mouseDownPos.Y) > 1)//SystemParameters.MinimumVerticalDragDistance)
                {
                    dragging = true;
                    Mouse.Capture(this);
                }
            }

            if (mouseLeftDown)
            {
                // Resizing with the selection box or moving keys.
                if (SelectingWithBox)
                {
                    SelectionBox.p2 = mousePos;
                    InvalidateVisual();
                }
                else if (dragging)
                {
                    if (TangentSelection.IsSelected && ToolMode == ToolMode.SelectMove)
                    {
                        // We're changing tangents, use a ChangeTangent command to do this, 
                        // when the mouse is released, the command goes to the history
                        // so it can be undone/redone.
                        if (currentTangentChanger == null)
                            currentTangentChanger = new ChangeTangentCommand(this, TangentSelection.Key.Id, TangentSelection.Mode);

                        var keyPos = TangentSelection.Key.GetPosition();
                        //keyPos = CurveCoordsToScreen(ref keyPos);

                        var mouseOnCurve = ScreenCoordsToCurve(ref mousePos);
                        double diffX = mouseOnCurve.X - keyPos.X;
                        double diffY = mouseOnCurve.Y - keyPos.Y;

                        if (diffX != 0)
                        {
                            float prevDist, nextDist;
                            TangentSelection.Key.GetPrevAndNextDistance(out prevDist, out nextDist);
                            float tangent = (float)(diffY / diffX);
                            if (TangentSelection.Mode == TangentSelectionMode.In)
                                tangent *= prevDist;
                            else
                                tangent *= nextDist;
                            currentTangentChanger.UpdateOffset(tangent);
                        }
                    }
                    else if (moveKeysWithMouse)
                    {
                        // If we just added a new key, and we're moving the mouse, we want
                        // to move the just added key so select it.
                        if (keyAddedOnMouseDown.HasValue)
                        {
                            SelectKeysCommand selector = new SelectKeysCommand(this, new [] { keyAddedOnMouseDown.Value });
                            UndoEngine.Execute(selector);
                            OnSelectionChanged();
                            keyAddedOnMouseDown = null;
                        }

                        if (Selection.Count > 0)
                        {
                            // We're moving keys, use a MoveKeys command to do this, 
                            // when the mouse is released, the command goes to the history
                            // so it can be undone/redone.
                            if (currentMover == null)
                                currentMover = new MoveKeysCommand(this, (float)(0), (float)(0));

                            // Update the offset.
                            System.Windows.Point offset = new System.Windows.Point((mousePos.X - mouseDownPos.X), (mousePos.Y - mouseDownPos.Y));
                            offset = ScreenCoordsToCurveNormal(ref offset);
                            currentMover.UpdateOffsets((float)offset.X, (float)offset.Y);
                            OnSelectedKeysMoved();
                        }
                    }
                    else if (ToolMode == ToolMode.ScaleKeys)
                    {
                        if (currentScaler == null)
                            currentScaler = new ScaleKeysCommand(this, scaleBox.Min, scaleBox.Max, scaleBox.Handle);

                        ComputeScaleBox();

                        System.Windows.Point offset = (System.Windows.Point)(mousePos - mouseDownPos);
                        currentScaler.UpdateOffsets(ScreenCoordsToCurveNormal(ref offset));
                        OnSelectedKeysMoved();
                    }
                    InvalidateVisual();
                }
            }
            else if (mouseRightDown)
            {
                // We're panning, calculate how much we're moving in screen coords and
                // translate go curve coords to offset max/min
                System.Windows.Point movement = new System.Windows.Point(mousePos.X - mouseDownPos.X, mousePos.Y - mouseDownPos.Y);
                movement = ScreenCoordsToCurveNormal(ref movement);

                MinX = downMinX - (float)movement.X;
                MinY = downMinY - (float)movement.Y;
                MaxX = downMaxX - (float)movement.X;
                MaxY = downMaxY - (float)movement.Y;
            }
            else
            {
                switch (ToolMode)
                {
                    case ToolMode.SelectMove:
                        SelectionBox.p1 = mousePos;
                        SelectionBox.p2 = mousePos;
                        if (GetKeysInSelectionBox(true).Count > 0)
                            Cursor = Cursors.Cross;
                        else
                            Cursor = Cursors.Arrow;
                        break;
                    case ToolMode.AddKeys:
                        break;
                    case ToolMode.ZoomBox:
                        break;
                    case ToolMode.ScaleKeys:
                        ScaleBoxHandle? handle = GetSelectedScaleBoxHandle(mousePos);
                        if (handle.HasValue)
                        {
                            switch (handle.Value)
                            {
                                case ScaleBoxHandle.TopLeft: Cursor = Cursors.SizeNWSE; break;
                                case ScaleBoxHandle.TopRight: Cursor = Cursors.SizeNESW; break;
                                case ScaleBoxHandle.BottomRight: Cursor = Cursors.SizeNWSE; break;
                                case ScaleBoxHandle.BottomLeft: Cursor = Cursors.SizeNESW; break;
                            }
                        }
                        else
                        {
                            Cursor = Cursors.Arrow;
                        }
                        break;
                }
                
            }
        }

        void CurveEditorControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Deselect the curve if we were not dragging.
                if (!dragging && mouseLeftDown)
                {
                    IList<long> clickSelection = GetKeysInSelectionBox(true);

                    // Make it easier to click-select.
                    SelectionBox.Expand(ClickSelectionExtents);

                    if (clickSelection.Count != 0)
                    {
                        // TODO: cycle between keys beneath the mouse, if there are more than one.
                    }
                    else
                    {
                        // Deselect whatever was selected.
                        if (Selection.Count > 0)
                        {
                            SelectKeysCommand command = new SelectKeysCommand(this, null);
                            UndoEngine.Execute(command);
                            OnSelectionChanged();
                        }
                    }
                }

                // Check if we just ended drawing a selection box.
                if (SelectingWithBox)
                {
                    if (ToolMode == ToolMode.SelectMove)
                    {
                        IList<long> newSelection = GetKeysInSelectionBox(false);
                        
                        bool isAltDown = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
                        bool isShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                        // Adding to selection?
                        if (isShiftDown && !isAltDown)
                            newSelection = (from k in Selection select k.Id).Union(newSelection).ToList();

                        // Removing from selection?
                        if (isAltDown && !isShiftDown)
                            newSelection = (from k in Selection select k.Id).Except(newSelection).ToList();

                        // Intersect with selection
                        if (isAltDown && isShiftDown)
                            newSelection = (from k in Selection select k.Id).Intersect(newSelection).ToList();

                        SelectKeysCommand command = new SelectKeysCommand(this, newSelection);
                        UndoEngine.Execute(command);
                        OnSelectionChanged();
                    }
                    else if (ToolMode == ToolMode.ZoomBox)
                    {
                        SelectionBox.EnsureMaxMin();
                        var p1 = SelectionBox.p1;
                        var p2 = SelectionBox.p2;
                        p1 = ScreenCoordsToCurve(ref p1);
                        p2 = ScreenCoordsToCurve(ref p2);
                        MinX = (float)p1.X;
                        MinY = (float)p2.Y; // NOTE: Y is swapped because screen max is not curve coords max.
                        MaxX = (float)p2.X;
                        MaxY = (float)p1.Y;

                        // Finnally, we only do one box zoom so go back to SelectMove.
                        ToolMode = ToolMode.SelectMove;
                    }
                    else
                    {
                        // We might have entered a Add quasimodal in the middle of the box dragging
                        // Make sure we invalidate to remove the selection box
                        InvalidateVisual();
                    }
                }

                if (currentMover != null)
                {
                    UndoEngine.AddCommand(currentMover);
                    currentMover = null;
                }

                if (currentTangentChanger != null)
                {
                    UndoEngine.AddCommand(currentTangentChanger);
                    currentTangentChanger = null;
                }

                if (currentScaler != null)
                {
                    UndoEngine.AddCommand(currentScaler);
                    currentScaler = null;
                }
                // Release the mouse
                mouseLeftDown = false;
                dragging = false;
                TangentSelection.IsSelected = false;
                SelectionBox.IsSelecting = false;
            }
            else
            {
                mouseRightDown = false;
            }

            // Relase the mouse.
            Mouse.Capture(null);
            // This could have been reset to null if the user moved the mouse.
            keyAddedOnMouseDown = null;
            moveKeysWithMouse = false;
        }

        void CurveEditorControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Factor variables makes the zoom center on the mouse.
            // Scale variables makes the zoom increase accuracy while the range
            // is small and viceversa.
            float scaleSpeed = ScaleSpeed * 0.2f;
            float factorX = 0.5f;
            float factorY = 0.5f;
            if (e.Delta > 0)
            {
                factorX = (float)(e.GetPosition(this).X / ActualWidth);
                factorY = (float)(e.GetPosition(this).Y / ActualHeight);
            }
            float scaleX = (e.Delta / 240f) * (MaxX - MinX) * scaleSpeed;
            float scaleY = (e.Delta / 240f) * (MaxY - MinY) * scaleSpeed;

            MinX += scaleX * (factorX);
            MaxX -= scaleX * (1 - factorX);

            MinY += scaleY * (1 - factorY);
            MaxY -= scaleY * (factorY);
        } 
        #endregion

        #region Key/Curve intersection with mouse/box
        /// <summary>
        /// Returns a list of key ids that lie inside the selection box, it takes into
        /// account the size of the key (i.e. selected vs unselected). 
        /// 
        /// If the box contains
        /// any tangent handles of the selected keys they will be returned as negative values
        /// (see TangentSelection enum) and keys without a tangent won't be returned. Each
        /// tangent will be followed by it's corresponding key id.
        /// </summary>
        private IList<long> GetKeysInSelectionBox(bool checkTangentHandles)
        {
            // TODO: this list could be reused.
            List<long> result = new List<long>();
            bool tangentFound = false;
            SelectionBox.EnsureMaxMin();

            System.Windows.Point min = SelectionBox.p1;
            System.Windows.Point max = SelectionBox.p2;

            if (checkTangentHandles)
            {
                foreach (var key in Selection)
                {
                    // Convert the key to screen coords.
                    System.Windows.Point inHandle, outHandle;
                    key.GetTangentHandleScreenPositions(TangentHandleLength, out inHandle, out outHandle);

                    // Make sure we consider the size of the key.
                    float extents = (key.IsSelected ? SelectedKeySize : KeySize) * 1.5f;

                    // Check rect point containment of tangent and add the key afterwards.
                    if (inHandle.X >= min.X - extents && inHandle.X <= max.X + extents && inHandle.Y >= min.Y - extents && inHandle.Y <= max.Y + extents)
                    {
                        result.Add((long)TangentSelectionMode.In);
                        result.Add(key.Id);
                    }
                    else if (outHandle.X >= min.X - extents && outHandle.X <= max.X + extents && outHandle.Y >= min.Y - extents && outHandle.Y <= max.Y + extents)
                    {
                        result.Add((long)TangentSelectionMode.Out);
                        result.Add(key.Id);
                    }
                }

                if (result.Count > 0)
                    tangentFound = true;
            }

            // Selection precedence:
            // - Selected Keys
            // - Tangents (of selected keys)
            // - Unselected keys.
            // If we've already found a tangent, only check for selected keys.
            IEnumerable<KeyWrapper> keysToCheck;
            if (tangentFound)
                keysToCheck = Selection;
            else
                keysToCheck = Keys;

            foreach (var key in keysToCheck)
            {
                if (!key.Curve.Visible)
                    continue;

                // Convert the key to screen coords.
                System.Windows.Point screenKey = new System.Windows.Point(key.Key.Position, key.Key.Value);
                screenKey = CurveCoordsToScreen(ref screenKey);

                // Make sure we consider the size of the key.
                float extents = (key.IsSelected ? SelectedKeySize : KeySize) * 2;

                // Check rect point containment.
                if (screenKey.X >= min.X - extents &&
                    screenKey.X <= max.X + extents &&
                    screenKey.Y >= min.Y - extents &&
                    screenKey.Y <= max.Y + extents)
                {
                    if (tangentFound)
                        result.Insert(0, key.Id);
                    else
                        result.Add(key.Id);
                }
            }
            return result;
        }

        private IList<long> GetCurvesAtPosition(double x, double y)
        {
            // TODO: this list could be reused.
            List<long> result = new List<long>();

            System.Windows.Point mouseScreen = new System.Windows.Point(x, y);
            System.Windows.Point mouseCurve = ScreenCoordsToCurve(ref mouseScreen);

            // This is the distance of one pixel in curve coords.
            System.Windows.Point curvePixelDelta = new System.Windows.Point(1, -1);
            curvePixelDelta = ScreenCoordsToCurveNormal(ref curvePixelDelta);

            // Although we're using "radius", the actual picking shape is a square of
            // 2 * radius side.
            int radius = 4;

            foreach (CurveWrapper curve in Curves)
            {
                if (!curve.Visible)
                    continue;
                double centerPos = mouseCurve.X;

                for (int i = -radius; i < radius; i++)
                {
                    float evalPos = (float)(centerPos + i * curvePixelDelta.X);
                    double distance = Math.Abs(curve.Evaluate(evalPos) - mouseCurve.Y);
                    if (distance < radius * curvePixelDelta.Y)
                    {
                        result.Add(curve.Id);
                        break;
                    }
                }
            }
            return result;
        }

        private ScaleBoxHandle? GetSelectedScaleBoxHandle(System.Windows.Point mousePos)
        {
            // Convert the key to screen coords.
            System.Windows.Point p;
            // Make sure we consider the size of the key.
            float extents = 3;

            // Check rect point containment.
            p = scaleBox.Min;
            p = CurveCoordsToScreen(ref p);
            if (mousePos.X >= p.X - extents && mousePos.X <= p.X + extents && mousePos.Y >= p.Y - extents && mousePos.Y <= p.Y + extents)
                return ScaleBoxHandle.BottomLeft;

            p = new System.Windows.Point(scaleBox.Max.X, scaleBox.Min.Y);
            p = CurveCoordsToScreen(ref p);
            if (mousePos.X >= p.X - extents && mousePos.X <= p.X + extents && mousePos.Y >= p.Y - extents && mousePos.Y <= p.Y + extents)
                return ScaleBoxHandle.BottomRight;

            p = scaleBox.Max;
            p = CurveCoordsToScreen(ref p);
            if (mousePos.X >= p.X - extents && mousePos.X <= p.X + extents && mousePos.Y >= p.Y - extents && mousePos.Y <= p.Y + extents)
                return ScaleBoxHandle.TopRight;

            p = new System.Windows.Point(scaleBox.Min.X, scaleBox.Max.Y);
            p = CurveCoordsToScreen(ref p);
            if (mousePos.X >= p.X - extents && mousePos.X <= p.X + extents && mousePos.Y >= p.Y - extents && mousePos.Y <= p.Y + extents)
                return ScaleBoxHandle.TopLeft;

            // Otherwise
            return null;
        }
        #endregion

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            // Instead of colliding with actual geometry, just collide with the whole box.
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
            //return base.HitTestCore(hitTestParameters);
        }


        #region Rendering
        private double[] times = new double[100];
        private int timeIndex = 0;
        Stopwatch watch = new Stopwatch();
        protected override void OnRender(DrawingContext dc)
        {
            //Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
            //watch.Reset();
            //watch.Start();
            if (!guidelinesFixed)
            {
                System.Windows.Media.Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                halfDpiX = m.M11 * 0.5;
                halfDpiY = m.M22 * 0.5;
                guidelinesFixed = true;
            }

            // Create a guidelines set
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(halfDpiX);
            guidelines.GuidelinesY.Add(halfDpiY);
            dc.PushGuidelineSet(guidelines);

            //dc.DrawRectangle(backgroundBrush, null, new Rect(0, 0, ActualWidth, ActualHeight));

            DrawGrid(dc);

            foreach (var curve in Curves)
            {
                DrawCurve(dc, curve);
            }

            // Draw unselected keys.
            foreach (var key in Keys)
            {
                if (!key.IsSelected)
                    DrawKey(dc, key);
            }

            // Draw selected keys on top of unselected.
            foreach (var key in Keys)
            {
                if (key.IsSelected)
                    DrawKey(dc, key);
            }

            if (SelectingWithBox)
            {
                DrawSelectionBox(dc);
            }

            if (ToolMode == ToolMode.ScaleKeys)
            {
                DrawScaleBox(dc);
            }

            dc.Pop();
            //base.OnRender(dc);
            //watch.Stop();
            
        }

        

        private void DrawGrid(DrawingContext dc)
        {
            double range;
            double pos;
            double orderOfMag;
            double stepSize;
            Pen pen;
            System.Windows.Point p0;
            System.Windows.Point p1;

            // Thin horizontal lines
            range = MaxY - MinY;
            orderOfMag = Math.Pow(10, Math.Truncate(Math.Log10(range)));
            stepSize = 0.1 * orderOfMag;
            pos = (Math.Truncate(MinY / stepSize) - 1) * stepSize;
            pen = gridLinePen;
            DrawGridHorizonalLines(dc, pos, stepSize, pen);

            // Thin vertical lines.
            range = MaxX - MinX;
            orderOfMag = Math.Pow(10, Math.Truncate(Math.Log10(range)));
            stepSize = 0.1 * orderOfMag;
            pos = Math.Truncate(MinX / stepSize) * stepSize;
            DrawGridVerticalLines(dc, pos, stepSize, pen);

            // Bold horizontal lines
            range = MaxY - MinY;
            orderOfMag = Math.Pow(10, Math.Truncate(Math.Log10(range)));
            stepSize = 1 * orderOfMag;
            pos = (Math.Truncate(MinY / stepSize) - 1) * stepSize;
            pen = gridBoldLinePen;
            DrawGridHorizonalLines(dc, pos, stepSize, pen);

            // Bold vertical lines
            range = MaxX - MinX;
            orderOfMag = Math.Pow(10, Math.Truncate(Math.Log10(range)));
            stepSize = 1 * orderOfMag;
            pos = (Math.Truncate(MinX / stepSize) - 1) * stepSize;
            pen = gridBoldLinePen;
            DrawGridVerticalLines(dc, pos, stepSize, pen);

            // Zero lines.
            p0 = new System.Windows.Point(MinX, 0);
            p1 = new System.Windows.Point(MaxX, 0);
            p0 = CurveCoordsToScreen(ref p0);
            p1 = CurveCoordsToScreen(ref p1);
            p0.Y = p1.Y = Math.Truncate(p0.Y);
            dc.DrawLine(gridZeroLinePen, p0, p1);

            p0 = new System.Windows.Point(0, MinY);
            p1 = new System.Windows.Point(0, MaxY);
            p0 = CurveCoordsToScreen(ref p0);
            p1 = CurveCoordsToScreen(ref p1);
            p0.X = p1.X = Math.Truncate(p0.X);
            dc.DrawLine(gridZeroLinePen, p0, p1);

        }

        private void DrawGridHorizonalLines(DrawingContext dc, double initialPos, double stepSize, Pen pen)
        {
            while (initialPos < MaxY)
            {
                initialPos += stepSize;

                System.Windows.Point p0 = new System.Windows.Point(MinX, initialPos);
                System.Windows.Point p1 = new System.Windows.Point(MaxX, initialPos);
                p0 = CurveCoordsToScreen(ref p0);
                p1 = CurveCoordsToScreen(ref p1);
                p0.Y = p1.Y = Math.Truncate(p0.Y);
                dc.DrawLine(pen, p0, p1);
            }
        }

        private void DrawGridVerticalLines(DrawingContext dc, double initialPos, double stepSize, Pen pen)
        {
            while (initialPos < MaxX)
            {
                initialPos += stepSize;

                System.Windows.Point p0 = new System.Windows.Point(initialPos, MinY);
                System.Windows.Point p1 = new System.Windows.Point(initialPos, MaxY);
                p0 = CurveCoordsToScreen(ref p0);
                p1 = CurveCoordsToScreen(ref p1);
                p0.X = p1.X = Math.Truncate(p0.X);
                dc.DrawLine(pen, p0, p1);
            }
        }

        private void DrawSelectionBox(DrawingContext dc)
        {
            // Get the key position in screen coords.
            System.Windows.Point p1 = SelectionBox.p1;
            System.Windows.Point p2 = SelectionBox.p2;

            dc.DrawRectangle(null, selectionBorderPen, new Rect(p1, p2));
        }

        private void DrawScaleBox(DrawingContext dc)
        {
            // Get the key position in screen coords.
            System.Windows.Point p1 = CurveCoordsToScreen(ref scaleBox.Min);
            System.Windows.Point p2 = CurveCoordsToScreen(ref scaleBox.Max);
            //System.Windows.Point anchor = CurveCoordsToScreen(ref scaleBox.Anchor);
            var rect = new Rect(p1, p2);
            dc.DrawRectangle(null, selectionBorderPen, rect);

            //dc.DrawEllipse(scaleHandleBrush, scaleHandlePen, anchor, 2, 2);

            dc.DrawRectangle(scaleHandleBrush, scaleHandlePen, Rect.Inflate(new Rect(rect.TopLeft, rect.TopLeft), 2, 2));
            dc.DrawRectangle(scaleHandleBrush, scaleHandlePen, Rect.Inflate(new Rect(rect.TopRight, rect.TopRight), 2, 2));
            dc.DrawRectangle(scaleHandleBrush, scaleHandlePen, Rect.Inflate(new Rect(rect.BottomLeft, rect.BottomLeft), 2, 2));
            dc.DrawRectangle(scaleHandleBrush, scaleHandlePen, Rect.Inflate(new Rect(rect.BottomRight, rect.BottomRight), 2, 2));
        }

        private void DrawKey(DrawingContext dc, KeyWrapper key)
        {
            if (!key.Curve.Visible)
                return;

            float handleSize = KeySize;
            Pen handlePen = key.Curve.Pen;
            Pen inHandlePen = manualTangentPen;
            Pen outHandlePen = manualTangentPen;
            Brush inHandleBrush = Brushes.Black;
            Brush outHandleBrush = Brushes.Black;
            Brush fillBrush = key.Curve.Pen.Brush;

            // Get the key position in screen coords.
            System.Windows.Point screenPos = new System.Windows.Point(key.Key.Position, key.Key.Value);
            screenPos = CurveCoordsToScreen(ref screenPos);

            if (key.IsSelected)// || true)
            {
                // FIXME: Remove this condition and the true above.
                //if (key.IsSelected)
                //{
                handleSize = SelectedKeySize;
                fillBrush = selectedKeyBrush;
                //}

                int size = 2;

                // Change parameter for auto tangents.
                if (key.IsTangentModeAuto(key.TangentInMode))
                {
                    inHandlePen = autoTangentPen;
                    inHandleBrush = autoTangentBrush;
                    size = 1;
                }
                if (key.IsTangentModeAuto(key.TangentOutMode))
                {
                    outHandlePen = autoTangentPen;
                    outHandleBrush = autoTangentBrush;
                    size = 1;
                }

                System.Windows.Point outPos, inPos;
                key.GetTangentHandleScreenPositions(TangentHandleLength, out inPos, out outPos);

                dc.DrawLine(inHandlePen, screenPos, inPos);
                dc.DrawLine(outHandlePen, screenPos, outPos);

                // Draw the two tangent handles.
                dc.DrawRectangle(inHandleBrush, inHandlePen,
                    new Rect(
                        new System.Windows.Point((int)(inPos.X - size), (int)(inPos.Y - size)),
                        new System.Windows.Point((int)(inPos.X + 1), (int)(inPos.Y + 1))));
                dc.DrawRectangle(outHandleBrush, outHandlePen,
                    new Rect(
                        new System.Windows.Point((int)(outPos.X - size), (int)(outPos.Y - size)),
                        new System.Windows.Point((int)(outPos.X + 1), (int)(outPos.Y + 1))));
            }

            dc.DrawRectangle(fillBrush, handlePen,
                new Rect(
                    new System.Windows.Point((int)(screenPos.X - handleSize), (int)(screenPos.Y - handleSize)),
                    new System.Windows.Point((int)(screenPos.X + handleSize), (int)(screenPos.Y + handleSize))));
        }

        private void DrawCurve(DrawingContext dc, CurveWrapper curve)
        {
            if (!curve.Visible)
                return;

            // Some config constants
            float screenStep = 4f;// (float)(ActualWidth / 100); // Sample every n pixels
            float curveStep = (float)(screenStep / ActualWidth) * (MaxX - MinX);
            int stepCount = (int)(ActualWidth / screenStep) + 1;
            Pen pen = curve.DashedPen;

            System.Windows.Point prevPoint = new System.Windows.Point(MinX, curve.Evaluate(MinX));
            prevPoint = CurveCoordsToScreen(ref prevPoint);

            // Calc the index of the first visible key.
            int nextKey = 0;
            for (int i = 0; i < curve.Curve.Keys.Count; i++)
            {
                if (curve.Curve.Keys[i].Position > MinX)
                {
                    nextKey = i;
                    break;
                }
            }

            if (nextKey > 0)
                pen = curve.Pen;

            float screenx = 0;
            float curvex = MinX;
            CurveKeyCollection keys = curve.Curve.Keys;
            for (int i = 1; i < stepCount; i++)
            {
                // Advance the needles
                screenx += screenStep;
                curvex += curveStep;

                System.Windows.Point point;
                // Did we just stepped over a key? Make sure we hit it.
                while (nextKey < keys.Count && curvex > keys[nextKey].Position)
                {
                    point = new System.Windows.Point(keys[nextKey].Position, keys[nextKey].Value);
                    point = CurveCoordsToScreen(ref point);
                    dc.DrawLine(curve.Pen, prevPoint, point);
                    prevPoint = point;
                    nextKey++;

                    // Use the solid pen.
                    if (nextKey == 1)
                        pen = curve.Pen;
                }
                if (nextKey == keys.Count)
                {
                    pen = curve.DashedPen;
                }

                // FIXME: There can be problems with this method if the curve makes sharp turns in not-key positions
                // This can be solved by detecting mins and maxs and always reaching them.
                // 

                point = new System.Windows.Point(curvex, curve.Evaluate(curvex));
                point = CurveCoordsToScreen(ref point);
                point.X = Math.Truncate(point.X);
                point.Y = Math.Truncate(point.Y);
                dc.DrawLine(pen, prevPoint, point);
                prevPoint = point;
            }
        } 

        //private void DrawCurves(DrawingContext dc)
        //{
        //    GeometryDrawing drawing = new GeometryDrawing();
        //    foreach (var curve in Curves)
        //    {
        //        if (!curve.Visible)
        //            continue;

        //        // Some config constants
        //        //float screenStep = 100f;// (float)(ActualWidth / 100); // Sample every n pixels
        //        //float curveStep = (float)(screenStep / ActualWidth) * (MaxX - MinX);
        //        //int stepCount = (int)(ActualWidth / screenStep) + 1;
        //        Pen pen = curve.DashedPen;

        //        System.Windows.Point point1 = new System.Windows.Point(MinX, curve.Evaluate(MinX));
        //        point1 = CurveCoordsToScreen(ref point1);

        //        // Calc the index of the first not visible key.
        //        int nextKey = 0;
        //        for (int i = 0; i < curve.Curve.Keys.Count; i++)
        //        {
        //            if (curve.Curve.Keys[i].Position > MinX)
        //            {
        //                nextKey = i;
        //                if (i > 0)
        //                    nextKey--;
        //                break;
        //            }
        //        }

        //        if (nextKey > 0)
        //            pen = curve.Pen;

        //        PathGeometry geometry = new PathGeometry();
        //        List<PathSegment> segments = new List<PathSegment>();
        //        for (int i = nextKey; i < curve.Curve.Keys.Count; i++)
        //        {
        //            System.Windows.Point point2 = new System.Windows.Point(curve.Curve.Keys[i].Position, curve.Curve.Keys[i].Value);
        //            //System.Windows.Point control2 = new System.Windows.Point(GetWrapper(curve.Curve.Keys[i]))
        //            point2 = CurveCoordsToScreen(ref point2);
        //            BezierSegment segment = new BezierSegment(point1, point2, point2, true);
        //            segments.Add(segment);

        //            point1 = point2;
        //        }
        //        geometry.Figures.Add(new PathFigure(point1, segments, false));

        //        dc.DrawGeometry(null, pen, geometry);

        //    }
        //} 
        #endregion

        
    }



    public enum TangentSelectionMode
    {
        In = -1,
        Out = -2,
    }

    public enum ToolMode
    {
        /// <summary>
        /// Default mode, select/move/etc.
        /// </summary>
        SelectMove,
        AddKeys,
        ZoomBox,
        ScaleKeys,
    }
}
