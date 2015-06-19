using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Gearset.UserInterface.Wpf.Bender
{
    public class GridRuler : FrameworkElement
    {
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(float), typeof(GridRuler), new PropertyMetadata(-0.5f, LimitsChangedCalllBack));
        public float Min { get { return (float)GetValue(MinProperty); } set { SetValue(MinProperty, value); } }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(float), typeof(GridRuler), new PropertyMetadata(2f, LimitsChangedCalllBack));
        public float Max { get { return (float)GetValue(MaxProperty); } set { SetValue(MaxProperty, value); } }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(RulerOrientation), typeof(GridRuler), new PropertyMetadata(RulerOrientation.Vertical));
        public RulerOrientation Orientation { get { return (RulerOrientation)GetValue(OrientationProperty); } set { SetValue(OrientationProperty, value); } }

        private float MinX { get { return Orientation == RulerOrientation.Horizontal ? Min : 0; } }
        private float MaxX { get { return Orientation == RulerOrientation.Horizontal ? Max : 0.1f; } }
        private float MinY { get { return Orientation == RulerOrientation.Vertical ? Min : 0; } }
        private float MaxY { get { return Orientation == RulerOrientation.Vertical ? Max : 0.1f; } }

        public static void LimitsChangedCalllBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridRuler control = d as GridRuler;
            if (control != null)
                control.InvalidateVisual();
        }

        public double halfDpiX;
        public double halfDpiY;
        private bool guidelinesFixed;

        private Pen gridLinePen;
        private Pen gridBoldLinePen;
        private Pen gridZeroLinePen;
        private Brush backgroundBrush;
        private SolidColorBrush textBrush;
        private Glyphs glyphs;
        private GeometryDrawing SeekNeedle;
        private bool mouseLeftDown;
        private bool dragging;
        private System.Windows.Point mouseDownPos;

        public GridRuler()
        {
            this.ClipToBounds = true;

            MouseWheel += new MouseWheelEventHandler(CurveEditorControl_MouseWheel);
            MouseMove += new MouseEventHandler(CurveEditorControl_MouseMove);
            MouseDown += new MouseButtonEventHandler(CurveEditorControl_MouseDown);
            MouseUp += new MouseButtonEventHandler(CurveEditorControl_MouseUp);
            
            InitializeResources();

           
        }

        /// <summary>
        /// Initializes all general pens and brushes.
        /// </summary>
        private void InitializeResources()
        {
            textBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(120, 120, 120));
            textBrush.Freeze();

            Brush gridLineBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(160, 160, 160)); ;
            gridLineBrush.Freeze();
            gridLinePen = new Pen(gridLineBrush, 1);
            gridLinePen.Freeze();

            Brush gridBoldLineBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(20, 20, 20));
            gridBoldLineBrush.Freeze();
            gridBoldLinePen = new Pen(gridBoldLineBrush, 1);
            gridZeroLinePen = new Pen(gridBoldLineBrush, 2);
            gridBoldLinePen.Freeze();
            gridZeroLinePen.Freeze();


            backgroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 48, 48, 48));
            backgroundBrush.Freeze();

            glyphs = new Glyphs();
            glyphs.FontUri = new Uri(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts) + "\\Arial.TTF");
            glyphs.FontRenderingEmSize = 11;
            glyphs.StyleSimulations = StyleSimulations.None;

            // Seek Needle
            Brush seekNeedleBrush = Brushes.Red;
            Pen seekNeedlePen = new Pen(Brushes.Black, 1);
            seekNeedlePen.Freeze();
            SeekNeedle = new GeometryDrawing();
            PathGeometry path = new PathGeometry();
            path.Figures.Add(new PathFigure(
                new System.Windows.Point(-4, 0), new[] { new PolyLineSegment(new [] {
                new System.Windows.Point(4, 0),
                new System.Windows.Point(0, -7),
                new System.Windows.Point(0, -20),
                new System.Windows.Point(0, -7)}, true) }, true));
            SeekNeedle.Geometry = path;
            SeekNeedle.Brush = seekNeedleBrush;
            SeekNeedle.Pen = seekNeedlePen;
        }

        #region Coords transforms
        public System.Windows.Point CurveCoordsToScreen(ref System.Windows.Point point)
        {
            return new System.Windows.Point(((point.X - MinX) / (MaxX - MinX)) * ActualWidth, -((point.Y - MinY) / (MaxY - MinY)) * ActualHeight + ActualHeight);
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
            mouseLeftDown = true;
            mouseDownPos = e.GetPosition(this);
        }

        void CurveEditorControl_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(this);

            // Detect a drag.
            if ((mouseLeftDown) && !dragging)
            {
                if (Math.Abs(mousePos.X - mouseDownPos.X) > 1 ||// SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(mousePos.Y - mouseDownPos.Y) > 1)//SystemParameters.MinimumVerticalDragDistance)
                {
                    dragging = true;
                    Mouse.Capture(this);
                }
            }

            if (dragging)
            {
                mousePos = ScreenCoordsToCurve(ref mousePos);
                NeedlePosition = mousePos.X;
                Dispatcher.BeginInvoke(new Action(InvalidateVisual), System.Windows.Threading.DispatcherPriority.SystemIdle);
                
            }
        }

        private bool invalidateRequested = false;
        public new void InvalidateVisual()
        {
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

        void CurveEditorControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseLeftDown = false;
            dragging = false;
            Mouse.Capture(null);
        }

        void CurveEditorControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
        } 
        #endregion
      
        #region Rendering
        protected override void OnRender(DrawingContext dc)
        {
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

            dc.DrawRectangle(backgroundBrush, null, new Rect(0, 0, ActualWidth, ActualHeight));

            double range;
            double pos = 0;
            double orderOfMag;
            double stepSize = 0.1;

            switch (Orientation)
            {
                case RulerOrientation.Vertical:
                    //// Thin horizontal lines
                    //range = MaxY - MinY;
                    //orderOfMag = Math.Pow(10, Math.Truncate(Math.Log10(range)));
                    //stepSize = 0.1 * orderOfMag;
                    pos = (Math.Truncate(MinY / stepSize) - 0) * stepSize;
                    //DrawGridHorizonalLines(dc, pos, stepSize, gridLinePen);
                    break;
                case RulerOrientation.Horizontal:
                    // Thin vertical lines.
                    //range = MaxX - MinX;
                    //orderOfMag = Math.Pow(10, Math.Truncate(Math.Log10(range)));
                    //stepSize = 0.1 * orderOfMag;
                    pos = Math.Truncate(MinX / stepSize) * stepSize;
                    //DrawGridVerticalLines(dc, pos, stepSize, gridLinePen);

                    // Draw the seek needle
                    var needlePos = new System.Windows.Point(NeedlePosition, 0);
                    needlePos = CurveCoordsToScreen(ref needlePos);
                    dc.PushTransform(new TranslateTransform(needlePos.X, needlePos.Y));
                    dc.DrawDrawing(SeekNeedle);
                    dc.Pop();
                    break;
            }

            dc.Pop();

            switch (Orientation)
            {
                case RulerOrientation.Vertical:
                    DrawHorizontalLinesText(dc, pos, stepSize);
                    break;
                case RulerOrientation.Horizontal:
                    DrawVerticalLinesText(dc, pos, stepSize);
                    break;
            }
            base.OnRender(dc);
        }

        private void DrawHorizontalLinesText(DrawingContext dc, double initialPos, double stepSize)
        {
            double previousTextPos = initialPos;
            while (initialPos < MaxY)
            {
                initialPos += stepSize;

                System.Windows.Point p0 = new System.Windows.Point(MinX, initialPos);
                p0 = CurveCoordsToScreen(ref p0);
                p0.Y = Math.Truncate(p0.Y);

                if (Math.Abs(p0.Y - previousTextPos) > 20)
                {
                    p0.Y = Math.Truncate(p0.Y) + 0.5;
                    // Draw the tick
                    System.Windows.Point p1 = new System.Windows.Point(0, p0.Y);
                    p0 = new System.Windows.Point(3, p0.Y);
                    dc.DrawLine(gridLinePen, p0, p1);

                    p0.Y = Math.Truncate(p0.Y);
                    glyphs.UnicodeString = String.Format("{0:0.###}", initialPos);
                    glyphs.OriginX = 4;
                    glyphs.OriginY = p0.Y + 5;

                    dc.DrawGlyphRun(textBrush, glyphs.ToGlyphRun());
                    previousTextPos = p0.Y;

                }
                
            }
        }

        private void DrawGridHorizonalLines(DrawingContext dc, double initialPos, double stepSize, Pen pen)
        {
            double previousTextPos = initialPos;
            while (initialPos < MaxY)
            {
                initialPos += stepSize;

                System.Windows.Point p1 = new System.Windows.Point(MaxX, initialPos);
                p1 = CurveCoordsToScreen(ref p1);
                p1.Y = Math.Truncate(p1.Y);
                System.Windows.Point p0 = new System.Windows.Point(p1.X - 5, p1.Y);
                dc.DrawLine(pen, p0, p1);
            }
        }

        private void DrawVerticalLinesText(DrawingContext dc, double initialPos, double stepSize)
        {
            double previousTextPos = initialPos;
            while (initialPos < MaxX)
            {
                initialPos += stepSize;

                System.Windows.Point p0 = new System.Windows.Point(initialPos, MinY);
                p0 = CurveCoordsToScreen(ref p0);
                p0.X = Math.Truncate(p0.X);

                if (Math.Abs(p0.X - previousTextPos) > 40)
                {
                    p0.X = Math.Truncate(p0.X) + 0.5;
                    // Draw the tick
                    System.Windows.Point p1 = new System.Windows.Point(p0.X, ActualHeight);
                    p0 = new System.Windows.Point(p0.X, ActualHeight - 3);
                    dc.DrawLine(gridLinePen, p0, p1);

                    p0.X = Math.Truncate(p0.X - 3);
                    glyphs.UnicodeString = String.Format("{0:0.###}", initialPos);
                    glyphs.OriginX = p0.X;
                    glyphs.OriginY = 12;

                    dc.DrawGlyphRun(textBrush, glyphs.ToGlyphRun());
                    previousTextPos = p0.X;
                }

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
        #endregion

        public double NeedlePosition { get; set; }
    }

    public enum RulerOrientation
    {
        Vertical,
        Horizontal,
    }
}
