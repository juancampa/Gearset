using System;
using System.ComponentModel;
using System.Text;
using Gearset.Components.Plotter;
using Gearset.Extensions;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Memory
{
    public class MemoryGraph : UI.Window
    {
        static class LabelName
        {
            public const string Title = "__memoryGraph";
            public const string Gen0Label = "__memoryGraph.Gen0Label";
            public const string Gen0 = "__memoryGraph.Gen0";
            public const string Gen1Label = "__memoryGraph.Gen1Label";
            public const string Gen1 = "__memoryGraph.Gen1";
            public const string Gen2Label = "__memoryGraph.Gen2Label";
            public const string Gen2 = "__memoryGraph.Gen2";
            public const string XboxLabel = "__memoryGraph.XboxLabel";
            public const string Xbox = "__memoryGraph.Xbox";
            public const string MinYAxis = "__memoryGraph.MinYAxis";
            public const string MaxYAxis = "__memoryGraph.MaxYAxis";
        }

        const  int LegendWidth = 100;

        readonly MemoryMonitor _memoryMonitor;

        public MemoryMonitorConfig Config => GearsetSettings.Instance.MemoryMonitorConfig;

        public event PropertyChangedEventHandler PropertyChanged;

        bool _visible;

        float _maxTotalYScaleMb = 1.0f;
        readonly Plot _plot = new Plot(new DataSampler("__memoryGraph.tickMemory", MemoryMonitor.MaxFrames), Vector2.Zero, Vector2.One);

        readonly StringBuilder _stringBuilder = new StringBuilder(64);

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;

                VisibleChanged?.Invoke(this, EventArgs.Empty);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Visible"));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        internal event EventHandler VisibleChanged;

        internal MemoryGraph(MemoryMonitor memoryMonitor, MemoryMonitorConfig.UIViewConfig memoryGraphUIConfig, Vector2 size)
            : base(memoryGraphUIConfig.Position, size)
        {
            _memoryMonitor = memoryMonitor;
        }

        internal void Draw(InternalLabeler labeler)
        {
            if (GearsetResources.CurrentRenderPass != RenderPass.BasicEffectPass)
                return;

            if (Visible == false)
            {
                HideLabels(labeler);
                return;
            }

            var graphWidth = Width - LegendWidth;

            //Draw base background, border, size nubbin, title, etc.
            DrawUIView(labeler);

            //Legend indicates collections stats.
            DrawLegend(labeler, Position + new Vector2(15, 15));

            //A bar graph of x frames showing total managed memmory for the process.
            DrawTotalMemoryGraph(graphWidth);

            //Overay a line graph of the managed memory allocated each tick.
            PlotTickMemoryGraph(labeler, graphWidth);
        }

        static void HideLabels(InternalLabeler labeler)
        {
            labeler.HideLabel(LabelName.Title);
            labeler.HideLabel(LabelName.Gen0Label);
            labeler.HideLabel(LabelName.Gen0);
            labeler.HideLabel(LabelName.Gen1Label);
            labeler.HideLabel(LabelName.Gen1);
            labeler.HideLabel(LabelName.Gen2Label);
            labeler.HideLabel(LabelName.Gen2);
            labeler.HideLabel(LabelName.XboxLabel);
            labeler.HideLabel(LabelName.Xbox);
            labeler.HideLabel(LabelName.MinYAxis);
            labeler.HideLabel(LabelName.MaxYAxis);

        }

        void DrawUIView(InternalLabeler labeler)
        {
            DrawBorderLines(Color.Gray, _memoryMonitor.LineDrawer);
            GearsetResources.Console.SolidBoxDrawer.ShowGradientBoxOnce(Position, Position + Size, new Color(56, 56, 56, 150), new Color(16, 16, 16, 127));

            if (ScaleNob.IsMouseOver)
                ScaleNob.DrawBorderLines(Color.Gray);

            labeler.ShowLabel(LabelName.Title, Position + new Vector2(0, -12), "Managed Memory - (allocation per tick in KB)");
        }

        /// <summary>
        /// //Draw an item on the legend.
        /// </summary>
        /// <param name="labeler"></param>
        /// <param name="position"></param>
        void DrawLegend(InternalLabeler labeler, Vector2 position)
        {
            var offset = new Vector2(0, 15);

            var currentFrame = _memoryMonitor.CurrentFrame;

            DrawLegendItem(labeler, position, LabelName.Gen0Label, LabelName.Gen0, "Gen0", currentFrame.Gen0CollectionCount, FlatTheme.Emerald);

            position += offset;
            DrawLegendItem(labeler, position, LabelName.Gen1Label, LabelName.Gen1, "Gen1", currentFrame.Gen1CollectionCount, FlatTheme.Carrot);

            position += offset;
            DrawLegendItem(labeler, position, LabelName.Gen2Label, LabelName.Gen2, "Gen2", currentFrame.Gen2CollectionCount, FlatTheme.Pomegrantate);

            position += offset;
            DrawLegendItem(labeler, position, LabelName.XboxLabel, LabelName.Xbox, "Xbox", currentFrame.Gen2CollectionCount, FlatTheme.Silver);
        }

        void DrawLegendItem(InternalLabeler labeler, Vector2 position, string labelName, string valueName, string labelText, int value, Color color)
        {
            //Format...[] GenX {count}
            _memoryMonitor.TempBoxDrawer.ShowGradientBoxOnce(position, position - new Vector2(10, 10), color, color);
            labeler.ShowLabel(labelName, position + new Vector2(4, -10), labelText, color);
            _stringBuilder.SetText(value);
            labeler.ShowLabelEx(valueName, position + new Vector2(44, -10), _stringBuilder, color);
        }

        void DrawTotalMemoryGraph(float graphWidth)
        {
            var barWidth = graphWidth/MemoryMonitor.MaxFrames;
            var graphFloor = Position.Y + Size.Y;
            var position = new Vector2(Position.X + Size.X, graphFloor);

            var barSize = new Vector2(barWidth, 0);

            var color = new Color(255, 255, 255)*0.5f;
            var i = 0;
            var id = _memoryMonitor.FrameId;

            //Draw each of the samples as a bar...
            while (i < MemoryMonitor.MaxFrames)
            {
                var memoryFrame = _memoryMonitor.Frames[id];

                var frameTotalMegabytes = memoryFrame.TotalMemoryBytes / 1024.0f/1024;

                if (frameTotalMegabytes > _maxTotalYScaleMb)
                    _maxTotalYScaleMb = frameTotalMegabytes;

                barSize.Y = Height * frameTotalMegabytes / _maxTotalYScaleMb;
                position.Y = graphFloor;

                //Total bytes bar graph
                _memoryMonitor.TempBoxDrawer.ShowGradientBoxOnce(position, position - barSize, color, color);

                //Events like collections?
                if (memoryFrame.CollectionType != MemoryMonitor.CollectionType.None)
                {
                    DrawCollections(memoryFrame, position, barSize);
                }

                position.X -= barWidth;

                id--;
                if (id < 0)
                    id = MemoryMonitor.MaxFrames - 1;

                i++;
            }
        }

        void DrawCollections(MemoryMonitor.MemoryFrame memoryFrame, Vector2 position, Vector2 barSize)
        {
            MaybeDrawCollection(memoryFrame, MemoryMonitor.CollectionType.Gen0, position, barSize, Vector2.Zero, new Vector2(10, 5), FlatTheme.Emerald);
            MaybeDrawCollection(memoryFrame, MemoryMonitor.CollectionType.Gen1, position, barSize, new Vector2(0, 5), new Vector2(10, 10), FlatTheme.Carrot);
            MaybeDrawCollection(memoryFrame, MemoryMonitor.CollectionType.Gen2, position, barSize, new Vector2(0, 10), new Vector2(10, 15), FlatTheme.Pomegrantate);
            MaybeDrawCollection(memoryFrame, MemoryMonitor.CollectionType.Xbox360, position, barSize, new Vector2(0, 15), new Vector2(10, 20), FlatTheme.Silver);
        }

        void MaybeDrawCollection(MemoryMonitor.MemoryFrame memoryFrame, MemoryMonitor.CollectionType collectionType, Vector2 position, Vector2 barSize, Vector2 offset1, Vector2 offset2, Color color)
        {
            if ((memoryFrame.CollectionType & collectionType) == 0)
                return;

            //Draw a horizontal line and a little square marker indicating a collection has taken place.
            _memoryMonitor.TempBoxDrawer.ShowGradientBoxOnce(position, position - barSize, color, color);
            _memoryMonitor.TempBoxDrawer.ShowGradientBoxOnce(position- offset1, position - offset2, color, color);
        }

        void PlotTickMemoryGraph(InternalLabeler labeler, float graphWidth)
        {
            _plot.Sampler.Values.Clear();
            _plot.Position = new Vector2(Position.X + LegendWidth, Position.Y);
            _plot.Size = new Vector2(graphWidth, Size.Y);

            var i = 0;
            var id = _memoryMonitor.FrameId;
            while (i < MemoryMonitor.MaxFrames)
            {
                id++;
                if (id == MemoryMonitor.MaxFrames)
                    id = 0;

                //Display Kilobytes per tick
                _plot.Sampler.Values.Enqueue(_memoryMonitor.Frames[id].TickMemoryBytes / 1024.0f);
                i++;
            }

            labeler.ShowLabel(LabelName.MinYAxis, Position + new Vector2(2 + LegendWidth, Size.Y - 12), "0", Color.White);

            _stringBuilder.SetText(_maxTotalYScaleMb).Append(" MB");
            labeler.ShowLabelEx(LabelName.MaxYAxis, Position + new Vector2(2 + LegendWidth, 0), _stringBuilder, Color.White);

            float actualmin, actualmax;
            _plot.Sampler.GetLimits(out actualmin, out actualmax);

            var min = _plot.Min = _plot.Min + (actualmin - _plot.Min) * 0.3f;
            var max = _plot.Max = _plot.Max + (actualmax - _plot.Max) * 0.3f;

            GearsetResources.Console.Plotter.PlotGraph(_plot, _memoryMonitor.LineDrawer, labeler, MemoryMonitor.MaxFrames, min, max, _plot.Position, _plot.Size, actualmin, actualmax, Size.X - LegendWidth - 40.0f);
        }
    }
}
