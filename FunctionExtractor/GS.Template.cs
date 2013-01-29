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
// FUNCTION WRAPPERS PLACEHOLDER
        #endregion
    }
}
