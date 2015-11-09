// Wrapper for Gearset. You should copy this class into your
// project and it should be used instead of accessing Gearset's
// methods directly. It allow you to completely enable/disable Gearset
// by setting/unsetting the USE_GEARSET symbol in your project
// properties (i.e. Properties -> Build -> Compilation Symbols).
// This is useful for when you're game is ready for release.
// 
// This file and also provide a level of threadsafeness. 
// If you need it, you can rename this class and move it to your
// own namespace, or even to Microsoft.Xna.Framework so it's
// quickly available from most classes in your project.
// 
// IMPORTANT: Calling Gearset methods from other threads will work but
// the results might appear unordered (e.g. out of order log items)
// 
// IMPORTANT: This file is auto generated and released with each version
// of Gearset. Newer version will probably contain new methods and improvements
// so it's recommended that you don't add code here because then you will
// have to move it when a new Gearset version is released.

// HOW TO USE
// ----------

// See http://www.thecomplot.com/gearset.html for details

// QUICK GUIDE
//------------

// Initialise Gearset
// protected override void Initialize()
// {
//     GS.Initialize(this);
//     base.Initialize();
// }

// PROFILER 
// You must call StartFrame at the top of Update to indicate to the Profiler that a new frame has started.
// protected override void Update(GameTime gameTime)
// {
//     GS.StartFrame();
// 
//     // Wrap code to profile in Begin/EndMArk calls (nested calls are allowed)
//     GS.BeginMark("SomeLabel", Color.Blue);
//     // Code to profile goes here...
//     GS.EndMark("SomeLabel");
//
//     GS.BeginMark("GameObjects", Color.Green);
//     GS.BeginMark("Bullets", Color.Yellow);
//     // Code to profile goes here...
//     GS.EndMark("Bullets");
//     GS.BeginMark("Enemies", Color.Pink);
//     // Oher code here...
//     GS.EndMark("Enemies");
//     GS.EndMark("GameObjects");
//
//     //rest of code...
// }

// LOGGER
// Logging is simple!
// GS.Log("I am a log message - and I write to the default stream");
// GS.Log("CustomStream", "I log to a custom stream");

// PLOTS
// Plot graph values over time
// protected override void Update(GameTime gameTime)
// {
//     GS.Plot("FPS", MyFPSComponent.Value);
// }

// LABELS
// Add labels to items in your scene / world
// protected override void Update(GameTime gameTime)
// {
//     GS.ShowLabel("I follow the mouse pointer!", new Vector2(mouseState.X, mouseState.Y));
// }

// ALERTS
// Indicate when certain game events occur
// protected override void Update(GameTime gameTime)
// {
//     if GC HAS RUN
//         GS.Alert("Garbage Collection!");
// }

// SOURCE CODE
// -----------
// Full source code is available from GITHUB.
// https://github.com/juancampa/Gearset
// https://github.com/PumpkinPaul/Gearset

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Gearset;
using Microsoft.Xna.Framework;

// to access the GS class.
namespace SampleGame.Gearset
{
    /// <summary>
    /// Wrapper for Gearset. It should be used instead of accessing Gearset's
    /// methods directly as it provides easy removal and threadsafeness.
    /// </summary>
    public static class GS
    {
        private static GearConsole Console { get; set; }

        /// <summary>
        /// This is the component that calls Update and Draw to make Gearset draw.
        /// You don't need to do anything special with this.
        /// </summary>
        public static GearsetComponent GearsetComponent { get; private set; }

        /// <summary>
        /// If you're using a transform for your 2D objects (e.g. in the SpriteBatch)
        /// make sure that Gearset knows about it either by setting it here or using 
        /// the SetMatrices overload.
        /// </summary>
        public static Matrix Transform2D { get { return Console.Transform2D; } set { Console.Transform2D = value; } }

        /// <summary>
        /// Returns the needle position of the curves in Bender. The game can use this
        /// value to let designers preview curve animations.
        /// </summary>
        public static float BenderNeedlePosition => Console.BenderNeedlePosition;

        /// <summary>
        /// The thread that initialized and owns this class.
        /// (As a side note: should always be the main thread of the Game class.)
        /// </summary>
        private static Thread _ownerThread;

        /// <summary>
        /// Actions that where queued because the thread that called them was not the owner thread.
        /// </summary>
        private static readonly Queue<Action> QueuedActions;

        /// <summary>
        /// An object to lock on for thread safety.
        /// </summary>
        private static readonly object SyncRoot;

        static GS()
        {
            SyncRoot = new object();
            QueuedActions = new Queue<Action>(16);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // NOTE - Change for 3.0.5
        // Added a boolean paramter to Initialize to control whether the full Gearset UI is created or not.
        // Pass true to create the Gearet UI (the full Gearset experience as before).

        // Pass false for no (WPF / EmptyKeysUI) UI - this mode is useful for monitoring managed memory allocations from your application 
        // (as the UIs generate quite a bit of garbage).

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This is the method you need to work for Gearset to work on your game.
        /// Remember to call SetMatrices to make Gearset's camera match yours.
        /// </summary>
        /// <param name="game">Your game instance</param>
        /// <param name="createUI">True to create the Gearet UI; otherwise false.</param>
        [Conditional("USE_GEARSET")]
        internal static void Initialize(Game game, bool createUI = true)
        {
            // Create the Gearset Component, this will be in charge of
            // Initializing Gearset and Updating/Drawing it every frame.
            GearsetComponent = new GearsetComponent(game, createUI);
            game.Components.Add(GearsetComponent);

            // This component updates this class allowing it to process
            // calls from other threads which are queued.
            game.Components.Add(new GearsetWrapperUpdater(game));

            Console = GearsetComponent.Console;
            _ownerThread = Thread.CurrentThread;
        }

        /// <summary>
        /// This class will call update on the Debug class so that it can pump
        /// queued calls from other threads.
        /// </summary>
        private class GearsetWrapperUpdater : GearsetComponentBase
        {
            public GearsetWrapperUpdater(Game game)
                : base(game)
            {
                // This is important since the GearsetComponent will have an
                // UpdateOrder of int.MaxValue - 1.
                UpdateOrder = int.MaxValue - 2;
            }

            public override void Update(GameTime gameTime)
            {
                // If you rename this file. Update this:
                GS.Update(gameTime);
            }
        }

        #region Update
        [Conditional("USE_GEARSET")]
        public static void Update(GameTime gameTime)
        {
            Debug.Assert(SameThread(), "The updating thread must be the same one that initialized this class");

            if (QueuedActions.Count <= 0)
                return;

            lock (SyncRoot)
            {
                while (QueuedActions.Count > 0)
                {
                    var action = QueuedActions.Dequeue();
                    action();
                }
            }
        }
        #endregion

        #region SetMatrices
        /// <summary>
        /// Use this method after every Update of your game to update the camera
        /// matrices so 3D overlays can be drawn correctly.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection)
        {
            if (SameThread())
                Console.SetMatrices(ref world, ref view, ref projection);
            else
                EnqueueSetMatrices(ref world, ref view, ref projection);
        }

        /// <summary>
        /// Use this method after every Update of your game to update the camera
        /// matrices so 3D overlays can be drawn correctly.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection, ref Matrix transform2D)
        {
            if (SameThread())
                Console.SetMatrices(ref world, ref view, ref projection, ref transform2D);
            else
                EnqueueSetMatrices(ref world, ref view, ref projection, ref transform2D);
        }
        #endregion

        #region Thread Safe Helpers
        private static bool SameThread()
        {
            return _ownerThread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Wrapper for method execution.
        /// It checks if the current calling thread is the same that initialized this class.
        /// If not, then the action is queued to be consumed during the update process.
        /// </summary>
        /// <param name="action"></param>
        private static void EnqueueAction(Action action)
        {
            lock (SyncRoot)
            {
                QueuedActions.Enqueue(action);
            }

        }
        #endregion
        #region Wrappers for Gearset methods
        /// <summary>
        /// Adds or modifiy a key without value on the overlaid tree view.
        /// </summary>
        /// <param name="key">A dot-separated list of keys.</param>
        [Conditional("USE_GEARSET")]
        public static void Show(string key)
        {
            if (SameThread())
                Console.Show(key);
            else
                EnqueueShow(key);
        }

        /// <summary>
        /// Adds or modifies a key/value pair to the overlaid tree view.
        /// </summary>
        /// <param name="key">A dot-separated list of keys.</param>
        /// <param name="value">The value to show.</param>
        [Conditional("USE_GEARSET")]
        public static void Show(string key, object value)
        {
            if (SameThread())
                Console.Show(key, value);
            else
                EnqueueShow(key, value);
        }

        /// <summary>
        /// Adds an action button to the bottom of the game window.
        /// </summary>
        /// <param name="name">Name of the action as it will appear on the button.</param>
        /// <param name="action">Action to perform when the button is clicked.</param>
        [Conditional("USE_GEARSET")]
        public static void AddQuickAction(string name, Action action)
        {
            if (SameThread())
                Console.AddQuickAction(name, action);
            else
                EnqueueAddQuickAction(name, action);
        }

        /// <summary>
        /// Adds the provided value to the plot with the provided plotName.
        /// </summary>
        /// <param name="plotName">A name that represent a data set.</param>
        /// <param name="value">The value to add to the sampler</param>
        [Conditional("USE_GEARSET")]
        public static void Plot(string plotName, float value)
        {
            if (SameThread())
                Console.Plot(plotName, value);
            else
                EnqueuePlot(plotName, value);
        }

        /// <summary>
        /// Adds the provided value to the plot with the provided plotName. At the same time modifies
        /// the history length of the sampler.
        /// </summary>
        /// <param name="plotName">A name that represent a data set.</param>
        /// <param name="value">The value to add to the sampler</param>
        /// <param name="historyLength">The number of samples that the sampler will remember at any given time.</param>
        [Conditional("USE_GEARSET")]
        public static void Plot(string plotName, float value, int historyLength)
        {
            if (SameThread())
                Console.Plot(plotName, value, historyLength);
            else
                EnqueuePlot(plotName, value, historyLength);
        }

        /// <summary>
        /// Los a message to the specified stream.
        /// </summary>
        /// <param name="streamName">Name of the Stream to log the message to</param>
        /// <param name="content">Message to log</param>
        [Conditional("USE_GEARSET")]
        public static void Log(string streamName, string content)
        {
            if (SameThread())
                Console.Log(streamName, content);
            else
                EnqueueLog(streamName, content);
        }

        /// <summary>
        /// Logs the specified message in the default stream.
        /// </summary>
        /// <param name="content">The message to log.</param>
        [Conditional("USE_GEARSET")]
        public static void Log(string content)
        {
            if (SameThread())
                Console.Log(content);
            else
                EnqueueLog(content);
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        [Conditional("USE_GEARSET")]
        public static void Log(string streamName, string format, object arg0)
        {
            if (SameThread())
                Console.Log(streamName, format, arg0);
            else
                EnqueueLog(streamName, format, arg0);
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        /// <param name="arg1">The second format parameter</param>
        [Conditional("USE_GEARSET")]
        public static void Log(string streamName, string format, object arg0, object arg1)
        {
            if (SameThread())
                Console.Log(streamName, format, arg0, arg1);
            else
                EnqueueLog(streamName, format, arg0, arg1);
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        /// <param name="arg1">The second format parameter</param>
        /// <param name="arg2">The third format parameter</param>
        [Conditional("USE_GEARSET")]
        public static void Log(string streamName, string format, object arg0, object arg1, object arg2)
        {
            if (SameThread())
                Console.Log(streamName, format, arg0, arg1, arg2);
            else
                EnqueueLog(streamName, format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="args">The format parameters</param>
        [Conditional("USE_GEARSET")]
        public static void Log(string streamName, string format, params object[] args)
        {
            if (SameThread())
                Console.Log(streamName, format, args);
            else
                EnqueueLog(streamName, format, args);
        }

        /// <summary>
        /// Shows a dialog asking for a filename and saves the log to the specified file.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void SaveLogToFile()
        {
            if (SameThread())
                Console.SaveLogToFile();
            else
                EnqueueSaveLogToFile();
        }

        /// <summary>
        /// Saves the log to the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to save the log (usually ending in .log)</param>
        [Conditional("USE_GEARSET")]
        public static void SaveLogToFile(string filename)
        {
            if (SameThread())
                Console.SaveLogToFile(filename);
            else
                EnqueueSaveLogToFile(filename);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowMark(string key, Vector3 position, Color color)
        {
            if (SameThread())
                Console.ShowMark(key, position, color);
            else
                EnqueueShowMark(key, position, color);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowMark(string key, Vector3 position)
        {
            if (SameThread())
                Console.ShowMark(key, position);
            else
                EnqueueShowMark(key, position);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowMark(string key, Vector2 position, Color color)
        {
            if (SameThread())
                Console.ShowMark(key, position, color);
            else
                EnqueueShowMark(key, position, color);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowMark(string key, Vector2 position)
        {
            if (SameThread())
                Console.ShowMark(key, position);
            else
                EnqueueShowMark(key, position);
        }

        /// <summary>
        /// Shows huge text on the center of the screen which fades
        /// out quickly.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void Alert(string message)
        {
            if (SameThread())
                Console.Alert(message);
            else
                EnqueueAlert(message);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLine(string key, Vector3 v1, Vector3 v2)
        {
            if (SameThread())
                Console.ShowLine(key, v1, v2);
            else
                EnqueueShowLine(key, v1, v2);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLine(string key, Vector3 v1, Vector3 v2, Color color)
        {
            if (SameThread())
                Console.ShowLine(key, v1, v2, color);
            else
                EnqueueShowLine(key, v1, v2, color);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLineOnce(Vector3 v1, Vector3 v2)
        {
            if (SameThread())
                Console.ShowLineOnce(v1, v2);
            else
                EnqueueShowLineOnce(v1, v2);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLineOnce(Vector3 v1, Vector3 v2, Color color)
        {
            if (SameThread())
                Console.ShowLineOnce(v1, v2, color);
            else
                EnqueueShowLineOnce(v1, v2, color);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLine(string key, Vector2 v1, Vector2 v2)
        {
            if (SameThread())
                Console.ShowLine(key, v1, v2);
            else
                EnqueueShowLine(key, v1, v2);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLine(string key, Vector2 v1, Vector2 v2, Color color)
        {
            if (SameThread())
                Console.ShowLine(key, v1, v2, color);
            else
                EnqueueShowLine(key, v1, v2, color);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLineOnce(Vector2 v1, Vector2 v2)
        {
            if (SameThread())
                Console.ShowLineOnce(v1, v2);
            else
                EnqueueShowLineOnce(v1, v2);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLineOnce(Vector2 v1, Vector2 v2, Color color)
        {
            if (SameThread())
                Console.ShowLineOnce(v1, v2, color);
            else
                EnqueueShowLineOnce(v1, v2, color);
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="key">Name of the persistent box</param>
        /// <param name="box">The box to draw</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBox(string key, BoundingBox box)
        {
            if (SameThread())
                Console.ShowBox(key, box);
            else
                EnqueueShowBox(key, box);
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBox(string key, Vector3 min, Vector3 max)
        {
            if (SameThread())
                Console.ShowBox(key, min, max);
            else
                EnqueueShowBox(key, min, max);
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="key">Name of the persistent box</param>
        /// <param name="box">The box to draw</param>
        /// <param name="color">The color that will be used to draw the box</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBox(string key, BoundingBox box, Color color)
        {
            if (SameThread())
                Console.ShowBox(key, box, color);
            else
                EnqueueShowBox(key, box, color);
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBox(string key, Vector3 min, Vector3 max, Color color)
        {
            if (SameThread())
                Console.ShowBox(key, min, max, color);
            else
                EnqueueShowBox(key, min, max, color);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="box">The BoundingBox to draw</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBoxOnce(BoundingBox box)
        {
            if (SameThread())
                Console.ShowBoxOnce(box);
            else
                EnqueueShowBoxOnce(box);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBoxOnce(Vector3 min, Vector3 max)
        {
            if (SameThread())
                Console.ShowBoxOnce(min, max);
            else
                EnqueueShowBoxOnce(min, max);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="box">The BoundingBox to draw</param>
        /// <param name="color">The color that will be used to draw the box</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBoxOnce(BoundingBox box, Color color)
        {
            if (SameThread())
                Console.ShowBoxOnce(box, color);
            else
                EnqueueShowBoxOnce(box, color);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// <param name="color">The color that will be used to draw the box</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBoxOnce(Vector3 min, Vector3 max, Color color)
        {
            if (SameThread())
                Console.ShowBoxOnce(min, max, color);
            else
                EnqueueShowBoxOnce(min, max, color);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphere(string key, BoundingSphere sphere)
        {
            if (SameThread())
                Console.ShowSphere(key, sphere);
            else
                EnqueueShowSphere(key, sphere);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphere(string key, Vector3 center, float radius)
        {
            if (SameThread())
                Console.ShowSphere(key, center, radius);
            else
                EnqueueShowSphere(key, center, radius);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphere(string key, BoundingSphere sphere, Color color)
        {
            if (SameThread())
                Console.ShowSphere(key, sphere, color);
            else
                EnqueueShowSphere(key, sphere, color);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphere(string key, Vector3 center, float radius, Color color)
        {
            if (SameThread())
                Console.ShowSphere(key, center, radius, color);
            else
                EnqueueShowSphere(key, center, radius, color);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphereOnce(BoundingSphere sphere)
        {
            if (SameThread())
                Console.ShowSphereOnce(sphere);
            else
                EnqueueShowSphereOnce(sphere);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphereOnce(Vector3 center, float radius)
        {
            if (SameThread())
                Console.ShowSphereOnce(center, radius);
            else
                EnqueueShowSphereOnce(center, radius);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphereOnce(BoundingSphere sphere, Color color)
        {
            if (SameThread())
                Console.ShowSphereOnce(sphere, color);
            else
                EnqueueShowSphereOnce(sphere, color);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphereOnce(Vector3 center, float radius, Color color)
        {
            if (SameThread())
                Console.ShowSphereOnce(center, radius, color);
            else
                EnqueueShowSphereOnce(center, radius, color);
        }

        /// <summary>
        /// Shows a cylinder on the screen for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowCylinderOnce(Vector3 center, Vector3 radius)
        {
            if (SameThread())
                Console.ShowCylinderOnce(center, radius);
            else
                EnqueueShowCylinderOnce(center, radius);
        }
    
        /// <summary>
        /// Shows a cylinder on the screen for one frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowCylinderOnce(Vector3 center, Vector3 radius, Color color)
        {
            if (SameThread())
                Console.ShowCylinderOnce(center, radius, color);
            else
                EnqueueShowCylinderOnce(center, radius, color);
        }

        /// <summary>
        /// Shows a label at the specified position (the text will be the label's name).
        /// </summary>
        /// <param name="name">Name of the label as well of the text to show. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position where the label will be shown</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(string name, Vector2 position)
        {
            if (SameThread())
                Console.ShowLabel(name, position);
            else
                EnqueueShowLabel(name, position);
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(string name, Vector2 position, string text)
        {
            if (SameThread())
                Console.ShowLabel(name, position, text);
            else
                EnqueueShowLabel(name, position, text);
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        /// <param name="color">Color of the text</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(string name, Vector2 position, string text, Color color)
        {
            if (SameThread())
                Console.ShowLabel(name, position, text, color);
            else
                EnqueueShowLabel(name, position, text, color);
        }

        /// <summary>
        /// Shows a label at the specified position (the text will be the label's name).
        /// </summary>
        /// <param name="name">Name of the label as well of the text to show. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position where the label will be shown</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(string name, Vector3 position)
        {
            if (SameThread())
                Console.ShowLabel(name, position);
            else
                EnqueueShowLabel(name, position);
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(string name, Vector3 position, string text)
        {
            if (SameThread())
                Console.ShowLabel(name, position, text);
            else
                EnqueueShowLabel(name, position, text);
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        /// <param name="color">Color of the text</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(string name, Vector3 position, string text, Color color)
        {
            if (SameThread())
                Console.ShowLabel(name, position, text, color);
            else
                EnqueueShowLabel(name, position, text, color);
        }

        /// <summary>
        /// Sends an object to the Inspector window.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void Inspect(string name, object o)
        {
            if (SameThread())
                Console.Inspect(name, o);
            else
                EnqueueInspect(name, o);
        }

        /// <summary>
        /// Sends an object to the Inspector window.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void Inspect(string name, object o, bool autoExpand)
        {
            if (SameThread())
                Console.Inspect(name, o, autoExpand);
            else
                EnqueueInspect(name, o, autoExpand);
        }

        /// <summary>
        /// Removes an object from the Inspector window.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void RemoveInspect(object o)
        {
            if (SameThread())
                Console.RemoveInspect(o);
            else
                EnqueueRemoveInspect(o);
        }

        /// <summary>
        /// Clears the Inspector Window.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ClearInspector()
        {
            if (SameThread())
                Console.ClearInspector();
            else
                EnqueueClearInspector();
        }

        /// <summary>
        /// Sets the function that is used by Gearset when a query is written to the
        /// Finder by the user. It usually searches through your game objects and returns
        /// a collection of the ones whose name or Type matches the query.
        /// A search function receives a String and return IEnumerable (e.g. a List)
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void SetFinderSearchFunction(SearchFunction searchFunction)
        {
            if (SameThread())
                Console.SetFinderSearchFunction(searchFunction);
            else
                EnqueueSetFinderSearchFunction(searchFunction);
        }

        /// <summary>
        /// Shows a persistent Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="name">Name of the persistent Matrix</param>
        /// <param name="transform">Transform to draw</param>
        /// <param name="axisScale">Scale to apply to each axis</param>
        [Conditional("USE_GEARSET")]
        public static void ShowTransform(string name, Matrix transform, float axisScale)
        {
            if (SameThread())
                Console.ShowTransform(name, transform, axisScale);
            else
                EnqueueShowTransform(name, transform, axisScale);
        }

        /// <summary>
        /// Shows a persistent Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="name">Name of the persistent Matrix</param>
        /// <param name="transform">Transform to draw</param>
        [Conditional("USE_GEARSET")]
        public static void ShowTransform(string name, Matrix transform)
        {
            if (SameThread())
                Console.ShowTransform(name, transform);
            else
                EnqueueShowTransform(name, transform);
        }

        /// <summary>
        /// Shows a one-frame Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="transform">Transform to draw</param>
        [Conditional("USE_GEARSET")]
        public static void ShowTransformOnce(Matrix transform)
        {
            if (SameThread())
                Console.ShowTransformOnce(transform);
            else
                EnqueueShowTransformOnce(transform);
        }

        /// <summary>
        /// Shows a one-frame Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="transform">Transform to draw</param>
        /// <param name="axisScale">Scale to apply to each axis</param>
        [Conditional("USE_GEARSET")]
        public static void ShowTransformOnce(Matrix transform, float axisScale)
        {
            if (SameThread())
                Console.ShowTransformOnce(transform, axisScale);
            else
                EnqueueShowTransformOnce(transform, axisScale);
        }

        /// <summary>
        /// Shows a persistent Vector3 on the screen as an arrow.
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector3(string name, Vector3 location, Vector3 vector, Color color)
        {
            if (SameThread())
                Console.ShowVector3(name, location, vector, color);
            else
                EnqueueShowVector3(name, location, vector, color);
        }

        /// <summary>
        /// Shows a persistent Vector3 on the screen as an arrow.
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector3(string name, Vector3 location, Vector3 vector)
        {
            if (SameThread())
                Console.ShowVector3(name, location, vector);
            else
                EnqueueShowVector3(name, location, vector);
        }

        /// <summary>
        /// Shows a Vector3 on the screen as an arrow for one frame.
        /// </summary>
        /// <param name="location">Location of the vector to show (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector3Once(Vector3 location, Vector3 vector)
        {
            if (SameThread())
                Console.ShowVector3Once(location, vector);
            else
                EnqueueShowVector3Once(location, vector);
        }

        /// <summary>
        /// Shows a Vector3 on the screen as an arrow for one frame.
        /// </summary>
        /// <param name="location">Location of the vector to show (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector3Once(Vector3 location, Vector3 vector, Color color)
        {
            if (SameThread())
                Console.ShowVector3Once(location, vector, color);
            else
                EnqueueShowVector3Once(location, vector, color);
        }

        /// <summary>
        /// Shows a persistent Vector2 on the screen as an arrow (Screen space coordinates).
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector2(string name, Vector2 location, Vector2 vector, Color color)
        {
            if (SameThread())
                Console.ShowVector2(name, location, vector, color);
            else
                EnqueueShowVector2(name, location, vector, color);
        }

        /// <summary>
        /// Shows a persistent Vector2 on the screen as an arrow (Screen space coordinates).
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector2(string name, Vector2 location, Vector2 vector)
        {
            if (SameThread())
                Console.ShowVector2(name, location, vector);
            else
                EnqueueShowVector2(name, location, vector);
        }

        /// <summary>
        /// Shows a Vector2 on the screen as an arrow for one frame (Screen space coordinates).
        /// </summary>
        /// <param name="location">Location of the vector to show (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector2Once(Vector2 location, Vector2 vector)
        {
            if (SameThread())
                Console.ShowVector2Once(location, vector);
            else
                EnqueueShowVector2Once(location, vector);
        }

        /// <summary>
        /// Shows a Vector2 on the screen as an arrow for one frame.
        /// </summary>
        /// <param name="location">Location of the vector to show (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector2Once(Vector2 location, Vector2 vector, Color color)
        {
            if (SameThread())
                Console.ShowVector2Once(location, vector, color);
            else
                EnqueueShowVector2Once(location, vector, color);
        }

        /// <summary>
        /// Adds a curve for editing in Bender
        /// </summary>
        /// <param name="name">Name of the curve to add. Group using dot separators.</param>
        /// <param name="curve">Curve to edit in Bender.</param>
        [Conditional("USE_GEARSET")]
        public static void AddCurve(string name, Curve curve)
        {
            if (SameThread())
                Console.AddCurve(name, curve);
            else
                EnqueueAddCurve(name, curve);
        }

        /// <summary>
        /// Removes the provided curve from Bender.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void RemoveCurve(Curve curve)
        {
            if (SameThread())
                Console.RemoveCurve(curve);
            else
                EnqueueRemoveCurve(curve);
        }

        /// <summary>
        /// Removes a Curve or a Group by name. The complete dot-separated
        /// path to the curve or group must be given.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void RemoveCurveOrGroup(string name)
        {
            if (SameThread())
                Console.RemoveCurveOrGroup(name);
            else
                EnqueueRemoveCurveOrGroup(name);
        }

        /// <summary>
        /// Clears all Gearset Components erasing all retained data. Inspector and Logger won't be cleared.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ClearAll()
        {
            if (SameThread())
                Console.ClearAll();
            else
                EnqueueClearAll();
        }

        /// <summary>
        /// Start new frame.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void StartFrame()
        {
            if (SameThread())
                Console.StartFrame();
            else
                EnqueueStartFrame();
        }

        /// <summary>
        /// Start measure time.
        /// </summary>
        /// <param name="markerName">name of marker.</param>
        /// <param name="color">color</param>
        [Conditional("USE_GEARSET")]
        public static void BeginMark(string markerName, Color color)
        {
            if (SameThread())
                Console.BeginMark(markerName, color);
            else
                EnqueueBeginMark(markerName, color);
        }

        /// <summary>
        /// End measuring.
        /// </summary>
        /// <param name="markerName">Name of marker.</param>
        [Conditional("USE_GEARSET")]
        public static void EndMark(string markerName)
        {
            if (SameThread())
                Console.EndMark(markerName);
            else
                EnqueueEndMark(markerName);
        }

        [Conditional("USE_GEARSET")]
        public static void Action(Action action)
        {
            if (SameThread())
                action();
            else
                EnqueueAction(action);
        }
        #endregion

        #region DelayedLambdas
        //These methods are used to delay the creation of the hidden class created for lambdas when called.
        //Basically, we delay them to reduce the garbage generated single threaded calls - multithreaded calls
        //eventually end up in here and BAM, garbage but I can live with that mostly :-)
        [Conditional("USE_GEARSET")]
        public static void EnqueueSetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection)
        {
            // Capture the parameters for lambda expr.
            var w = world; var v = view; var p = projection;
            EnqueueAction(() => Console.SetMatrices(ref w, ref v, ref p));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueSetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection, ref Matrix transform2D)
        {
            // Capture the parameters for lambda expr.
            var w = world; var v = view; var p = projection; var t = transform2D;
            EnqueueAction((() => Console.SetMatrices(ref w, ref v, ref p, ref t)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShow(string key)
        {
            EnqueueAction(() => Console.Show(key));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShow(string key, object value)
        {
            EnqueueAction((() => Console.Show(key, value)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueAddQuickAction(string name, Action action)
        {
            EnqueueAction((() => Console.AddQuickAction(name, action)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueuePlot(string plotName, float value)
        {
            EnqueueAction((() => Console.Plot(plotName, value)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueuePlot(string plotName, float value, int historyLength)
        {
            EnqueueAction((() => Console.Plot(plotName, value, historyLength)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueLog(string streamName, string content)
        {
            EnqueueAction((() => Console.Log(streamName, content)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueLog(string content)
        {
            EnqueueAction((() => Console.Log(content)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueLog(string streamName, string format, object arg0)
        {
            EnqueueAction((() => Console.Log(streamName, format, arg0)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueLog(string streamName, string format, object arg0, object arg1)
        {
            EnqueueAction((() => Console.Log(streamName, format, arg0, arg1)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueLog(string streamName, string format, object arg0, object arg1, object arg2)
        {
            EnqueueAction((() => Console.Log(streamName, format, arg0, arg1, arg2)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueLog(string streamName, string format, params object[] args)
        {
            EnqueueAction((() => Console.Log(streamName, format, args)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueSaveLogToFile()
        {
            EnqueueAction((() => Console.SaveLogToFile()));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueSaveLogToFile(string filename)
        {
            EnqueueAction((() => Console.SaveLogToFile(filename)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowMark(string key, Vector3 position, Color color)
        {
            EnqueueAction((() => Console.ShowMark(key, position, color)));
        }

        public static void EnqueueShowMark(string key, Vector3 position)
        {
            EnqueueAction((() => Console.ShowMark(key, position)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowMark(string key, Vector2 position, Color color)
        {
            EnqueueAction((() => Console.ShowMark(key, position, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowMark(string key, Vector2 position)
        {
            EnqueueAction((() => Console.ShowMark(key, position)));
        }

        /// <summary>
        /// Shows huge text on the center of the screen which fades
        /// out quickly.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void EnqueueAlert(string message)
        {
            EnqueueAction((() => Console.Alert(message)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLine(string key, Vector3 v1, Vector3 v2)
        {
            EnqueueAction((() => Console.ShowLine(key, v1, v2)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLine(string key, Vector3 v1, Vector3 v2, Color color)
        {
            EnqueueAction((() => Console.ShowLine(key, v1, v2, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLineOnce(Vector3 v1, Vector3 v2)
        {
            EnqueueAction((() => Console.ShowLineOnce(v1, v2)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLineOnce(Vector3 v1, Vector3 v2, Color color)
        {
            EnqueueAction((() => Console.ShowLineOnce(v1, v2, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLine(string key, Vector2 v1, Vector2 v2)
        {
            EnqueueAction((() => Console.ShowLine(key, v1, v2)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLine(string key, Vector2 v1, Vector2 v2, Color color)
        {
            EnqueueAction((() => Console.ShowLine(key, v1, v2, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLineOnce(Vector2 v1, Vector2 v2)
        {
            EnqueueAction((() => Console.ShowLineOnce(v1, v2)));
        }

        [Conditional("USE_GEARSET")]
        static void EnqueueShowLineOnce(Vector2 v1, Vector2 v2, Color color)
        {
            EnqueueAction(() => Console.ShowLineOnce(v1, v2, color));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowBox(string key, BoundingBox box)
        {
            EnqueueAction((() => Console.ShowBox(key, box)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowBox(string key, Vector3 min, Vector3 max)
        {
            EnqueueAction((() => Console.ShowBox(key, min, max)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowBox(string key, BoundingBox box, Color color)
        {
            EnqueueAction((() => Console.ShowBox(key, box, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowBox(string key, Vector3 min, Vector3 max, Color color)
        {
            EnqueueAction((() => Console.ShowBox(key, min, max, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowBoxOnce(BoundingBox box)
        {
            EnqueueAction((() => Console.ShowBoxOnce(box)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowBoxOnce(Vector3 min, Vector3 max)
        {
            EnqueueAction((() => Console.ShowBoxOnce(min, max)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowBoxOnce(BoundingBox box, Color color)
        {
            EnqueueAction((() => Console.ShowBoxOnce(box, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowBoxOnce(Vector3 min, Vector3 max, Color color)
        {
            EnqueueAction((() => Console.ShowBoxOnce(min, max, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowSphere(string key, BoundingSphere sphere)
        {
            EnqueueAction((() => Console.ShowSphere(key, sphere)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowSphere(string key, Vector3 center, float radius)
        {
            EnqueueAction((() => Console.ShowSphere(key, center, radius)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowSphere(string key, BoundingSphere sphere, Color color)
        {
            EnqueueAction((() => Console.ShowSphere(key, sphere, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowSphere(string key, Vector3 center, float radius, Color color)
        {
            EnqueueAction((() => Console.ShowSphere(key, center, radius, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowSphereOnce(BoundingSphere sphere)
        {
            EnqueueAction((() => Console.ShowSphereOnce(sphere)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowSphereOnce(Vector3 center, float radius)
        {
            EnqueueAction((() => Console.ShowSphereOnce(center, radius)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowSphereOnce(BoundingSphere sphere, Color color)
        {
            EnqueueAction((() => Console.ShowSphereOnce(sphere, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowSphereOnce(Vector3 center, float radius, Color color)
        {
            EnqueueAction((() => Console.ShowSphereOnce(center, radius, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowCylinderOnce(Vector3 center, Vector3 radius)
        {
            EnqueueAction((() => Console.ShowCylinderOnce(center, radius)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowCylinderOnce(Vector3 center, Vector3 radius, Color color)
        {
            EnqueueAction((() => Console.ShowCylinderOnce(center, radius, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLabel(string name, Vector2 position)
        {
            EnqueueAction((() => Console.ShowLabel(name, position)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLabel(string name, Vector2 position, string text)
        {
            EnqueueAction((() => Console.ShowLabel(name, position, text)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLabel(string name, Vector2 position, string text, Color color)
        {
            EnqueueAction((() => Console.ShowLabel(name, position, text, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLabel(string name, Vector3 position)
        {
            EnqueueAction((() => Console.ShowLabel(name, position)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLabel(string name, Vector3 position, string text)
        {
            EnqueueAction((() => Console.ShowLabel(name, position, text)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowLabel(string name, Vector3 position, string text, Color color)
        {
            EnqueueAction((() => Console.ShowLabel(name, position, text, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueInspect(string name, object o)
        {
            EnqueueAction((() => Console.Inspect(name, o)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueInspect(string name, object o, bool autoExpand)
        {
            EnqueueAction((() => Console.Inspect(name, o, autoExpand)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueRemoveInspect(object o)
        {
            EnqueueAction((() => Console.RemoveInspect(o)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueClearInspector()
        {
            EnqueueAction((() => Console.ClearInspector()));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueSetFinderSearchFunction(SearchFunction searchFunction)
        {
            EnqueueAction((() => Console.SetFinderSearchFunction(searchFunction)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowTransform(string name, Matrix transform, float axisScale)
        {
            EnqueueAction((() => Console.ShowTransform(name, transform, axisScale)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowTransform(string name, Matrix transform)
        {
            EnqueueAction((() => Console.ShowTransform(name, transform)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowTransformOnce(Matrix transform)
        {
            EnqueueAction((() => Console.ShowTransformOnce(transform)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowTransformOnce(Matrix transform, float axisScale)
        {
            EnqueueAction((() => Console.ShowTransformOnce(transform, axisScale)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowVector3(string name, Vector3 location, Vector3 vector, Color color)
        {
            EnqueueAction((() => Console.ShowVector3(name, location, vector, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowVector3(string name, Vector3 location, Vector3 vector)
        {
            EnqueueAction((() => Console.ShowVector3(name, location, vector)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowVector3Once(Vector3 location, Vector3 vector)
        {
            EnqueueAction((() => Console.ShowVector3Once(location, vector)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowVector3Once(Vector3 location, Vector3 vector, Color color)
        {
            EnqueueAction((() => Console.ShowVector3Once(location, vector, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowVector2(string name, Vector2 location, Vector2 vector, Color color)
        {
            EnqueueAction((() => Console.ShowVector2(name, location, vector, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowVector2(string name, Vector2 location, Vector2 vector)
        {
            EnqueueAction((() => Console.ShowVector2(name, location, vector)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowVector2Once(Vector2 location, Vector2 vector)
        {
            EnqueueAction((() => Console.ShowVector2Once(location, vector)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueShowVector2Once(Vector2 location, Vector2 vector, Color color)
        {
            EnqueueAction((() => Console.ShowVector2Once(location, vector, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueAddCurve(string name, Curve curve)
        {
            EnqueueAction((() => Console.AddCurve(name, curve)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueRemoveCurve(Curve curve)
        {
            EnqueueAction((() => Console.RemoveCurve(curve)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueRemoveCurveOrGroup(string name)
        {
            EnqueueAction((() => Console.RemoveCurveOrGroup(name)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueClearAll()
        {
            EnqueueAction((() => Console.ClearAll()));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueStartFrame()
        {
            EnqueueAction((() => Console.StartFrame()));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueBeginMark(string markerName, Color color)
        {
            EnqueueAction((() => Console.BeginMark(markerName, color)));
        }

        [Conditional("USE_GEARSET")]
        public static void EnqueueEndMark(string markerName)
        {
            EnqueueAction((() => Console.EndMark(markerName)));
        }

        #endregion
    }
}
