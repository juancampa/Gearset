using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace ProfilerTestGame.Gearset
{
    /// <summary>
    /// Component for measuring FPS.
    /// </summary>
    public class FpsCounter : DrawableGameComponent
    {
        /// <summary>
        /// Gets current FPS
        /// </summary>
        public float Fps { get; private set; }

        /// <summary>
        /// Gets/Sets FPS sample duration.
        /// </summary>
        public TimeSpan SampleSpan { get; set; }

        // Stopwatch for fps measuring.
        Stopwatch _stopwatch;

        int _sampleFrames;

        public FpsCounter(Game game)
            : base(game)
        {
            SampleSpan = TimeSpan.FromSeconds(1);
        }

        public override void Initialize()
        {
            // Initialize parameters.
            Fps = 0;
            _sampleFrames = 0;
            _stopwatch = Stopwatch.StartNew();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (_stopwatch.Elapsed <= SampleSpan)
                return;

            // Update FPS value and start next sampling period.
            Fps = _sampleFrames / (float)_stopwatch.Elapsed.TotalSeconds;

            _stopwatch.Reset();
            _stopwatch.Start();
            _sampleFrames = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            _sampleFrames++;
        }
    }
}
