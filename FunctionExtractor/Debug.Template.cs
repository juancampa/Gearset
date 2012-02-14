#define USE_GEARSET // Comment this if you want to completely remove
                    // Gearset from your project.

using Gearset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Diagnostics;
using System.Threading;
#if WINDOWS
using Gearset.Components;
#endif

// You might want to set your own namespace here so it's easier for your
// to access the Debug class.
namespace Gearset
{
    /// <summary>
    /// Wrapper for Gearset. It should be used instead of accessing Gearset's
    /// methods directly. This allow for quickly enabling and disabling of
    /// Gearset by uncommenting/commenting the USE_GEARSET symbol at the top of
    /// this file and also provide a level of threadsaveness. 
    /// If you need it, you can rename this class.
    /// 
    /// IMPORTANT: Calling Gearset methods from other threads will work but
    /// the results might appear unordered (e.g. out of order log items)
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Gearset's main object. You should not access this object directly. Instead
        /// use the methods in this class that provide thread-safeness and easy removal
        /// of Gearset.
        /// </summary>
        public static GearConsole Console { get; private set; }

        /// <summary>
        /// This is the component that calls Update and Draw to make Gearset draw.
        /// You don't need to do anything special with this.
        /// </summary>
        public static GearsetComponent GearsetComponent { get; private set; }

        /// <summary>
        /// If you're using a transform for your 2D objects (e.g. using the SpriteBatch)
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
        private static Object _syncRoot;

        static Debug()
        {
            _syncRoot = new Object();
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
            GearsetComponent = new GearsetComponent(game);
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
        private class GearsetWrapperUpdater : GameComponent
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
                Debug.Update(gameTime);
            }

            public override void Draw(GameTime gameTime)
            {
                // If you rename this file. Update this:
                Debug.Draw(gameTime);
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
                lock (_syncRoot)
                {
                    while (queuedActions.Count > 0)
                    {
                        Action action = queuedActions.Dequeue();
                        action();
                    }
                }
            }

            Console.Update(gameTime);
        } 
        #endregion

        #region Draw
        [Conditional("USE_GEARSET")]
        public static void Draw(GameTime gameTime)
        {
            System.Diagnostics.Debug.Assert(SameThread(), "The drawing thread must be the same one that initialized this class");
            Console.Draw(gameTime);
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
            lock (_syncRoot)
            {
                queuedActions.Enqueue(action);
            }

        }
        #endregion

        #region Wrappers for Gearset methods
// FUNCTION WRAPPERS PLACEHOLDER
        #endregion
    }
}
