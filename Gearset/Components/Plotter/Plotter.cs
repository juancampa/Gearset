using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Gearset.Extensions;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Plotter
{
    public class Plot : UI.Window
#if WINDOWS || LINUX || MONOMAC
, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
#else
    {
#endif
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Plot"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
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
#if WINDOWS || LINUX || MONOMAC
                OnPropertyChanged(nameof(Visible));
#endif
            }
        }
        private bool _visible;

        internal event EventHandler VisibleChanged;


        public DataSampler Sampler { get; }
        internal string TitleLabelName;

        internal string MinLabelName;
        internal string MaxLabelName;

        internal float Min;
        internal float Max;

        public Plot(DataSampler sampler, Vector2 position, Vector2 size)
            : base(position, size)
        {
            TitleBarSize = 14;
            Sampler = sampler;
            Visible = true;
            TitleLabelName = "__Plot" + sampler.Name;
            MinLabelName = "lo" + TitleLabelName;
            MaxLabelName = "hi" + TitleLabelName;
        }
    }

    /// <summary>
    /// Class that takes a DataSampler and plots its values to the screen.
    /// </summary>
    public class Plotter : Gear
    {
        /// <summary>
        /// Current plots.
        /// </summary>
#if WINDOWS || LINUX || MONOMAC
        private readonly ObservableCollection<Plot> _plots;
        public ObservableCollection<Plot> Plots => _plots;
#else
        private List<Plot> plots;
        public List<Plot> Plots { get { return plots; } }
#endif

        /// <summary>
        /// If a plot is beign removed, the change of visibility to false will not
        /// imply that the plot should be added to the hidden list.
        /// </summary>
        private Plot _plotBeignRemoved;
        private readonly InternalLineDrawer _lines;
        private readonly InternalLabeler _labels;

        readonly StringBuilder _stringBuilder = new StringBuilder(32);

        /// <summary>
        /// Gets or sets the config object.
        /// </summary>
        public PlotterConfig Config => GearsetResources.Console.Settings.PlotterConfig;

        public Plotter()
            : base(GearsetResources.Console.Settings.PlotterConfig)
        {
#if WINDOWS || LINUX || MONOMAC
            _plots = new ObservableCollection<Plot>();
#else
            plots = new List<Plot>();
#endif
            Config.Cleared += Config_Cleared;
            // Create the line drawer
            _lines = new InternalLineDrawer();
            Children.Add(_lines);
            _labels = new InternalLabeler();
            Children.Add(_labels);
        }

        void Config_Cleared(object sender, EventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// Shows a plot of the sampler with the specified name if the sampler does
        /// not exist it is created.
        /// </summary>
        public void ShowPlot(string samplerName)
        {
            // Check if the plot exist already.
            for (var i = 0; i <  _plots.Count; i++)
            {
                if (_plots[i].Sampler.Name == samplerName)
                    return;
            }
            var sampler = GearsetResources.Console.DataSamplerManager.GetSampler(samplerName);
            if (sampler == null)
                return;

            var position = GetNextPosition();
            var plot = new Plot(sampler, position, Config.DefaultSize);
            _plots.Add(plot);

            // Hide all plots that are beign hidden.
            plot.Visible = !Config.HiddenPlots.Contains(samplerName);

            plot.VisibleChanged += plot_VisibleChanged;
        }

        void plot_VisibleChanged(object sender, EventArgs e)
        {
            var plot = sender as Plot;
            if (plot == null)
                return;

            if (plot.Visible == false)
            {
                // Disabled
                _labels.HideLabel(plot.TitleLabelName);
                _labels.HideLabel(plot.MinLabelName);
                _labels.HideLabel(plot.MaxLabelName);
                if (!Config.HiddenPlots.Contains(plot.Sampler.Name) && _plotBeignRemoved != plot)
                    Config.HiddenPlots.Add(plot.Sampler.Name);
            }
            else
            { 
                // Enabled
                if (Config.HiddenPlots.Contains(plot.Sampler.Name))
                    Config.HiddenPlots.Remove(plot.Sampler.Name);
            }
        }

        /// <summary>
        /// Calculates a position for a new plot.
        /// </summary>
        private Vector2 GetNextPosition()
        {
            var padding = new Vector2(3, 15);
            var screenSize = new Vector2(GearsetResources.Device.Viewport.Width, GearsetResources.Device.Viewport.Height);

            if (_plots.Count == 0)
                return new Vector2(padding.X, screenSize.Y - padding.Y - Config.DefaultSize.Y);
            
            var lastPlot = _plots[_plots.Count - 1];

            var result = lastPlot.Position + (padding + lastPlot.Size) * Vector2.UnitX;
            if (result.X + Config.DefaultSize.X > GearsetResources.Device.Viewport.Width)
            {
                result = new Vector2(padding.X, lastPlot.Position.Y - padding.Y - Config.DefaultSize.Y);
            }
            return result;
        }

        public override void Draw(GameTime gameTime)
        {
            // Just to make sure we're only doing this one per frame.
            if (GearsetResources.CurrentRenderPass != RenderPass.BasicEffectPass)
                return;

            for (var i = 0; i < _plots.Count; i++)
            {
                var plot = _plots[i];
                if (plot.Visible == false)
                {
                    _labels.HideLabelEx(plot.MinLabelName);
                    _labels.HideLabelEx(plot.MaxLabelName);
                    continue;
                }
                    


                var count = plot.Sampler.Values.Capacity;
                float actualmin, actualmax;
                var position = plot.Position;
                var size = plot.Size;
                plot.Sampler.GetLimits(out actualmin, out actualmax);

                var min = plot.Min = plot.Min + (actualmin - plot.Min) * 0.3f;
                var max = plot.Max = plot.Max + (actualmax - plot.Max) * 0.3f;

                // Draw the background
                GearsetResources.Console.SolidBoxDrawer.ShowGradientBoxOnce(position, position + size, new Color(56, 56, 56, 150), new Color(16, 16, 16, 127));

                // Draw the border
                plot.DrawBorderLines(Color.Gray, _lines);
                
                if (plot.ScaleNob.IsMouseOver)
                    plot.ScaleNob.DrawBorderLines(Color.White, _lines);

                _labels.ShowLabel(plot.TitleLabelName, position + new Vector2(0, -12), plot.Sampler.Name);

                PlotGraph(plot, _lines, _labels, count, min, max, position, size, actualmin, actualmax, 2);
            }
        }

        public void PlotGraph(Plot plot, InternalLineDrawer lines, InternalLabeler labels, int count, float min, float max, Vector2 position, Vector2 size, float actualMin, float actualMax, float labelOffsetX)
        {
            if (Math.Abs(min - max) > 0.00001f)
            {
                // Draw zero
                if (min < 0 && max > 0)
                {
                    var normalValue = (0 - min) / (max - min);
                    var yoffset = new Vector2 { X = 0, Y = size.Y * (1 - normalValue) };
                    lines.ShowLineOnce(position + yoffset, position + new Vector2(size.X, 0) + yoffset, new Color(230, 0, 0, 220));
                }

                var previousPoint = Vector2.Zero;
                var pixelOffset = Vector2.UnitY;
                var i = 0;
                foreach (var value in plot.Sampler.Values)
                {
                    var normalValue = (value - min) / (max - min);
                    var point = new Vector2
                    {
                        X = position.X + i / ((float)count - 1) * size.X,
                        Y = position.Y + (size.Y - 1f) * MathHelper.Clamp((1 - normalValue), 0, 1)
                    };

                    if (i != 0)
                    {
                        lines.ShowLineOnce(previousPoint, point, FlatTheme.PeterRiver);
                        lines.ShowLineOnce(previousPoint + pixelOffset, point + pixelOffset, FlatTheme.PeterRiver);
                    }

                    i++;
                    previousPoint = point;
                }

                // Show the min/max labels.
                _stringBuilder.SetText(actualMin);
                labels.ShowLabelEx(plot.MinLabelName, position + new Vector2(labelOffsetX, size.Y - 12), _stringBuilder, Color.White);

                _stringBuilder.SetText(actualMax);
                labels.ShowLabelEx(plot.MaxLabelName, position + new Vector2(labelOffsetX, 0), _stringBuilder, Color.White);
            }
            else if (plot.Sampler.Values.Count > 0)
            {
                lines.ShowLineOnce(new Vector2(position.X, position.Y + size.Y * .5f), new Vector2(position.X + size.X, position.Y + size.Y * .5f), FlatTheme.PeterRiver);
                lines.ShowLineOnce(new Vector2(position.X, position.Y + size.Y * .5f + 1), new Vector2(position.X + size.X, position.Y + size.Y * .5f + 1), FlatTheme.PeterRiver);

                _stringBuilder.SetText(actualMin);
                labels.ShowLabelEx(plot.MinLabelName, position + new Vector2(labelOffsetX, size.Y * .5f - 12), _stringBuilder, Color.White);
            }
            else
            {
                plot.DrawCrossLines(Color.Gray);
            }
        }

        /// <summary>
        /// Removes a plot, if ShowPlot is called again for this plot, it will be shown
        /// again
        /// </summary>
        /// <param name="name">Name of the plot to remove.</param>
        public void RemovePlot(string name)
        {
            _plotBeignRemoved = null;
            foreach (var plot in _plots)
            {
                if (plot.Sampler.Name == name)
                {
                    _plotBeignRemoved = plot;
                    break;
                }
            }
            if (_plotBeignRemoved != null)
            {
                _plotBeignRemoved.Visible = false;
                _plots.Remove(_plotBeignRemoved);
                _plotBeignRemoved = null;
            }
        }

        /// <summary>
        /// Removes all plots, if ShowPlot is called again, plots will be shown
        /// again.
        /// </summary>
        public void Clear()
        {
            HideAll();
            _plots.Clear();
        }

        /// <summary>
        /// Hides all plots, data will still be captured.
        /// </summary>
        public void HideAll()
        {
            foreach (var plot in _plots)
            {
                plot.Visible = false;
            }
        }

        /// <summary>
        /// Hides all plots, data will still be captured.
        /// </summary>
        public void ShowAll()
        {
            foreach (var plot in _plots)
            {
                plot.Visible = true;
            }
        }

        /// <summary>
        /// Resets the positions of all overlaid plots.
        /// </summary>
        public void ResetPositions()
        {
            var plotsAux = new List<Plot>();
            foreach (var plot in _plots)
                plotsAux.Add(plot);
            _plots.Clear();
            foreach (var plot in plotsAux)
            {
                plot.Position = GetNextPosition();
                plot.Size = Config.DefaultSize;
                _plots.Add(plot);
            }
            plotsAux.Clear();
        }
    }
}
