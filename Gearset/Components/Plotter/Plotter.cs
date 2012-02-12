using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset.Components;
using Microsoft.Xna.Framework;
using Gearset;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gearset.Components.Data
{
    public class Plot : UI.Window
#if WINDOWS
        , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
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
                return visible;
            }
            set
            {
                visible = value;
                if (VisibleChanged != null)
                    VisibleChanged(this, EventArgs.Empty);
#if WINDOWS
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Visible"));
#endif
            }
        }
        private bool visible;

        internal event EventHandler VisibleChanged;


        public DataSampler Sampler { get; private set; }
        internal String TitleLabelName;

        internal String MinLabelName;
        internal String MaxLabelName;

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
#if WINDOWS
        private ObservableCollection<Plot> plots;
        public ObservableCollection<Plot> Plots { get { return plots; } }
#else
        private List<Plot> plots;
        public List<Plot> Plots { get { return plots; } }
#endif

        /// <summary>
        /// Keep a reference to the last plot added so we can position the next one.
        /// </summary>
        private Plot lastPlotAdded;
        /// <summary>
        /// If a plot is beign removed, the change of visibility to false will not
        /// imply that the plot should be added to the hidden list.
        /// </summary>
        private Plot plotBeignRemoved;
        private InternalLineDrawer lines;
        private InternalLabeler labels;

        /// <summary>
        /// Gets or sets the config object.
        /// </summary>
        public PlotterConfig Config { get { return GearsetResources.Console.Settings.PlotterConfig; } }

        public Plotter()
            : base(GearsetResources.Console.Settings.PlotterConfig)
        {
#if WINDOWS
            plots = new ObservableCollection<Plot>();
#else
            plots = new List<Plot>();
#endif
            Config.Cleared += new EventHandler(Config_Cleared);
            // Create the line drawer
            lines = new InternalLineDrawer();
            Children.Add(lines);
            labels = new InternalLabeler();
            Children.Add(labels);
        }

        void Config_Cleared(object sender, EventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// Shows a plot of the sampler with the specified name if the sampler does
        /// not exist it is created.
        /// </summary>
        public void ShowPlot(String samplerName)
        {
            // Check if the plot exist already.
            foreach (var p in plots)
            {
                if (p.Sampler.Name == samplerName)
                    return;
            }
            DataSampler sampler = GearsetResources.Console.DataSamplerManager.GetSampler(samplerName);
            if (sampler == null)
                return;

            Vector2 position = GetNextPosition();
            Plot plot = new Plot(sampler, position, Config.DefaultSize);
            plots.Add(plot);

            // Hide all plots that are beign hidden.
            plot.Visible = !Config.HiddenPlots.Contains(samplerName);

            plot.VisibleChanged += new EventHandler(plot_VisibleChanged);

            lastPlotAdded = plot;
        }

        void plot_VisibleChanged(object sender, EventArgs e)
        {
            Plot plot = sender as Plot;

            if (!plot.Visible)
            {
                // Disabled
                labels.HideLabel(plot.TitleLabelName);
                labels.HideLabel(plot.MinLabelName);
                labels.HideLabel(plot.MaxLabelName);
                if (!Config.HiddenPlots.Contains(plot.Sampler.Name) && plotBeignRemoved != plot)
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
            Vector2 padding = new Vector2(3, 15);
            Vector2 screenSize = new Vector2(GearsetResources.Device.Viewport.Width, GearsetResources.Device.Viewport.Height);

            Plot lastPlotAdded;
            if (plots.Count == 0)
                return new Vector2(padding.X, screenSize.Y - padding.Y - Config.DefaultSize.Y);
            else
                lastPlotAdded = plots[plots.Count - 1];

            Vector2 result = lastPlotAdded.Position + (padding + lastPlotAdded.Size) * Vector2.UnitX;
            if (result.X + Config.DefaultSize.X > GearsetResources.Device.Viewport.Width)
            {
                result = new Vector2(padding.X, lastPlotAdded.Position.Y - padding.Y - Config.DefaultSize.Y);
            }
            return result;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            // Just to make sure we're only doing this one per frame.
            if (GearsetResources.CurrentRenderPass != RenderPass.BasicEffectPass)
                return;

            foreach (Plot plot in plots)
            {
                if (!plot.Visible) continue;
                int count = plot.Sampler.Values.Capacity;
                float max, min, actualmin, actualmax;
                Vector2 position = plot.Position;
                Vector2 size = plot.Size;
                GetLimits(plot, out actualmin, out actualmax);


                min = plot.Min = plot.Min + (actualmin - plot.Min) * 0.3f;
                max = plot.Max = plot.Max + (actualmax - plot.Max) * 0.3f;

                // Draw the background
                GearsetResources.Console.SolidBoxDrawer.ShowGradientBoxOnce(position, position + size, new Color(56, 56, 56, 150), new Color(16, 16, 16, 127));

                // Draw the border
                plot.DrawBorderLines(Color.Gray, lines);
                //if (plot.TitleBar.IsMouseOver)
                //    plot.TitleBar.DrawBorderLines(Color.White, lines);
                if (plot.ScaleNob.IsMouseOver)
                    plot.ScaleNob.DrawBorderLines(Color.White, lines);
                labels.ShowLabel(plot.TitleLabelName, position + new Vector2(0, -12), plot.Sampler.Name);

                if (min != max)
                {
                    // Draw zero
                    if (min < 0 && max > 0)
                    {
                        float normalValue = (0 - min) / (max - min);
                        Vector2 yoffset = new Vector2 { X = 0, Y = size.Y * (1 - normalValue) };
                        lines.ShowLineOnce(position + yoffset, position + new Vector2(size.X, 0) + yoffset, new Color(230, 0, 0, 220));
                    }

                    Vector2 previousPoint = Vector2.Zero;
                    Vector2 pixelOffset = Vector2.UnitY;
                    int i = 0;
                    foreach (float value in plot.Sampler.Values)
                    {
                        float normalValue = (value - min) / (max - min);
                        Vector2 point = new Vector2
                        {
                            X = position.X + i / ((float)count - 1) * size.X,
                            Y = position.Y + (size.Y - 1f) * MathHelper.Clamp((1 - normalValue), 0, 1)
                        };

                        if (i != 0)
                        {
                            lines.ShowLineOnce(previousPoint, point, new Color(138, 198, 49));
                            lines.ShowLineOnce(previousPoint + pixelOffset, point + pixelOffset, new Color(138, 198, 49));
                        }

                        i++;
                        previousPoint = point;
                    }

                    // Show the min/max labels.
                    labels.ShowLabel(plot.MinLabelName, position + new Vector2(2, size.Y - 12), actualmin.ToString(), Color.White);
                    labels.ShowLabel(plot.MaxLabelName, position + new Vector2(2, 0), actualmax.ToString(), Color.White);
                }
                else if (plot.Sampler.Values.Count > 0)
                {
                    lines.ShowLineOnce(new Vector2(position.X, position.Y + size.Y * .5f), new Vector2(position.X + size.X, position.Y + size.Y * .5f), new Color(138, 198, 49));
                    lines.ShowLineOnce(new Vector2(position.X, position.Y + size.Y * .5f + 1), new Vector2(position.X + size.X, position.Y + size.Y * .5f + 1), new Color(138, 198, 49));
                    labels.ShowLabel(plot.MinLabelName, position + new Vector2(2, size.Y * .5f - 12), actualmin.ToString(), Color.White);
                }
                else
                {
                    plot.DrawCrossLines(Color.Gray);
                }
            }
        }

        // TODO: move this to the DataSampler class.
        private static void GetLimits(Plot plot, out float min, out float max)
        {
            max = float.MinValue;
            min = float.MaxValue;
            foreach (float value in plot.Sampler.Values)
            {
                if (value > max)
                    max = value;
                if (value < min)
                    min = value;
            }
            max = (float)Math.Ceiling(max);
            min = (float)Math.Floor(min);
        }

        /// <summary>
        /// Removes a plot, if ShowPlot is called again for this plot, it will be shown
        /// again
        /// </summary>
        /// <param name="name">Name of the plot to remove.</param>
        public void RemovePlot(String name)
        {
            plotBeignRemoved = null;
            foreach (var plot in plots)
            {
                if (plot.Sampler.Name == name)
                {
                    plotBeignRemoved = plot;
                    break;
                }
            }
            if (plotBeignRemoved != null)
            {
                plotBeignRemoved.Visible = false;
                plots.Remove(plotBeignRemoved);
                plotBeignRemoved = null;
            }
        }

        /// <summary>
        /// Removes all plots, if ShowPlot is called again, plots will be shown
        /// again.
        /// </summary>
        public void Clear()
        {
            HideAll();
            plots.Clear();
        }

        /// <summary>
        /// Hides all plots, data will still be captured.
        /// </summary>
        public void HideAll()
        {
            foreach (Plot plot in plots)
            {
                plot.Visible = false;
            }
        }

        /// <summary>
        /// Hides all plots, data will still be captured.
        /// </summary>
        public void ShowAll()
        {
            foreach (Plot plot in plots)
            {
                plot.Visible = true;
            }
        }

        /// <summary>
        /// Resets the positions of all overlaid plots.
        /// </summary>
        public void ResetPositions()
        {
            List<Plot> plotsAux = new List<Plot>();
            foreach (var plot in plots)
                plotsAux.Add(plot);
            plots.Clear();
            foreach (var plot in plotsAux)
            {
                plot.Position = GetNextPosition();
                plot.Size = Config.DefaultSize;
                plots.Add(plot);
            }
            plotsAux.Clear();
        }
    }
}
