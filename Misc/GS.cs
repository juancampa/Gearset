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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Diagnostics;
using System.Threading;

// TODO: You might want to set your own namespace here so it's easier for your
// to access the GS class.
namespace Gearset
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
        public static float BenderNeedlePosition { get { return Console.BenderNeedlePosition; } }

        /// <summary>
        /// The thread that initialized and owns this class.
        /// (As a side note: should always be the main thread of the Game class.)
        /// </summary>
        private static Thread ownerThread;

        /// <summary>
        /// Actions that where queued because the thread that called them was not the owner thread.
        /// </summary>
        private static Queue<Action> queuedActions;

        /// <summary>
        /// An object to lock on for thread safety.
        /// </summary>
        private static Object syncRoot;

        static GS()
        {
            syncRoot = new Object();
            queuedActions = new Queue<Action>(16);
        }

        /// <summary>
        /// This is the method you need to work for Gearset to work on your game.
        /// Remember to call SetMatrices to make Gearset's camera match yours.
        /// </summary>
        /// <param name="game">Your game instance</param>
        [Conditional("USE_GEARSET")]
        internal static void Initialize(Game game)
        {
            // Create the Gearset Component, this will be in charge of
            // Initializing Gearset and Updating/Drawing it every frame.
            GearsetComponent = new Gearset.GearsetComponent(game);
            game.Components.Add(GearsetComponent);

            // This component updates this class allowing it to process
            // calls from other threads which are queued.
            game.Components.Add(new GearsetWrapperUpdater(game));

            Console = GearsetComponent.Console;
            ownerThread = Thread.CurrentThread;
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
                this.UpdateOrder = int.MaxValue - 2;
            }

            public override void Update(GameTime gameTime)
            {
                // If you rename this file. Update this:
                GS.Update(gameTime);
            }
        }

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
            {
                // Capture the parameters for lambda expr.
                Matrix w = world; Matrix v = view; Matrix p = projection;
                EnqueueAction(new Action(() => Console.SetMatrices(ref w, ref v, ref p)));
            }
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
            {
                // Capture the parameters for lambda expr.
                Matrix w = world; Matrix v = view; Matrix p = projection; Matrix t = transform2D;
                EnqueueAction(new Action(() => Console.SetMatrices(ref w, ref v, ref p, ref t)));
            }
        }
        #endregion

        #region Update
        [Conditional("USE_GEARSET")]
        public static void Update(GameTime gameTime)
        {
            System.Diagnostics.Debug.Assert(SameThread(), "The updating thread must be the same one that initialized this class");

            if (queuedActions.Count > 0)
            {
                lock (syncRoot)
                {
                    while (queuedActions.Count > 0)
                    {
                        Action action = queuedActions.Dequeue();
                        action();
                    }
                }
            }
        }
        #endregion

        #region Thread Safe Helpers
        private static bool SameThread()
        {
            return ownerThread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Wrapper for method execution.
        /// It checks if the current calling thread is the same that initialized this class.
        /// If not, then the action is queued to be consumed during the update process.
        /// </summary>
        /// <param name="action"></param>
        private static void EnqueueAction(Action action)
        {
            lock (syncRoot)
            {
                queuedActions.Enqueue(action);
            }

        }
        #endregion
        #region Wrappers for Gearset methods
        /// <summary>
        /// Adds or modifiy a key without value on the overlaid tree view.
        /// </summary>
        /// <param name="key">A dot-separated list of keys.</param>
        [Conditional("USE_GEARSET")]
        public static void Show(String key)
        {
            if (SameThread())
                Console.Show(key);
            else
                EnqueueAction(new Action(() => Console.Show(key)));
        }

        /// <summary>
        /// Adds or modifies a key/value pair to the overlaid tree view.
        /// </summary>
        /// <param name="key">A dot-separated list of keys.</param>
        /// <param name="value">The value to show.</param>
        [Conditional("USE_GEARSET")]
        public static void Show(String key, object value)
        {
            if (SameThread())
                Console.Show(key, value);
            else
                EnqueueAction(new Action(() => Console.Show(key, value)));
        }

        /// <summary>
        /// Adds an action button to the bottom of the game window.
        /// </summary>
        /// <param name="name">Name of the action as it will appear on the button.</param>
        /// <param name="action">Action to perform when the button is clicked.</param>
        [Conditional("USE_GEARSET")]
        public static void AddQuickAction(String name, Action action)
        {
            if (SameThread())
                Console.AddQuickAction(name, action);
            else
                EnqueueAction(new Action(() => Console.AddQuickAction(name, action)));
        }

        /// <summary>
        /// Adds the provided value to the plot with the provided plotName.
        /// </summary>
        /// <param name="plotName">A name that represent a data set.</param>
        /// <param name="value">The value to add to the sampler</param>
        [Conditional("USE_GEARSET")]
        public static void Plot(String plotName, float value)
        {
            if (SameThread())
                Console.Plot(plotName, value);
            else
                EnqueueAction(new Action(() => Console.Plot(plotName, value)));
        }

        /// <summary>
        /// Adds the provided value to the plot with the provided plotName. At the same time modifies
        /// the history length of the sampler.
        /// </summary>
        /// <param name="plotName">A name that represent a data set.</param>
        /// <param name="value">The value to add to the sampler</param>
        /// <param name="historyLength">The number of samples that the sampler will remember at any given time.</param>
        [Conditional("USE_GEARSET")]
        public static void Plot(String plotName, float value, int historyLength)
        {
            if (SameThread())
                Console.Plot(plotName, value, historyLength);
            else
                EnqueueAction(new Action(() => Console.Plot(plotName, value, historyLength)));
        }

        /// <summary>
        /// Los a message to the specified stream.
        /// </summary>
        /// <param name="streamName">Name of the Stream to log the message to</param>
        /// <param name="message">Message to log</param>
        [Conditional("USE_GEARSET")]
        public static void Log(String streamName, String content)
        {
            if (SameThread())
                Console.Log(streamName, content);
            else
                EnqueueAction(new Action(() => Console.Log(streamName, content)));
        }

        /// <summary>
        /// Logs the specified message in the default stream.
        /// </summary>
        /// <param name="content">The message to log.</param>
        [Conditional("USE_GEARSET")]
        public static void Log(String content)
        {
            if (SameThread())
                Console.Log(content);
            else
                EnqueueAction(new Action(() => Console.Log(content)));
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        [Conditional("USE_GEARSET")]
        public static void Log(String streamName, String format, Object arg0) 
        {
            if (SameThread())
                Console.Log(streamName, format, arg0);
            else
                EnqueueAction(new Action(() => Console.Log(streamName, format, arg0)));
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        /// <param name="arg1">The second format parameter</param>
        [Conditional("USE_GEARSET")]
        public static void Log(String streamName, String format, Object arg0, Object arg1) 
        {
            if (SameThread())
                Console.Log(streamName, format, arg0, arg1);
            else
                EnqueueAction(new Action(() => Console.Log(streamName, format, arg0, arg1)));
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
        public static void Log(String streamName, String format, Object arg0, Object arg1, Object arg2)
        {
            if (SameThread())
                Console.Log(streamName, format, arg0, arg1, arg2);
            else
                EnqueueAction(new Action(() => Console.Log(streamName, format, arg0, arg1, arg2)));
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The format parameters</param>
        [Conditional("USE_GEARSET")]
        public static void Log(String streamName, String format, params Object[] args)
        {
            if (SameThread())
                Console.Log(streamName, format, args);
            else
                EnqueueAction(new Action(() => Console.Log(streamName, format, args)));
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
                EnqueueAction(new Action(() => Console.SaveLogToFile()));
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
                EnqueueAction(new Action(() => Console.SaveLogToFile(filename)));
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowMark(String key, Vector3 position, Color color)
        {
            if (SameThread())
                Console.ShowMark(key, position, color);
            else
                EnqueueAction(new Action(() => Console.ShowMark(key, position, color)));
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowMark(String key, Vector3 position)
        {
            if (SameThread())
                Console.ShowMark(key, position);
            else
                EnqueueAction(new Action(() => Console.ShowMark(key, position)));
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowMark(String key, Vector2 position, Color color)
        {
            if (SameThread())
                Console.ShowMark(key, position, color);
            else
                EnqueueAction(new Action(() => Console.ShowMark(key, position, color)));
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowMark(String key, Vector2 position)
        {
            if (SameThread())
                Console.ShowMark(key, position);
            else
                EnqueueAction(new Action(() => Console.ShowMark(key, position)));
        }

        /// <summary>
        /// Shows huge text on the center of the screen which fades
        /// out quickly.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void Alert(String message)
        {
            if (SameThread())
                Console.Alert(message);
            else
                EnqueueAction(new Action(() => Console.Alert(message)));
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLine(String key, Vector3 v1, Vector3 v2)
        {
            if (SameThread())
                Console.ShowLine(key, v1, v2);
            else
                EnqueueAction(new Action(() => Console.ShowLine(key, v1, v2)));
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLine(String key, Vector3 v1, Vector3 v2, Color color)
        {
            if (SameThread())
                Console.ShowLine(key, v1, v2, color);
            else
                EnqueueAction(new Action(() => Console.ShowLine(key, v1, v2, color)));
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
                EnqueueAction(new Action(() => Console.ShowLineOnce(v1, v2)));
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
                EnqueueAction(new Action(() => Console.ShowLineOnce(v1, v2, color)));
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLine(String key, Vector2 v1, Vector2 v2)
        {
            if (SameThread())
                Console.ShowLine(key, v1, v2);
            else
                EnqueueAction(new Action(() => Console.ShowLine(key, v1, v2)));
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowLine(String key, Vector2 v1, Vector2 v2, Color color)
        {
            if (SameThread())
                Console.ShowLine(key, v1, v2, color);
            else
                EnqueueAction(new Action(() => Console.ShowLine(key, v1, v2, color)));
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
                EnqueueAction(new Action(() => Console.ShowLineOnce(v1, v2)));
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
                EnqueueAction(new Action(() => Console.ShowLineOnce(v1, v2, color)));
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="key">Name of the persistent box</param>
        /// <param name="box">The box to draw</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBox(String key, BoundingBox box)
        {
            if (SameThread())
                Console.ShowBox(key, box);
            else
                EnqueueAction(new Action(() => Console.ShowBox(key, box)));
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBox(String key, Vector3 min, Vector3 max)
        {
            if (SameThread())
                Console.ShowBox(key, min, max);
            else
                EnqueueAction(new Action(() => Console.ShowBox(key, min, max)));
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="key">Name of the persistent box</param>
        /// <param name="box">The box to draw</param>
        /// <param name="color">The color that will be used to draw the box</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBox(String key, BoundingBox box, Color color)
        {
            if (SameThread())
                Console.ShowBox(key, box, color);
            else
                EnqueueAction(new Action(() => Console.ShowBox(key, box, color)));
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowBox(String key, Vector3 min, Vector3 max, Color color)
        {
            if (SameThread())
                Console.ShowBox(key, min, max, color);
            else
                EnqueueAction(new Action(() => Console.ShowBox(key, min, max, color)));
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
                EnqueueAction(new Action(() => Console.ShowBoxOnce(box)));
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
                EnqueueAction(new Action(() => Console.ShowBoxOnce(min, max)));
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
                EnqueueAction(new Action(() => Console.ShowBoxOnce(box, color)));
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
                EnqueueAction(new Action(() => Console.ShowBoxOnce(min, max, color)));
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphere(String key, BoundingSphere sphere)
        {
            if (SameThread())
                Console.ShowSphere(key, sphere);
            else
                EnqueueAction(new Action(() => Console.ShowSphere(key, sphere)));
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphere(String key, Vector3 center, float radius)
        {
            if (SameThread())
                Console.ShowSphere(key, center, radius);
            else
                EnqueueAction(new Action(() => Console.ShowSphere(key, center, radius)));
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphere(String key, BoundingSphere sphere, Color color)
        {
            if (SameThread())
                Console.ShowSphere(key, sphere, color);
            else
                EnqueueAction(new Action(() => Console.ShowSphere(key, sphere, color)));
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphere(String key, Vector3 center, float radius, Color color)
        {
            if (SameThread())
                Console.ShowSphere(key, center, radius, color);
            else
                EnqueueAction(new Action(() => Console.ShowSphere(key, center, radius, color)));
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphereOnce(BoundingSphere sphere)
        {
            if (SameThread())
                Console.ShowSphereOnce(sphere);
            else
                EnqueueAction(new Action(() => Console.ShowSphereOnce(sphere)));
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphereOnce(Vector3 center, float radius)
        {
            if (SameThread())
                Console.ShowSphereOnce(center, radius);
            else
                EnqueueAction(new Action(() => Console.ShowSphereOnce(center, radius)));
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphereOnce(BoundingSphere sphere, Color color)
        {
            if (SameThread())
                Console.ShowSphereOnce(sphere, color);
            else
                EnqueueAction(new Action(() => Console.ShowSphereOnce(sphere, color)));
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void ShowSphereOnce(Vector3 center, float radius, Color color)
        {
            if (SameThread())
                Console.ShowSphereOnce(center, radius, color);
            else
                EnqueueAction(new Action(() => Console.ShowSphereOnce(center, radius, color)));
        }

        /// <summary>
        /// Shows a label at the specified position (the text will be the label's name).
        /// </summary>
        /// <param name="name">Name of the label as well of the text to show. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position where the label will be shown</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(String name, Vector2 position)
        {
            if (SameThread())
                Console.ShowLabel(name, position);
            else
                EnqueueAction(new Action(() => Console.ShowLabel(name, position)));
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(String name, Vector2 position, String text)
        {
            if (SameThread())
                Console.ShowLabel(name, position, text);
            else
                EnqueueAction(new Action(() => Console.ShowLabel(name, position, text)));
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        /// <param name="color">Color of the text</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(String name, Vector2 position, String text, Color color)
        {
            if (SameThread())
                Console.ShowLabel(name, position, text, color);
            else
                EnqueueAction(new Action(() => Console.ShowLabel(name, position, text, color)));
        }

        /// <summary>
        /// Shows a label at the specified position (the text will be the label's name).
        /// </summary>
        /// <param name="name">Name of the label as well of the text to show. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position where the label will be shown</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(String name, Vector3 position)
        {
            if (SameThread())
                Console.ShowLabel(name, position);
            else
                EnqueueAction(new Action(() => Console.ShowLabel(name, position)));
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(String name, Vector3 position, String text)
        {
            if (SameThread())
                Console.ShowLabel(name, position, text);
            else
                EnqueueAction(new Action(() => Console.ShowLabel(name, position, text)));
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        /// <param name="color">Color of the text</param>
        [Conditional("USE_GEARSET")]
        public static void ShowLabel(String name, Vector3 position, String text, Color color)
        {
            if (SameThread())
                Console.ShowLabel(name, position, text, color);
            else
                EnqueueAction(new Action(() => Console.ShowLabel(name, position, text, color)));
        }

        /// <summary>
        /// Sends an object to the Inspector window.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void Inspect(String name, Object o)
        {
            if (SameThread())
                Console.Inspect(name, o);
            else
                EnqueueAction(new Action(() => Console.Inspect(name, o)));
        }

        /// <summary>
        /// Sends an object to the Inspector window.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void Inspect(String name, Object o, bool autoExpand)
        {
            if (SameThread())
                Console.Inspect(name, o, autoExpand);
            else
                EnqueueAction(new Action(() => Console.Inspect(name, o, autoExpand)));
        }

        /// <summary>
        /// Removes an object from the Inspector window.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void RemoveInspect(Object o)
        {
            if (SameThread())
                Console.RemoveInspect(o);
            else
                EnqueueAction(new Action(() => Console.RemoveInspect(o)));
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
                EnqueueAction(new Action(() => Console.ClearInspector()));
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
                EnqueueAction(new Action(() => Console.SetFinderSearchFunction(searchFunction)));
        }

        /// <summary>
        /// Shows a persistent Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="name">Name of the persistent Matrix</param>
        /// <param name="transform">Transform to draw</param>
        /// <param name="axisScale">Scale to apply to each axis</param>
        [Conditional("USE_GEARSET")]
        public static void ShowTransform(String name, Matrix transform, float axisScale)
        {
            if (SameThread())
                Console.ShowTransform(name, transform, axisScale);
            else
                EnqueueAction(new Action(() => Console.ShowTransform(name, transform, axisScale)));
        }

        /// <summary>
        /// Shows a persistent Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="name">Name of the persistent Matrix</param>
        /// <param name="transform">Transform to draw</param>
        [Conditional("USE_GEARSET")]
        public static void ShowTransform(String name, Matrix transform)
        {
            if (SameThread())
                Console.ShowTransform(name, transform);
            else
                EnqueueAction(new Action(() => Console.ShowTransform(name, transform)));
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
                EnqueueAction(new Action(() => Console.ShowTransformOnce(transform)));
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
                EnqueueAction(new Action(() => Console.ShowTransformOnce(transform, axisScale)));
        }

        /// <summary>
        /// Shows a persistent Vector3 on the screen as an arrow.
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector3(String name, Vector3 location, Vector3 vector, Color color)
        {
            if (SameThread())
                Console.ShowVector3(name, location, vector, color);
            else
                EnqueueAction(new Action(() => Console.ShowVector3(name, location, vector, color)));
        }

        /// <summary>
        /// Shows a persistent Vector3 on the screen as an arrow.
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector3(String name, Vector3 location, Vector3 vector)
        {
            if (SameThread())
                Console.ShowVector3(name, location, vector);
            else
                EnqueueAction(new Action(() => Console.ShowVector3(name, location, vector)));
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
                EnqueueAction(new Action(() => Console.ShowVector3Once(location, vector)));
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
                EnqueueAction(new Action(() => Console.ShowVector3Once(location, vector, color)));
        }

        /// <summary>
        /// Shows a persistent Vector2 on the screen as an arrow (Screen space coordinates).
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector2(String name, Vector2 location, Vector2 vector, Color color)
        {
            if (SameThread())
                Console.ShowVector2(name, location, vector, color);
            else
                EnqueueAction(new Action(() => Console.ShowVector2(name, location, vector, color)));
        }

        /// <summary>
        /// Shows a persistent Vector2 on the screen as an arrow (Screen space coordinates).
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        [Conditional("USE_GEARSET")]
        public static void ShowVector2(String name, Vector2 location, Vector2 vector)
        {
            if (SameThread())
                Console.ShowVector2(name, location, vector);
            else
                EnqueueAction(new Action(() => Console.ShowVector2(name, location, vector)));
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
                EnqueueAction(new Action(() => Console.ShowVector2Once(location, vector)));
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
                EnqueueAction(new Action(() => Console.ShowVector2Once(location, vector, color)));
        }

        /// <summary>
        /// Adds a curve for editing in Bender
        /// </summary>
        /// <param name="name">Name of the curve to add. Group using dot separators.</param>
        /// <param name="curve">Curve to edit in Bender.</param>
        [Conditional("USE_GEARSET")]
        public static void AddCurve(String name, Curve curve)
        {
            if (SameThread())
                Console.AddCurve(name, curve);
            else
                EnqueueAction(new Action(() => Console.AddCurve(name, curve)));
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
                EnqueueAction(new Action(() => Console.RemoveCurve(curve)));
        }

        /// <summary>
        /// Removes a Curve or a Group by name. The complete dot-separated
        /// path to the curve or group must be given.
        /// </summary>
        [Conditional("USE_GEARSET")]
        public static void RemoveCurveOrGroup(String name)
        {
            if (SameThread())
                Console.RemoveCurveOrGroup(name);
            else
                EnqueueAction(new Action(() => Console.RemoveCurveOrGroup(name)));
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
                EnqueueAction(new Action(() => Console.ClearAll()));
        }


        #endregion
    }
}
