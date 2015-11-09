using System;
using System.Collections.Generic;
using System.Text;
using Gearset.Extensions;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Profiler
{
    public class ProfilerSummary : UIView
    {  
        public ProfilerConfig Config { get { return GearsetSettings.Instance.ProfilerConfig; } }

        const float Padding = 5.0f;

        // Marker log string.
        readonly StringBuilder _logString = new StringBuilder(512);
        readonly StringBuilder _logStringTimings = new StringBuilder(512);

        internal ProfilerSummary(ProfilerManager profiler, ProfilerConfig.UIViewConfig uiviewConfig, Vector2 size) : base(profiler, uiviewConfig, size)
        {

        }

        internal void Draw(InternalLabeler labeler, ProfilerManager.FrameLog frameLog)
        {
            if (frameLog == null || Visible == false || Config.ProfilerSummaryConfig.VisibleLevelsFlags == 0)
            {
                labeler.HideLabel("__profilerSummary");
                return;
            }

            var font = GearsetResources.Font;
            
            // Generate log string.
            _logString.Length = 0;
            _logStringTimings.Length = 0;

            foreach (var markerInfo in Profiler.Markers)
            {
                for (var i = 0; i < ProfilerManager.MaxLevels; ++i)
                {
                    if (!markerInfo.Logs[i].Initialized)
                        continue;

                    if (IsVisibleLevelsFlagSet(i) == false)
                        continue;
                        
                    if (_logString.Length > 0)
                    {
                        _logString.Append("\n");
                        _logStringTimings.Append("\n");
                    }

                    _logString.Append(" Level ");
                    _logString.AppendNumber(i);

                    _logString.Append(" ");

                    //Indent!
                    for(var x = 0; x < i; x++)
                        _logString.Append("--");    

                    _logString.Append(markerInfo.Name);

                    //_logStringTimings.Append(" Avg.:");
                    _logStringTimings.AppendNumber(markerInfo.Logs[i].SnapAvg);
                    _logStringTimings.Append(" ms ");
                }
            }

            var namesSize = font.MeasureString(_logString);
            var timingsSize = font.MeasureString(_logStringTimings);
            Size = namesSize + new Vector2(timingsSize.X, 0) + new Vector2(Padding * 5, Padding * 2);

            if (GearsetResources.CurrentRenderPass == RenderPass.BasicEffectPass)
            {
                DrawBorderLines(Color.Gray, Profiler.LineDrawer);
                GearsetResources.Console.SolidBoxDrawer.ShowGradientBoxOnce(Position, Position + Size, new Color(56, 56, 56, 150), new Color(16, 16, 16, 127));

                //Fixed size based on summary contrents
                //if (ScaleNob.IsMouseOver)
                //    ScaleNob.DrawBorderLines(Color.Gray);

                labeler.ShowLabel("__profilerSummary", Position + new Vector2(0, -12), "Profiler Summary");

                // Draw log color boxes.
                var position = Position;
                position += new Vector2(Padding);

                foreach (var markerInfo in Profiler.Markers)
                {
                    for (var i = 0; i < ProfilerManager.MaxLevels; ++i)
                    {
                        if (IsVisibleLevelsFlagSet(i) == false)
                            continue;

                        if (markerInfo.Logs[i].Initialized == false)
                            continue;

                        Profiler.TempBoxDrawer.ShowGradientBoxOnce(position, position + new Vector2(10), markerInfo.Logs[i].Color, markerInfo.Logs[i].Color);

                        position.Y += font.LineSpacing;
                    }
                }
            }

            if (GearsetResources.CurrentRenderPass == RenderPass.SpriteBatchPass)
            {
                // Draw log string.
                var position = Position;
                position += new Vector2(Padding * 3, Padding);
                GearsetResources.SpriteBatch.DrawString(font, _logString, position, new Color(180, 180, 180));

                position = Position;
                position += new Vector2(namesSize.X + (Padding * 5), Padding);
                GearsetResources.SpriteBatch.DrawString(font, _logStringTimings, position, new Color(220, 220, 220));
            }
        }
    }
}
