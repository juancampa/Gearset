using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gearset;
using Microsoft.Xna.Framework;

namespace ProfilerTestGame.Gearset
{
    /// <summary>
    /// Component to monitor memory allocations and garbage collection.
    /// </summary>
    public class MemoryMonitor : GameComponent
    {
        const int MaxGraphFrames = 60;

        // Stopwatch for GC measuring.
        Stopwatch _stopwatch;

        WeakReference _gcTracker = new WeakReference(new object());

        readonly Queue<long> _tickBytes = new Queue<long>(MaxGraphFrames);

        const int OneMegabyte = 1024 * 1024;

        long _previousMemory;
        long _baseMemory;
        long _registeredTickMemory;

        public long TotalMemoryBytes;
        public long TickMemoryBytes;

        /// <summary>Gets current GC</summary>
        public float Collections { get; private set; }

        /// <summary>Gets/Sets GC sample duration.</summary>
        public TimeSpan SampleSpan { get; set; }

        public long TotalMemoryK { get { return TotalMemoryBytes / 1024; } }
        public long TickMemoryK { get { return TickMemoryBytes / 1024; } }

        public MemoryMonitor(Game game)
            : base(game)
        {
            SampleSpan = TimeSpan.FromSeconds(1);
        }

        public override void Initialize()
        {
            // Initialize parameters.
            Collections = 0;
            _stopwatch = Stopwatch.StartNew();

            TotalMemoryBytes = GC.GetTotalMemory(false);
            _previousMemory = TickMemoryBytes;

            for (var i = 0; i < MaxGraphFrames; i++)
                _tickBytes.Enqueue(0);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            TotalMemoryBytes = GC.GetTotalMemory(false);
            TickMemoryBytes = TotalMemoryBytes - _previousMemory;
            _registeredTickMemory += TickMemoryBytes;
            _baseMemory += TickMemoryBytes;
            _previousMemory = TotalMemoryBytes;

            if (_tickBytes.Count == MaxGraphFrames)
                _tickBytes.Dequeue();

            _tickBytes.Enqueue(TickMemoryBytes);

            if (!_gcTracker.IsAlive)
            {
                GS.Log("Garbage", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:fffffff") + " : Garbage Collection");
                _gcTracker = new WeakReference(new object());
                TickMemoryBytes = 0;
                _registeredTickMemory = 0;
                _baseMemory = 0;
            }

            if (_stopwatch.Elapsed <= SampleSpan)
                return;

            //Need to know if > 1mb as that is when we get a collection on the xbox
            if (_baseMemory > OneMegabyte)
            {
                //XBox collection
#if WINDOWS || WINDOWS_STOREAPP || LINUX || MONOMAC
                GS.Log("Garbage", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:fffffff") + " : XBOX360 Garbage Collection");
#endif
                _baseMemory = 0;
            }

            _stopwatch.Reset();
            _stopwatch.Start();

            _registeredTickMemory = 0;
        }
    }
}
